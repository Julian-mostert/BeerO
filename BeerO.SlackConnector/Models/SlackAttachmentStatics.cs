using System.Collections.Generic;

namespace BeerO.SlackConnector.Models
{
    public partial class SlackAttachment
    {
        public const string MarkdownInPretext = "pretext";
        public const string MarkdownInText = "text";
        public const string MarkdownInFields = "fields";

        public static List<string> GetAllMarkdownInTypes()
        {
            return new List<string>
            {
                MarkdownInFields,
                MarkdownInPretext,
                MarkdownInText
            };
        }
    }
}