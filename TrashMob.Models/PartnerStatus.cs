#nullable disable

namespace TrashMob.Models
{
    /// <summary>
    /// Represents the status of a partner organization (e.g., Active, Inactive).
    /// </summary>
    public class PartnerStatus : LookupModel
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="PartnerStatus"/> class.
        /// </summary>
        public PartnerStatus()
        {
            Partners = new HashSet<Partner>();
        }

        /// <summary>
        /// Gets or sets the collection of partners with this status.
        /// </summary>
        public virtual ICollection<Partner> Partners { get; set; }
    }
}