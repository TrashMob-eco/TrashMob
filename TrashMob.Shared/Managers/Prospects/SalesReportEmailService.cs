#nullable enable

namespace TrashMob.Shared.Managers.Prospects
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading;
    using System.Threading.Tasks;
    using Microsoft.EntityFrameworkCore;
    using Microsoft.Extensions.Logging;
    using TrashMob.Models;
    using TrashMob.Models.Poco.V2;
    using TrashMob.Shared.Engine;
    using TrashMob.Shared.Managers.Interfaces;
    using TrashMob.Shared.Persistence;
    using TrashMob.Shared.Poco;

    /// <summary>
    /// Dispatches the weekly and monthly sales pipeline emails on send-days
    /// (Project 63 Phase 4b). See <see cref="ISalesReportEmailService"/> for
    /// the cadence contract.
    /// </summary>
    public class SalesReportEmailService(
        MobDbContext db,
        IWeeklySalesReportService weeklyReportService,
        IMonthlySalesReportService monthlyReportService,
        ISalesReportSubscriberService subscriberService,
        IEmailManager emailManager,
        ILogger<SalesReportEmailService> logger,
        TimeProvider? timeProvider = null) : ISalesReportEmailService
    {
        private readonly TimeProvider clock = timeProvider ?? TimeProvider.System;

        /// <inheritdoc />
        public async Task<int> SendWeeklyReportIfDueAsync(CancellationToken cancellationToken = default)
        {
            var todayUtc = DateOnly.FromDateTime(clock.GetUtcNow().UtcDateTime);
            if (todayUtc.DayOfWeek != DayOfWeek.Monday)
            {
                return 0;
            }

            // Just-ended week: Sunday one day ago; the isoWeek that ends there.
            var weekEnding = todayUtc.AddDays(-1);
            var weekStart = weekEnding.AddDays(-6);

            var alreadySent = await IsAlreadySentAsync(
                SalesReportPeriodTypeEnum.Weekly, weekStart, cancellationToken);
            if (alreadySent)
            {
                logger.LogInformation(
                    "Weekly sales report for week ending {WeekEnding} already emailed; skipping.",
                    weekEnding);
                return 0;
            }

            var report = await weeklyReportService.GenerateAsync(weekEnding, cancellationToken);
            if (IsEmpty(report))
            {
                logger.LogInformation(
                    "Weekly sales report for week ending {WeekEnding} has no activity; skipping.",
                    weekEnding);
                return 0;
            }

            var subscribers = await subscriberService.GetForCadenceAsync(
                SalesReportPeriodTypeEnum.Weekly, cancellationToken);
            if (subscribers.Count == 0)
            {
                logger.LogInformation(
                    "Weekly sales report for week ending {WeekEnding} has 0 subscribers; skipping.",
                    weekEnding);
                return 0;
            }

            var subject = $"TrashMob municipal sales pipeline — week of {weekStart:MMM d}–{weekEnding:MMM d, yyyy}";
            var html = BuildWeeklyHtml(report, weekStart, weekEnding);
            var count = await DispatchAsync(subject, html, subscribers, cancellationToken);

            await MarkSentAsync(
                SalesReportPeriodTypeEnum.Weekly, weekStart, weekEnding, subscribers, cancellationToken);

            logger.LogInformation(
                "Weekly sales report for week ending {WeekEnding} sent to {Count} subscribers.",
                weekEnding, count);
            return count;
        }

        /// <inheritdoc />
        public async Task<int> SendMonthlyReportIfDueAsync(CancellationToken cancellationToken = default)
        {
            var todayUtc = DateOnly.FromDateTime(clock.GetUtcNow().UtcDateTime);
            if (todayUtc.Day != 1)
            {
                return 0;
            }

            // Just-ended calendar month.
            var monthAnchor = todayUtc.AddMonths(-1);
            var monthStart = new DateOnly(monthAnchor.Year, monthAnchor.Month, 1);
            var monthEnd = monthStart.AddMonths(1).AddDays(-1);

            var alreadySent = await IsAlreadySentAsync(
                SalesReportPeriodTypeEnum.Monthly, monthStart, cancellationToken);
            if (alreadySent)
            {
                logger.LogInformation(
                    "Monthly sales report for {Month:yyyy-MM} already emailed; skipping.",
                    monthStart);
                return 0;
            }

            var report = await monthlyReportService.GenerateAsync(monthAnchor, cancellationToken);
            if (IsEmpty(report))
            {
                logger.LogInformation(
                    "Monthly sales report for {Month:yyyy-MM} has no activity; skipping.",
                    monthStart);
                return 0;
            }

            var subscribers = await subscriberService.GetForCadenceAsync(
                SalesReportPeriodTypeEnum.Monthly, cancellationToken);
            if (subscribers.Count == 0)
            {
                logger.LogInformation(
                    "Monthly sales report for {Month:yyyy-MM} has 0 subscribers; skipping.",
                    monthStart);
                return 0;
            }

            var subject = $"TrashMob municipal sales pipeline — {monthStart:MMMM yyyy}";
            var html = BuildMonthlyHtml(report, monthStart);
            var count = await DispatchAsync(subject, html, subscribers, cancellationToken);

            await MarkSentAsync(
                SalesReportPeriodTypeEnum.Monthly, monthStart, monthEnd, subscribers, cancellationToken);

            logger.LogInformation(
                "Monthly sales report for {Month:yyyy-MM} sent to {Count} subscribers.",
                monthStart, count);
            return count;
        }

        private async Task<bool> IsAlreadySentAsync(
            SalesReportPeriodTypeEnum periodType,
            DateOnly periodStart,
            CancellationToken cancellationToken)
        {
            var start = periodStart.ToDateTime(TimeOnly.MinValue);
            return await db.SalesReports.AnyAsync(
                r => r.PeriodType == (int)periodType
                     && r.PeriodStart == start
                     && r.EmailSentDate != null,
                cancellationToken);
        }

        private async Task MarkSentAsync(
            SalesReportPeriodTypeEnum periodType,
            DateOnly periodStart,
            DateOnly periodEnd,
            IReadOnlyCollection<SalesReportSubscriber> subscribers,
            CancellationToken cancellationToken)
        {
            var start = periodStart.ToDateTime(TimeOnly.MinValue);
            var end = periodEnd.ToDateTime(TimeOnly.MinValue);
            var now = clock.GetUtcNow();

            // Attribute the audit trail to the first recipient. The daily job has
            // no natural user context, and the SalesReport CreatedBy/LastUpdatedBy
            // FKs must resolve to real Users rows (see the Project 64 P1 backfill
            // failure on 2026-07-05 for what happens when you use a fake
            // 00000000-0000-0000-0000-000000000001 sentinel). "First subscriber
            // received this report" is a fine placeholder.
            var attributionUserId = subscribers.First().UserId;

            var existing = await db.SalesReports.FirstOrDefaultAsync(
                r => r.PeriodType == (int)periodType && r.PeriodStart == start,
                cancellationToken);

            if (existing == null)
            {
                db.SalesReports.Add(new SalesReport
                {
                    Id = Guid.NewGuid(),
                    PeriodType = (int)periodType,
                    PeriodStart = start,
                    PeriodEnd = end,
                    EmailSentDate = now,
                    CreatedByUserId = attributionUserId,
                    CreatedDate = now,
                    LastUpdatedByUserId = attributionUserId,
                    LastUpdatedDate = now,
                });
            }
            else
            {
                existing.EmailSentDate = now;
                existing.LastUpdatedByUserId = attributionUserId;
                existing.LastUpdatedDate = now;
            }

            await db.SaveChangesAsync(cancellationToken);
        }

        private async Task<int> DispatchAsync(
            string subject,
            string emailCopy,
            IReadOnlyCollection<SalesReportSubscriber> subscribers,
            CancellationToken cancellationToken)
        {
            var recipients = subscribers
                .Where(s => !string.IsNullOrWhiteSpace(s.User?.Email))
                .Select(s => new EmailAddress { Name = s.User.DisplayFirstName ?? s.User.UserName, Email = s.User.Email })
                .ToList();

            if (recipients.Count == 0)
            {
                return 0;
            }

            var dynamicTemplateData = new
            {
                username = "TrashMob subscriber",
                emailCopy,
                subject,
            };

            await emailManager.SendTemplatedEmailAsync(
                subject,
                SendGridEmailTemplateId.GenericEmail,
                SendGridEmailGroupId.General,
                dynamicTemplateData,
                recipients,
                cancellationToken);

            return recipients.Count;
        }

        private static bool IsEmpty(WeeklySalesReportDto r) =>
            r.ProspectsResearched == 0
            && r.NewContactsAdded == 0
            && r.OutreachTouches == 0
            && r.FollowUpTouches == 0
            && r.Responses == 0
            && r.MeetingsRequested == 0
            && r.MeetingsScheduled == 0
            && r.MeetingsHeld == 0
            && r.KeyMunicipalFeedback.Count == 0
            && r.PricingFeedback.Count == 0;

        private static bool IsEmpty(MonthlySalesReportDto r) =>
            r.Metrics.All(m => m.Actual == 0)
            && r.BestRespondingDepartments.Count == 0
            && r.CommonObjections.Count == 0
            && r.PricingFeedback.Count == 0;

        private string BuildWeeklyHtml(WeeklySalesReportDto report, DateOnly periodStart, DateOnly periodEnd)
        {
            var template = emailManager.GetHtmlEmailCopy(NotificationTypeEnum.SalesReportWeekly.ToString());
            var periodLabel = $"{periodStart:MMM d}–{periodEnd:MMM d, yyyy}";

            return template
                .Replace("{PeriodLabel}", periodLabel)
                .Replace("{ProspectsResearched}", report.ProspectsResearched.ToString())
                .Replace("{NewContactsAdded}", report.NewContactsAdded.ToString())
                .Replace("{OutreachTouches}", report.OutreachTouches.ToString())
                .Replace("{FollowUpTouches}", report.FollowUpTouches.ToString())
                .Replace("{Responses}", report.Responses.ToString())
                .Replace("{MeetingsRequested}", report.MeetingsRequested.ToString())
                .Replace("{MeetingsScheduled}", report.MeetingsScheduled.ToString())
                .Replace("{MeetingsHeld}", report.MeetingsHeld.ToString())
                .Replace("{KeyMunicipalFeedbackHtml}", RenderBulletList(report.KeyMunicipalFeedback))
                .Replace("{PricingFeedbackHtml}", RenderBulletList(report.PricingFeedback))
                .Replace("{NextStepsHtml}", RenderNarrative(report.NextSteps));
        }

        private string BuildMonthlyHtml(MonthlySalesReportDto report, DateOnly monthStart)
        {
            var template = emailManager.GetHtmlEmailCopy(NotificationTypeEnum.SalesReportMonthly.ToString());
            var periodLabel = monthStart.ToString("MMMM yyyy");

            return template
                .Replace("{PeriodLabel}", periodLabel)
                .Replace("{MetricRowsHtml}", RenderMetricRows(report.Metrics))
                .Replace("{DepartmentsHtml}", RenderCountList(report.BestRespondingDepartments))
                .Replace("{ObjectionsHtml}", RenderCountList(report.CommonObjections))
                .Replace("{PricingHtml}", RenderCountList(report.PricingFeedback))
                .Replace("{NextMonthPriorityHtml}", RenderNarrative(report.NextMonthPriority));
        }

        private static string RenderBulletList(IReadOnlyCollection<string> items)
        {
            if (items.Count == 0)
            {
                return "<p style=\"color: #666666; margin: 0;\">None captured for this window.</p>";
            }

            var sb = new StringBuilder();
            sb.Append("<ul style=\"margin: 0; padding-left: 20px;\">");
            foreach (var item in items)
            {
                sb.Append("<li style=\"margin-bottom: 4px;\">").Append(HtmlEncode(item)).Append("</li>");
            }
            sb.Append("</ul>");
            return sb.ToString();
        }

        private static string RenderCountList(IReadOnlyCollection<MarketIntelligenceCountDto> items)
        {
            if (items.Count == 0)
            {
                return "<p style=\"color: #666666; margin: 0;\">None captured for this window.</p>";
            }

            var sb = new StringBuilder();
            sb.Append("<ul style=\"margin: 0; padding-left: 20px;\">");
            foreach (var item in items)
            {
                sb.Append("<li style=\"margin-bottom: 4px;\">")
                    .Append(HtmlEncode(item.Label))
                    .Append(" <span style=\"color: #666666;\">(")
                    .Append(item.Count)
                    .Append(")</span></li>");
            }
            sb.Append("</ul>");
            return sb.ToString();
        }

        private static string RenderMetricRows(IReadOnlyCollection<MonthlySalesMetricDto> metrics)
        {
            var sb = new StringBuilder();
            foreach (var m in metrics)
            {
                sb.Append("<tr style=\"border-bottom: 1px solid #eeeeee;\">")
                    .Append("<td style=\"padding: 6px 12px 6px 0;\">").Append(HtmlEncode(m.Label)).Append("</td>")
                    .Append("<td style=\"padding: 6px 8px; text-align: right;\">").Append(m.Target).Append("</td>")
                    .Append("<td style=\"padding: 6px 8px; text-align: right; font-weight: bold;\">").Append(m.Actual).Append("</td>")
                    .Append("<td style=\"padding: 6px 0 6px 8px;\">").Append(HtmlEncode(m.Status)).Append("</td>")
                    .Append("</tr>");
            }
            return sb.ToString();
        }

        private static string RenderNarrative(string? text)
        {
            if (string.IsNullOrWhiteSpace(text))
            {
                return "<p style=\"color: #666666; margin: 0;\">The salesperson did not add a narrative for this period.</p>";
            }

            return "<p style=\"margin: 0; white-space: pre-wrap;\">" + HtmlEncode(text) + "</p>";
        }

        private static string HtmlEncode(string s) =>
            System.Net.WebUtility.HtmlEncode(s);
    }
}
