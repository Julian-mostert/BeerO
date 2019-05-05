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
        internal const string JoinDmPath = "/api/im.open";
        internal const string ChannelCreatePath = "/api/channels.create";
        internal const string ChannelJoinPath = "/api/channels.join";
        internal const string ChannelArchivePath = "/api/channels.archive";
        internal const string ChannelSetPurposePath = "/api/channels.setPurpose";
        internal const string ChannelSetTopicPath = "/api/channels.setTopic";
        internal const string ChannelsListPath = "/api/channels.list";
        internal const string GroupsListPath = "/api/groups.list";
        internal const string UsersListPath = "/api/users.list";
        
        public FlurlChannelClient(IResponseVerifier responseVerifier)
        {
            this._responseVerifier = responseVerifier;
        }

        public async Task<Models.Channel> JoinDirectMessageChannel(string slackKey, string user)
        {
            var response = await ClientConstants
                       .SlackApiHost
                       .AppendPathSegment(JoinDmPath)
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
                .AppendPathSegment(ChannelCreatePath)
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
                .AppendPathSegment(ChannelJoinPath)
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
                .AppendPathSegment(ChannelArchivePath)
                .SetQueryParam("token", slackKey)
                .SetQueryParam("channel", channelName)
                .GetJsonAsync<StandardResponse>();

            this._responseVerifier.VerifyResponse(response);
        }

        public async Task<string> SetPurpose(string slackKey, string channelName, string purpose)
        {
            var response = await ClientConstants
                .SlackApiHost
                .AppendPathSegment(ChannelSetPurposePath)
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
                .AppendPathSegment(ChannelSetTopicPath)
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
                       .AppendPathSegment(ChannelsListPath)
                       .SetQueryParam("token", slackKey)
                       .GetJsonAsync<ChannelsResponse>();

            this._responseVerifier.VerifyResponse(response);
            return response.Channels;
        }

        public async Task<Group[]> GetGroups(string slackKey)
        {
            var response = await ClientConstants
                       .SlackApiHost
                       .AppendPathSegment(GroupsListPath)
                       .SetQueryParam("token", slackKey)
                       .GetJsonAsync<GroupsResponse>();

            this._responseVerifier.VerifyResponse(response);
            return response.Groups;
        }

        public async Task<User[]> GetUsers(string slackKey)
        {
            var response = await ClientConstants
                       .SlackApiHost
                       .AppendPathSegment(UsersListPath)
                       .SetQueryParam("token", slackKey)
                       .SetQueryParam("presence", "1")
                       .GetJsonAsync<UsersResponse>();

            this._responseVerifier.VerifyResponse(response);
            return response.Members;
        }
    }
}