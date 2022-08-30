namespace TrashMobMobileApp.Models
{
    using System;

    public class EventSummary
    {
        public Guid EventId { get; set; }

        public int NumberOfBuckets { get; set; }

        public int NumberOfBags { get; set; }

        public int DurationInMinutes { get; set; }

        public int ActualNumberOfAttendees { get; set; }

        public string Notes { get; set; }

        public Guid CreatedByUserId { get; set; }

        public DateTimeOffset? CreatedDate { get; set; }

        public Guid LastUpdatedByUserId { get; set; }

        public DateTimeOffset? LastUpdatedDate { get; set; }
    }
}
