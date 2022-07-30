#nullable disable

namespace TrashMob.Shared.Models
{
    using System.Collections.Generic;

    public partial class CommunityContactType
    {
        public int Id { get; set; }
        
        public string Name { get; set; }
        
        public string Description { get; set; }
        
        public int? DisplayOrder { get; set; }
        
        public bool? IsActive { get; set; }

        public virtual ICollection<CommunityContact> CommunityContacts { get; set; }

        public virtual ICollection<CommunityStarterKitContact> CommunityStarterKitContacts { get; set; }
    }
}
