#nullable disable

namespace TrashMob.Shared.Models
{
    using System;

    public partial class CommunityUser
    {
        public CommunityUser()
        {
        }

        public Guid CommunityId { get; set; }
        
        public Guid UserId { get; set; }

        public Guid CreatedByUserId { get; set; }

        public DateTimeOffset? CreatedDate { get; set; }

        public Guid LastUpdatedByUserId { get; set; }

        public DateTimeOffset? LastUpdatedDate { get; set; }

        public virtual User CreatedByUser { get; set; }

        public virtual User LastUpdatedByUser { get; set; }

        public virtual Community Community { get; set; }

        public virtual User User { get; set; }
    }
}
