using System.Collections.Generic;

namespace BeerO.SlackConnector.Models
{
    public class BotMessage
    {
        public IList<SlackAttachment> Attachments { get; set; }
        public SlackChatHub ChatHub { get; set; }
        public string Text { get; set; }

        public BotMessage()
        {
            this.Attachments = new List<SlackAttachment>();
        }
    }
}