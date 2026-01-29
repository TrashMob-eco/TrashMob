#nullable disable

namespace TrashMob.Models
{
    /// <summary>
    /// Represents the status of a partner request (e.g., Sent, Approved, Denied, Pending).
    /// </summary>
    public class PartnerRequestStatus : LookupModel
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="PartnerRequestStatus"/> class.
        /// </summary>
        public PartnerRequestStatus()
        {
            PartnerRequests = new HashSet<PartnerRequest>();
        }

        /// <summary>
        /// Gets or sets the collection of partner requests with this status.
        /// </summary>
        public virtual ICollection<PartnerRequest> PartnerRequests { get; set; }
    }
}