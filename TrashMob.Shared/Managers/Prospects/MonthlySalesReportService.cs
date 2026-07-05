namespace TrashMob.Shared.Managers.Prospects
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading;
    using System.Threading.Tasks;
    using Microsoft.EntityFrameworkCore;
    using TrashMob.Models;
    using TrashMob.Models.Poco.V2;
    using TrashMob.Shared.Managers.Interfaces;
    using TrashMob.Shared.Persistence;

    /// <summary>
    /// EF-backed implementation of the Monthly Municipal Sales Pipeline Report
    /// (Project 63 Phase 3). One DB pass for the base metrics, one for market
    /// intelligence aggregation.
    /// </summary>
    public class MonthlySalesReportService(
        MobDbContext db,
        ISalesReportNarrativeService narrativeService) : IMonthlySalesReportService
    {
        /// <summary>
        /// Cynthia's baseline targets per Project 63 (20 / 20 / 15 / 10 / 3 / 2 / 1).
        /// Used as fallback when the salesperson has not yet edited targets
        /// for the month.
        /// </summary>
        private static readonly IReadOnlyDictionary<SalesMetricEnum, int> DefaultTargets = new Dictionary<SalesMetricEnum, int>
        {
            [SalesMetricEnum.ProspectsResearched] = 20,
            [SalesMetricEnum.NewContacts] = 20,
            [SalesMetricEnum.OutreachTouches] = 15,
            [SalesMetricEnum.FollowUpTouches] = 10,
            [SalesMetricEnum.Responses] = 3,
            [SalesMetricEnum.MeetingsRequested] = 2,
            [SalesMetricEnum.MeetingsScheduled] = 1,
        };

        private static readonly IReadOnlyDictionary<SalesMetricEnum, string> MetricLabels = new Dictionary<SalesMetricEnum, string>
        {
            [SalesMetricEnum.ProspectsResearched] = "Prospects researched",
            [SalesMetricEnum.NewContacts] = "New contacts added",
            [SalesMetricEnum.OutreachTouches] = "Outreach touches",
            [SalesMetricEnum.FollowUpTouches] = "Follow-up touches",
            [SalesMetricEnum.Responses] = "Responses",
            [SalesMetricEnum.MeetingsRequested] = "Meetings requested",
            [SalesMetricEnum.MeetingsScheduled] = "Meetings scheduled",
        };

        /// <inheritdoc />
        public async Task<MonthlySalesReportDto> GenerateAsync(DateOnly anyDateInMonth, CancellationToken cancellationToken = default)
        {
            var monthStart = new DateOnly(anyDateInMonth.Year, anyDateInMonth.Month, 1);
            var monthStartUtc = new DateTimeOffset(monthStart.ToDateTime(TimeOnly.MinValue), TimeSpan.Zero);
            var nextMonthUtc = monthStartUtc.AddMonths(1);
            var monthEndUtc = nextMonthUtc.AddTicks(-1);

            var prospectsResearched = await db.CommunityProspects
                .Where(p => p.CreatedDate >= monthStartUtc && p.CreatedDate < nextMonthUtc)
                .CountAsync(cancellationToken);

            var newContacts = await db.ProspectContacts
                .Where(c => c.CreatedDate >= monthStartUtc && c.CreatedDate < nextMonthUtc)
                .CountAsync(cancellationToken);

            var activities = await db.ProspectActivities
                .Where(a => a.CreatedDate >= monthStartUtc && a.CreatedDate < nextMonthUtc)
                .Select(a => new { a.ActivityType, a.ProspectId })
                .ToListAsync(cancellationToken);

            var counts = new Dictionary<string, int>(StringComparer.OrdinalIgnoreCase);
            var respondedProspectIds = new HashSet<Guid>();
            var touchedProspectIds = new HashSet<Guid>();
            foreach (var a in activities)
            {
                touchedProspectIds.Add(a.ProspectId);
                if (string.IsNullOrWhiteSpace(a.ActivityType))
                {
                    continue;
                }

                counts.TryGetValue(a.ActivityType, out var current);
                counts[a.ActivityType] = current + 1;

                if (string.Equals(a.ActivityType, ProspectActivityTypeEnum.ResponseReceived.ToString(), StringComparison.OrdinalIgnoreCase))
                {
                    respondedProspectIds.Add(a.ProspectId);
                }
            }

            int CountOf(ProspectActivityTypeEnum type) =>
                counts.TryGetValue(type.ToString(), out var n) ? n : 0;

            var actuals = new Dictionary<SalesMetricEnum, int>
            {
                [SalesMetricEnum.ProspectsResearched] = prospectsResearched,
                [SalesMetricEnum.NewContacts] = newContacts,
                [SalesMetricEnum.OutreachTouches] = CountOf(ProspectActivityTypeEnum.Outreach),
                [SalesMetricEnum.FollowUpTouches] = CountOf(ProspectActivityTypeEnum.FollowUp),
                [SalesMetricEnum.Responses] = CountOf(ProspectActivityTypeEnum.ResponseReceived),
                [SalesMetricEnum.MeetingsRequested] = CountOf(ProspectActivityTypeEnum.MeetingRequested),
                [SalesMetricEnum.MeetingsScheduled] = CountOf(ProspectActivityTypeEnum.MeetingScheduled),
            };

            var storedTargets = await db.SalesMonthlyTargets
                .Where(t => t.Month == monthStart.ToDateTime(TimeOnly.MinValue))
                .Select(t => new { t.Metric, t.Target })
                .ToListAsync(cancellationToken);

            var targetLookup = storedTargets.ToDictionary(t => (SalesMetricEnum)t.Metric, t => t.Target);

            var metricRows = new List<MonthlySalesMetricDto>();
            foreach (var metric in DefaultTargets.Keys)
            {
                var target = targetLookup.TryGetValue(metric, out var stored) ? stored : DefaultTargets[metric];
                var actual = actuals.TryGetValue(metric, out var a) ? a : 0;
                metricRows.Add(new MonthlySalesMetricDto
                {
                    Metric = (int)metric,
                    MetricName = metric.ToString(),
                    Label = MetricLabels[metric],
                    Target = target,
                    Actual = actual,
                    Status = ComputeStatus(target, actual),
                });
            }

            // Market intelligence: pull the touched-prospect universe once and
            // derive the three top-N breakdowns from the same rows.
            var touchedIdList = touchedProspectIds.ToList();
            var respondedIdList = respondedProspectIds.ToList();

            var touchedProspects = touchedIdList.Count == 0
                ? []
                : await db.CommunityProspects
                    .Where(p => touchedIdList.Contains(p.Id))
                    .Select(p => new { p.Id, p.Department, p.KeyObjection, p.PricingFeedback })
                    .ToListAsync(cancellationToken);

            var bestRespondingDepartments = touchedProspects
                .Where(p => respondedIdList.Contains(p.Id))
                .Select(p => (p.Department ?? string.Empty).Trim())
                .Where(d => !string.IsNullOrWhiteSpace(d))
                .GroupBy(d => d, StringComparer.OrdinalIgnoreCase)
                .Select(g => new MarketIntelligenceCountDto { Label = g.Key, Count = g.Count() })
                .OrderByDescending(x => x.Count)
                .ThenBy(x => x.Label)
                .Take(5)
                .ToList();

            var commonObjections = touchedProspects
                .Select(p => (p.KeyObjection ?? string.Empty).Trim())
                .Where(s => !string.IsNullOrWhiteSpace(s))
                .GroupBy(s => s, StringComparer.OrdinalIgnoreCase)
                .Select(g => new MarketIntelligenceCountDto { Label = g.Key, Count = g.Count() })
                .OrderByDescending(x => x.Count)
                .ThenBy(x => x.Label)
                .Take(5)
                .ToList();

            var pricingFeedback = touchedProspects
                .Select(p => (p.PricingFeedback ?? string.Empty).Trim())
                .Where(s => !string.IsNullOrWhiteSpace(s))
                .GroupBy(s => s, StringComparer.OrdinalIgnoreCase)
                .Select(g => new MarketIntelligenceCountDto { Label = g.Key, Count = g.Count() })
                .OrderByDescending(x => x.Count)
                .ThenBy(x => x.Label)
                .Take(5)
                .ToList();

            var narrative = await narrativeService.GetAsync(
                SalesReportPeriodTypeEnum.Monthly, monthStart, cancellationToken);

            return new MonthlySalesReportDto
            {
                PeriodStart = monthStartUtc,
                PeriodEnd = monthEndUtc,
                Metrics = metricRows,
                BestRespondingDepartments = bestRespondingDepartments,
                CommonObjections = commonObjections,
                PricingFeedback = pricingFeedback,
                NextMonthPriority = narrative?.NextMonthPriority,
            };
        }

        /// <inheritdoc />
        public async Task UpdateTargetsAsync(
            DateOnly month,
            IReadOnlyCollection<MonthlyTargetUpdateDto> targets,
            Guid actingUserId,
            CancellationToken cancellationToken = default)
        {
            if (targets == null || targets.Count == 0)
            {
                return;
            }

            var monthStart = new DateOnly(month.Year, month.Month, 1);
            var monthDateTime = monthStart.ToDateTime(TimeOnly.MinValue);
            var now = DateTimeOffset.UtcNow;

            var existing = await db.SalesMonthlyTargets
                .Where(t => t.Month == monthDateTime)
                .ToListAsync(cancellationToken);

            foreach (var update in targets)
            {
                var match = existing.FirstOrDefault(t => t.Metric == update.Metric);
                if (match == null)
                {
                    db.SalesMonthlyTargets.Add(new SalesMonthlyTarget
                    {
                        Id = Guid.NewGuid(),
                        Month = monthDateTime,
                        Metric = update.Metric,
                        Target = update.Target,
                        CreatedByUserId = actingUserId,
                        CreatedDate = now,
                        LastUpdatedByUserId = actingUserId,
                        LastUpdatedDate = now,
                    });
                }
                else if (match.Target != update.Target)
                {
                    match.Target = update.Target;
                    match.LastUpdatedByUserId = actingUserId;
                    match.LastUpdatedDate = now;
                }
            }

            await db.SaveChangesAsync(cancellationToken);
        }

        /// <summary>
        /// Applies the plan's status thresholds: &lt; 70% target = Behind,
        /// 70–110% = OnTrack, &gt; 110% = Exceeded. When target is 0,
        /// returns <c>NoTarget</c> so the UI can render an unmarked row.
        /// </summary>
        private static string ComputeStatus(int target, int actual)
        {
            if (target <= 0)
            {
                return "NoTarget";
            }

            var ratio = (double)actual / target;
            return ratio switch
            {
                < 0.70 => "Behind",
                > 1.10 => "Exceeded",
                _ => "OnTrack",
            };
        }
    }
}
