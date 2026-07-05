namespace TrashMobDailyJobs
{
    using System;
    using System.Threading.Tasks;
    using Microsoft.Extensions.Logging;
    using TrashMob.Shared.Managers.Interfaces;

    /// <summary>
    /// Daily-job processor for the monthly municipal sales pipeline email
    /// (Project 63 Phase 4b). The service itself decides whether today is
    /// the 1st (UTC) and skips otherwise.
    /// </summary>
    public class MonthlySalesReportEmailer(
        ISalesReportEmailService salesReportEmailService,
        ILogger<MonthlySalesReportEmailer> logger)
    {
        public async Task RunAsync()
        {
            logger.LogInformation("MonthlySalesReportEmailer started at: {Time}", DateTime.UtcNow);
            var sent = await salesReportEmailService.SendMonthlyReportIfDueAsync();
            logger.LogInformation(
                "MonthlySalesReportEmailer completed at: {Time}. Emails dispatched: {Count}",
                DateTime.UtcNow, sent);
        }
    }
}
