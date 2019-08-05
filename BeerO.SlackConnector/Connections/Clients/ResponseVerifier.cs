using BeerO.SlackConnector.Connections.Responses;
using BeerO.SlackConnector.Exceptions;

namespace BeerO.SlackConnector.Connections.Clients
{
    internal class ResponseVerifier : IResponseVerifier
    {
        public void VerifyResponse(StandardResponse response)
        {
            if (!response.Ok)
            {
                throw new CommunicationException($"Error occured while posting message '{response.Error}'");
            }
        }
    }
}