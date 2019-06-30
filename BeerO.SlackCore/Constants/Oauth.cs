namespace BeerO.SlackCore.Constants
{
    internal static partial class Api
    {
        /// <summary>
        /// Exchanges a temporary OAuth verifier code for an access token.
        /// </summary>
        internal const string OauthAccess = "oauth.access";

        /// <summary>
        /// Exchanges a temporary OAuth verifier code for a workspace token.
        /// </summary>
        internal const string OauthToken = "oauth.token";
    }
}