using BeerO.SlackConnector.Connections.Models;

namespace BeerO.SlackConnector.Connections.Sockets.Messages.Inbound
{
    internal class ChannelJoinedMessage : InboundMessage
    {
        public ChannelJoinedMessage()
        {
            this.MessageType = MessageType.ChannelJoined;
        }

        public Channel Channel { get; set; }
    }
}
