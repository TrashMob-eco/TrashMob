#nullable disable

namespace TrashMob.Models
{
    /// <summary>
    /// Represents a report of litter that has been identified and may be addressed by an event.
    /// </summary>
    public class LitterReport : KeyedModel
    {
        /// <summary>
        /// Gets or sets the name or title of the litter report.
        /// </summary>
        public string Name { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets the description of the litter report.
        /// </summary>
        public string Description { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets the identifier of the litter report status.
        /// </summary>
        public int LitterReportStatusId { get; set; }

        /// <summary>
        /// Gets or sets the status of the litter report.
        /// </summary>
        public virtual LitterReportStatus LitterReportStatus { get; set; }

        /// <summary>
        /// Gets or sets the collection of images associated with this litter report.
        /// </summary>
        public virtual ICollection<LitterImage> LitterImages { get; set; }

        /// <summary>
        /// Gets or sets the collection of events associated with this litter report.
        /// </summary>
        public virtual ICollection<EventLitterReport> EventLitterReports { get; set; }
    }
}