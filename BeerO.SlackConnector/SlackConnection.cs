using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using BeerO.SlackConnector.BotHelpers;
using BeerO.SlackConnector.Connections;
using BeerO.SlackConnector.Connections.Clients.Channel;
using BeerO.SlackConnector.Connections.Models;
using BeerO.SlackConnector.Connections.Monitoring;
using BeerO.SlackConnector.Connections.Sockets;
using BeerO.SlackConnector.Connections.Sockets.Messages.Inbound;
using BeerO.SlackConnector.Connections.Sockets.Messages.Inbound.ReactionItem;
using BeerO.SlackConnector.Connections.Sockets.Messages.Outbound;
using BeerO.SlackConnector.EventHandlers;
using BeerO.SlackConnector.Exceptions;
using BeerO.SlackConnector.Extensions;
using BeerO.SlackConnector.Models;
using BeerO.SlackConnector.Models.Reactions;
using Flurl.Http;

namespace BeerO.SlackConnector
{
    internal class SlackConnection : ISlackConnection
    {
        private readonly IConnectionFactory _connectionFactory;
        private readonly IMentionDetector _mentionDetector;
        private readonly IMonitoringFactory _monitoringFactory;
        private IWebSocketClient _webSocketClient;
        private IPingPongMonitor _pingPongMonitor;

        private Dictionary<string, SlackChatHub> _connectedHubs { get; set; }
        public IReadOnlyDictionary<string, SlackChatHub> ConnectedHubs => this._connectedHubs;

        private Dictionary<string, SlackUser> _userCache { get; set; }
        public IReadOnlyDictionary<string, SlackUser> UserCache => this._userCache;

        public bool IsConnected => this._webSocketClient?.IsAlive ?? false;
        public DateTime? ConnectedSince { get; private set; }
        public string SlackKey { get; private set; }

        public ContactDetails Team { get; private set; }
        public ContactDetails Self { get; private set; }

        public SlackConnection(IConnectionFactory connectionFactory, IMentionDetector mentionDetector, IMonitoringFactory monitoringFactory)
        {
            this._connectionFactory = connectionFactory;
            this._mentionDetector = mentionDetector;
            this._monitoringFactory = monitoringFactory;
        }

        public async Task Initialise(ConnectionInformation connectionInformation)
        {
            this.SlackKey = connectionInformation.SlackKey;
            this.Team = connectionInformation.Team;
            this.Self = connectionInformation.Self;
            this._userCache = connectionInformation.Users;
            this._connectedHubs = connectionInformation.SlackChatHubs;

            this._webSocketClient = connectionInformation.WebSocket;
            this._webSocketClient.OnClose += (sender, args) =>
            {
                this.ConnectedSince = null;
                this.RaiseOnDisconnect();
            };

            this._webSocketClient.OnMessage += async (sender, message) => await this.ListenTo(message);

            this.ConnectedSince = DateTime.Now;

            this._pingPongMonitor = this._monitoringFactory.CreatePingPongMonitor();
            await this._pingPongMonitor.StartMonitor(this.Ping, this.Reconnect, TimeSpan.FromMinutes(2));
        }

        private async Task Reconnect()
        {
            var reconnectingEvent = this.RaiseOnReconnecting();

            var handshakeClient = this._connectionFactory.CreateHandshakeClient();
            var handshake = await handshakeClient.FirmShake(this.SlackKey);
            await this._webSocketClient.Connect(handshake.WebSocketUrl);

            await Task.WhenAll(reconnectingEvent, this.RaiseOnReconnect());
        }

        private Task ListenTo(InboundMessage inboundMessage)
        {
            if (inboundMessage == null)
            {
                return Task.CompletedTask;
            }

            //TODO: Visitor pattern?
            switch (inboundMessage.MessageType)
            {
                case MessageType.Message: return this.HandleMessage((ChatMessage)inboundMessage);
                case MessageType.GroupJoined: return this.HandleGroupJoined((GroupJoinedMessage)inboundMessage);
                case MessageType.ChannelJoined: return this.HandleChannelJoined((ChannelJoinedMessage)inboundMessage);
                case MessageType.ImCreated: return this.HandleDmJoined((DmChannelJoinedMessage)inboundMessage);
                case MessageType.TeamJoin: return this.HandleUserJoined((UserJoinedMessage)inboundMessage);
                case MessageType.Pong: return this.HandlePong((PongMessage)inboundMessage);
                case MessageType.ReactionAdded: return this.HandleReaction((ReactionMessage)inboundMessage);
                case MessageType.ChannelCreated: return this.HandleChannelCreated((ChannelCreatedMessage)inboundMessage);
            }

            return Task.CompletedTask;
        }

