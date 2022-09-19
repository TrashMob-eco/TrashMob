#nullable disable

namespace TrashMob.Shared.Models
{
    using System;

    public partial class CommunitySocialMediaAccount : BaseModel
    {
        public Guid CommunityId { get; set; }

        public Guid SocialMediaAccountId { get; set; }

        public bool? IsActive { get; set; }

        public virtual Community Community { get; set; }

        public virtual SocialMediaAccount SocialMediaAccount { get; set; }
    }
}
