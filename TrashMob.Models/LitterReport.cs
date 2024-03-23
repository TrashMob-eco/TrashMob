#nullable disable

namespace TrashMob.Models
{
    public class LitterReport : KeyedModel
    {
        public LitterReport()
        {
            
        }

        public string Name { get; set; }

        public string Description { get; set; }

        public int LitterReportStatusId { get; set; }

        public virtual LitterReportStatus LitterReportStatus { get; set; }

        public virtual ICollection<LitterImage> LitterImages { get; set; }

        public virtual ICollection<EventLitterReport> EventLitterReports { get; set; }
    }
}