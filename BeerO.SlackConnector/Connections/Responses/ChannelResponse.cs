using BeerO.SlackConnector.Connections.Models;

namespace BeerO.SlackConnector.Connections.Responses
{
    internal class ChannelResponse : StandardResponse
    {
        public Channel Channel { get; set; }
    }
}
