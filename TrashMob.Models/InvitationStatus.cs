#nullable disable

namespace TrashMob.Models
{
    /// <summary>
    /// Represents the status of an invitation (e.g., New, Sent, Accepted, Canceled, Declined).
    /// </summary>
    public class InvitationStatus : LookupModel
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="InvitationStatus"/> class.
        /// </summary>
        public InvitationStatus()
        {
            PartnerAdminInvitations = new HashSet<PartnerAdminInvitation>();
            DependentInvitations = new HashSet<DependentInvitation>();
        }

        /// <summary>
        /// Gets or sets the collection of partner admin invitations with this status.
        /// </summary>
        public virtual ICollection<PartnerAdminInvitation> PartnerAdminInvitations { get; set; }

        /// <summary>
        /// Gets or sets the collection of dependent invitations with this status.
        /// </summary>
        public virtual ICollection<DependentInvitation> DependentInvitations { get; set; }
    }
}