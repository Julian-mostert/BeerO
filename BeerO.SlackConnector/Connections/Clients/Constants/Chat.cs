namespace BeerO.SlackConnector.Connections.Clients.Constants
{
    internal static partial class Api
    {
        /// <summary>
        /// Deletes a message.
        /// </summary>
        internal const string ChatDelete = "chat.delete";

        /// <summary>
        /// Deletes a pending scheduled message from the queue.
        /// </summary>
        internal const string ChatDeleteScheduledMessage = "chat.deleteScheduledMessage";

        /// <summary>
        /// Retrieve a permalink URL for a specific extant message
        /// </summary>
        internal const string ChatGetPermalink = "chat.getPermalink";

        /// <summary>
        /// Share a me message into a channel.
        /// </summary>
        internal const string ChatMeMessage = "chat.meMessage";

        /// <summary>
        /// Sends an ephemeral message to a user in a channel.
        /// </summary>
        internal const string ChatPostEphemeral = "chat.postEphemeral";

        /// <summary>
        /// Sends a message to a channel.
        /// </summary>
        internal const string ChatPostMessage = "chat.postMessage";

        /// <summary>
        /// Schedules a message to be sent to a channel.
        /// </summary>
        internal const string ChatScheduleMessage = "chat.scheduleMessage";

        /// <summary>
        /// Provide custom unfurl behavior for user-posted URLs
        /// </summary>
        internal const string ChatUnfurl = "chat.unfurl";

        /// <summary>
        /// Updates a message.
        /// </summary>
        internal const string ChatUpdate = "chat.update";

        /// <summary>
        /// Returns a list of scheduled messages.
        /// </summary>
        internal const string ChatScheduledMessagesList = "chat.scheduledMessages.list";
    }
}