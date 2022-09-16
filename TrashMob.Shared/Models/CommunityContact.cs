#nullable disable

namespace TrashMob.Shared.Models
{
    using System;

    public partial class CommunityContact : ExtendedBaseModel
    {
        public Guid CommunityId { get; set; }

        public int CommunityContactTypeId { get; set; }

        public string Name { get; set; }
        
        public string Email { get; set; }
        
        public string Phone { get; set; }

        public string Notes { get; set; }

        public bool? IsActive { get; set; }

        public virtual Community Community { get; set; }

        public virtual CommunityContactType CommunityContactType { get; set; }
    }
}
