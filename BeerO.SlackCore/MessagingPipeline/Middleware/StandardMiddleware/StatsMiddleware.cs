using System;
using System.Collections.Generic;
using System.Linq;
using BeerO.SlackCore.MessagingPipeline.Middleware.ValidHandles;
using BeerO.SlackCore.MessagingPipeline.Request;
using BeerO.SlackCore.MessagingPipeline.Response;
using BeerO.SlackCore.Plugins.StandardPlugins;

namespace BeerO.SlackCore.MessagingPipeline.Middleware.StandardMiddleware
{
    internal class StatsMiddleware : MiddlewareBase
    {
        private readonly StatsPlugin _statsPlugin;

        public StatsMiddleware(IMiddleware next, StatsPlugin statsPlugin) : base(next)
        {
            this._statsPlugin = statsPlugin;
            this.HandlerMappings = new[]
            {
                new HandlerMapping
                {
                    ValidHandles = ExactMatchHandle.For("stats"),
                    Description = "Returns interesting stats about your noobot installation",
                    EvaluatorFunc = this.StatsHandler
                }
            };
        }

        private IEnumerable<ResponseMessage> StatsHandler(IncomingMessage message, IValidHandle matchedHandle)
        {
            string textMessage = string.Join(Environment.NewLine, this._statsPlugin.GetStats().OrderBy(x => x));

            if (!string.IsNullOrEmpty(textMessage))
            {
                yield return message.ReplyToChannel(">>>" + textMessage);
            }
            else
            {
                yield return message.ReplyToChannel("No stats have been recorded yet.");
            }
        }
    }
}