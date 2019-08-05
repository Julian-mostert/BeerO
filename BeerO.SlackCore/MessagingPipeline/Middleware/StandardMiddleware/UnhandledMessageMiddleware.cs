using System.Collections.Generic;
using BeerO.SlackCore.MessagingPipeline.Request;
using BeerO.SlackCore.MessagingPipeline.Response;
using Microsoft.Extensions.Logging;

namespace BeerO.SlackCore.MessagingPipeline.Middleware.StandardMiddleware
{
    /// <summary>
    /// Should always be the last middleware. Simply logs and stops the chain.
    /// </summary>
    internal class UnhandledMessageMiddleware : IMiddleware
    {
        private readonly ILogger _logger;

        public UnhandledMessageMiddleware(ILogger logger)
        {
            this._logger = logger;
        }

        public IEnumerable<ResponseMessage> Invoke(IncomingMessage message)
        {
            this._logger.LogInformation("Unhandled message.");

            if(message.ChannelType == ResponseType.DirectMessage)
            {
                yield return message.ReplyToChannel("Sorry, I didn't understand that request.");
                yield return message.ReplyToChannel("Just type `help` so I can show you what I can do!");
            }
        }

        public IEnumerable<CommandDescription> GetSupportedCommands() => new CommandDescription[0];
    }
}