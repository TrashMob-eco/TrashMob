namespace TrashMob.Shared.Managers.Interfaces
{
    using System;
    using System.Collections.Generic;
    using System.Threading;
    using System.Threading.Tasks;
    using TrashMob.Models;

    /// <summary>
    /// Defines operations for managing bulk email invitations.
    /// </summary>
    public interface IEmailInviteManager : IKeyedManager<EmailInviteBatch>
    {
        /// <summary>
        /// Creates a new email invite batch and schedules the individual invites.
        /// </summary>
        /// <param name="emails">The list of email addresses to invite.</param>
        /// <param name="senderUserId">The user ID of the sender.</param>
        /// <param name="batchType">The type of batch (Admin, Community, Team, User).</param>
        /// <param name="communityId">Optional community ID for community invites.</param>
        /// <param name="teamId">Optional team ID for team invites.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns>The created batch with statistics.</returns>
        Task<EmailInviteBatch> CreateBatchAsync(
            IEnumerable<string> emails,
            Guid senderUserId,
            string batchType,
            Guid? communityId = null,
            Guid? teamId = null,
            CancellationToken cancellationToken = default);

        /// <summary>
        /// Processes a batch by sending emails for all pending invites.
        /// </summary>
        /// <param name="batchId">The batch ID to process.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns>The updated batch with send results.</returns>
        Task<EmailInviteBatch> ProcessBatchAsync(Guid batchId, CancellationToken cancellationToken = default);

        /// <summary>
        /// Gets all email invite batches for admin view.
        /// </summary>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns>A list of all batches with summary statistics.</returns>
        Task<IEnumerable<EmailInviteBatch>> GetAdminBatchesAsync(CancellationToken cancellationToken = default);

        /// <summary>
        /// Gets a batch with its individual invite details.
        /// </summary>
        /// <param name="batchId">The batch ID.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns>The batch with invites loaded.</returns>
        Task<EmailInviteBatch> GetBatchDetailsAsync(Guid batchId, CancellationToken cancellationToken = default);

        /// <summary>
        /// Gets email invite batches for a specific community.
        /// </summary>
        /// <param name="communityId">The community (partner) ID.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns>A list of batches for the community.</returns>
        Task<IEnumerable<EmailInviteBatch>> GetCommunityBatchesAsync(Guid communityId, CancellationToken cancellationToken = default);

        /// <summary>
        /// Gets email invite batches for a specific team.
        /// </summary>
        /// <param name="teamId">The team ID.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns>A list of batches for the team.</returns>
        Task<IEnumerable<EmailInviteBatch>> GetTeamBatchesAsync(Guid teamId, CancellationToken cancellationToken = default);
    }
}
