using System.Collections.Generic;
using BeerO.SlackBotCore.MessagingPipeline.Request;
using BeerO.SlackBotCore.MessagingPipeline.Response;

namespace BeerO.SlackBotCore.MessagingPipeline.Middleware
{
    public interface IMiddleware
    {
        IEnumerable<ResponseMessage> Invoke(IncomingMessage message);
        IEnumerable<CommandDescription> GetSupportedCommands();
    }
}