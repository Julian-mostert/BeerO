using BeerO.SlackConnector.Connections.Models;

namespace BeerO.SlackConnector.Connections.Sockets.Messages.Inbound
{
    internal class ChannelJoinedMessage : InboundMessage
    {
        public ChannelJoinedMessage()
        {
            this.MessageType = MessageType.Channel_Joined;
        }

        public Channel Channel { get; set; }
    }
}
