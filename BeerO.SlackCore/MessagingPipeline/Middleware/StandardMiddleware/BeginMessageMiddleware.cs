using System.Collections.Generic;
using BeerO.SlackCore.MessagingPipeline.Request;
using BeerO.SlackCore.MessagingPipeline.Response;
using BeerO.SlackCore.Plugins.StandardPlugins;
using Microsoft.Extensions.Logging;

namespace BeerO.SlackCore.MessagingPipeline.Middleware.StandardMiddleware
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