        private Task HandleMessage(ChatMessage inboundMessage)
        {
            if (string.IsNullOrEmpty(inboundMessage.User))
                return Task.CompletedTask;

            if (!string.IsNullOrEmpty(this.Self.Id) && inboundMessage.User == this.Self.Id)
                return Task.CompletedTask;

            //TODO: Insert into connectedHubs when DM is missing

            var message = new SlackMessage
            {
                User = this.GetMessageUser(inboundMessage.User),
                Timestamp = inboundMessage.Timestamp,
                Text = inboundMessage.Text,
                ChatHub = this.GetChatHub(inboundMessage.Channel),
                RawData = inboundMessage.RawData,
                MentionsBot = this._mentionDetector.WasBotMentioned(this.Self.Name, this.Self.Id, inboundMessage.Text),
                MessageSubType = inboundMessage.MessageSubType.ToSlackMessageSubType(),
                Files = inboundMessage.Files.ToSlackFiles()
            };

            return this.RaiseMessageReceived(message);
        }

        private Task HandleReaction(ReactionMessage inboundMessage)
        {
            if (string.IsNullOrEmpty(inboundMessage.User))
                return Task.CompletedTask;

            if (!string.IsNullOrEmpty(this.Self.Id) && inboundMessage.User == this.Self.Id)
                return Task.CompletedTask;

            if (inboundMessage.ReactingTo is MessageReaction messageReaction)
            {
                //TODO: Interface methods? Extension methods?
                return this.RaiseReactionReceived(
                    new SlackMessageReaction
                    {
                        User = this.GetMessageUser(inboundMessage.User),
                        Timestamp = inboundMessage.Timestamp,
                        ChatHub = this.GetChatHub(messageReaction.Channel),
                        RawData = inboundMessage.RawData,
                        Reaction = inboundMessage.Reaction,
                        ReactingToUser = this.GetMessageUser(inboundMessage.ReactingToUser)
                    });
            }

            if (inboundMessage.ReactingTo is FileReaction fileReaction)
            {
                return this.RaiseReactionReceived(
                    new SlackFileReaction
                    {
                        User = this.GetMessageUser(inboundMessage.User),
                        Timestamp = inboundMessage.Timestamp,
                        File = fileReaction.File,
                        RawData = inboundMessage.RawData,
                        Reaction = inboundMessage.Reaction,
                        ReactingToUser = this.GetMessageUser(inboundMessage.ReactingToUser)
                    });
            }

            if (inboundMessage.ReactingTo is FileCommentReaction fileCommentReaction)
            {
                return this.RaiseReactionReceived(
                    new SlackFileCommentReaction
                    {
                        User = this.GetMessageUser(inboundMessage.User),
                        Timestamp = inboundMessage.Timestamp,
                        File = fileCommentReaction.File,
                        FileComment = fileCommentReaction.FileComment,
                        RawData = inboundMessage.RawData,
                        Reaction = inboundMessage.Reaction,
                        ReactingToUser = this.GetMessageUser(inboundMessage.ReactingToUser)
                    });
            }

            return this.RaiseReactionReceived(
                new SlackUnknownReaction
                {
                    User = this.GetMessageUser(inboundMessage.User),
                    Timestamp = inboundMessage.Timestamp,
                    RawData = inboundMessage.RawData,
                    Reaction = inboundMessage.Reaction,
                    ReactingToUser = this.GetMessageUser(inboundMessage.ReactingToUser)
                });
        }

        private Task HandleGroupJoined(GroupJoinedMessage inboundMessage)
        {
            string channelId = inboundMessage?.Channel?.Id;
            if (channelId == null) return Task.CompletedTask;

            var hub = inboundMessage.Channel.ToChatHub();
            this._connectedHubs[channelId] = hub;

            return this.RaiseChatHubJoined(hub);
        }

        private Task HandleChannelJoined(ChannelJoinedMessage inboundMessage)
        {
            string channelId = inboundMessage?.Channel?.Id;
            if (channelId == null) return Task.CompletedTask;

            var hub = inboundMessage.Channel.ToChatHub();
            this._connectedHubs[channelId] = hub;

            return this.RaiseChatHubJoined(hub);
        }

        private Task HandleDmJoined(DmChannelJoinedMessage inboundMessage)
        {
            string channelId = inboundMessage?.Channel?.Id;
            if (channelId == null) return Task.CompletedTask;

            var hub = inboundMessage.Channel.ToChatHub(this._userCache.Values.ToArray());
            this._connectedHubs[channelId] = hub;

            return this.RaiseChatHubJoined(hub);
        }

        private Task HandleUserJoined(UserJoinedMessage inboundMessage)
        {
            SlackUser slackUser = inboundMessage.User.ToSlackUser();
            this._userCache[slackUser.Id] = slackUser;

            return this.RaiseUserJoined(slackUser);
        }

