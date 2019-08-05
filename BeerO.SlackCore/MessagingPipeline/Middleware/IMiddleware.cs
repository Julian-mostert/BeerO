using System.Collections.Generic;
using BeerO.SlackCore.MessagingPipeline.Request;
using BeerO.SlackCore.MessagingPipeline.Response;

namespace BeerO.SlackCore.MessagingPipeline.Middleware
{
    public interface IMiddleware
    {
        IEnumerable<ResponseMessage> Invoke(IncomingMessage message);
        IEnumerable<CommandDescription> GetSupportedCommands();
    }
}