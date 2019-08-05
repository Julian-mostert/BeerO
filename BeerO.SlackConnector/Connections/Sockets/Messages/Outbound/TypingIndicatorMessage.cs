using Newtonsoft.Json;

namespace BeerO.SlackConnector.Connections.Sockets.Messages.Outbound
{
    internal class TypingIndicatorMessage : BaseMessage
    {
        public TypingIndicatorMessage()
        {
            this.Type = "typing";
        }

        [JsonProperty("channel")]
        public string Channel { get; set; } 
    }
}