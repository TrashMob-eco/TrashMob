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
    }
}
