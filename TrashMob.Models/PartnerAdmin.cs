#nullable disable

namespace TrashMob.Models
{
    using System;

    public partial class PartnerAdmin : BaseModel
    {
        public PartnerAdmin()
        {
        }

        public Guid PartnerId { get; set; }

        public Guid UserId { get; set; }

        public virtual Partner Partner { get; set; }

        public virtual User User { get; set; }
    }
}
