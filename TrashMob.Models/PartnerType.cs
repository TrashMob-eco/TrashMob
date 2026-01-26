#nullable disable

namespace TrashMob.Models
{
    /// <summary>
    /// Represents the type of partner organization (e.g., Government, Business).
    /// </summary>
    public class PartnerType : LookupModel
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="PartnerType"/> class.
        /// </summary>
        public PartnerType()
        {
            PartnerRequests = new HashSet<PartnerRequest>();
            Partners = new HashSet<Partner>();
        }

        /// <summary>
        /// Gets or sets the collection of partners of this type.
        /// </summary>
        public virtual ICollection<Partner> Partners { get; set; }

        /// <summary>
        /// Gets or sets the collection of partner requests of this type.
        /// </summary>
        public virtual ICollection<PartnerRequest> PartnerRequests { get; set; }
    }
}