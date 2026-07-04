#nullable enable

namespace TrashMob.Models.Poco.V2
{
    using System;

    /// <summary>
    /// V2 API representation of an active <see cref="UserRole"/> grant.
    /// </summary>
    /// <remarks>
    /// The admin UI reads a list of these when rendering the per-user roles page.
    /// Revoked grants are not surfaced; the audit log lives server-side.
    /// </remarks>
    public class UserRoleGrantDto
    {
        /// <summary>
        /// Gets or sets the grant identifier.
        /// </summary>
        public Guid Id { get; set; }

        /// <summary>
        /// Gets or sets the user this grant applies to.
        /// </summary>
        public Guid UserId { get; set; }

        /// <summary>
        /// Gets or sets the role identifier.
        /// </summary>
        public int RoleId { get; set; }

        /// <summary>
        /// Gets or sets the machine-friendly role name.
        /// </summary>
        public string RoleName { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets the human-readable role description.
        /// </summary>
        public string? RoleDescription { get; set; }

        /// <summary>
        /// Gets or sets the user id that granted this role.
        /// </summary>
        public Guid GrantedByUserId { get; set; }

        /// <summary>
        /// Gets or sets when the grant took effect.
        /// </summary>
        public DateTimeOffset GrantedDate { get; set; }

        /// <summary>
        /// Gets or sets the optional expiry date for time-boxed grants.
        /// </summary>
        public DateTimeOffset? ExpiryDate { get; set; }
    }

    /// <summary>
    /// Request body for granting a role to a user.
    /// </summary>
    public class GrantRoleRequest
    {
        /// <summary>
        /// Gets or sets the machine-friendly role name to grant (e.g. <c>SalesRep</c>).
        /// </summary>
        public string RoleName { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets the optional expiry date for a time-boxed grant
        /// (contractor trials, incident-response elevation).
        /// </summary>
        public DateTimeOffset? ExpiryDate { get; set; }
    }

    /// <summary>
    /// Request body for revoking a role from a user.
    /// </summary>
    public class RevokeRoleRequest
    {
        /// <summary>
        /// Gets or sets an optional free-text reason recorded on the audit row.
        /// </summary>
        public string? Reason { get; set; }
    }
}
