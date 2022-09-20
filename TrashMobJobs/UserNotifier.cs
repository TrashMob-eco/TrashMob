namespace TrashMobJobs
{
    using System;
    using System.Threading.Tasks;
    using Microsoft.Azure.Functions.Worker;
    using Microsoft.Extensions.Logging;
    using TrashMob.Shared.Engine;

    public class UserNotifier
    {
        private readonly IUserNotificationManager userNotificationManager;

        public UserNotifier(IUserNotificationManager userNotificationManager)
        {
            this.userNotificationManager = userNotificationManager;
        }

        // Runs at the top of each hour
        [Function("UserNotifierHosted")]
        public async Task Run([TimerTrigger("0 0 * * * *")] MyInfo myTimer, FunctionContext context)
        {
            var log = context.GetLogger("UserNotifier");
            log.LogInformation($"UserNotifier trigger function executed at: {DateTime.Now}");
            await userNotificationManager.RunAllNotificatons().ConfigureAwait(false);
        }
    }
}
