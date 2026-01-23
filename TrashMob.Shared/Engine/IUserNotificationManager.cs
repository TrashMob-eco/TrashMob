namespace TrashMob.Shared.Engine
{
    using System.Threading.Tasks;

    /// <summary>
    /// Interface for managing and executing all user notification engines.
    /// </summary>
    public interface IUserNotificationManager
    {
        /// <summary>
        /// Executes all registered notification engines to generate and send notifications.
        /// </summary>
        /// <returns>A task representing the asynchronous operation.</returns>
        public Task RunAllNotifications();
    }
}