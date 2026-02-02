namespace TrashMob.Shared.Managers.Interfaces
{
    using System;
    using System.Collections.Generic;
    using System.Threading;
    using System.Threading.Tasks;
    using TrashMob.Models;

    /// <summary>
    /// Defines operations for managing waiver versions and community waiver assignments.
    /// </summary>
    public interface IWaiverVersionManager : IKeyedManager<WaiverVersion>
    {
        /// <summary>
        /// Gets all waiver versions.
        /// </summary>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns>A collection of all waiver versions.</returns>
        Task<IEnumerable<WaiverVersion>> GetAllAsync(CancellationToken cancellationToken = default);

        /// <summary>
        /// Gets all active waiver versions.
        /// </summary>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns>A collection of active waiver versions.</returns>
        Task<IEnumerable<WaiverVersion>> GetActiveWaiversAsync(CancellationToken cancellationToken = default);

        /// <summary>
        /// Gets the current global waiver (TrashMob platform waiver).
        /// </summary>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns>The current global waiver or null if none exists.</returns>
        Task<WaiverVersion> GetCurrentGlobalWaiverAsync(CancellationToken cancellationToken = default);

        /// <summary>
        /// Gets all waivers assigned to a community.
        /// </summary>
        /// <param name="communityId">The community (partner) ID.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns>A collection of waiver versions assigned to the community.</returns>
        Task<IEnumerable<WaiverVersion>> GetCommunityWaiversAsync(Guid communityId, CancellationToken cancellationToken = default);

        /// <summary>
        /// Assigns a waiver to a community.
        /// </summary>
        /// <param name="waiverId">The waiver version ID.</param>
        /// <param name="communityId">The community (partner) ID.</param>
        /// <param name="userId">The user making the assignment.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns>The created community waiver assignment.</returns>
        Task<CommunityWaiver> AssignToCommunityAsync(Guid waiverId, Guid communityId, Guid userId, CancellationToken cancellationToken = default);

        /// <summary>
        /// Removes a waiver assignment from a community.
        /// </summary>
        /// <param name="waiverId">The waiver version ID.</param>
        /// <param name="communityId">The community (partner) ID.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        Task RemoveFromCommunityAsync(Guid waiverId, Guid communityId, CancellationToken cancellationToken = default);

        /// <summary>
        /// Gets all community waiver assignments for a community.
        /// </summary>
        /// <param name="communityId">The community (partner) ID.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns>A collection of community waiver assignments.</returns>
        Task<IEnumerable<CommunityWaiver>> GetCommunityWaiverAssignmentsAsync(Guid communityId, CancellationToken cancellationToken = default);

        /// <summary>
        /// Deactivates a waiver version.
        /// </summary>
        /// <param name="waiverId">The waiver version ID.</param>
        /// <param name="userId">The user making the change.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        Task DeactivateAsync(Guid waiverId, Guid userId, CancellationToken cancellationToken = default);
    }
}
