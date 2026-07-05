#nullable enable

namespace TrashMob.Models.Poco.V2
{
    /// <summary>
    /// Request body for upserting a weekly or monthly sales report narrative
    /// (Project 63 Phase 4).
    /// </summary>
    public class SalesReportNarrativeDto
    {
        /// <summary>
        /// Gets or sets the free-text "Next Steps" section shown on weekly
        /// reports. Ignored on monthly upserts.
        /// </summary>
        public string? NextSteps { get; set; }

        /// <summary>
        /// Gets or sets the free-text "Recommended next-month priority"
        /// section shown on monthly reports. Ignored on weekly upserts.
        /// </summary>
        public string? NextMonthPriority { get; set; }
    }
}
