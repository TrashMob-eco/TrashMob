namespace TrashMob.Shared.Managers.Areas
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading;
    using System.Threading.Tasks;
    using Microsoft.EntityFrameworkCore;
    using TrashMob.Models;
    using TrashMob.Models.Poco;
    using TrashMob.Shared.Managers.Interfaces;
    using TrashMob.Shared.Persistence.Interfaces;

    /// <summary>
    /// Manager for staged adoptable area operations (review workflow).
    /// </summary>
    public class StagedAdoptableAreaManager(
        IKeyedRepository<StagedAdoptableArea> repository,
        IAdoptableAreaManager adoptableAreaManager,
        IAreaGenerationBatchManager batchManager)
        : KeyedManager<StagedAdoptableArea>(repository), IStagedAdoptableAreaManager
    {
        /// <inheritdoc />
        public async Task<IEnumerable<StagedAdoptableArea>> GetByBatchAsync(
            Guid batchId,
            CancellationToken cancellationToken = default)
        {
            return await Repo.Get()
                .Where(s => s.BatchId == batchId)
                .OrderBy(s => s.Name)
                .ToListAsync(cancellationToken);
        }

        /// <inheritdoc />
        public async Task ApproveAsync(Guid id, Guid userId, CancellationToken cancellationToken = default)
        {
            await SetReviewStatusAsync(id, "Approved", userId, cancellationToken);
        }

        /// <inheritdoc />
        public async Task RejectAsync(Guid id, Guid userId, CancellationToken cancellationToken = default)
        {
            await SetReviewStatusAsync(id, "Rejected", userId, cancellationToken);
        }

        /// <inheritdoc />
        public async Task<int> BulkApproveAsync(
            Guid batchId,
            Guid userId,
            IEnumerable<Guid> ids = null,
            CancellationToken cancellationToken = default)
        {
            return await BulkSetStatusAsync(batchId, "Approved", userId, ids, cancellationToken);
        }

        /// <inheritdoc />
        public async Task<int> BulkRejectAsync(
            Guid batchId,
            Guid userId,
            IEnumerable<Guid> ids = null,
            CancellationToken cancellationToken = default)
        {
            return await BulkSetStatusAsync(batchId, "Rejected", userId, ids, cancellationToken);
        }

        /// <inheritdoc />
        public async Task<AreaBulkImportResult> CreateApprovedAreasAsync(
            Guid batchId,
            Guid userId,
            CancellationToken cancellationToken = default)
        {
            var approvedAreas = await Repo.Get()
                .Where(s => s.BatchId == batchId && s.ReviewStatus == "Approved")
                .ToListAsync(cancellationToken);

            var adoptableAreas = approvedAreas.Select(staged => new AdoptableArea
            {
                Name = staged.Name,
                Description = staged.Description,
                AreaType = staged.AreaType,
                GeoJson = staged.GeoJson,
                PartnerId = staged.PartnerId,
            });

            var batch = await batchManager.GetAsync(batchId, cancellationToken);
            var result = await adoptableAreaManager.BulkCreateAsync(batch.PartnerId, userId, adoptableAreas, cancellationToken);

            // Update batch counts
            batch.CreatedCount = result.CreatedCount;
            batch.LastUpdatedByUserId = userId;
            batch.LastUpdatedDate = DateTimeOffset.UtcNow;
            await batchManager.UpdateAsync(batch, cancellationToken);

            return result;
        }

        /// <inheritdoc />
        public async Task UpdateNameAsync(Guid id, string name, Guid userId, CancellationToken cancellationToken = default)
        {
            var staged = await Repo.GetAsync(id, cancellationToken);
            staged.Name = name;
            staged.LastUpdatedByUserId = userId;
            staged.LastUpdatedDate = DateTimeOffset.UtcNow;
            await Repo.UpdateAsync(staged);
        }

        private async Task SetReviewStatusAsync(Guid id, string status, Guid userId, CancellationToken cancellationToken)
        {
            var staged = await Repo.GetAsync(id, cancellationToken);
            staged.ReviewStatus = status;
            staged.LastUpdatedByUserId = userId;
            staged.LastUpdatedDate = DateTimeOffset.UtcNow;
            await Repo.UpdateAsync(staged);

            // Update batch counters
            await UpdateBatchCountersAsync(staged.BatchId, userId, cancellationToken);
        }

        private async Task<int> BulkSetStatusAsync(
            Guid batchId,
            string status,
            Guid userId,
            IEnumerable<Guid> ids,
            CancellationToken cancellationToken)
        {
            var query = Repo.Get()
                .Where(s => s.BatchId == batchId && s.ReviewStatus == "Pending");

            if (ids != null)
            {
                var idSet = ids.ToHashSet();
                query = query.Where(s => idSet.Contains(s.Id));
            }

            var areas = await query.ToListAsync(cancellationToken);
            var now = DateTimeOffset.UtcNow;

            foreach (var area in areas)
            {
                area.ReviewStatus = status;
                area.LastUpdatedByUserId = userId;
                area.LastUpdatedDate = now;
                await Repo.UpdateAsync(area);
            }

            await UpdateBatchCountersAsync(batchId, userId, cancellationToken);

            return areas.Count;
        }

        private async Task UpdateBatchCountersAsync(Guid batchId, Guid userId, CancellationToken cancellationToken)
        {
            var batch = await batchManager.GetAsync(batchId, cancellationToken);

            batch.ApprovedCount = await Repo.Get()
                .CountAsync(s => s.BatchId == batchId && s.ReviewStatus == "Approved", cancellationToken);
            batch.RejectedCount = await Repo.Get()
                .CountAsync(s => s.BatchId == batchId && s.ReviewStatus == "Rejected", cancellationToken);

            batch.LastUpdatedByUserId = userId;
            batch.LastUpdatedDate = DateTimeOffset.UtcNow;
            await batchManager.UpdateAsync(batch, cancellationToken);
        }
    }
}
