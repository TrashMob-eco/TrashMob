#nullable enable

namespace TrashMob.Models.Poco.V2
{
    using System;
    using System.Collections.Generic;

    /// <summary>
    /// Monthly municipal sales pipeline report (Project 63 Phase 3). Mirrors
    /// the layout of Cynthia's Monthly Goals worksheet — the salesperson
    /// consumes it on <c>/siteadmin/prospects/reports/monthly</c>.
    /// </summary>
    public class MonthlySalesReportDto
    {
        /// <summary>
        /// First day of the reporting month (UTC).
        /// </summary>
        public DateTimeOffset PeriodStart { get; set; }

        /// <summary>
        /// Last day of the reporting month (UTC, end-of-day).
        /// </summary>
        public DateTimeOffset PeriodEnd { get; set; }

        /// <summary>
        /// One row per metric — target vs. actual with a color-coded status.
        /// </summary>
        public List<MonthlySalesMetricDto> Metrics { get; set; } = [];

        /// <summary>
        /// Best-responding departments derived from prospects with a
        /// <c>ResponseReceived</c> activity in the month.
        /// </summary>
        public List<MarketIntelligenceCountDto> BestRespondingDepartments { get; set; } = [];

        /// <summary>
        /// Top objection / open-question snippets from prospects touched in
        /// the month, ordered by frequency.
        /// </summary>
        public List<MarketIntelligenceCountDto> CommonObjections { get; set; } = [];

        /// <summary>
        /// Top pricing / business-model feedback snippets from prospects
        /// touched in the month, ordered by frequency.
        /// </summary>
        public List<MarketIntelligenceCountDto> PricingFeedback { get; set; } = [];

        /// <summary>
        /// Free-text "Recommended next-month priority" section captured by
        /// the salesperson. Persisted via the Phase 4 <c>SalesReport</c>
        /// entity; null until Phase 4 ships.
        /// </summary>
        public string? NextMonthPriority { get; set; }
    }

    /// <summary>
    /// Row on the monthly report — one tracked metric with its target,
    /// actual, and a status label.
    /// </summary>
    public class MonthlySalesMetricDto
    {
        /// <summary>
        /// Gets or sets the <see cref="SalesMetricEnum"/> numeric value.
        /// </summary>
        public int Metric { get; set; }

        /// <summary>
        /// Gets or sets the machine-friendly metric name
        /// (e.g. <c>OutreachTouches</c>).
        /// </summary>
        public string MetricName { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets the human-readable metric label
        /// (e.g. <c>Outreach touches</c>).
        /// </summary>
        public string Label { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets the numeric target for the month.
        /// </summary>
        public int Target { get; set; }

        /// <summary>
        /// Gets or sets the observed actual for the month.
        /// </summary>
        public int Actual { get; set; }

        /// <summary>
        /// Gets or sets a status label: <c>Behind</c> (actual &lt; 70% of
        /// target), <c>OnTrack</c> (70–110%), <c>Exceeded</c> (&gt; 110%).
        /// When target = 0, status is <c>NoTarget</c>.
        /// </summary>
        public string Status { get; set; } = string.Empty;
    }

    /// <summary>
    /// A single Market Intelligence Notes entry: a label plus the number of
    /// prospects that expressed it.
    /// </summary>
    public class MarketIntelligenceCountDto
    {
        /// <summary>
        /// Gets or sets the aggregated label (department, objection, or
        /// pricing snippet).
        /// </summary>
        public string Label { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets the number of prospects mentioning it.
        /// </summary>
        public int Count { get; set; }
    }

    /// <summary>
    /// Request body for updating monthly targets. Any metric omitted is left
    /// untouched.
    /// </summary>
    public class UpdateMonthlyTargetsRequest
    {
        /// <summary>
        /// Gets or sets the per-metric target updates.
        /// </summary>
        public List<MonthlyTargetUpdateDto> Targets { get; set; } = [];
    }

    /// <summary>
    /// One target row in an <see cref="UpdateMonthlyTargetsRequest"/>.
    /// </summary>
    public class MonthlyTargetUpdateDto
    {
        /// <summary>
        /// Gets or sets the metric identifier (see <see cref="SalesMetricEnum"/>).
        /// </summary>
        public int Metric { get; set; }

        /// <summary>
        /// Gets or sets the new target value.
        /// </summary>
        public int Target { get; set; }
    }
}
