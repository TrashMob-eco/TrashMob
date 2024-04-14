#nullable disable

namespace TrashMob.Models
{
    public class LitterReport : KeyedModel
    {
        public LitterReport()
        {
            
        }

        public string Name { get; set; } = string.Empty;

        public string Description { get; set; } = string.Empty;

        public int LitterReportStatusId { get; set; }

        public virtual LitterReportStatus LitterReportStatus { get; set; }

        public virtual ICollection<LitterImage> LitterImages { get; set; }

        public virtual ICollection<EventLitterReport> EventLitterReports { get; set; }
    }
}