#nullable enable

namespace TrashMob.Shared.Managers.Interfaces
{
    using System;
    using System.Threading;
    using System.Threading.Tasks;
    using TrashMob.Models;

    /// <summary>
    /// Persists the free-text sections of the weekly and monthly sales
    /// pipeline reports (Project 63 Phase 4). Numeric aggregations remain in
    /// <c>IWeeklySalesReportService</c> / <c>IMonthlySalesReportService</c>
    /// and are computed live at read time.
    /// </summary>
    public interface ISalesReportNarrativeService
    {
        /// <summary>
        /// Returns the persisted <see cref="SalesReport"/> row for the given
        /// period, or null when the salesperson has not yet typed a narrative.
        /// </summary>
        Task<SalesReport?> GetAsync(
            SalesReportPeriodTypeEnum periodType,
            DateOnly periodStart,
            CancellationToken cancellationToken = default);

        /// <summary>
        /// Upserts the narrative sections for the given period. On weekly
        /// upserts <c>NextMonthPriority</c> is ignored; on monthly upserts
        /// <c>NextSteps</c> is ignored. Null values clear existing text.
        /// </summary>
        Task<SalesReport> UpsertAsync(
            SalesReportPeriodTypeEnum periodType,
            DateOnly periodStart,
            DateOnly periodEnd,
            string? nextSteps,
            string? nextMonthPriority,
            Guid actingUserId,
            CancellationToken cancellationToken = default);
    }
}
