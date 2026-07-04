namespace TrashMob.Shared.Managers.Interfaces
{
    using System.Threading;
    using System.Threading.Tasks;
    using TrashMob.Models;

    /// <summary>
    /// Sends notification emails to users when their role assignments change
    /// (Project 64 Phase 3).
    /// </summary>
    public interface IRoleGrantNotificationService
    {
        /// <summary>
        /// Sends the "role granted" email to the affected user.
        /// </summary>
        /// <param name="grant">The newly-created (or just-updated) grant with the
        /// <see cref="UserRole.Role"/> navigation loaded.</param>
        /// <param name="recipient">The user receiving the role.</param>
        /// <param name="grantedBy">The admin who performed the grant, used for
        /// the "granted by" line in the email.</param>
        Task SendRoleGrantedAsync(UserRole grant, User recipient, User grantedBy, CancellationToken cancellationToken = default);

        /// <summary>
        /// Sends the "role revoked" email to the affected user.
        /// </summary>
        /// <param name="grant">The just-revoked grant with the <see cref="UserRole.Role"/>
        /// navigation loaded and revocation fields populated.</param>
        /// <param name="recipient">The user whose role was revoked.</param>
        /// <param name="revokedBy">The admin who performed the revocation.</param>
        Task SendRoleRevokedAsync(UserRole grant, User recipient, User revokedBy, CancellationToken cancellationToken = default);
    }
}
