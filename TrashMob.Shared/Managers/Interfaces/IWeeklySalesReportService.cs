namespace TrashMob.Shared.Managers.Interfaces
{
    using System;
    using System.Threading;
    using System.Threading.Tasks;
    using TrashMob.Models.Poco.V2;

    /// <summary>
    /// Aggregates a single week of municipal sales pipeline activity into the
    /// <see cref="WeeklySalesReportDto"/> shape shown to the salesperson and
    /// emailed to Cynthia + the Board (Project 63).
    /// </summary>
    public interface IWeeklySalesReportService
    {
        /// <summary>
        /// Builds the weekly report for the seven-day window ending on
        /// <paramref name="weekEnding"/> (inclusive). The window is
        /// <c>weekEnding - 6 days</c> at 00:00 UTC through <c>weekEnding</c>
        /// at 23:59:59.999 UTC.
        /// </summary>
        Task<WeeklySalesReportDto> GenerateAsync(DateOnly weekEnding, CancellationToken cancellationToken = default);
    }
}
