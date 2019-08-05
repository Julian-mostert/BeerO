namespace BeerO.SlackConnector.Connections.Clients.Constants
{
    internal static partial class Api
    {
        /// <summary>
        /// Close a direct message channel.
        /// </summary>
        internal const string ImClose = "im.close";

        /// <summary>
        /// Fetches history of messages and events from direct message channel.
        /// </summary>
        internal const string ImHistory = "im.history";

        /// <summary>
        /// Lists direct message channels for the calling user.
        /// </summary>
        internal const string ImList = "im.list";

        /// <summary>
        /// Sets the read cursor in a direct message channel.
        /// </summary>
        internal const string ImMark = "im.mark";

        /// <summary>
        /// Opens a direct message channel.
        /// </summary>
        internal const string ImOpen = "im.open";

        /// <summary>
        /// Retrieve a thread of messages posted to a direct message conversation
        /// </summary>
        internal const string ImReplies = "im.replies";
    }
}