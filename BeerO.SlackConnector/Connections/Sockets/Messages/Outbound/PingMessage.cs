using System;

namespace BeerO.SlackConnector.Connections.Sockets.Messages.Outbound
{
    internal class PingMessage : BaseMessage
    {
        public DateTime Timestamp { get; } = DateTime.Now;

        public PingMessage()
        {
            this.Type = "ping";
        }
    }
}
