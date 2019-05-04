using BeerO.SlackConnector.Serialising;
using Newtonsoft.Json;

namespace BeerO.SlackConnector.Connections.Sockets.Messages.Inbound
{
    internal class ChatMessage : InboundMessage
    {
        public ChatMessage()
        {
            this.MessageType = MessageType.Message;
        }

        [JsonProperty("subtype")]
        [JsonConverter(typeof(EnumConverter))]
        public MessageSubType MessageSubType { get; set; }

        public string Channel { get; set; }
        public string User { get; set; }
        public string Text { get; set; }
        public string Team { get; set; }
        public File[] Files { get; set; }

        [JsonProperty("ts")]
        public double Timestamp { get; set; }

    }
}
