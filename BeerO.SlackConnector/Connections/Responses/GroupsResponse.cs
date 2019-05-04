using BeerO.SlackConnector.Connections.Models;

namespace BeerO.SlackConnector.Connections.Responses
{
    internal class GroupsResponse : StandardResponse
    {
         public Group[] Groups { get; set; }
    }
}