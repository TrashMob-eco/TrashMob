#nullable disable

namespace TrashMob.Models
{
    public class PartnerAdmin : BaseModel
    {
        public Guid PartnerId { get; set; }

        public Guid UserId { get; set; }

        public virtual Partner Partner { get; set; }

        public virtual User User { get; set; }
    }
}