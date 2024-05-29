#nullable disable

namespace TrashMob.Models
{
    public class InvitationStatus : LookupModel
    {
        public InvitationStatus()
        {
            PartnerAdminInvitations = new HashSet<PartnerAdminInvitation>();
        }

        public virtual ICollection<PartnerAdminInvitation> PartnerAdminInvitations { get; set; }
    }
}