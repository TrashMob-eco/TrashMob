#nullable disable

namespace TrashMob.Models
{
    public class WeightUnit : LookupModel
    {
        public WeightUnit()
        {
            EventSummaries = new HashSet<EventSummary>();
        }

        public virtual ICollection<EventSummary> EventSummaries { get; set; }
    }
}