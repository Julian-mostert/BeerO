using System.Collections.Generic;
using System.Linq;
using BeerO.SlackCore.MessagingPipeline.Middleware.ValidHandles;
using BeerO.SlackCore.MessagingPipeline.Request;
using BeerO.SlackCore.MessagingPipeline.Response;

namespace BeerO.SlackCore.MessagingPipeline.Middleware
{
    public abstract class MiddlewareBase : IMiddleware
    {
        protected HandlerMapping[] HandlerMappings;
        private readonly IMiddleware _next;

        protected MiddlewareBase(IMiddleware next)
        {
            this._next = next;
            this.HandlerMappings = this.HandlerMappings ?? new HandlerMapping[0];
        }

        protected internal IEnumerable<ResponseMessage> Next(IncomingMessage message)
        {
            return this._next?.Invoke(message) ?? new ResponseMessage[0];
        }

        public virtual IEnumerable<ResponseMessage> Invoke(IncomingMessage message)
        {
            foreach (var handlerMapping in this.HandlerMappings)
            {
                foreach (IValidHandle handle in handlerMapping.ValidHandles)
                {
                    string messageText = message.FullText;
                    if (handlerMapping.MessageShouldTargetBot)
                    {
                        messageText = message.TargetedText;
                    }
                    
                    if (handle.IsMatch(messageText))
                    {
                        foreach (var responseMessage in handlerMapping.EvaluatorFunc(message, handle))
                        {
                            yield return responseMessage;
                        }

                        if (!handlerMapping.ShouldContinueProcessing)
                        {
                            yield break;
                        }
                    }
                }
            }

            foreach (ResponseMessage responseMessage in this.Next(message))
            {
                yield return responseMessage;
            }
        }

        public IEnumerable<CommandDescription> GetSupportedCommands()
        {
            foreach (var handlerMapping in this.HandlerMappings)
            {
                if (!handlerMapping.VisibleInHelp)
                {
                    continue;
                }

                yield return new CommandDescription
                {
                    Command = string.Join(" | ", handlerMapping.ValidHandles.Select(x => $"`{x.HandleHelpText}`").OrderBy(x => x)),
                    Description = handlerMapping.Description
                };
            }

            foreach (var commandDescription in this._next.GetSupportedCommands())
            {
                yield return commandDescription;
            }
        }
    }
}