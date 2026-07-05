#nullable disable

namespace TrashMob.Models
{
    /// <summary>
    /// Represents an authorization role that can be granted to users.
    /// </summary>
    /// <remarks>
    /// Roles are coarse-grained capability bundles (e.g. <c>SiteAdmin</c>,
    /// <c>SalesRep</c>). Users are assigned roles via <see cref="UserRole"/>
    /// records. Authorization policies check for role membership through
    /// <c>IUserRoleService.HasRoleAsync</c> rather than reading the legacy
    /// <see cref="User.IsSiteAdmin"/> boolean directly.
    ///
    /// New roles are added by seeding a row and adding a corresponding
    /// authorization handler / policy — never by user action from the UI.
    /// The admin UI can only grant / revoke existing roles.
    /// </remarks>
    public class Role : LookupModel
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="Role"/> class.
        /// </summary>
        public Role()
        {
            UserRoles = [];
        }

        /// <summary>
        /// Gets or sets the collection of assignments of this role to users.
        /// </summary>
        public virtual ICollection<UserRole> UserRoles { get; set; }
    }
}
