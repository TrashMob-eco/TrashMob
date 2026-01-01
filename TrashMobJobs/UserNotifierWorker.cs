namespace TrashMobJobs
{
    using System;
    using System.Threading;
    using System.Threading.Tasks;
    using Cronos;
    using Microsoft.Extensions.Hosting;
    using Microsoft.Extensions.Logging;
    using TrashMob.Shared.Engine;

    public class UserNotifierWorker : BackgroundService
    {
        private readonly ILogger<UserNotifierWorker> logger;
        private readonly IUserNotificationManager userNotificationManager;
        private readonly CronExpression cronExpression;

        public UserNotifierWorker(ILogger<UserNotifierWorker> logger, IUserNotificationManager userNotificationManager)
        {
            this.logger = logger;
            this.userNotificationManager = userNotificationManager;
            // Runs at the top of each hour
            cronExpression = CronExpression.Parse("0 0 * * * *");
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                var next = cronExpression.GetNextOccurrence(DateTimeOffset.UtcNow, TimeZoneInfo.Utc);
                if (next.HasValue)
                {
                    var delay = next.Value - DateTimeOffset.UtcNow;
                    if (delay.TotalMilliseconds > 0)
                    {
                        logger.LogInformation("UserNotifier waiting until {NextRun}", next.Value);
                        await Task.Delay(delay, stoppingToken);
                    }
                }

                if (!stoppingToken.IsCancellationRequested)
                {
                    await RunAsync(stoppingToken);
                }
            }
        }

        private async Task RunAsync(CancellationToken cancellationToken)
        {
            logger.LogInformation("UserNotifier trigger function executed at: {Time}", DateTime.UtcNow);
            await userNotificationManager.RunAllNotificatons();
        }
    }
}