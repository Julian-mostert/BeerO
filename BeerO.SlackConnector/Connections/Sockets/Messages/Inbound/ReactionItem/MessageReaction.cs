using Newtonsoft.Json;

namespace BeerO.SlackConnector.Connections.Sockets.Messages.Inbound.ReactionItem
{
    internal class MessageReaction : IReactionItem
    {
        [JsonProperty("channel")]
        public string Channel { get; set; }
    }
}
