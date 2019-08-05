using BeerO.SlackConnector.Connections.Models;

namespace BeerO.SlackConnector.Connections.Sockets.Messages.Inbound
{
    internal class GroupJoinedMessage : InboundMessage
    {
        public GroupJoinedMessage()
        {
            this.MessageType = MessageType.Group_Joined;
        }

        public Group Channel { get; set; }
    }
}
