using System.Threading.Tasks;
using BeerO.SlackConnector.Connections.Responses;

namespace BeerO.SlackConnector.Connections.Clients.Handshake
{
    internal interface IHandshakeClient
    {
        /// <summary>
        /// No one likes a limp shake - AMIRITE?
        /// </summary>
        Task<HandshakeResponse> FirmShake(string slackKey);
    }
}