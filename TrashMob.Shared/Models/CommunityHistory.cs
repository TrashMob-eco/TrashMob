#nullable disable

namespace TrashMob.Shared.Models
{
    using System;

    public partial class CommunityHistory
    {
        public CommunityHistory()
        {
        }

        public int Id { get; set; }

        public Guid CommunityId { get; set; }

        public Guid RouteToUserId { get; set; }

        public string Notes { get; set; }

        public bool? IsActive { get; set; }

        public Guid CreatedByUserId { get; set; }

        public DateTimeOffset? CreatedDate { get; set; }

        public Guid LastUpdatedByUserId { get; set; }

        public DateTimeOffset? LastUpdatedDate { get; set; }

        public virtual Community Community { get; set; }

        public virtual User RouteToUser { get; set; }

        public virtual User CreatedByUser { get; set; }

        public virtual User LastUpdatedByUser { get; set; }
    }
}
