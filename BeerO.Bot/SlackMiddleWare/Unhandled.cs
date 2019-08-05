using System.Collections.Generic;
using BeerO.SlackCore.MessagingPipeline.Middleware;
using BeerO.SlackCore.MessagingPipeline.Request;
using BeerO.SlackCore.MessagingPipeline.Response;

namespace BeerO.Bot.SlackMiddleWare
{
    internal class UnhandledMessageMiddleware : IMiddleware
    {
        public IEnumerable<ResponseMessage> Invoke(IncomingMessage message)
        {
            yield return
                message.ReplyDirectlyToUser($"Hey {message.UserEmail} this is not the command you are looking for.");
        }

        public IEnumerable<CommandDescription> GetSupportedCommands()
        {
            return new CommandDescription[0];
        }
    }
}