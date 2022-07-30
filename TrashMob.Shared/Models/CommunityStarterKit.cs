#nullable disable

namespace TrashMob.Shared.Models
{
    using System;
    using System.Collections.Generic;

    public partial class CommunityStarterKit
    {
        public int Id { get; set; }

        public int CommunityId { get; set; }

        public int StarterKitDeliveryTypeId { get; set; }

        public bool? IsActive { get; set; }

        public Guid CreatedByUserId { get; set; }

        public DateTimeOffset? CreatedDate { get; set; }

        public Guid LastUpdatedByUserId { get; set; }

        public DateTimeOffset? LastUpdatedDate { get; set; }

        public virtual Community Community { get; set; }

        public virtual User CreatedByUser { get; set; }

        public virtual User LastUpdatedByUser { get; set; }

        public virtual StarterKitDeliveryType StarterKitDeliveryType {get;set;}
    }
}
