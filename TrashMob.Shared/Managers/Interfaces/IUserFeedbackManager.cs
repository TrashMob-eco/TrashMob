namespace TrashMob.Shared.Managers.Interfaces
{
    using System;
    using System.Collections.Generic;
    using System.Threading;
    using System.Threading.Tasks;
    using TrashMob.Models;

    /// <summary>
    /// Defines operations for managing user feedback submissions.
    /// </summary>
    public interface IUserFeedbackManager : IKeyedManager<UserFeedback>
    {
        /// <summary>
        /// Gets all feedback, optionally filtered by status.
        /// </summary>
        /// <param name="status">Optional status filter.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns>A list of user feedback items.</returns>
        Task<IEnumerable<UserFeedback>> GetByStatusAsync(string status = null, CancellationToken cancellationToken = default);

        /// <summary>
        /// Gets all feedback submitted by a specific user.
        /// </summary>
        /// <param name="userId">The user ID.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns>A list of user feedback items.</returns>
        Task<IEnumerable<UserFeedback>> GetByUserAsync(Guid userId, CancellationToken cancellationToken = default);

        /// <summary>
        /// Updates the status of a feedback item.
        /// </summary>
        /// <param name="id">The feedback ID.</param>
        /// <param name="status">The new status.</param>
        /// <param name="internalNotes">Optional internal notes.</param>
        /// <param name="reviewedByUserId">The ID of the admin reviewing.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns>The updated feedback item.</returns>
        Task<UserFeedback> UpdateStatusAsync(Guid id, string status, string internalNotes, Guid reviewedByUserId, CancellationToken cancellationToken = default);
    }
}
