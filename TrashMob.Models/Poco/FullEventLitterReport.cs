namespace TrashMob.Models.Poco
{
    /// <summary>
    /// Represents an event-litter report association with full report details.
    /// </summary>
    public class FullEventLitterReport
    {
        /// <summary>
        /// Gets or sets the identifier of the event.
        /// </summary>
        public Guid EventId { get; set; }

        /// <summary>
        /// Gets or sets the identifier of the litter report.
        /// </summary>
        public Guid LitterReportId { get; set; }

        /// <summary>
        /// Gets or sets the full litter report details.
        /// </summary>
        public required FullLitterReport LitterReport { get; set; }
    }
}