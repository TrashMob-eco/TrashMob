namespace TrashMob.Services
{
    using System;
    using System.Threading;
    using System.Threading.Channels;
    using System.Threading.Tasks;

    /// <summary>
    /// Channel-based queue for area generation batch processing.
    /// </summary>
    public class AreaGenerationQueue : IAreaGenerationQueue
    {
        private readonly Channel<Guid> _channel = Channel.CreateUnbounded<Guid>(
            new UnboundedChannelOptions { SingleReader = true });

        /// <inheritdoc />
        public ValueTask QueueAsync(Guid batchId, CancellationToken cancellationToken = default)
        {
            return _channel.Writer.WriteAsync(batchId, cancellationToken);
        }

        /// <inheritdoc />
        public ValueTask<Guid> DequeueAsync(CancellationToken cancellationToken)
        {
            return _channel.Reader.ReadAsync(cancellationToken);
        }
    }
}