        private Task HandlePong(PongMessage inboundMessage)
        {
            this._pingPongMonitor.Pong();
            return this.RaisePong(inboundMessage.Timestamp);
        }

        private Task HandleChannelCreated(ChannelCreatedMessage inboundMessage)
        {
            string channelId = inboundMessage?.Channel?.Id;
            if (channelId == null) return Task.CompletedTask;

            var hub = inboundMessage.Channel.ToChatHub();
            this._connectedHubs[channelId] = hub;
            
            var slackChannelCreated = new SlackChannelCreated
            {
                Id = channelId,
                Name = inboundMessage.Channel.Name,
                Creator = this.GetMessageUser(inboundMessage.Channel.Creator)
            };
            return this.RaiseOnChannelCreated(slackChannelCreated);
        }

        private SlackUser GetMessageUser(string userId)
        {
            if (string.IsNullOrEmpty(userId))
            {
                return null;
            }

            return this.UserCache.ContainsKey(userId)
                ? this.UserCache[userId]
                : new SlackUser { Id = userId, Name = string.Empty };
        }

        private SlackChatHub GetChatHub(string channel)
        {
            return channel != null && this._connectedHubs.ContainsKey(channel)
                ? this._connectedHubs[channel]
                : null;
        }

        public async Task Close()
        {
            if (this._webSocketClient != null && this._webSocketClient.IsAlive)
            {
                await this._webSocketClient.Close();
            }
        }

        public async Task Say(BotMessage message)
        {
            if (string.IsNullOrEmpty(message.ChatHub?.Id))
            {
                throw new MissingChannelException("When calling the Say() method, the message parameter must have its ChatHub property set.");
            }

            var client = this._connectionFactory.CreateChatClient();
            await client.PostMessage(this.SlackKey, message.ChatHub.Id, message.Text, message.Attachments);
        }

        public async Task Upload(SlackChatHub chatHub, string filePath)
        {
            var client = this._connectionFactory.CreateFileClient();
            await client.PostFile(this.SlackKey, chatHub.Id, filePath);
        }

        public async Task Upload(SlackChatHub chatHub, Stream stream, string fileName)
        {
            var client = this._connectionFactory.CreateFileClient();
            await client.PostFile(this.SlackKey, chatHub.Id, stream, fileName);
        }

        public async Task<IEnumerable<SlackChatHub>> GetChannels()
        {
            IChannelClient client = this._connectionFactory.CreateChannelClient();

            var channelsTask = client.GetChannels(this.SlackKey);
            var groupsTask = client.GetGroups(this.SlackKey);
            await Task.WhenAll(channelsTask, groupsTask);

            var fromChannels = channelsTask.Result.Select(c => c.ToChatHub());
            var fromGroups = groupsTask.Result.Select(g => g.ToChatHub());
            return fromChannels.Concat(fromGroups);
        }

        public async Task<IEnumerable<SlackUser>> GetUsers()
        {
            IChannelClient client = this._connectionFactory.CreateChannelClient();
            var users = await client.GetUsers(this.SlackKey);

            //TODO: Update user cache
            return users.Select(u => u.ToSlackUser());
        }

        //TODO: Cache newly created channel, and return if already exists
        public async Task<SlackChatHub> JoinDirectMessageChannel(string user)
        {
            if (string.IsNullOrEmpty(user))
            {
                throw new ArgumentNullException(nameof(user));
            }

            IChannelClient client = this._connectionFactory.CreateChannelClient();
            Channel channel = await client.JoinDirectMessageChannel(this.SlackKey, user);

            return new SlackChatHub
            {
                Id = channel.Id,
                Name = channel.Name,
                Type = SlackChatHubType.Dm
            };
        }

        public async Task<SlackChatHub> JoinChannel(string channelName)
        {
            if (string.IsNullOrEmpty(channelName))
            {
                throw new ArgumentNullException(nameof(channelName));
            }

            IChannelClient client = this._connectionFactory.CreateChannelClient();
            Channel channel = await client.JoinChannel(this.SlackKey, channelName);

            return new SlackChatHub
            {
                Id = channel.Id,
                Name = channel.Name,
                Type = SlackChatHubType.Channel
            };
        }

        public async Task<SlackChatHub> CreateChannel(string channelName)
        {
            if (string.IsNullOrEmpty(channelName))
            {
                throw new ArgumentNullException(nameof(channelName));
            }

            IChannelClient client = this._connectionFactory.CreateChannelClient();
            Channel channel = await client.CreateChannel(this.SlackKey, channelName);

            return new SlackChatHub
            {
                Id = channel.Id,
                Name = channel.Name,
                Type = SlackChatHubType.Channel
            };
        }

