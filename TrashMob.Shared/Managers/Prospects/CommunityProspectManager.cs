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

    public class CommunityProspectManager : KeyedManager<CommunityProspect>, ICommunityProspectManager
    {
        public CommunityProspectManager(IKeyedRepository<CommunityProspect> repository)
            : base(repository)
        {
        }

        public async Task<IEnumerable<CommunityProspect>> GetByPipelineStageAsync(int stage, CancellationToken cancellationToken = default)
        {
            return await Repo.Get()
                .Where(p => p.PipelineStage == stage)
                .OrderByDescending(p => p.FitScore)
                .ToListAsync(cancellationToken);
        }

        public async Task<IEnumerable<CommunityProspect>> SearchAsync(string searchTerm, CancellationToken cancellationToken = default)
        {
            var term = searchTerm.Trim().ToLowerInvariant();

            return await Repo.Get()
                .Where(p =>
                    EF.Functions.Like(p.Name, $"%{term}%") ||
                    EF.Functions.Like(p.City, $"%{term}%") ||
                    EF.Functions.Like(p.Region, $"%{term}%") ||
                    EF.Functions.Like(p.ContactName, $"%{term}%") ||
                    EF.Functions.Like(p.ContactEmail, $"%{term}%"))
                .OrderByDescending(p => p.FitScore)
                .ToListAsync(cancellationToken);
        }

        public async Task<CommunityProspect> UpdatePipelineStageAsync(Guid id, int newStage, Guid userId, CancellationToken cancellationToken = default)
        {
            var prospect = await Repo.GetAsync(id, cancellationToken);

            if (prospect == null)
            {
                return null;
            }

            prospect.PipelineStage = newStage;
            prospect.LastUpdatedByUserId = userId;
            prospect.LastUpdatedDate = DateTimeOffset.UtcNow;

            return await Repo.UpdateAsync(prospect);
        }
    }
}
