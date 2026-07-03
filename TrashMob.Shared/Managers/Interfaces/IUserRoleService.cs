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
    }
}
