namespace TrashMob.Shared.Models
{
    using System;

    public class EventMedia : ExtendedBaseModel
    {
        public Guid EventId { get; set; }

        public string MediaUrl { get; set; }

        public int MediaTypeId { get; set; }

        public int MediaUsageTypeId { get; set; }

        public virtual MediaType MediaType { get; set; }

        public virtual MediaUsageType MediaUsageType { get; set; }

        public virtual Event Event { get; set; }
    }
}
