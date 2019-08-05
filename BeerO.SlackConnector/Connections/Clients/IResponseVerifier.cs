using BeerO.SlackConnector.Connections.Responses;

namespace BeerO.SlackConnector.Connections.Clients
{
    internal interface IResponseVerifier
    {
        void VerifyResponse(StandardResponse response);
    }
}