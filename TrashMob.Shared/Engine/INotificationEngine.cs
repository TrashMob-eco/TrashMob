namespace TrashMob.Shared.Engine
{
    using System.Threading;
    using System.Threading.Tasks;

    /// <summary>
    /// Interface for notification engines that generate and send notifications to users.
    /// </summary>
    public interface INotificationEngine
    {
        /// <summary>
        /// Generates and sends notifications based on the engine's specific criteria.
        /// </summary>
        /// <param name="cancellationToken">Token to cancel the operation.</param>
        /// <returns>A task representing the asynchronous operation.</returns>
        public Task GenerateNotificationsAsync(CancellationToken cancellationToken = default);
    }
}