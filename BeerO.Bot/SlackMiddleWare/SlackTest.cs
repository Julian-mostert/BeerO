using System;
using System.Collections.Generic;
using Noobot.Core.MessagingPipeline.Middleware;
using Noobot.Core.MessagingPipeline.Middleware.ValidHandles;
using Noobot.Core.MessagingPipeline.Request;
using Noobot.Core.MessagingPipeline.Response;
using Noobot.Core.Plugins.StandardPlugins;

namespace BeerOBot.ConsoleApp.SlackMiddleWare
{
    public class SlackTest : MiddlewareBase
    {
        public SlackTest(IMiddleware next, StatsPlugin statsPlugin) : base(next)
        {
            this.HandlerMappings = new HandlerMapping[]
            {
                new HandlerMapping()
                {
                    ValidHandles = ExactMatchHandle.For("can i have a beer", "what time is it"),
                    Description = "Checks if it is Beer O Clock",
                    EvaluatorFunc =
                        new Func<IncomingMessage, IValidHandle, IEnumerable<ResponseMessage>>(this.CheckWhatTimeItIs),
                    VisibleInHelp = false
                },
                new HandlerMapping()
                {
                    ValidHandles = ExactMatchHandle.For("test"),
                    Description = "Checks if it is Beer O Clock",
                    EvaluatorFunc =
                        new Func<IncomingMessage, IValidHandle, IEnumerable<ResponseMessage>>(this.Booop),
                    VisibleInHelp = false
                },
            };
        }

        public IEnumerable<ResponseMessage> CheckWhatTimeItIs(IncomingMessage message, IValidHandle matchedHandle)
        {
            DateTime beerOclockTime = new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day, 16, 0, 0);

            string replyMessage = DateTime.Now > beerOclockTime
                                      ? "Its beer o clock go have a drink"
                                      : "Keep Working its not time yet";

            yield return message.ReplyToChannel(replyMessage, (Attachment) null);
        }

        public IEnumerable<ResponseMessage> Booop(IncomingMessage message, IValidHandle matchedHandle)
        {
            string replyMessage = "boopp im testing ";

            Attachment test = new Attachment
            {
                Text = "test text",
                Title = "Test Title",
                Color = "Red",
                ImageUrl =
                    "https://image.shutterstock.com/image-vector/cool-bear-illustration-tshirt-other-450w-766363144.jpg",
                ThumbUrl =
                    "https://image.shutterstock.com/image-vector/cool-bear-illustration-tshirt-other-450w-766363144.jpg",
                Fallback = "fall back string",
                AuthorName = "Author string",
                AttachmentFields = new List<AttachmentField>()
                {
                    new AttachmentField()
                    {
                        Title = "test title one",
                        Value = "test value one",
                        IsShort = false
                    },
                    new AttachmentField()
                    {
                        Title = "test title two",
                        Value = "test value two",
                        IsShort = true
                    }
                }
            };

            yield return message.ReplyToChannel(replyMessage, (Attachment) null);
        }
    }
}