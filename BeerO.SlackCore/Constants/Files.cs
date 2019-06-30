namespace BeerO.SlackCore.Constants
{
    internal static partial class Api
    {
        /// <summary>
        /// Deletes an existing comment on a file.
        /// </summary>
        internal const string FilesCommentsDelete = "files.comments.delete";

        /// <summary>
        /// Deletes a file.
        /// </summary>
        internal const string FilesDelete = "files.delete";

        /// <summary>
        /// Gets information about a team file.
        /// </summary>
        internal const string FilesInfo = "files.info";

        /// <summary>
        /// Lists & filters team files.
        /// </summary>
        internal const string FilesList = "files.list";

        /// <summary>
        /// Revokes public/external sharing access for a file
        /// </summary>
        internal const string FilesRevokePublicURL = "files.revokePublicURL";

        /// <summary>
        /// Enables a file for public/external sharing.
        /// </summary>
        internal const string FilesSharedPublicURL = "files.sharedPublicURL";

        /// <summary>
        /// Uploads or creates a file.
        /// </summary>
        internal const string FilesUpload = "files.upload";
    }
}