using Newtonsoft.Json;

namespace BeerO.SlackConnector.Connections.Sockets.Messages.Outbound
{
    internal abstract class BaseMessage
    {
        [JsonProperty("id")]
        public int Id { get; set; }

        [JsonProperty("type")]
        public string Type { get; set; }
    }
}