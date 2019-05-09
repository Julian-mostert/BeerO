using System.Collections.Generic;
using Noobot.Core.MessagingPipeline.Middleware;
using Noobot.Core.MessagingPipeline.Request;
using Noobot.Core.MessagingPipeline.Response;

namespace BeerOBot.ConsoleApp.SlackMiddleWare
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