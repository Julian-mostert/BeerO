using System;
using System.Collections.Generic;
using BeerO.SlackCore.MessagingPipeline.Middleware.ValidHandles;
using BeerO.SlackCore.MessagingPipeline.Request;
using BeerO.SlackCore.MessagingPipeline.Response;

namespace BeerO.SlackCore.MessagingPipeline.Middleware.StandardMiddleware
{
    internal class AboutMiddleware : MiddlewareBase
    {
        public AboutMiddleware(IMiddleware next) : base(next)
        {
            this.HandlerMappings = new[]
            {
                new HandlerMapping
                {
                    ValidHandles = ExactMatchHandle.For("about"),
                    Description = "Tells you some stuff about this bot :-)",
                    EvaluatorFunc = this.AboutHandler
                }
            };
        }

        private IEnumerable<ResponseMessage> AboutHandler(IncomingMessage message, IValidHandle matchedHandle)
        {
            yield return message.ReplyDirectlyToUser("Noobot - Created by Simon Colmer " + DateTime.Now.Year);
            yield return message.ReplyDirectlyToUser("I am an extensible SlackBot built in C# using loads of awesome open source projects.");
            yield return message.ReplyDirectlyToUser("Please find more at http://github.com/noobot/noobot");
        }
    }
}