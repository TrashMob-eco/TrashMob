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

    public class PipelineAnalyticsManager : IPipelineAnalyticsManager
    {
        private static readonly string[] StageLabels =
            ["New", "Contacted", "Responded", "Interested", "Onboarding", "Active", "Declined"];

        private readonly IKeyedRepository<CommunityProspect> prospectRepository;
        private readonly IKeyedRepository<ProspectOutreachEmail> outreachEmailRepository;

        public PipelineAnalyticsManager(
            IKeyedRepository<CommunityProspect> prospectRepository,
            IKeyedRepository<ProspectOutreachEmail> outreachEmailRepository)
        {
            this.prospectRepository = prospectRepository;
            this.outreachEmailRepository = outreachEmailRepository;
        }

        public async Task<PipelineAnalytics> GetAnalyticsAsync(CancellationToken cancellationToken = default)
        {
            var prospects = await prospectRepository.Get().ToListAsync(cancellationToken);
            var emails = await outreachEmailRepository.Get().ToListAsync(cancellationToken);

            var analytics = new PipelineAnalytics
            {
                TotalProspects = prospects.Count,
            };

            // Stage counts
            var stageGroups = prospects.GroupBy(p => p.PipelineStage).ToDictionary(g => g.Key, g => g.Count());
            for (var i = 0; i < StageLabels.Length; i++)
            {
                analytics.StageCounts.Add(new PipelineStageStat
                {
                    Stage = i,
                    Label = StageLabels[i],
                    Count = stageGroups.GetValueOrDefault(i, 0),
                });
            }

            // Outreach email metrics
            analytics.TotalEmailsSent = emails.Count(e => e.Status == "Sent" || e.Status == "Delivered" || e.Status == "Opened" || e.Status == "Clicked");
            analytics.TotalEmailsOpened = emails.Count(e => e.Status == "Opened" || e.Status == "Clicked");
            analytics.TotalEmailsClicked = emails.Count(e => e.Status == "Clicked");
            analytics.TotalEmailsBounced = emails.Count(e => e.Status == "Bounced");

            if (analytics.TotalEmailsSent > 0)
            {
                analytics.OpenRate = Math.Round((double)analytics.TotalEmailsOpened / analytics.TotalEmailsSent * 100, 1);
                analytics.ClickRate = Math.Round((double)analytics.TotalEmailsClicked / analytics.TotalEmailsSent * 100, 1);
                analytics.BounceRate = Math.Round((double)analytics.TotalEmailsBounced / analytics.TotalEmailsSent * 100, 1);
            }

            // Conversion metrics
            analytics.ConvertedCount = prospects.Count(p => p.ConvertedPartnerId.HasValue);
            if (analytics.TotalProspects > 0)
            {
                analytics.ConversionRate = Math.Round((double)analytics.ConvertedCount / analytics.TotalProspects * 100, 1);
            }

            // Average days in pipeline for converted prospects
            var convertedProspects = prospects
                .Where(p => p.ConvertedPartnerId.HasValue && p.PipelineStage == 5)
                .ToList();

            if (convertedProspects.Count > 0)
            {
                analytics.AverageDaysInPipeline = Math.Round(
                    convertedProspects.Average(p => (p.LastUpdatedDate - p.CreatedDate)?.TotalDays ?? 0), 1);
            }

            // Type breakdown
            analytics.TypeBreakdown = prospects
                .GroupBy(p => p.Type ?? "Unknown")
                .Select(g => new ProspectTypeStat
                {
                    Type = g.Key,
                    Count = g.Count(),
                    ConvertedCount = g.Count(p => p.ConvertedPartnerId.HasValue),
                })
                .OrderByDescending(t => t.Count)
                .ToList();

            return analytics;
        }
    }
}
