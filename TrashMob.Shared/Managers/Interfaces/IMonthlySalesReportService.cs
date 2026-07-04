namespace TrashMob.Shared.Managers.Interfaces
{
    using System;
    using System.Collections.Generic;
    using System.Threading;
    using System.Threading.Tasks;
    using TrashMob.Models.Poco.V2;

    /// <summary>
    /// Aggregates a single calendar month of municipal sales pipeline activity
    /// against configurable per-metric targets, and layers on Market Intelligence
    /// Notes (best-responding departments, common objections, pricing feedback)
    /// derived from the same window (Project 63 Phase 3).
    /// </summary>
    public interface IMonthlySalesReportService
    {
        /// <summary>
        /// Builds the monthly report for the calendar month containing
        /// <paramref name="anyDateInMonth"/>. Targets fall back to Cynthia's
        /// baseline (20 / 20 / 15 / 10 / 3 / 2 / 1) when no <c>SalesMonthlyTarget</c>
        /// row exists yet for the month.
        /// </summary>
        Task<MonthlySalesReportDto> GenerateAsync(DateOnly anyDateInMonth, CancellationToken cancellationToken = default);

        /// <summary>
        /// Upserts targets for the given month. Any metric omitted from
        /// <paramref name="targets"/> is left untouched.
        /// </summary>
        Task UpdateTargetsAsync(
            DateOnly month,
            IReadOnlyCollection<MonthlyTargetUpdateDto> targets,
            Guid actingUserId,
            CancellationToken cancellationToken = default);
    }
}
