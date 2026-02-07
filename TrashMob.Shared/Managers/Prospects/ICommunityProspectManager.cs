namespace TrashMob.Shared.Managers.Prospects
{
    using System;
    using System.Collections.Generic;
    using System.Threading;
    using System.Threading.Tasks;
    using TrashMob.Models;
    using TrashMob.Shared.Managers.Interfaces;

    public interface ICommunityProspectManager : IKeyedManager<CommunityProspect>
    {
        Task<IEnumerable<CommunityProspect>> GetByPipelineStageAsync(int stage, CancellationToken cancellationToken = default);

        Task<IEnumerable<CommunityProspect>> SearchAsync(string searchTerm, CancellationToken cancellationToken = default);

        Task<CommunityProspect> UpdatePipelineStageAsync(Guid id, int newStage, Guid userId, CancellationToken cancellationToken = default);
    }
}
