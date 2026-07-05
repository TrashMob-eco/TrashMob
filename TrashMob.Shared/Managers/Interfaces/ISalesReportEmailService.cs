namespace TrashMob.Shared.Managers.Interfaces
{
    using System.Threading;
    using System.Threading.Tasks;

    /// <summary>
    /// Sends the weekly and monthly municipal sales pipeline emails to the
    /// distribution list (Project 63 Phase 4b). Called by processors in
    /// <c>TrashMobDailyJobs</c>; each method decides internally whether
    /// today is a "send day" (Monday for weekly, day-1 for monthly) and
    /// whether the previous period's report already went out.
    /// </summary>
    /// <remarks>
    /// Cadence:
    /// <list type="bullet">
    /// <item>Weekly — fires on any Monday. Reports on the just-ended
    /// Mon–Sun window ending on the previous Sunday.</item>
    /// <item>Monthly — fires on the 1st of any month. Reports on the
    /// just-ended calendar month.</item>
    /// </list>
    /// The daily job container fires twice a day (00:00 and 12:00 UTC), so
    /// each method dedupes via <c>SalesReport.EmailSentDate</c> — the second
    /// firing on the same send-day is a no-op.
    ///
    /// Empty periods are skipped: if there was no activity in the window,
    /// no email is sent and no dedupe row is written.
    /// </remarks>
    public interface ISalesReportEmailService
    {
        /// <summary>
        /// Sends last week's report to weekly subscribers if today is Monday
        /// (UTC), the report has activity, and it has not already been sent.
        /// Returns the number of recipient emails dispatched (0 on any skip).
        /// </summary>
        Task<int> SendWeeklyReportIfDueAsync(CancellationToken cancellationToken = default);

        /// <summary>
        /// Sends last month's report to monthly subscribers if today is the
        /// 1st (UTC), the report has activity, and it has not already been
        /// sent. Returns the number of recipient emails dispatched.
        /// </summary>
        Task<int> SendMonthlyReportIfDueAsync(CancellationToken cancellationToken = default);
    }
}
