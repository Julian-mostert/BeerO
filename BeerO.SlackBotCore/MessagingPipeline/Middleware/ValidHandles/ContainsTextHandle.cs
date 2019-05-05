using System;
using System.Linq;

namespace BeerO.SlackBotCore.MessagingPipeline.Middleware.ValidHandles
{
    public class ContainsTextHandle : IValidHandle
    {
        private readonly string _containsText;

        public ContainsTextHandle(string containsText)
        {
            this._containsText = containsText ?? string.Empty;
        }

        public bool IsMatch(string message)
        {
            return (message ?? string.Empty).IndexOf(this._containsText, StringComparison.OrdinalIgnoreCase) >= 0;
        }

        public string HandleHelpText => this._containsText;

        public static IValidHandle[] For(params string[] containsText)
        {
            return containsText
                .Select(x => new ContainsTextHandle(x))
                .Cast<IValidHandle>()
                .ToArray();
        }
    }
}