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
    /// EF-backed implementation of the weekly municipal sales pipeline report
    /// (Project 63 Phase 2). All counts are computed in a single scoped DB pass.
    /// </summary>
    public class WeeklySalesReportService(MobDbContext db) : IWeeklySalesReportService
    {
        /// <inheritdoc />
        public async Task<WeeklySalesReportDto> GenerateAsync(DateOnly weekEnding, CancellationToken cancellationToken = default)
        {
            var end = new DateTimeOffset(weekEnding.ToDateTime(new TimeOnly(23, 59, 59, 999)), TimeSpan.Zero);
            var start = new DateTimeOffset(weekEnding.AddDays(-6).ToDateTime(TimeOnly.MinValue), TimeSpan.Zero);

            var prospectsResearched = await db.CommunityProspects
                .Where(p => p.CreatedDate >= start && p.CreatedDate <= end)
                .CountAsync(cancellationToken);

            var newContactsAdded = await db.ProspectContacts
                .Where(c => c.CreatedDate >= start && c.CreatedDate <= end)
                .CountAsync(cancellationToken);

            // Load activities for the window once — the same rows drive the
            // counts, the touched-prospect fan-out, and the feedback aggregation.
            var activities = await db.ProspectActivities
                .Where(a => a.CreatedDate >= start && a.CreatedDate <= end)
                .Select(a => new { a.ActivityType, a.ProspectId })
                .ToListAsync(cancellationToken);

            var counts = new Dictionary<string, int>(StringComparer.OrdinalIgnoreCase);
            var touchedProspectIds = new HashSet<Guid>();
            foreach (var a in activities)
            {
                if (!string.IsNullOrWhiteSpace(a.ActivityType))
                {
                    counts.TryGetValue(a.ActivityType, out var current);
                    counts[a.ActivityType] = current + 1;
                }

                touchedProspectIds.Add(a.ProspectId);
            }

            int CountOf(ProspectActivityTypeEnum type) =>
                counts.TryGetValue(type.ToString(), out var n) ? n : 0;

            var outreach = CountOf(ProspectActivityTypeEnum.Outreach);
            var followUp = CountOf(ProspectActivityTypeEnum.FollowUp);
            var responses = CountOf(ProspectActivityTypeEnum.ResponseReceived);
            var meetingsRequested = CountOf(ProspectActivityTypeEnum.MeetingRequested);
            var meetingsScheduled = CountOf(ProspectActivityTypeEnum.MeetingScheduled);
            var meetingsHeld = CountOf(ProspectActivityTypeEnum.MeetingHeld);

            var touchedIdList = touchedProspectIds.ToList();
            var feedback = touchedIdList.Count == 0
                ? []
                : await db.CommunityProspects
                    .Where(p => touchedIdList.Contains(p.Id))
                    .Select(p => new { p.KeyObjection, p.PricingFeedback })
                    .ToListAsync(cancellationToken);

            var keyMunicipalFeedback = feedback
                .Select(f => (f.KeyObjection ?? string.Empty).Trim())
                .Where(s => !string.IsNullOrWhiteSpace(s))
                .Distinct(StringComparer.OrdinalIgnoreCase)
                .OrderBy(s => s)
                .ToList();

            var pricingFeedback = feedback
                .Select(f => (f.PricingFeedback ?? string.Empty).Trim())
                .Where(s => !string.IsNullOrWhiteSpace(s))
                .Distinct(StringComparer.OrdinalIgnoreCase)
                .OrderBy(s => s)
                .ToList();

            return new WeeklySalesReportDto
            {
                PeriodStart = start,
                PeriodEnd = end,
                ProspectsResearched = prospectsResearched,
                NewContactsAdded = newContactsAdded,
                OutreachTouches = outreach,
                FollowUpTouches = followUp,
                Responses = responses,
                MeetingsRequested = meetingsRequested,
                MeetingsScheduled = meetingsScheduled,
                MeetingsHeld = meetingsHeld,
                KeyMunicipalFeedback = keyMunicipalFeedback,
                PricingFeedback = pricingFeedback,
                NextSteps = null,
            };
        }

    }
}
