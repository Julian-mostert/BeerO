namespace BeerO.SlackCore.Constants
{
    internal static partial class Api
    {
        /// <summary>
        /// List conversations the calling user may access.
        /// </summary>
        internal const string UsersConversations = "users.conversations";

        /// <summary>
        /// Delete the user profile photo
        /// </summary>
        internal const string UsersDeletePhoto = "users.deletePhoto";

        /// <summary>
        /// Gets user presence information.
        /// </summary>
        internal const string UsersGetPresence = "users.getPresence";

        /// <summary>
        /// Get a user's identity.
        /// </summary>
        internal const string UsersIdentity = "users.identity";

        /// <summary>
        /// Gets information about a user.
        /// </summary>
        internal const string UsersInfo = "users.info";

        /// <summary>
        /// Lists all users in a Slack team.
        /// </summary>
        internal const string UsersList = "users.list";

        /// <summary>
        /// Find a user with an email address.
        /// </summary>
        internal const string UsersLookupByEmail = "users.lookupByEmail";

        /// <summary>
        /// Marked a user as active. Deprecated and non-functional.
        /// </summary>
        internal const string UsersSetActive = "users.setActive";

        /// <summary>
        /// Set the user profile photo
        /// </summary>
        internal const string UsersSetPhoto = "users.setPhoto";

        /// <summary>
        /// Manually sets user presence.
        /// </summary>
        internal const string UsersSetPresence = "users.setPresence";

        /// <summary>
        /// Retrieves a user's profile information.
        /// </summary>
        internal const string UsersProfileGet = "users.profile.get";

        /// <summary>
        /// Set the profile information for a user.
        /// </summary>
        internal const string UsersProfileSet = "users.profile.set";
    }
}