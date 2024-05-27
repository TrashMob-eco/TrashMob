#nullable disable

namespace TrashMob.Models
{
    public class PartnerAdminInvitation : KeyedModel
    {
        public Guid PartnerId { get; set; }

        public string Email { get; set; }

        public int InvitationStatusId { get; set; }

        public DateTimeOffset DateInvited { get; set; }

        public virtual Partner Partner { get; set; }

        public virtual InvitationStatus InvitationStatus { get; set; }
    }
}