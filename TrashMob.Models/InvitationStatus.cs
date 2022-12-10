#nullable disable

namespace TrashMob.Models
{
    using System.Collections.Generic;

    public partial class InvitationStatus : LookupModel
    {
        public InvitationStatus()
        {
            PartnerAdminInvitations = new HashSet<PartnerAdminInvitation>();
        }

        public virtual ICollection<PartnerAdminInvitation> PartnerAdminInvitations { get; set; }
    }
}
