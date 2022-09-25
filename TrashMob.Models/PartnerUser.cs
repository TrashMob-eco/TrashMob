#nullable disable

using TrashMob;

namespace TrashMob.Models
{
    using System;

    public partial class PartnerUser : BaseModel
    {
        public PartnerUser()
        {
        }

        public Guid PartnerId { get; set; }

        public Guid UserId { get; set; }

        public virtual Partner Partner { get; set; }

        public virtual User User { get; set; }
    }
}
