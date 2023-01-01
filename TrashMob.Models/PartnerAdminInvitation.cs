#nullable disable

namespace TrashMob.Models
{
    using System;

    public partial class PartnerAdminInvitation : KeyedModel
    {
        public PartnerAdminInvitation()
        {
        }

        public Guid PartnerId { get; set; }

        public string Email { get; set; }

        public int InvitationStatusId { get; set; }

        public DateTimeOffset DateInvited { get; set; }

        public virtual Partner Partner { get; set; }

        public virtual InvitationStatus InvitationStatus { get; set; }
    }
}
