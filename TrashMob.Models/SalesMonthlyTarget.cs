#nullable disable

namespace TrashMob.Models
{
    using System;

    /// <summary>
    /// Per-month, per-metric target for the municipal sales pipeline
    /// (Project 63 Phase 3). One row per <c>(Month, Metric)</c> pair;
    /// the salesperson (or Cynthia) edits these via the Monthly Report screen.
    /// </summary>
    public class SalesMonthlyTarget : KeyedModel
    {
        /// <summary>
        /// Gets or sets the first day of the reporting month (day = 1) in UTC.
        /// Uses <see cref="DateTime"/> instead of <c>DateOnly</c> for cleaner
        /// EF6-era mapping alongside the rest of the schema.
        /// </summary>
        public DateTime Month { get; set; }

        /// <summary>
        /// Gets or sets the tracked metric (see <see cref="SalesMetricEnum"/>).
        /// </summary>
        public int Metric { get; set; }

        /// <summary>
        /// Gets or sets the numeric target the salesperson is aiming for.
        /// </summary>
        public int Target { get; set; }

        /// <summary>
        /// Gets or sets an optional free-text note (e.g. "revised after Q1 pipeline review").
        /// </summary>
        public string Notes { get; set; }
    }
}
