using System;
using Newtonsoft.Json;

namespace BeerO.SlackConnector.Connections.Sockets.Messages.Inbound
{
    internal class PongMessage : InboundMessage
    {
        public PongMessage()
        {
            this.MessageType = MessageType.Pong;
        }

        [JsonProperty("time")]
        public DateTime Timestamp { get; set; }
        [JsonProperty("reply_to")]
        public int ReplyTo { get; set; }
    }
}