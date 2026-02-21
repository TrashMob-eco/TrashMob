namespace TrashMob.Shared.Managers.Areas
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading;
    using System.Threading.Tasks;
    using Microsoft.EntityFrameworkCore;
    using TrashMob.Models;
    using TrashMob.Shared.Managers.Interfaces;
    using TrashMob.Shared.Persistence.Interfaces;

    /// <summary>
    /// Manager for area generation batch operations.
    /// </summary>
    public class AreaGenerationBatchManager(
        IKeyedRepository<AreaGenerationBatch> repository,
        IKeyedRepository<StagedAdoptableArea> stagedRepository)
        : KeyedManager<AreaGenerationBatch>(repository), IAreaGenerationBatchManager
    {
        private static readonly string[] TerminalStatuses = ["Complete", "Failed", "Cancelled"];

        /// <inheritdoc />
        public async Task<IEnumerable<AreaGenerationBatch>> GetByPartnerAsync(
            Guid partnerId,
            CancellationToken cancellationToken = default)
        {
            return await Repo.Get()
                .Where(b => b.PartnerId == partnerId)
                .OrderByDescending(b => b.CreatedDate)
                .ToListAsync(cancellationToken);
        }

        /// <inheritdoc />
        public async Task<AreaGenerationBatch> GetActiveByPartnerAsync(
            Guid partnerId,
            CancellationToken cancellationToken = default)
        {
            return await Repo.Get()
                .Where(b => b.PartnerId == partnerId && !TerminalStatuses.Contains(b.Status))
                .FirstOrDefaultAsync(cancellationToken);
        }

        /// <inheritdoc />
        public async Task<(int BatchesDeleted, int StagedAreasDeleted)> DeleteAllByPartnerAsync(
            Guid partnerId,
            CancellationToken cancellationToken = default)
        {
            var batches = await Repo.Get()
                .Where(b => b.PartnerId == partnerId)
                .ToListAsync(cancellationToken);

            if (batches.Count == 0)
            {
                return (0, 0);
            }

            var batchIds = batches.Select(b => b.Id).ToHashSet();

            // Count staged areas before cascade-deleting batches
            var stagedCount = await stagedRepository.Get()
                .CountAsync(s => batchIds.Contains(s.BatchId), cancellationToken);

            foreach (var batch in batches)
            {
                await Repo.DeleteAsync(batch.Id);
            }

            return (batches.Count, stagedCount);
        }
    }
}
