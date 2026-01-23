#nullable disable

namespace TrashMob.Models
{
    /// <summary>
    /// Represents the status of a litter report (e.g., New, Assigned, Cleaned, Cancelled).
    /// </summary>
    public class LitterReportStatus : LookupModel
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="LitterReportStatus"/> class.
        /// </summary>
        public LitterReportStatus()
        {
            LitterReports = new HashSet<LitterReport>();
        }

        /// <summary>
        /// Gets or sets the collection of litter reports with this status.
        /// </summary>
        public virtual ICollection<LitterReport> LitterReports { get; set; }
    }
}