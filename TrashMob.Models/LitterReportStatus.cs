#nullable disable

namespace TrashMob.Models
{
    using System.Collections.Generic;

    public class LitterReportStatus : LookupModel
    {
        public LitterReportStatus()
        {
            LitterReports = new HashSet<LitterReport>();
        }

        public virtual ICollection<LitterReport> LitterReports{get; set;}
    }
}