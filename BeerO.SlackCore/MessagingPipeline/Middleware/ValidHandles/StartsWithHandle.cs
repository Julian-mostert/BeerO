using System;
using System.Linq;

namespace BeerO.SlackCore.MessagingPipeline.Middleware.ValidHandles
{
    public class StartsWithHandle : IValidHandle
    {
        private readonly string _messageStartsWith;

        public StartsWithHandle(string messageStartsWith)
        {
            this._messageStartsWith = messageStartsWith ?? string.Empty;
        }

        public bool IsMatch(string message)
        {
            return (message ?? string.Empty).StartsWith(this._messageStartsWith, StringComparison.OrdinalIgnoreCase);
        }

        public string HandleHelpText => this._messageStartsWith;

        public static IValidHandle[] For(params string[] messagesStartsWith)
        {
            return messagesStartsWith
                .Select(x => new StartsWithHandle(x))
                .Cast<IValidHandle>()
                .ToArray();
        }
    }
}