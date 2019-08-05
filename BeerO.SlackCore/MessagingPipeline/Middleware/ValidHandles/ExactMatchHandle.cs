using System;
using System.Linq;

namespace BeerO.SlackCore.MessagingPipeline.Middleware.ValidHandles
{
    public class ExactMatchHandle : IValidHandle
    {
        private readonly string _messageToMatch;

        public ExactMatchHandle(string messageToMatch)
        {
            this._messageToMatch = messageToMatch ?? string.Empty;
        }

        public bool IsMatch(string message)
        {
            return (message ?? string.Empty).Equals(this._messageToMatch, StringComparison.OrdinalIgnoreCase);
        }

        public string HandleHelpText => this._messageToMatch;

        public static IValidHandle[] For(params string[] messagesToMatch)
        {
            return messagesToMatch
                .Select(x => new ExactMatchHandle(x))
                .Cast<IValidHandle>()
                .ToArray();
        }
    }
}