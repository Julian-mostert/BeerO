using BeerO.SlackConnector.Connections.Sockets.Messages.Inbound.ReactionItem;
using Newtonsoft.Json;

namespace BeerO.SlackConnector.Connections.Sockets.Messages.Inbound
{
    internal class ReactionMessage : InboundMessage
    {
        public ReactionMessage()
        {
            this.MessageType = MessageType.Reaction_Added;
        }
        
        [JsonProperty("user")]
        public string User { get; set; }

        [JsonProperty("reaction")]
        public string Reaction { get; set; }
      
        [JsonProperty("event_ts")]
        public double Timestamp { get; set; }

        public IReactionItem ReactingTo { get; set; }

        [JsonProperty("item_user")]
        public string ReactingToUser { get; set; }
    }
}
