using BeerO.SlackConnector.Connections.Models;

namespace BeerO.SlackConnector.Connections.Responses
{
    internal class UsersResponse : StandardResponse
    {
         public User[] Members { get; set; }
    }
}