using BeerO.SlackConnector.Connections.Models;

namespace BeerO.SlackConnector.Connections.Sockets.Messages.Inbound
{
    internal class UserJoinedMessage : InboundMessage
    {
        public UserJoinedMessage()
        {
            this.MessageType = MessageType.TeamJoin;
        }

        public User User { get; set; }
    }
}
