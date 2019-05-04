using BeerO.SlackConnector.Connections.Models;

namespace BeerO.SlackConnector.Connections.Sockets.Messages.Inbound
{
    internal class DmChannelJoinedMessage : InboundMessage
    {
        public DmChannelJoinedMessage()
        {
            this.MessageType = MessageType.Im_Created;
        }

        public Im Channel { get; set; }
    }
}
