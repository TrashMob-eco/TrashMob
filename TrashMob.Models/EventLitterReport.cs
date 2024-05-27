#nullable disable

namespace TrashMob.Models
{
    public class EventLitterReport : BaseModel
    {
        public Guid EventId { get; set; }

        public Guid LitterReportId { get; set; }

        public string Notes { get; set; }

        public virtual Event Event { get; set; }

        public virtual LitterReport LitterReport { get; set; }
    }
}