using BeerO.SlackConnector.Connections.Models;

namespace BeerO.SlackConnector.Connections.Responses
{
    internal class ChannelsResponse : StandardResponse
    {
         public Channel[] Channels { get; set; }
    }
}