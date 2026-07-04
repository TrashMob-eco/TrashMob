#nullable enable

namespace TrashMob.Models.Poco.V2
{
    /// <summary>
    /// V2 API representation of a <see cref="Role"/>.
    /// </summary>
    /// <remarks>
    /// Roles are seeded from code — the API exposes them read-only. Grants and
    /// revocations flow through the per-user <c>/users/{userId}/roles</c> endpoints.
    /// </remarks>
    public class RoleDto
    {
        /// <summary>
        /// Gets or sets the role identifier.
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// Gets or sets the machine-friendly role name (e.g. <c>SiteAdmin</c>,
        /// <c>SalesRep</c>). Used in authorization policy checks.
        /// </summary>
        public string Name { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets the human-readable description shown in the admin UI.
        /// </summary>
        public string? Description { get; set; }

        /// <summary>
        /// Gets or sets the display order used to sort roles in the UI.
        /// </summary>
        public int? DisplayOrder { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether the role is active. Inactive
        /// roles are still honored on existing grants but hidden from the grant UI.
        /// </summary>
        public bool? IsActive { get; set; }

        /// <summary>
        /// Gets or sets the number of users who currently hold an active grant of
        /// this role. Populated on list responses; ignored on write.
        /// </summary>
        public int MemberCount { get; set; }
    }
}
