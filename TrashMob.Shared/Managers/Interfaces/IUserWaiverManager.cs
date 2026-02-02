namespace TrashMob.Shared.Managers.Interfaces
{
    using System;
    using System.Collections.Generic;
    using System.Threading;
    using System.Threading.Tasks;
    using TrashMob.Models;
    using TrashMob.Shared.Poco;

    /// <summary>
    /// Manager interface for user waiver operations.
    /// </summary>
    public interface IUserWaiverManager : IKeyedManager<UserWaiver>
    {
        /// <summary>
        /// Gets waivers required for a user to participate in an event.
        /// </summary>
        /// <param name="userId">The user ID.</param>
        /// <param name="eventId">The event ID.</param>
        /// <param name="cancellationToken">Cancellation token.</param>
        /// <returns>Collection of waiver versions the user needs to sign.</returns>
        Task<IEnumerable<WaiverVersion>> GetRequiredWaiversForEventAsync(Guid userId, Guid eventId, CancellationToken cancellationToken = default);

        /// <summary>
        /// Gets pending waivers a user needs to sign (optionally for a specific community).
        /// </summary>
        /// <param name="userId">The user ID.</param>
        /// <param name="communityId">Optional community ID to filter community waivers.</param>
        /// <param name="cancellationToken">Cancellation token.</param>
        /// <returns>Collection of waiver versions the user needs to sign.</returns>
        Task<IEnumerable<WaiverVersion>> GetPendingWaiversForUserAsync(Guid userId, Guid? communityId = null, CancellationToken cancellationToken = default);

        /// <summary>
        /// Checks if a user has a valid waiver for an event.
        /// </summary>
        /// <param name="userId">The user ID.</param>
        /// <param name="eventId">The event ID.</param>
        /// <param name="cancellationToken">Cancellation token.</param>
        /// <returns>True if the user has valid waivers for the event.</returns>
        Task<bool> HasValidWaiverForEventAsync(Guid userId, Guid eventId, CancellationToken cancellationToken = default);

        /// <summary>
        /// Accepts a waiver and creates a UserWaiver record with full audit trail.
        /// </summary>
        /// <param name="request">The waiver acceptance request.</param>
        /// <param name="cancellationToken">Cancellation token.</param>
        /// <returns>Service result containing the created UserWaiver or error.</returns>
        Task<ServiceResult<UserWaiver>> AcceptWaiverAsync(AcceptWaiverRequest request, CancellationToken cancellationToken = default);

        /// <summary>
        /// Gets all waivers signed by a user.
        /// </summary>
        /// <param name="userId">The user ID.</param>
        /// <param name="cancellationToken">Cancellation token.</param>
        /// <returns>Collection of user waivers.</returns>
        Task<IEnumerable<UserWaiver>> GetUserWaiversAsync(Guid userId, CancellationToken cancellationToken = default);

        /// <summary>
        /// Gets users with waivers expiring within the specified number of days.
        /// </summary>
        /// <param name="daysUntilExpiry">Number of days until expiry to check.</param>
        /// <param name="cancellationToken">Cancellation token.</param>
        /// <returns>Collection of users with expiring waivers.</returns>
        Task<IEnumerable<User>> GetUsersWithExpiringWaiversAsync(int daysUntilExpiry, CancellationToken cancellationToken = default);

        /// <summary>
        /// Gets a user waiver by ID with waiver version details.
        /// </summary>
        /// <param name="userWaiverId">The user waiver ID.</param>
        /// <param name="cancellationToken">Cancellation token.</param>
        /// <returns>The user waiver with waiver version details.</returns>
        Task<UserWaiver> GetUserWaiverWithDetailsAsync(Guid userWaiverId, CancellationToken cancellationToken = default);
    }
}
