#nullable disable

namespace TrashMob.Shared.Models
{
    using System;

    public partial class CommunityStarterKitContact
    {
        public int Id { get; set; }

        public Guid CommunityStarterKitId { get; set; }

        public int CommunityContactTypeId { get; set; }

        public string Name { get; set; }
        
        public string Email { get; set; }
        
        public string PhoneNumber { get; set; }

        public string Url { get; set; }

        public string Notes { get; set; }

        public bool? IsActive { get; set; }

        public Guid CreatedByUserId { get; set; }

        public DateTimeOffset? CreatedDate { get; set; }

        public Guid LastUpdatedByUserId { get; set; }

        public DateTimeOffset? LastUpdatedDate { get; set; }

        public virtual CommunityStarterKit CommunityStarterKit { get; set; }

        public virtual CommunityContactType CommunityContactType { get; set; }

        public virtual User CreatedByUser { get; set; }

        public virtual User LastUpdatedByUser { get; set; }
    }
}
