namespace TrashMob.Services
{
    using System;
    using System.Threading;
    using System.Threading.Tasks;

    /// <summary>
    /// Queue for scheduling area generation batch jobs.
    /// </summary>
    public interface IAreaGenerationQueue
    {
        /// <summary>
        /// Queues a batch for processing.
        /// </summary>
        ValueTask QueueAsync(Guid batchId, CancellationToken cancellationToken = default);

        /// <summary>
        /// Dequeues the next batch ID to process.
        /// </summary>
        ValueTask<Guid> DequeueAsync(CancellationToken cancellationToken);
    }
}
