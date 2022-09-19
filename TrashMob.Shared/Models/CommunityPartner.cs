#nullable disable

namespace TrashMob.Shared.Models
{
    using System;

    public partial class CommunityPartner : BaseModel
    {
        public CommunityPartner()
        {
        }

        public Guid CommunityId { get; set; }

        public Guid PartnerId { get; set; }

        public Guid PartnerLocationId { get; set; }

        public virtual Partner Partner { get; set; }

        public virtual PartnerLocation PartnerLocation { get; set; }

        public virtual Community Community { get; set; }
    }
}
