using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using BeerO.SlackBotCore.Configuration;
using BeerO.SlackBotCore.DependencyResolution;
using BeerO.SlackBotCore.Extensions;
using BeerO.SlackBotCore.Logging;
using BeerO.SlackBotCore.MessagingPipeline.Middleware;
using BeerO.SlackBotCore.MessagingPipeline.Request;
using BeerO.SlackBotCore.MessagingPipeline.Request.Extensions;
using BeerO.SlackBotCore.MessagingPipeline.Response;
using BeerO.SlackBotCore.Plugins;
using BeerO.SlackBotCore.Plugins.StandardPlugins;
using BeerO.SlackConnector;
using BeerO.SlackConnector.Models;


namespace BeerO.SlackBotCore
{
    internal class NoobotCore : INoobotCore
    {
        private readonly IConfigReader _configReader;
        private readonly ILogger _logger;
        private readonly INoobotContainer _container;
        private readonly AverageStat _averageResponse;
        private ISlackConnection _connection;

        public NoobotCore(IConfigReader configReader, ILogger logger, INoobotContainer container)
        {
            this._configReader = configReader;
            this._logger = logger;
            this._container = container;
            this._averageResponse = new AverageStat("milliseconds");
        }

        public async Task Connect()
        {
            string slackKey = this._configReader.SlackApiKey;

            var connector = new SlackConnector.SlackConnector();
            this._connection = await connector.Connect(slackKey);
            this._connection.OnMessageReceived += this.MessageReceived;
            this._connection.OnDisconnect += this.OnDisconnect;
            this._connection.OnReconnecting += this.OnReconnecting;
            this._connection.OnReconnect += this.OnReconnect;

            this._logger.LogInformation("Connected!");
            this._logger.LogInformation($"Bots Name: {this._connection.Self.Name}");
            this._logger.LogInformation($"Team Name: {this._connection.Team.Name}");

            this._container.GetPlugin<StatsPlugin>()?.RecordStat("Connected:Since", DateTime.Now.ToString("G"));
            this._container.GetPlugin<StatsPlugin>()?.RecordStat("Response:Average", this._averageResponse);

            this.StartPlugins();
        }

        private Task OnReconnect()
        {
            this._logger.LogInformation("Connection Restored!");
            this._container.GetPlugin<StatsPlugin>().IncrementState("ConnectionsRestored");
            return Task.CompletedTask;
        }

        private Task OnReconnecting()
        {
            this._logger.LogInformation("Attempting to reconnect to Slack...");
            this._container.GetPlugin<StatsPlugin>().IncrementState("Reconnecting");
            return Task.CompletedTask;
        }

        private bool _isDisconnecting;
        public void Disconnect()
        {
            this._isDisconnecting = true;

            if (this._connection != null && this._connection.IsConnected)
            {
                this._connection
                    .Close()
                    .GetAwaiter()
                    .GetResult();
            }
        }

        private void OnDisconnect()
        {
            this.StopPlugins();

            if (this._isDisconnecting)
            {
                this._logger.LogInformation("Disconnected.");
            }
            else
            {
                this._logger.LogInformation("Disconnected from server, attempting to reconnect...");
                this.Reconnect();
            }
        }

        internal void Reconnect()
        {
            this._logger.LogInformation("Reconnecting...");
            if (this._connection != null)
            {
                this._connection.OnMessageReceived -= this.MessageReceived;
                this._connection.OnDisconnect -= this.OnDisconnect;
                this._connection = null;
            }

            this._isDisconnecting = false;
            this.Connect()
                .ContinueWith(task =>
                {
                    if (task.IsCompleted && !task.IsCanceled && !task.IsFaulted)
                    {
                        this._logger.LogInformation("Connection restored.");
                        this._container.GetPlugin<StatsPlugin>().IncrementState("ConnectionsRestored");
                    }
                    else
                    {
                        this._logger.LogInformation($"Error while reconnecting: {task.Exception}");
                    }
                });
        }

        public async Task MessageReceived(SlackMessage message)
        {
            Stopwatch stopwatch = Stopwatch.StartNew();
            this._logger.LogInformation($"[Message found from '{message.User.Name}']");

            IMiddleware pipeline = this._container.GetMiddlewarePipeline();
            var incomingMessage = new IncomingMessage
            {
                RawText = message.Text,
                FullText = message.Text,
                UserId = message.User.Id,
                Username = this.GetUsername(message),
                UserEmail = message.User.Email,
                Channel = message.ChatHub.Id,
                ChannelType = message.ChatHub.Type == SlackChatHubType.Dm ? ResponseType.DirectMessage : ResponseType.Channel,
                UserChannel = await this.GetUserChannel(message),
                BotName = this._connection.Self.Name,
                BotId = this._connection.Self.Id,
                BotIsMentioned = message.MentionsBot
            };

            incomingMessage.TargetedText = incomingMessage.GetTargetedText();

            try
            {
                foreach (ResponseMessage responseMessage in pipeline.Invoke(incomingMessage))
                {
                    await this.SendMessage(responseMessage);
                }
            }
            catch (Exception ex)
            {
                this._logger.LogError($"ERROR WHILE PROCESSING MESSAGE: {ex}");
            }

            stopwatch.Stop();

            this._logger.LogInformation($"[Message ended - Took {stopwatch.ElapsedMilliseconds} milliseconds]");
            this._averageResponse.Log(stopwatch.ElapsedMilliseconds);
        }

        public async Task Ping()
        {
            await this._connection.Ping();
        }

