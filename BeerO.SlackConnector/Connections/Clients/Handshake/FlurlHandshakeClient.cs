using System.Threading.Tasks;
using BeerO.SlackConnector.Connections.Responses;
using Flurl;
using Flurl.Http;

namespace BeerO.SlackConnector.Connections.Clients.Handshake
{
    internal class FlurlHandshakeClient : IHandshakeClient
    {
        private readonly IResponseVerifier _responseVerifier;
        internal const string HANDSHAKE_PATH = "/api/rtm.start";

        public FlurlHandshakeClient(IResponseVerifier responseVerifier)
        {
            this._responseVerifier = responseVerifier;
        }

        public async Task<HandshakeResponse> FirmShake(string slackKey)
        {
            var response = await ClientConstants
                       .SlackApiHost
                       .AppendPathSegment(HANDSHAKE_PATH)
                       .SetQueryParam("token", slackKey)
                       .GetJsonAsync<HandshakeResponse>();

            this._responseVerifier.VerifyResponse(response);
            return response;
        }
    }
}