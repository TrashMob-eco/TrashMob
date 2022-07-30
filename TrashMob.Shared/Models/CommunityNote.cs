#nullable disable

namespace TrashMob.Shared.Models
{
    using System;

    public partial class CommunityNote
    {
        public int Id { get; set; }

        public Guid CommunityId { get; set; }

        public string Notes { get; set; }

        public Guid CreatedByUserId { get; set; }

        public DateTimeOffset? CreatedDate { get; set; }

        public Guid LastUpdatedByUserId { get; set; }

        public DateTimeOffset? LastUpdatedDate { get; set; }

        public virtual Community Community { get; set; }

        public virtual User CreatedByUser { get; set; }

        public virtual User LastUpdatedByUser { get; set; }
    }
}