        public async Task SendMessage(ResponseMessage responseMessage)
        {
            SlackChatHub chatHub = await this.GetChatHub(responseMessage);

            if (chatHub != null)
            {
                if (responseMessage is TypingIndicatorMessage)
                {
                    this._logger.LogInformation($"Indicating typing on channel '{chatHub.Name}'");
                    await this._connection.IndicateTyping(chatHub);
                }
                else
                {
                    var botMessage = new BotMessage
                    {
                        ChatHub = chatHub,
                        Text = responseMessage.Text,
                        Attachments = this.GetAttachments(responseMessage.Attachments)
                    };

                    string textTrimmed = botMessage.Text.Length > 50 ? botMessage.Text.Substring(0, 50) + "..." : botMessage.Text;
                    this._logger.LogInformation($"Sending message '{textTrimmed}'");
                    await this._connection.Say(botMessage);
                }
            }
            else
            {
                this._logger.LogError($"Unable to find channel for message '{responseMessage.Text}'. Message not sent");
            }
        }

        private IList<SlackAttachment> GetAttachments(List<Attachment> attachments)
        {
            var slackAttachments = new List<SlackAttachment>();

            if (attachments != null)
            {
                foreach (var attachment in attachments)
                {
                    slackAttachments.Add(new SlackAttachment
                    {
                        Text = attachment.Text,
                        Title = attachment.Title,
                        Fallback = attachment.Fallback,
                        ImageUrl = attachment.ImageUrl,
                        ThumbUrl = attachment.ThumbUrl,
                        AuthorName = attachment.AuthorName,
                        ColorHex = attachment.Color,
                        Fields = this.GetAttachmentFields(attachment)
                    });
                }
            }

            return slackAttachments;
        }

        private IList<SlackAttachmentField> GetAttachmentFields(Attachment attachment)
        {
            var attachmentFields = new List<SlackAttachmentField>();

            if (attachment?.AttachmentFields != null)
            {
                foreach (var attachmentField in attachment.AttachmentFields)
                {
                    attachmentFields.Add(new SlackAttachmentField
                    {
                        Title = attachmentField.Title,
                        Value = attachmentField.Value,
                        IsShort = attachmentField.IsShort
                    });
                }
            }

            return attachmentFields;
        }

        public string GetUserIdForUsername(string username)
        {
            var user = this._connection.UserCache.FirstOrDefault(x => x.Value.Name.Equals(username, StringComparison.OrdinalIgnoreCase));
            return string.IsNullOrEmpty(user.Key) ? string.Empty : user.Key;
        }

        public string GetUserIdForUserEmail(string email)
        {
            var user = this._connection.UserCache.WithEmailSet().FindByEmail(email);
            return user?.Id ?? string.Empty;
        }

        public string GetChannelId(string channelName)
        {
            var channel = this._connection.ConnectedChannels().FirstOrDefault(x => x.Name.Equals(channelName, StringComparison.OrdinalIgnoreCase));
            return channel != null ? channel.Id : string.Empty;
        }

        public Dictionary<string, string> ListChannels()
        {
            return this._connection.ConnectedHubs.Values.ToDictionary(channel => channel.Id, channel => channel.Name);
        }

        public string GetBotUserName()
        {
            return this._connection?.Self.Name;
        }

        private string GetUsername(SlackMessage message)
        {
            return this._connection.UserCache.ContainsKey(message.User.Id) ? this._connection.UserCache[message.User.Id].Name : string.Empty;
        }

        private async Task<string> GetUserChannel(SlackMessage message)
        {
            return (await this.GetUserChatHub(message.User.Id, joinChannel: false) ?? new SlackChatHub()).Id;
        }

        private async Task<SlackChatHub> GetChatHub(ResponseMessage responseMessage)
        {
            SlackChatHub chatHub = null;

            if (responseMessage.ResponseType == ResponseType.Channel)
            {
                chatHub = new SlackChatHub { Id = responseMessage.Channel };
            }
            else if (responseMessage.ResponseType == ResponseType.DirectMessage)
            {
                if (string.IsNullOrEmpty(responseMessage.Channel))
                {
                    chatHub = await this.GetUserChatHub(responseMessage.UserId);
                }
                else
                {
                    chatHub = new SlackChatHub { Id = responseMessage.Channel };
                }
            }

            return chatHub;
        }

        private async Task<SlackChatHub> GetUserChatHub(string userId, bool joinChannel = true)
        {
            SlackChatHub chatHub = null;

            if (this._connection.UserCache.ContainsKey(userId))
            {
                string username = "@" + this._connection.UserCache[userId].Name;
                chatHub = this._connection.ConnectedDMs().FirstOrDefault(x => x.Name.Equals(username, StringComparison.OrdinalIgnoreCase));
            }

            if (chatHub == null && joinChannel)
            {
                chatHub = await this._connection.JoinDirectMessageChannel(userId);
            }

            return chatHub;
        }

        /// <summary>
        /// TODO: Move these methods into container?
        /// </summary>
        private void StartPlugins()
        {
            IPlugin[] plugins = this._container.GetPlugins();
            foreach (IPlugin plugin in plugins)
            {
                plugin.Start();
            }
        }

        /// <summary>
        /// TODO: Move these methods into container?
        /// </summary>
        private void StopPlugins()
        {
            IPlugin[] plugins = this._container.GetPlugins();
            foreach (IPlugin plugin in plugins)
            {
                plugin.Stop();
            }
        }
    }
}