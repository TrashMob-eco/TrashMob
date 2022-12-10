#nullable disable

namespace TrashMob.Models
{
    using System;

    public partial class EventSummary : BaseModel
    {
        public EventSummary()
        {
        }

        public Guid EventId { get; set; }

        public int NumberOfBuckets { get; set; }

        public int NumberOfBags { get; set; }

        public int DurationInMinutes { get; set; }

        public int ActualNumberOfAttendees { get; set; }

        public string Notes { get; set; }

        public virtual Event Event { get; set; }
    }
}