        public async Task ArchiveChannel(string channelName)
        {
            if (string.IsNullOrEmpty(channelName))
            {
                throw new ArgumentNullException(nameof(channelName));
            }

            IChannelClient client = this._connectionFactory.CreateChannelClient();
            await client.ArchiveChannel(this.SlackKey, channelName);
        }

        public async Task<SlackPurpose> SetChannelPurpose(string channelName, string purpose)
        {
            if (string.IsNullOrEmpty(channelName))
            {
                throw new ArgumentNullException(nameof(channelName));
            }

            if (string.IsNullOrEmpty(purpose))
            {
                throw new ArgumentNullException(nameof(purpose));
            }

            IChannelClient client = this._connectionFactory.CreateChannelClient();
            string purposeSet = await client.SetPurpose(this.SlackKey, channelName, purpose);

            return new SlackPurpose
            {
                ChannelName = channelName,
                Purpose = purposeSet
            };
        }

        public async Task<SlackTopic> SetChannelTopic(string channelName, string topic)
        {
            if (string.IsNullOrEmpty(channelName))
            {
                throw new ArgumentNullException(nameof(channelName));
            }

            if (string.IsNullOrEmpty(topic))
            {
                throw new ArgumentNullException(nameof(topic));
            }

            IChannelClient client = this._connectionFactory.CreateChannelClient();
            string topicSet = await client.SetTopic(this.SlackKey, channelName, topic);

            return new SlackTopic
            {
                ChannelName = channelName,
                Topic = topicSet
            };
        }

        public async Task IndicateTyping(SlackChatHub chatHub)
        {
            var message = new TypingIndicatorMessage
            {
                Channel = chatHub.Id
            };

            await this._webSocketClient.SendMessage(message);
        }

        public async Task Ping()
        {
            await this._webSocketClient.SendMessage(new PingMessage());
        }

        public Task<Stream> DownloadFile(Uri downloadUri)
        {
            if (!downloadUri.Host.Equals("files.slack.com"))
            {
                throw new ArgumentException("Invalid uri. Should be targetting files.slack.com", nameof(downloadUri));
            }

            return downloadUri.AbsoluteUri
                .WithOAuthBearerToken(this.SlackKey)
                .AllowHttpStatus()
                .GetStreamAsync();
        }

        public event DisconnectEventHandler OnDisconnect;
        private void RaiseOnDisconnect()
        {
            this.OnDisconnect?.Invoke();
        }

        public event ReconnectEventHandler OnReconnecting;
        private async Task RaiseOnReconnecting()
        {
            var e = this.OnReconnecting;
            if (e != null)
            {
                try
                {
                    await e();
                }
                catch (Exception)
                {

                }
            }
        }

        public event ReconnectEventHandler OnReconnect;
        private async Task RaiseOnReconnect()
        {
            var e = this.OnReconnect;
            if (e != null)
            {
                try
                {
                    await e();
                }
                catch (Exception)
                {

                }
            }
        }

        public event MessageReceivedEventHandler OnMessageReceived;
        private async Task RaiseMessageReceived(SlackMessage message)
        {
            var e = this.OnMessageReceived;
            if (e != null)
            {
                try
                {
                    await e(message);
                }
                catch (Exception)
                {

                }
            }
        }

        public event ReactionReceivedEventHandler OnReaction;
        private async Task RaiseReactionReceived(ISlackReaction reaction)
        {
            var e = this.OnReaction;
            if (e != null)
            {
                try
                {
                    await e(reaction);
                }
                catch (Exception)
                {

                }
            }
        }

        public event ChatHubJoinedEventHandler OnChatHubJoined;
        private async Task RaiseChatHubJoined(SlackChatHub hub)
        {
            var e = this.OnChatHubJoined;
            if (e != null)
            {
                try
                {
                    await e(hub);
                }
                catch
                {
                }
            }
        }

        public event UserJoinedEventHandler OnUserJoined;
        private async Task RaiseUserJoined(SlackUser user)
        {
            var e = this.OnUserJoined;
            try
            {
                if (e != null)
                {
                    await e(user);
                }
            }
            catch
            {
            }
        }

        public event PongEventHandler OnPong;
        private async Task RaisePong(DateTime timestamp)
        {
            var e = this.OnPong;
            if (e != null)
            {
                try
                {
                    await e(timestamp);
                }
                catch
                {
                }
            }
        }

        public event ChannelCreatedHandler OnChannelCreated;
        private async Task RaiseOnChannelCreated(SlackChannelCreated chatHub)
        {
            var e = this.OnChannelCreated;
            if (e != null)
            {
                try
                {
                    await e(chatHub);
                }
                catch
                {
                }
            }
        }
        //TODO: USER JOINED EVENT HANDLING
    }
}
