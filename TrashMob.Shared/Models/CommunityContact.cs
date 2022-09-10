#nullable disable

namespace TrashMob.Shared.Models
{
    using System;
    using System.Collections.Generic;

    public partial class CommunityContact
    {
        public int Id { get; set; }

        public Guid CommunityId { get; set; }

        public int CommunityContactTypeId { get; set; }

        public string Name { get; set; }
        
        public string Email { get; set; }
        
        public string PhoneNumber { get; set; }

        public string Notes { get; set; }

        public bool? IsActive { get; set; }

        public Guid CreatedByUserId { get; set; }

        public DateTimeOffset? CreatedDate { get; set; }

        public Guid LastUpdatedByUserId { get; set; }

        public DateTimeOffset? LastUpdatedDate { get; set; }

        public virtual Community Community { get; set; }

        public virtual CommunityContactType CommunityContactType { get; set; }

        public virtual User CreatedByUser { get; set; }

        public virtual User LastUpdatedByUser { get; set; }
    }
}
