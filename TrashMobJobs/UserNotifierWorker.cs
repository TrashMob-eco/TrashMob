namespace TrashMobJobs
{
    using System;
    using System.Threading;
    using System.Threading.Tasks;
    using Microsoft.Extensions.Hosting;
    using Microsoft.Extensions.Logging;
    using TrashMob.Shared.Engine;

    public class UserNotifierWorker : BackgroundService
    {
        private readonly ILogger<UserNotifierWorker> logger;
        private readonly IUserNotificationManager userNotificationManager;

        public UserNotifierWorker(ILogger<UserNotifierWorker> logger, IUserNotificationManager userNotificationManager)
        {
            this.logger = logger;
            this.userNotificationManager = userNotificationManager;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            await RunAsync(stoppingToken);
        }

        private async Task RunAsync(CancellationToken cancellationToken)
        {
            logger.LogInformation("UserNotifier trigger function executed at: {Time}", DateTime.UtcNow);
            await userNotificationManager.RunAllNotificatons();
        }
    }
}