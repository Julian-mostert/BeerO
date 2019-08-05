using BeerO.SlackConnector.Connections.Sockets.Messages.Inbound;
using BeerO.SlackConnector.Models;

namespace BeerO.SlackConnector.Extensions
{
    internal static class MessageSubTypeExtensions
    {
        public static SlackMessageSubType ToSlackMessageSubType(this MessageSubType subType)
        {
            return (SlackMessageSubType)subType;
        }
    }
}