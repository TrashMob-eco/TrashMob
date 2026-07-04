namespace TrashMob.Shared.Managers.Interfaces
{
    using System;
    using System.Collections.Generic;
    using System.Threading;
    using System.Threading.Tasks;
    using TrashMob.Models;

    /// <summary>
    /// Read-side accessor for a user's active role membership.
    /// </summary>
    /// <remarks>
    /// Authorization handlers call <see cref="HasRoleAsync"/> instead of reading
    /// <see cref="User.IsSiteAdmin"/> directly. Grants and revocations flow through
    /// separate write endpoints; this service is read-only.
    ///
    /// A grant is considered active when its <c>RevokedDate</c> is null and
    /// either <c>ExpiryDate</c> is null or in the future.
    ///
    /// During the Project 64 migration window, the implementation includes a
    /// compatibility bridge: <c>HasRoleAsync(userId, "SiteAdmin")</c> returns
    /// true if the user has an active <c>SiteAdmin</c> grant OR the legacy
    /// <see cref="User.IsSiteAdmin"/> boolean is true. That bridge is removed
    /// when the boolean column is dropped in Phase 4.
    /// </remarks>
    public interface IUserRoleService
    {
        /// <summary>
        /// Returns true when the user has an active grant of the named role.
        /// Role names are compared case-insensitively.
        /// </summary>
        Task<bool> HasRoleAsync(Guid userId, string roleName, CancellationToken cancellationToken = default);

        /// <summary>
        /// Returns the names of every role the user is actively granted.
        /// </summary>
        Task<IReadOnlyCollection<string>> GetRoleNamesAsync(Guid userId, CancellationToken cancellationToken = default);

        /// <summary>
        /// Returns every user with an active grant of the named role. Used by
        /// notification jobs (e.g. photo moderation) that need to fan out to
        /// role members.
        /// </summary>
        Task<IReadOnlyCollection<User>> GetUsersInRoleAsync(string roleName, CancellationToken cancellationToken = default);

        /// <summary>
        /// Returns every seeded <see cref="Role"/>, ordered by <c>DisplayOrder</c>.
        /// </summary>
        Task<IReadOnlyCollection<Role>> ListRolesAsync(CancellationToken cancellationToken = default);

        /// <summary>
        /// Returns the active <see cref="UserRole"/> grants for the given user,
        /// with the <see cref="Role"/> navigation populated. Excludes revoked
        /// grants and expired grants.
        /// </summary>
        Task<IReadOnlyCollection<UserRole>> GetActiveGrantsForUserAsync(Guid userId, CancellationToken cancellationToken = default);

        /// <summary>
        /// Grants the named role to the given user. If the user already has an
        /// active grant of that role, returns the existing grant unchanged.
        /// </summary>
        /// <exception cref="System.InvalidOperationException">Thrown when the
        /// role name does not match any seeded role.</exception>
        Task<UserRole> GrantRoleAsync(Guid userId, string roleName, Guid grantedByUserId, DateTimeOffset? expiryDate, CancellationToken cancellationToken = default);

        /// <summary>
        /// Revokes an active grant of the named role for the given user.
        /// Soft-delete: the row is retained with <see cref="UserRole.RevokedDate"/>
        /// and <see cref="UserRole.RevokedByUserId"/> populated. Returns
        /// <c>null</c> if the user has no active grant of the role.
        /// </summary>
        Task<UserRole> RevokeRoleAsync(Guid userId, string roleName, Guid revokedByUserId, string revokedReason, CancellationToken cancellationToken = default);
    }
}
