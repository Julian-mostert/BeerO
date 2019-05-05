using BeerO.SlackConnector.Connections.Models;

namespace BeerO.SlackConnector.Connections.Sockets.Messages.Inbound
{
    internal class ChannelCreatedMessage : InboundMessage
    {
        public ChannelCreatedMessage()
        {
            this.MessageType = MessageType.ChannelCreated;
        }

        public Channel Channel { get; set; }
    }
}