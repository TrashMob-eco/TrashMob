namespace TrashMob.Shared.Managers.Interfaces
{
    using System;
    using System.Collections.Generic;
    using System.Threading;
    using System.Threading.Tasks;
    using TrashMob.Models;

    /// <summary>
    /// Defines operations for managing non-event user notifications.
    /// </summary>
    public interface INonEventUserNotificationManager : IKeyedManager<NonEventUserNotification>
    {
        /// <summary>
        /// Gets non-event notifications for a specific user and notification type.
        /// </summary>
        /// <param name="userId">The unique identifier of the user.</param>
        /// <param name="userNotificationTypeId">The type of notification to retrieve.</param>
        /// <param name="cancellationToken">A token to cancel the asynchronous operation.</param>
        /// <returns>A collection of non-event user notifications matching the criteria.</returns>
        Task<IEnumerable<NonEventUserNotification>> GetByUserIdAsync(Guid userId, int userNotificationTypeId,
            CancellationToken cancellationToken = default);
    }
}
