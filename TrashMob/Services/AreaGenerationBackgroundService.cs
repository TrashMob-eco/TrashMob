namespace TrashMob.Services
{
    using System;
    using System.Threading;
    using System.Threading.Tasks;
    using Microsoft.Extensions.DependencyInjection;
    using Microsoft.Extensions.Hosting;
    using Microsoft.Extensions.Logging;
    using TrashMob.Shared.Managers.Areas;

    /// <summary>
    /// Background service that processes area generation batches from the queue.
    /// </summary>
    public class AreaGenerationBackgroundService(
        IAreaGenerationQueue queue,
        IServiceScopeFactory scopeFactory,
        ILogger<AreaGenerationBackgroundService> logger)
        : BackgroundService
    {
        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            logger.LogInformation("Area generation background service started");

            while (!stoppingToken.IsCancellationRequested)
            {
                try
                {
                    var batchId = await queue.DequeueAsync(stoppingToken);

                    logger.LogInformation("Processing area generation batch {BatchId}", batchId);

                    using var scope = scopeFactory.CreateScope();
                    var orchestrator = scope.ServiceProvider.GetRequiredService<IAreaGenerationOrchestrator>();

                    await orchestrator.ExecuteAsync(batchId, stoppingToken);

                    logger.LogInformation("Completed area generation batch {BatchId}", batchId);
                }
                catch (OperationCanceledException) when (stoppingToken.IsCancellationRequested)
                {
                    // Normal shutdown
                    break;
                }
                catch (Exception ex)
                {
                    logger.LogError(ex, "Error processing area generation batch");
                    // Continue processing next batch
                }
            }

            logger.LogInformation("Area generation background service stopped");
        }
    }
}
