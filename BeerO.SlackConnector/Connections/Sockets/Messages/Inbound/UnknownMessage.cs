namespace BeerO.SlackConnector.Connections.Sockets.Messages.Inbound
{
    internal class UnknownMessage : InboundMessage
    {
        public UnknownMessage()
        {
            this.MessageType = MessageType.Unknown;
        }
    }
}