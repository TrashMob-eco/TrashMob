namespace TrashMob.Shared.Managers.Prospects
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading;
    using System.Threading.Tasks;
    using Microsoft.EntityFrameworkCore;
    using TrashMob.Models;
    using TrashMob.Models.Poco;
    using TrashMob.Shared.Persistence.Interfaces;

    public class PipelineAnalyticsManager(
        IKeyedRepository<CommunityProspect> prospectRepository,
        IKeyedRepository<ProspectOutreachEmail> outreachEmailRepository,
        IKeyedRepository<ProspectActivity> activityRepository,
        IKeyedRepository<User> userRepository)
        : IPipelineAnalyticsManager
    {
        private static readonly string[] StageLabels =
            ["New", "Contacted", "Responded", "Interested", "Onboarding", "Active", "Declined"];

        public async Task<PipelineAnalytics> GetAnalyticsAsync(CancellationToken cancellationToken = default)
        {
            var analytics = new PipelineAnalytics();

            // Total prospects via DB count
            analytics.TotalProspects = await prospectRepository.Get().CountAsync(cancellationToken);

            // Stage counts via DB GroupBy (single query instead of loading all prospects)
            var stageGroups = await prospectRepository.Get()
                .GroupBy(p => p.PipelineStage)
                .Select(g => new { Stage = g.Key, Count = g.Count() })
                .ToListAsync(cancellationToken);

            var stageDict = stageGroups.ToDictionary(g => g.Stage, g => g.Count);
            for (var i = 0; i < StageLabels.Length; i++)
            {
                analytics.StageCounts.Add(new PipelineStageStat
                {
                    Stage = i,
                    Label = StageLabels[i],
                    Count = stageDict.GetValueOrDefault(i, 0),
                });
            }

            // Outreach email metrics via DB GroupBy (single query instead of loading all emails)
            var emailStatusCounts = await outreachEmailRepository.Get()
                .GroupBy(e => e.Status)
                .Select(g => new { Status = g.Key, Count = g.Count() })
                .ToListAsync(cancellationToken);

            var sentStatuses = new HashSet<string> { "Sent", "Delivered", "Opened", "Clicked" };
            var openedStatuses = new HashSet<string> { "Opened", "Clicked" };

            analytics.TotalEmailsSent = emailStatusCounts.Where(e => sentStatuses.Contains(e.Status)).Sum(e => e.Count);
            analytics.TotalEmailsOpened = emailStatusCounts.Where(e => openedStatuses.Contains(e.Status)).Sum(e => e.Count);
            analytics.TotalEmailsClicked = emailStatusCounts.Where(e => e.Status == "Clicked").Sum(e => e.Count);
            analytics.TotalEmailsBounced = emailStatusCounts.Where(e => e.Status == "Bounced").Sum(e => e.Count);

            if (analytics.TotalEmailsSent > 0)
            {
                analytics.OpenRate = Math.Round((double)analytics.TotalEmailsOpened / analytics.TotalEmailsSent * 100, 1);
                analytics.ClickRate = Math.Round((double)analytics.TotalEmailsClicked / analytics.TotalEmailsSent * 100, 1);
                analytics.BounceRate = Math.Round((double)analytics.TotalEmailsBounced / analytics.TotalEmailsSent * 100, 1);
            }

            // Conversion metrics via DB count
            analytics.ConvertedCount = await prospectRepository.Get()
                .CountAsync(p => p.ConvertedPartnerId.HasValue, cancellationToken);

            if (analytics.TotalProspects > 0)
            {
                analytics.ConversionRate = Math.Round((double)analytics.ConvertedCount / analytics.TotalProspects * 100, 1);
            }

            // Average days in pipeline - load only converted prospects (small subset)
            var convertedProspects = await prospectRepository.Get()
                .Where(p => p.ConvertedPartnerId.HasValue && p.PipelineStage == 5)
                .ToListAsync(cancellationToken);

            if (convertedProspects.Count > 0)
            {
                analytics.AverageDaysInPipeline = Math.Round(
                    convertedProspects.Average(p => (p.LastUpdatedDate - p.CreatedDate)?.TotalDays ?? 0), 1);
            }

            // Type breakdown via DB GroupBy
            analytics.TypeBreakdown = await prospectRepository.Get()
                .GroupBy(p => p.Type ?? "Unknown")
                .Select(g => new ProspectTypeStat
                {
                    Type = g.Key,
                    Count = g.Count(),
                    ConvertedCount = g.Count(p => p.ConvertedPartnerId.HasValue),
                })
                .OrderByDescending(t => t.Count)
                .ToListAsync(cancellationToken);

            // Project 60 Phase 4: per-user touchpoint tally for the last 30 and 90 days.
            var now = DateTimeOffset.UtcNow;
            analytics.TouchpointsByUserLast30Days = await GetTouchpointsByUserAsync(now.AddDays(-30), cancellationToken);
            analytics.TouchpointsByUserLast90Days = await GetTouchpointsByUserAsync(now.AddDays(-90), cancellationToken);

            return analytics;
        }

        private async Task<List<UserTouchpointStat>> GetTouchpointsByUserAsync(
            DateTimeOffset since,
            CancellationToken cancellationToken)
        {
            // Activity counts per user since the cutoff.
            var activityCounts = await activityRepository.Get()
                .Where(a => a.CreatedDate >= since)
                .GroupBy(a => a.CreatedByUserId)
                .Select(g => new { UserId = g.Key, Count = g.Count() })
                .ToListAsync(cancellationToken);

            // Outreach email counts per user since the cutoff.
            var outreachCounts = await outreachEmailRepository.Get()
                .Where(e => e.CreatedDate >= since)
                .GroupBy(e => e.CreatedByUserId)
                .Select(g => new { UserId = g.Key, Count = g.Count() })
                .ToListAsync(cancellationToken);

            // Stitch the two together by user.
            var userIds = activityCounts.Select(a => a.UserId)
                .Concat(outreachCounts.Select(o => o.UserId))
                .Distinct()
                .Where(id => id != Guid.Empty)
                .ToList();

            if (userIds.Count == 0)
            {
                return [];
            }

            // Fetch display names in a single round-trip.
            var users = await userRepository.Get()
                .Where(u => userIds.Contains(u.Id))
                .Select(u => new { u.Id, u.UserName, u.GivenName, u.Surname })
                .ToListAsync(cancellationToken);
            var userLookup = users.ToDictionary(u => u.Id);

            return userIds
                .Select(userId =>
                {
                    var activityCount = activityCounts.FirstOrDefault(a => a.UserId == userId)?.Count ?? 0;
                    var outreachCount = outreachCounts.FirstOrDefault(o => o.UserId == userId)?.Count ?? 0;
                    userLookup.TryGetValue(userId, out var user);
                    var displayName = user is null
                        ? "(unknown)"
                        : !string.IsNullOrWhiteSpace(user.GivenName) && !string.IsNullOrWhiteSpace(user.Surname)
                            ? $"{user.GivenName} {user.Surname}"
                            : user.UserName ?? "(unknown)";

                    return new UserTouchpointStat
                    {
                        UserId = userId,
                        UserName = displayName,
                        ActivityCount = activityCount,
                        OutreachEmailCount = outreachCount,
                        TotalTouchpoints = activityCount + outreachCount,
                    };
                })
                .OrderByDescending(s => s.TotalTouchpoints)
                .ToList();
        }
    }
}
