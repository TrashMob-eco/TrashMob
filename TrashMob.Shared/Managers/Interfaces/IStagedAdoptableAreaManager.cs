namespace TrashMob.Shared.Managers.Interfaces
{
    using System;
    using System.Collections.Generic;
    using System.Threading;
    using System.Threading.Tasks;
    using TrashMob.Models;
    using TrashMob.Models.Poco;

    /// <summary>
    /// Defines operations for managing staged adoptable areas (review workflow).
    /// </summary>
    public interface IStagedAdoptableAreaManager : IKeyedManager<StagedAdoptableArea>
    {
        /// <summary>
        /// Gets all staged areas for a specific batch.
        /// </summary>
        Task<IEnumerable<StagedAdoptableArea>> GetByBatchAsync(
            Guid batchId,
            CancellationToken cancellationToken = default);

        /// <summary>
        /// Approves a single staged area.
        /// </summary>
        Task ApproveAsync(Guid id, Guid userId, CancellationToken cancellationToken = default);

        /// <summary>
        /// Rejects a single staged area.
        /// </summary>
        Task RejectAsync(Guid id, Guid userId, CancellationToken cancellationToken = default);

        /// <summary>
        /// Bulk-approves staged areas in a batch. If ids is null, approves all pending.
        /// </summary>
        /// <returns>The count of areas approved.</returns>
        Task<int> BulkApproveAsync(
            Guid batchId,
            Guid userId,
            IEnumerable<Guid> ids = null,
            CancellationToken cancellationToken = default);

        /// <summary>
        /// Bulk-rejects staged areas in a batch. If ids is null, rejects all pending.
        /// </summary>
        /// <returns>The count of areas rejected.</returns>
        Task<int> BulkRejectAsync(
            Guid batchId,
            Guid userId,
            IEnumerable<Guid> ids = null,
            CancellationToken cancellationToken = default);

        /// <summary>
        /// Promotes all approved staged areas in a batch to real adoptable areas.
        /// </summary>
        Task<AreaBulkImportResult> CreateApprovedAreasAsync(
            Guid batchId,
            Guid userId,
            CancellationToken cancellationToken = default);

        /// <summary>
        /// Updates the name of a staged area.
        /// </summary>
        Task UpdateNameAsync(Guid id, string name, Guid userId, CancellationToken cancellationToken = default);
    }
}
