using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BeerO.SlackConnector.Connections.Responses;
using BeerO.SlackConnector.Models;
using Flurl;
using Flurl.Http;
using Newtonsoft.Json;

namespace BeerO.SlackConnector.Connections.Clients.Chat
{
    internal class FlurlChatClient : IChatClient
    {
        private readonly IResponseVerifier _responseVerifier;
        internal const string SEND_MESSAGE_PATH = "/api/chat.postMessage";

        public FlurlChatClient(IResponseVerifier responseVerifier)
        {
            this._responseVerifier = responseVerifier;
        }

        public async Task PostMessage(string slackKey, string channel, string text, IList<SlackAttachment> attachments)
        {
            var request = ClientConstants
                       .SlackApiHost
                       .AppendPathSegment(SEND_MESSAGE_PATH)
                       .SetQueryParam("token", slackKey)
                       .SetQueryParam("channel", channel)
                       .SetQueryParam("text", text)
                       .SetQueryParam("as_user", "true")
                       .SetQueryParam("link_names", "true");
            
            if (attachments != null && attachments.Any())
            {
                request.SetQueryParam("attachments", JsonConvert.SerializeObject(attachments));
            }

            var response = await request.GetJsonAsync<StandardResponse>();
            this._responseVerifier.VerifyResponse(response);
        }
    }
}