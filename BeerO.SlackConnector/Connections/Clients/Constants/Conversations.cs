namespace BeerO.SlackConnector.Connections.Clients.Constants
{
    internal static partial class Api
    {
        /// <summary>
        /// Archives a conversation.
        /// </summary>
        internal const string ConversationsArchive = "conversations.archive";

        /// <summary>
        /// Closes a direct message or multi-person direct message.
        /// </summary>
        internal const string ConversationsClose = "conversations.close";

        /// <summary>
        /// Initiates a public or private channel-based conversation
        /// </summary>
        internal const string ConversationsCreate = "conversations.create";

        /// <summary>
        /// Fetches a conversation's history of messages and events.
        /// </summary>
        internal const string ConversationsHistory = "conversations.history";

        /// <summary>
        /// Retrieve information about a conversation.
        /// </summary>
        internal const string ConversationsInfo = "conversations.info";

        /// <summary>
        /// Invites users to a channel.
        /// </summary>
        internal const string ConversationsInvite = "conversations.invite";

        /// <summary>
        /// Joins an existing conversation.
        /// </summary>
        internal const string ConversationsJoin = "conversations.join";

        /// <summary>
        /// Removes a user from a conversation.
        /// </summary>
        internal const string ConversationsKick = "conversations.kick";

        /// <summary>
        /// Leaves a conversation.
        /// </summary>
        internal const string ConversationsLLeave = "conversations.leave";

        /// <summary>
        /// Lists all channels in a Slack team.
        /// </summary>
        internal const string ConversationsList = "conversations.list";

        /// <summary>
        /// Retrieve members of a conversation.
        /// </summary>
        internal const string ConversationsMembers = "conversations.members";

        /// <summary>
        /// Opens or resumes a direct message or multi-person direct message.
        /// </summary>
        internal const string ConversationsOpen = "conversations.open";

        /// <summary>
        /// Renames a conversation.
        /// </summary>
        internal const string ConversationsRename = "conversations.rename";

        /// <summary>
        /// Retrieve a thread of messages posted to a conversation
        /// </summary>
        internal const string ConversationsReplies = "conversations.replies";

        /// <summary>
        /// Sets the purpose for a conversation.
        /// </summary>
        internal const string ConversationsSetPurpose = "conversations.setPurpos";

        /// <summary>
        /// Sets the topic for a conversation.
        /// </summary>
        internal const string ConversationsSetTopic = "conversations.setTopic";

        /// <summary>
        /// Reverses conversation archival.
        /// </summary>
        internal const string ConversationsUnarchive = "conversations.unarchive";
    }
}