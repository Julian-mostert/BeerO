using System.Threading.Tasks;
using BeerO.SlackConnector.Connections.Clients;
using BeerO.SlackConnector.Connections.Clients.Channel;
using BeerO.SlackConnector.Connections.Clients.Chat;
using BeerO.SlackConnector.Connections.Clients.File;
using BeerO.SlackConnector.Connections.Clients.Handshake;
using BeerO.SlackConnector.Connections.Sockets;
using BeerO.SlackConnector.Connections.Sockets.Messages.Inbound;
using BeerO.SlackConnector.Logging;

namespace BeerO.SlackConnector.Connections
{
    internal class ConnectionFactory : IConnectionFactory
    {
        public async Task<IWebSocketClient> CreateWebSocketClient(string url, ProxySettings proxySettings)
        {
            var socket = new WebSocketClientLite(new MessageInterpreter(new Logger()));
            await socket.Connect(url);
            return socket;
        }

        public IHandshakeClient CreateHandshakeClient()
        {
            return new FlurlHandshakeClient(new ResponseVerifier());
        }

        public IChatClient CreateChatClient()
        {
            return new FlurlChatClient(new ResponseVerifier());
        }

        public IFileClient CreateFileClient()
        {
            return new FlurlFileClient(new ResponseVerifier());
        }

        public IChannelClient CreateChannelClient()
        {
            return new FlurlChannelClient(new ResponseVerifier());
        }
    }
}