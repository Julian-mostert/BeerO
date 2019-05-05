using System.Collections.Generic;
using BeerO.SlackBotCore.MessagingPipeline.Request;
using BeerO.SlackBotCore.MessagingPipeline.Response;
using BeerO.SlackBotCore.Plugins.StandardPlugins;
using Microsoft.Extensions.Logging;

namespace BeerO.SlackBotCore.MessagingPipeline.Middleware.StandardMiddleware
{
    internal class BeginMessageMiddleware : MiddlewareBase
    {
        private readonly StatsPlugin _statsPlugin;
        private readonly ILogger _logger;

        public BeginMessageMiddleware(IMiddleware next, StatsPlugin statsPlugin, ILogger logger) : base(next)
        {
            this._statsPlugin = statsPlugin;
            this._logger = logger;
        }

        public override IEnumerable<ResponseMessage> Invoke(IncomingMessage message)
        {
            this._statsPlugin.IncrementState("Messages:Received");
            this._logger.LogInformation($"Message from {message.Username}: {message.FullText}");

            foreach (ResponseMessage responseMessage in this.Next(message))
            {
                this._statsPlugin.IncrementState("Messages:Sent");
                yield return responseMessage;
            }
        }
    }
}