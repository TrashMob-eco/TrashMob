#nullable disable

namespace TrashMob.Models
{
    using System;

    /// <summary>
    /// Persistent narrative sidecar for a weekly or monthly sales pipeline
    /// report (Project 63 Phase 4). One row per <c>(PeriodType, PeriodStart)</c>
    /// pair. Numeric aggregations still come from live queries; only the
    /// free-text sections that the salesperson types live here.
    /// </summary>
    public class SalesReport : KeyedModel
    {
        /// <summary>
        /// Gets or sets the reporting cadence (see <see cref="SalesReportPeriodTypeEnum"/>).
        /// </summary>
        public int PeriodType { get; set; }

        /// <summary>
        /// Gets or sets the first day of the reporting window (UTC). Weekly rows use
        /// the Monday of the window; monthly rows use the first day of the month.
        /// Stored as a <see cref="DateTime"/> for cleaner SQL Server DATE mapping.
        /// </summary>
        public DateTime PeriodStart { get; set; }

        /// <summary>
        /// Gets or sets the last day of the reporting window (UTC).
        /// </summary>
        public DateTime PeriodEnd { get; set; }

        /// <summary>
        /// Gets or sets the free-text "Next Steps" section shown on weekly reports.
        /// Persisted from the report screen; null on monthly rows.
        /// </summary>
        public string NextSteps { get; set; }

        /// <summary>
        /// Gets or sets the free-text "Recommended next-month priority" section
        /// shown on monthly reports. Null on weekly rows.
        /// </summary>
        public string NextMonthPriority { get; set; }

        /// <summary>
        /// Gets or sets the timestamp the scheduled email job dispatched this report.
        /// Null until sent. Populated in Phase 4b so the hourly job can skip
        /// duplicate sends.
        /// </summary>
        public DateTimeOffset? EmailSentDate { get; set; }
    }
}
