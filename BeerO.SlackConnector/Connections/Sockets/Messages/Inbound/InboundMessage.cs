using BeerO.SlackConnector.Serialising;
using Newtonsoft.Json;

namespace BeerO.SlackConnector.Connections.Sockets.Messages.Inbound
{
    //TOOD: Turn into interface?
    internal abstract class InboundMessage
    {
        [JsonProperty("type")]
        [JsonConverter(typeof(EnumConverter))]
        public MessageType MessageType { get; set; }

        public string RawData { get; set; }
    }
}
