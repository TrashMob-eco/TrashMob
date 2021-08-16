namespace TrashMob.Shared.Models
{
    using System;

    public class EventMedia
    {
        public Guid Id { get; set; }

        public Guid EventId { get; set; }

        public Guid CreatedByUserId { get; set; }

        public DateTimeOffset? CreatedDate { get; set; }

        public DateTimeOffset? LastUpdatedDate { get; set; }

        public string MediaUrl { get; set; }

        public int MediaTypeId { get; set; }

        public int MediaUsageTypeId { get; set; }

        public virtual MediaType MediaType { get; set; }

        public virtual MediaUsageType MediaUsageType { get; set; }

        public virtual Event Event { get; set; }

        public virtual User CreatedByUser { get; set; }
    }
}
