namespace TrashMob.Shared.Managers.Prospects
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading;
    using System.Threading.Tasks;
    using Microsoft.EntityFrameworkCore;
    using TrashMob.Models;
    using TrashMob.Shared.Persistence.Interfaces;

    public class ProspectActivityManager : KeyedManager<ProspectActivity>, IProspectActivityManager
    {
        private readonly IKeyedRepository<CommunityProspect> prospectRepository;
        private readonly ISentimentAnalysisService sentimentAnalysisService;

        public ProspectActivityManager(
            IKeyedRepository<ProspectActivity> repository,
            IKeyedRepository<CommunityProspect> prospectRepository,
            ISentimentAnalysisService sentimentAnalysisService)
            : base(repository)
        {
            this.prospectRepository = prospectRepository;
            this.sentimentAnalysisService = sentimentAnalysisService;
        }

        public async Task<IEnumerable<ProspectActivity>> GetByProspectIdAsync(Guid prospectId, CancellationToken cancellationToken = default)
        {
            return await Repo.Get()
                .Where(a => a.ProspectId == prospectId)
                .OrderByDescending(a => a.CreatedDate)
                .ToListAsync(cancellationToken);
        }

        public override async Task<ProspectActivity> AddAsync(ProspectActivity instance, Guid userId, CancellationToken cancellationToken = default)
        {
            // Auto-analyze sentiment for Reply activities
            if (string.Equals(instance.ActivityType, "Reply", StringComparison.OrdinalIgnoreCase)
                && !string.IsNullOrWhiteSpace(instance.Details))
            {
                instance.SentimentScore = await sentimentAnalysisService
                    .AnalyzeSentimentAsync(instance.Details, cancellationToken);
            }

            var result = await base.AddAsync(instance, userId, cancellationToken);

            // Auto-advance pipeline stage on reply
            if (string.Equals(instance.ActivityType, "Reply", StringComparison.OrdinalIgnoreCase))
            {
                var prospect = await prospectRepository.GetAsync(instance.ProspectId, cancellationToken);
                if (prospect != null)
                {
                    var advanced = false;

                    // Contacted -> Responded
                    if (prospect.PipelineStage == 1)
                    {
                        prospect.PipelineStage = 2;
                        advanced = true;
                    }

                    // Responded -> Interested (on positive sentiment)
                    if (prospect.PipelineStage == 2
                        && string.Equals(instance.SentimentScore, "Positive", StringComparison.OrdinalIgnoreCase))
                    {
                        prospect.PipelineStage = 3;
                        advanced = true;
                    }

                    if (advanced)
                    {
                        prospect.LastUpdatedByUserId = userId;
                        prospect.LastUpdatedDate = DateTimeOffset.UtcNow;
                        await prospectRepository.UpdateAsync(prospect);
                    }
                }
            }

            return result;
        }
    }
}
