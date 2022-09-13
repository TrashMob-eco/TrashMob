#nullable disable

namespace TrashMob.Shared.Models
{
    using System;

    public partial class CommunityUser : ExtendedBaseModel
    {
        public CommunityUser()
        {
        }

        public Guid CommunityId { get; set; }
        
        public Guid UserId { get; set; }

        public virtual Community Community { get; set; }

        public virtual User User { get; set; }
    }
}
