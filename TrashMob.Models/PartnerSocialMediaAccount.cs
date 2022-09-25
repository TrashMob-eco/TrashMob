#nullable disable

using TrashMob;

namespace TrashMob.Models
{
    using System;

    public partial class PartnerSocialMediaAccount : KeyedModel
    {
        public Guid PartnerId { get; set; }

        public string AccountIdentifier { get; set; }

        public bool? IsActive { get; set; }

        public virtual Partner Partner { get; set; }

        public int SocialMediaAccountTypeId { get; set; }

        public virtual SocialMediaAccountType SocialMediaAccountType { get; set; }
    }
}
