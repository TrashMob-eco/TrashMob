namespace TrashMob.Shared.Managers.Areas
{
    using System;
    using System.Threading;
    using System.Threading.Tasks;

    /// <summary>
    /// Orchestrates the end-to-end area generation process for a batch.
    /// </summary>
    public interface IAreaGenerationOrchestrator
    {
        /// <summary>
        /// Executes the full generation pipeline: discover features from OSM, process and stage them for review.
        /// </summary>
        /// <param name="batchId">The batch to process.</param>
        /// <param name="cancellationToken">Cancellation token.</param>
        Task ExecuteAsync(Guid batchId, CancellationToken cancellationToken = default);
    }
}
