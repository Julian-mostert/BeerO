﻿using System.Collections.Generic;
using System.Linq;
using System.Text;
using BeerO.SlackCore.MessagingPipeline.Middleware.ValidHandles;
using BeerO.SlackCore.MessagingPipeline.Request;
using BeerO.SlackCore.MessagingPipeline.Response;

namespace BeerO.SlackCore.MessagingPipeline.Middleware.StandardMiddleware
{
    internal class HelpMiddleware : MiddlewareBase
    {
        private readonly ISlackBotCore _slackBotCore;

        public HelpMiddleware(IMiddleware next, ISlackBotCore slackBotCore) : base(next)
        {
            this._slackBotCore = slackBotCore;

            this.HandlerMappings = new[]
            {
                new HandlerMapping
                {
                    ValidHandles = new IValidHandle[]
                    {
                        new StartsWithHandle("help")
                    },
                    Description = "Returns supported commands and descriptions of how to use them",
                    EvaluatorFunc = this.HelpHandler
                }
            };
        }

        private IEnumerable<ResponseMessage> HelpHandler(IncomingMessage message, IValidHandle matchedHandle)
        {
            var builder = new StringBuilder();
            builder.Append(">>>");

            IEnumerable<CommandDescription> supportedCommands = this.GetSupportedCommands().OrderBy(x => x.Command);

            foreach (CommandDescription commandDescription in supportedCommands)
            {
                string description = commandDescription.Description.Replace("@{bot}", $"@{this._slackBotCore.GetBotUserName()}");
                builder.AppendFormat("{0}\t- {1}\n", commandDescription.Command, description);
            }

            yield return message.ReplyToChannel(builder.ToString());
        }
    }
}
