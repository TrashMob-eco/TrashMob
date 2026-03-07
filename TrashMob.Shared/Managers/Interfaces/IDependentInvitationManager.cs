namespace TrashMob.Shared.Managers.Interfaces
{
    using System;
    using System.Collections.Generic;
    using System.Threading;
    using System.Threading.Tasks;
    using TrashMob.Models;
    using TrashMob.Models.Poco;

    /// <summary>
    /// Manages the lifecycle of dependent invitations (inviting minors to create their own accounts).
    /// </summary>
    public interface IDependentInvitationManager : IKeyedManager<DependentInvitation>
    {
        /// <summary>
        /// Creates an invitation and sends the invite email to the minor.
        /// </summary>
        Task<DependentInvitation> CreateInvitationAsync(Guid dependentId, string email, Guid parentUserId, CancellationToken cancellationToken = default);

        /// <summary>
        /// Verifies a token and returns invite info for the public accept page.
        /// </summary>
        Task<DependentInvitationInfo> VerifyTokenAsync(string token, CancellationToken cancellationToken = default);

        /// <summary>
        /// Checks if a newly-created user's email matches a pending invitation and links accounts.
        /// Called from the auth handler after auto-creating a user.
        /// </summary>
        Task<bool> TryAcceptByEmailAsync(string email, User minorUser, CancellationToken cancellationToken = default);

        /// <summary>
        /// Gets all invitations for a specific dependent.
        /// </summary>
        Task<IEnumerable<DependentInvitation>> GetByDependentIdAsync(Guid dependentId, CancellationToken cancellationToken = default);

        /// <summary>
        /// Gets all invitations sent by a parent user.
        /// </summary>
        Task<IEnumerable<DependentInvitation>> GetByParentUserIdAsync(Guid parentUserId, CancellationToken cancellationToken = default);

        /// <summary>
        /// Cancels an existing invitation.
        /// </summary>
        Task CancelInvitationAsync(Guid invitationId, Guid userId, CancellationToken cancellationToken = default);

        /// <summary>
        /// Resends an invitation with a new token and email.
        /// </summary>
        Task<DependentInvitation> ResendInvitationAsync(Guid invitationId, Guid userId, CancellationToken cancellationToken = default);
    }
}
