namespace TrashMobDailyJobs
{
    using System;
    using System.Threading.Tasks;
    using Microsoft.Extensions.Logging;
    using TrashMob.Shared.Managers.Interfaces;

    /// <summary>
    /// Daily-job processor for the weekly municipal sales pipeline email
    /// (Project 63 Phase 4b). The service itself decides whether today is a
    /// send day (Monday UTC) and skips otherwise, so this class is a thin
    /// shell that fits the daily-job dispatch pattern.
    /// </summary>
    public class WeeklySalesReportEmailer(
        ISalesReportEmailService salesReportEmailService,
        ILogger<WeeklySalesReportEmailer> logger)
    {
        public async Task RunAsync()
        {
            logger.LogInformation("WeeklySalesReportEmailer started at: {Time}", DateTime.UtcNow);
            var sent = await salesReportEmailService.SendWeeklyReportIfDueAsync();
            logger.LogInformation(
                "WeeklySalesReportEmailer completed at: {Time}. Emails dispatched: {Count}",
                DateTime.UtcNow, sent);
        }
    }
}
