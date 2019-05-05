using System.Linq;
using BeerO.SlackConnector.Connections.Models;
using BeerO.SlackConnector.Models;

namespace BeerO.SlackConnector.Extensions
{
    internal static class ImExtensions
    {
        public static SlackChatHub ToChatHub(this Im im, SlackUser[] users)
        {
            SlackUser user = users.FirstOrDefault(x => x.Id == im.User);
            return new SlackChatHub
            {
                Id = im.Id,
                Name = "@" + (user == null ? im.User : user.Name),
                Type = SlackChatHubType.Dm
            };
        }
    }
}
