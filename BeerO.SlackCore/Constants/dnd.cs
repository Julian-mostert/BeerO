namespace BeerO.SlackCore.Constants
{
    internal static partial class Api
    {
        /// <summary>
        /// Ends the current user's Do Not Disturb session immediately.
        /// </summary>
        internal const string DndEndDnd = "dnd.endDnd";

        /// <summary>
        /// Ends the current user's snooze mode immediately.
        /// </summary>
        internal const string DndEndSnooze = "dnd.endSnooze";

        /// <summary>
        /// Retrieves a user's current Do Not Disturb status.
        /// </summary>
        internal const string DndInfo = "dnd.info";

        /// <summary>
        /// Turns on Do Not Disturb mode for the current user, or changes its duration.
        /// </summary>
        internal const string DndSetSnooze = "dnd.setSnooze";

        /// <summary>
        /// Retrieves the Do Not Disturb status for up to 50 users on a team.
        /// </summary>
        internal const string DndTeamInfo = "dnd.teamInfo";
    }
}