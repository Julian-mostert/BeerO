namespace BeerO.SlackConnector.Connections.Clients.Constants
{
    internal static partial class Api
    {
        /// <summary>
        /// Creates a reminder.
        /// </summary>
        internal const string RemindersAdd = "reminders.add";

        /// <summary>
        /// Marks a reminder as complete.
        /// </summary>
        internal const string RemindersComplete = "reminders.complete";

        /// <summary>
        /// Deletes a reminder.
        /// </summary>
        internal const string RemindersDelete = "reminders.delete";

        /// <summary>
        /// Gets information about a reminder.
        /// </summary>
        internal const string RemindersInfo = "reminders.info";

        /// <summary>
        /// Lists all reminders created by or for a given user.
        /// </summary>
        internal const string RemindersList = "reminders.list";
    }
}