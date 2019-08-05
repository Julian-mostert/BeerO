namespace BeerO.SlackConnector.Connections.Clients.Constants
{
    internal static partial class Api
    {
        /// <summary>
        /// Archives a channel.
        /// </summary>
        internal const string ChannelsArchive = "channels.archive";

        /// <summary>
        /// Creates a channel.
        /// </summary>
        internal const string ChannelsCreate = "channels.create";

        /// <summary>
        /// Fetches history of messages and events from a channel.
        /// </summary>
        internal const string ChannelsHistory = "channels.history";

        /// <summary>
        /// Gets information about a channel.
        /// </summary>
        internal const string ChannelsInfo = "channels.info";

        /// <summary>
        /// Invites a user to a channel.
        /// </summary>
        internal const string ChannelsInvite = "channels.invite";

        /// <summary>
        /// Joins a channel, creating it if needed.
        /// </summary>
        internal const string ChannelsJoin = "channels.join";

        /// <summary>
        /// Removes a user from a channel.
        /// </summary>
        internal const string ChannelsKick = "channels.kick";

        /// <summary>
        /// Leaves a channel.
        /// </summary>
        internal const string ChannelsLeave = "channels.leave";

        /// <summary>
        /// Lists all channels in a Slack team.
        /// </summary>
        internal const string ChannelsList = "channels.list";

        /// <summary>
        /// Sets the read cursor in a channel.
        /// </summary>
        internal const string ChannelsMark = "channels.mark";

        /// <summary>
        /// Renames a channel.
        /// </summary>
        internal const string ChannelsRename = "channels.rename";

        /// <summary>
        /// Retrieve a thread of messages posted to a channel
        /// </summary>
        internal const string ChannelsReplies = "channels.replies";

        /// <summary>
        /// Sets the purpose for a channel.
        /// </summary>
        internal const string ChannelsSetPurpose = "channels.setPurpose";

        /// <summary>
        /// Sets the topic for a channel.
        /// </summary>
        internal const string ChannelsSetTopic = "channels.setTopic";

        /// <summary>
        /// Unarchives a channel.
        /// </summary>
        internal const string ChannelsUnarchive = "channels.unarchive";
    }
}