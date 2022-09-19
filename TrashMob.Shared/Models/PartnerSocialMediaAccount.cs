#nullable disable

namespace TrashMob.Shared.Models
{
    using System;

    public partial class PartnerSocialMediaAccount : BaseModel
    {
        public Guid PartnerId { get; set; }

        public Guid SocialMediaAccountId { get; set; }

        public bool? IsActive { get; set; }

        public virtual Partner Partner { get; set; }

        public virtual SocialMediaAccount SocialMediaAccount { get; set; }
    }
}
