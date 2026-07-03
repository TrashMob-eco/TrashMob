#nullable disable

namespace TrashMob.Models
{
    /// <summary>
    /// Represents a grant of a <see cref="Role"/> to a <see cref="User"/>.
    /// </summary>
    /// <remarks>
    /// A grant is considered active when <see cref="RevokedDate"/> is null
    /// and either <see cref="ExpiryDate"/> is null or in the future.
    /// Revocation is soft — the row is retained for audit and never deleted.
    ///
    /// The unique index on (<c>UserId</c>, <c>RoleId</c>) is filtered on
    /// <c>RevokedDate IS NULL</c>, so a user can be granted the same role
    /// again after a prior revocation without violating the constraint.
    /// </remarks>
    public class UserRole : KeyedModel
    {
        /// <summary>
        /// Gets or sets the user this grant applies to.
        /// </summary>
        public Guid UserId { get; set; }

        /// <summary>
        /// Gets or sets the role granted to the user.
        /// </summary>
        public int RoleId { get; set; }

        /// <summary>
        /// Gets or sets the identifier of the user who granted this role.
        /// </summary>
        public Guid GrantedByUserId { get; set; }

        /// <summary>
        /// Gets or sets the date and time when this grant took effect.
        /// </summary>
        public DateTimeOffset GrantedDate { get; set; }

        /// <summary>
        /// Gets or sets the optional expiry date for time-boxed grants
        /// (contractor trials, incident-response elevation). When set and
        /// in the past, the grant is treated as inactive without needing
        /// an explicit revocation.
        /// </summary>
        public DateTimeOffset? ExpiryDate { get; set; }

        /// <summary>
        /// Gets or sets the date the grant was revoked, or null if the
        /// grant has never been revoked.
        /// </summary>
        public DateTimeOffset? RevokedDate { get; set; }

        /// <summary>
        /// Gets or sets the identifier of the user who revoked this grant.
        /// </summary>
        public Guid? RevokedByUserId { get; set; }

        /// <summary>
        /// Gets or sets an optional free-text reason for revocation, for
        /// audit / operational context.
        /// </summary>
        public string RevokedReason { get; set; }

        /// <summary>
        /// Gets or sets the user this grant applies to.
        /// </summary>
        public virtual User User { get; set; }

        /// <summary>
        /// Gets or sets the role granted to the user.
        /// </summary>
        public virtual Role Role { get; set; }
    }
}
