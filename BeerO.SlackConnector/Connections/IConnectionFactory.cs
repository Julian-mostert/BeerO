using System.Threading.Tasks;
using BeerO.SlackConnector.Connections.Clients.Channel;
using BeerO.SlackConnector.Connections.Clients.Chat;
using BeerO.SlackConnector.Connections.Clients.File;
using BeerO.SlackConnector.Connections.Clients.Handshake;
using BeerO.SlackConnector.Connections.Sockets;

namespace BeerO.SlackConnector.Connections
{
    internal interface IConnectionFactory
    {
        Task<IWebSocketClient> CreateWebSocketClient(string url, ProxySettings proxySettings);
        IHandshakeClient CreateHandshakeClient();
        IChatClient CreateChatClient();
        IFileClient CreateFileClient();
        IChannelClient CreateChannelClient();
    }
}