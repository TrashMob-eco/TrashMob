namespace TrashMob.Shared.Managers.Interfaces
{
    using System;
    using System.Collections.Generic;
    using System.Threading;
    using System.Threading.Tasks;
    using TrashMob.Models;
    using TrashMob.Models.Poco;

    /// <summary>
    /// Defines operations for managing partner administrator invitations.
    /// </summary>
    public interface IPartnerAdminInvitationManager : IKeyedManager<PartnerAdminInvitation>
    {
        /// <summary>
        /// Gets the partner associated with a specific invitation.
        /// </summary>
        /// <param name="partnerAdminInvitationId">The unique identifier of the partner admin invitation.</param>
        /// <param name="cancellationToken">A token to cancel the asynchronous operation.</param>
        /// <returns>The partner associated with the invitation.</returns>
        Task<Partner> GetPartnerForInvitation(Guid partnerAdminInvitationId, CancellationToken cancellationToken);

        /// <summary>
        /// Gets all partner admin invitations for a specific user.
        /// </summary>
        /// <param name="userId">The unique identifier of the user.</param>
        /// <param name="cancellationToken">A token to cancel the asynchronous operation.</param>
        /// <returns>A collection of display partner admin invitations for the user.</returns>
        Task<IEnumerable<DisplayPartnerAdminInvitation>> GetInvitationsForUser(Guid userId,
            CancellationToken cancellationToken);

        /// <summary>
        /// Accepts a partner admin invitation for a user.
        /// </summary>
        /// <param name="partnerAdminInvitationId">The unique identifier of the invitation to accept.</param>
        /// <param name="UserId">The unique identifier of the user accepting the invitation.</param>
        /// <param name="cancellationToken">A token to cancel the asynchronous operation.</param>
        /// <returns>A task representing the asynchronous operation.</returns>
        Task AcceptInvitationAsync(Guid partnerAdminInvitationId, Guid UserId, CancellationToken cancellationToken);

        /// <summary>
        /// Declines a partner admin invitation for a user.
        /// </summary>
        /// <param name="partnerAdminInvitationId">The unique identifier of the invitation to decline.</param>
        /// <param name="UserId">The unique identifier of the user declining the invitation.</param>
        /// <param name="cancellationToken">A token to cancel the asynchronous operation.</param>
        /// <returns>A task representing the asynchronous operation.</returns>
        Task DeclineInvitationAsync(Guid partnerAdminInvitationId, Guid UserId, CancellationToken cancellationToken);

        /// <summary>
        /// Resends a partner admin invitation.
        /// </summary>
        /// <param name="partnerAdminInvitationId">The unique identifier of the invitation to resend.</param>
        /// <param name="UserId">The unique identifier of the user resending the invitation.</param>
        /// <param name="cancellationToken">A token to cancel the asynchronous operation.</param>
        /// <returns>The resent partner admin invitation.</returns>
        Task<PartnerAdminInvitation> ResendPartnerAdminInvitationAsync(Guid partnerAdminInvitationId, Guid UserId,
            CancellationToken cancellationToken);
    }
}
