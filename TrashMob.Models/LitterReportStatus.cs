#nullable disable

namespace TrashMob.Models
{
    public class LitterReportStatus : LookupModel
    {
        public LitterReportStatus()
        {
            LitterReports = new HashSet<LitterReport>();
        }

        public virtual ICollection<LitterReport> LitterReports { get; set; }
    }
}