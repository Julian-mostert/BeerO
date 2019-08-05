using BeerO.SlackConnector.Connections.Models;

namespace BeerO.SlackConnector.Connections.Responses
{
    internal class JoinChannelResponse : StandardResponse
    {
        public Channel Channel { get; set; }
    }
}