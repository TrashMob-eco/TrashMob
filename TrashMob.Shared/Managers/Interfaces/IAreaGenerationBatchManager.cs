namespace TrashMob.Shared.Managers.Interfaces
{
    using System;
    using System.Collections.Generic;
    using System.Threading;
    using System.Threading.Tasks;
    using TrashMob.Models;

    /// <summary>
    /// Defines operations for managing area generation batches.
    /// </summary>
    public interface IAreaGenerationBatchManager : IKeyedManager<AreaGenerationBatch>
    {
        /// <summary>
        /// Gets all generation batches for a community.
        /// </summary>
        Task<IEnumerable<AreaGenerationBatch>> GetByPartnerAsync(
            Guid partnerId,
            CancellationToken cancellationToken = default);

        /// <summary>
        /// Gets the currently active (non-terminal) batch for a community, if any.
        /// </summary>
        Task<AreaGenerationBatch> GetActiveByPartnerAsync(
            Guid partnerId,
            CancellationToken cancellationToken = default);

        /// <summary>
        /// Deletes all generation batches (and cascaded staged areas) for a community.
        /// </summary>
        /// <param name="partnerId">The community (partner) ID.</param>
        /// <param name="cancellationToken">A token to cancel the asynchronous operation.</param>
        /// <returns>A tuple of (batchesDeleted, stagedAreasDeleted).</returns>
        Task<(int BatchesDeleted, int StagedAreasDeleted)> DeleteAllByPartnerAsync(
            Guid partnerId,
            CancellationToken cancellationToken = default);
    }
}
