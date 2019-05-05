using BeerO.SlackConnector.Connections.Models;

namespace BeerO.SlackConnector.Connections.Sockets.Messages.Inbound
{
    internal class GroupJoinedMessage : InboundMessage
    {
        public GroupJoinedMessage()
        {
            this.MessageType = MessageType.GroupJoined;
        }

        public Group Channel { get; set; }
    }
}
