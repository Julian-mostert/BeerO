namespace BeerO.SlackCore.Constants
{
    internal static partial class Api
    {
        /// <summary>
        /// Closes a multiparty direct message channel.
        /// </summary>
        internal const string MpimClose = "mpim.close";

        /// <summary>
        /// Fetches history of messages and events from a multiparty direct message.
        /// </summary>
        internal const string MpiHistory = "mpim.history";

        /// <summary>
        /// Lists multiparty direct message channels for the calling user.
        /// </summary>
        internal const string MpimList = "mpim.list";

        /// <summary>
        /// Sets the read cursor in a multiparty direct message channel.
        /// </summary>
        internal const string MpimMark = "mpim.mark";

        /// <summary>
        /// This method opens a multiparty direct message.
        /// </summary>
        internal const string MpimOpen = "mpim.open";

        /// <summary>
        /// Retrieve a thread of messages posted to a direct message conversation from a multiparty direct message.
        /// </summary>
        internal const string MpimReplies = "mpim.replies";
    }
}