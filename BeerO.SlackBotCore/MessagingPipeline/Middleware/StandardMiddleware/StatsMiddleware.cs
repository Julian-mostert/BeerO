using System;
using System.Collections.Generic;
using System.Linq;
using BeerO.SlackBotCore.MessagingPipeline.Middleware.ValidHandles;
using BeerO.SlackBotCore.MessagingPipeline.Request;
using BeerO.SlackBotCore.MessagingPipeline.Response;
using BeerO.SlackBotCore.Plugins.StandardPlugins;

namespace BeerO.SlackBotCore.MessagingPipeline.Middleware.StandardMiddleware
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
                    Description = "Returns interesting stats about your BeerO.SlackBotCore.SlackBotCore installation",
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