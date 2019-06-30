namespace BeerO.SlackCore.Constants
{
    internal static partial class Api
    {
        /// <summary>
        /// Returns list of permissions this app has on a team.
        /// </summary>
        internal const string AppsPermissionsInfo = "apps.permissions.info";
        /// <summary>
        /// Allows an app to request additional scopes
        /// </summary>
        internal const string AppsPermissionsRequest = "apps.permissions.request";

        /// <summary>
        /// Returns list of resource grants this app has on a team.
        /// </summary>
        internal const string AppsPermissionsResourcesList = "apps.permissions.resources.list";

        /// <summary>
        /// Returns list of scopes this app has on a team.
        /// </summary>
        internal const string AppsPermissionsScopesList = "apps.permissions.scopes.list";
            
        //apps.permissions.users
        /// <summary>
        /// Returns list of user grants and corresponding scopes this app has on a team.
        /// </summary>
        internal const string AppsPermissionsUsersList = "apps.permissions.users.list";

        /// <summary>
        /// Enables an app to trigger a permissions modal to grant an app access to a user access scope.
        /// </summary>
        internal const string AppsPermissionsUsersRequest = "apps.permissions.users.request";
        
        /// <summary>
        /// Uninstalls your app from a workspace.
        /// </summary>
        internal const string AppsUninstall = "apps.uninstall";
            
        
    }
}
