#nullable disable

namespace TrashMob.Models
{
    public class EventSummary : BaseModel
    {
        public Guid EventId { get; set; }

        public int NumberOfBuckets { get; set; }

        public int NumberOfBags { get; set; }

        public int DurationInMinutes { get; set; }

        public int ActualNumberOfAttendees { get; set; }

        public int PickedWeight { get; set; }

        public int PickedWeightUnitId { get; set; }

        public string Notes { get; set; }

        public virtual Event Event { get; set; }

        public virtual WeightUnit PickedWeightUnit { get; set; }
    }
}