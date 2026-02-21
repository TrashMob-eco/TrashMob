namespace TrashMob.Shared.Managers.Interfaces
{
    using System;
    using System.Collections.Generic;
    using System.Threading;
    using System.Threading.Tasks;
    using TrashMob.Models;
    using TrashMob.Shared.Poco;

    /// <summary>
    /// Defines operations for managing user achievements.
    /// </summary>
    public interface IAchievementManager
    {
        /// <summary>
        /// Gets all achievements with earned status for a specific user.
        /// </summary>
        /// <param name="userId">The user ID.</param>
        /// <param name="cancellationToken">A token to cancel the asynchronous operation.</param>
        /// <returns>The user's achievements response.</returns>
        Task<UserAchievementsResponse> GetUserAchievementsAsync(
            Guid userId,
            CancellationToken cancellationToken = default);

        /// <summary>
        /// Gets all available achievement types.
        /// </summary>
        /// <param name="cancellationToken">A token to cancel the asynchronous operation.</param>
        /// <returns>List of all active achievement types.</returns>
        Task<IEnumerable<AchievementType>> GetAchievementTypesAsync(
            CancellationToken cancellationToken = default);

        /// <summary>
        /// Gets unread (un-notified) achievements for a user.
        /// </summary>
        /// <param name="userId">The user ID.</param>
        /// <param name="cancellationToken">A token to cancel the asynchronous operation.</param>
        /// <returns>List of newly earned achievements that haven't been notified.</returns>
        Task<IEnumerable<NewAchievementNotification>> GetUnreadAchievementsAsync(
            Guid userId,
            CancellationToken cancellationToken = default);

        /// <summary>
        /// Marks achievements as notified/read for a user.
        /// </summary>
        /// <param name="userId">The user ID.</param>
        /// <param name="achievementTypeIds">The achievement type IDs to mark as read.</param>
        /// <param name="cancellationToken">A token to cancel the asynchronous operation.</param>
        Task MarkAchievementsAsReadAsync(
            Guid userId,
            IEnumerable<int> achievementTypeIds,
            CancellationToken cancellationToken = default);

        /// <summary>
        /// Awards an achievement to a user (used by the daily job processor).
        /// </summary>
        /// <param name="userId">The user ID.</param>
        /// <param name="achievementTypeId">The achievement type ID to award.</param>
        /// <param name="cancellationToken">A token to cancel the asynchronous operation.</param>
        /// <returns>True if the achievement was newly awarded, false if already earned.</returns>
        Task<bool> AwardAchievementAsync(
            Guid userId,
            int achievementTypeId,
            CancellationToken cancellationToken = default);
    }
}
