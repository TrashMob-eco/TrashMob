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

        /// <summary>
        /// Creates or updates the primary <see cref="ProspectContact"/> for a prospect using
        /// the legacy flat contact fields. Phase 1 backward-compat path — the dedicated
        /// contacts API in Phase 2 supersedes this.
        /// </summary>
        Task<ProspectContact> UpsertPrimaryContactAsync(
            Guid prospectId,
            string name,
            string email,
            string title,
            string phone,
            Guid userId,
            CancellationToken cancellationToken = default);
    }
}
