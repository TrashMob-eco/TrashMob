#nullable disable

namespace TrashMob.Models
{
    /// <summary>
    /// Represents a report of litter that has been identified and may be addressed by an event.
    /// </summary>
    /// <remarks>
    /// A litter report is filed by a TrashMob.eco user to report areas where cleanup is needed.
    /// Each report contains 1-5 geo-tagged photos. The report's location is determined by the first image.
    /// Status transitions: New (default) → Assigned (linked to an event) → Cleaned (cleanup completed)
    /// or Canceled (deleted by user). Reports can be assigned to events so attendees can target nearby
    /// areas that need cleaning.
    /// </remarks>
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