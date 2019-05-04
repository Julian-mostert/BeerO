using System.Threading.Tasks;
using BeerO.SlackConnector.Connections.Models;
using BeerO.SlackConnector.Connections.Responses;
using Flurl;
using Flurl.Http;

namespace BeerO.SlackConnector.Connections.Clients.Channel
{
    internal class FlurlChannelClient : IChannelClient
    {
        private readonly IResponseVerifier _responseVerifier;
        internal const string JOIN_DM_PATH = "/api/im.open";
        internal const string CHANNEL_CREATE_PATH = "/api/channels.create";
        internal const string CHANNEL_JOIN_PATH = "/api/channels.join";
        internal const string CHANNEL_ARCHIVE_PATH = "/api/channels.archive";
        internal const string CHANNEL_SET_PURPOSE_PATH = "/api/channels.setPurpose";
        internal const string CHANNEL_SET_TOPIC_PATH = "/api/channels.setTopic";
        internal const string CHANNELS_LIST_PATH = "/api/channels.list";
        internal const string GROUPS_LIST_PATH = "/api/groups.list";
        internal const string USERS_LIST_PATH = "/api/users.list";
        
        public FlurlChannelClient(IResponseVerifier responseVerifier)
        {
            this._responseVerifier = responseVerifier;
        }

        public async Task<Models.Channel> JoinDirectMessageChannel(string slackKey, string user)
        {
            var response = await ClientConstants
                       .SlackApiHost
                       .AppendPathSegment(JOIN_DM_PATH)
                       .SetQueryParam("token", slackKey)
                       .SetQueryParam("user", user)
                       .GetJsonAsync<JoinChannelResponse>();

            this._responseVerifier.VerifyResponse(response);
            return response.Channel;
        }

        public async Task<Models.Channel> CreateChannel(string slackKey, string channelName)
        {
            var response = await ClientConstants
                .SlackApiHost
                .AppendPathSegment(CHANNEL_CREATE_PATH)
                .SetQueryParam("token", slackKey)
                .SetQueryParam("name", channelName)
                .GetJsonAsync<ChannelResponse>();

            this._responseVerifier.VerifyResponse(response);
            return response.Channel;
        }

        public async Task<Models.Channel> JoinChannel(string slackKey, string channelName)
        {
            var response = await ClientConstants
                .SlackApiHost
                .AppendPathSegment(CHANNEL_JOIN_PATH)
                .SetQueryParam("token", slackKey)
                .SetQueryParam("name", channelName)
                .GetJsonAsync<ChannelResponse>();

            this._responseVerifier.VerifyResponse(response);
            return response.Channel;
        }

        public async Task ArchiveChannel(string slackKey, string channelName)
        {
            var response = await ClientConstants
                .SlackApiHost
                .AppendPathSegment(CHANNEL_ARCHIVE_PATH)
                .SetQueryParam("token", slackKey)
                .SetQueryParam("channel", channelName)
                .GetJsonAsync<StandardResponse>();

            this._responseVerifier.VerifyResponse(response);
        }

        public async Task<string> SetPurpose(string slackKey, string channelName, string purpose)
        {
            var response = await ClientConstants
                .SlackApiHost
                .AppendPathSegment(CHANNEL_SET_PURPOSE_PATH)
                .SetQueryParam("token", slackKey)
                .SetQueryParam("channel", channelName)
                .SetQueryParam("purpose", purpose)
                .GetJsonAsync<PurposeResponse>();

            this._responseVerifier.VerifyResponse(response);
            return response.Purpose;
        }

        public async Task<string> SetTopic(string slackKey, string channelName, string topic)
        {
            var response = await ClientConstants
                .SlackApiHost
                .AppendPathSegment(CHANNEL_SET_TOPIC_PATH)
                .SetQueryParam("token", slackKey)
                .SetQueryParam("channel", channelName)
                .SetQueryParam("topic", topic)
                .GetJsonAsync<TopicResponse>();

            this._responseVerifier.VerifyResponse(response);
            return response.Topic;
        }

        public async Task<Models.Channel[]> GetChannels(string slackKey)
        {
            var response = await ClientConstants
                       .SlackApiHost
                       .AppendPathSegment(CHANNELS_LIST_PATH)
                       .SetQueryParam("token", slackKey)
                       .GetJsonAsync<ChannelsResponse>();

            this._responseVerifier.VerifyResponse(response);
            return response.Channels;
        }

        public async Task<Group[]> GetGroups(string slackKey)
        {
            var response = await ClientConstants
                       .SlackApiHost
                       .AppendPathSegment(GROUPS_LIST_PATH)
                       .SetQueryParam("token", slackKey)
                       .GetJsonAsync<GroupsResponse>();

            this._responseVerifier.VerifyResponse(response);
            return response.Groups;
        }

        public async Task<User[]> GetUsers(string slackKey)
        {
            var response = await ClientConstants
                       .SlackApiHost
                       .AppendPathSegment(USERS_LIST_PATH)
                       .SetQueryParam("token", slackKey)
                       .SetQueryParam("presence", "1")
                       .GetJsonAsync<UsersResponse>();

            this._responseVerifier.VerifyResponse(response);
            return response.Members;
        }
    }
}