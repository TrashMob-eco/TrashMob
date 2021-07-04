namespace TrashMobJobs
{
    using System;
    using System.Threading.Tasks;
    using Microsoft.Azure.Functions.Worker;
    using Microsoft.Extensions.Logging;
    using TrashMob.Shared;

    public class UserNotifier
    {
        private readonly IUserNotificationManager userNotificationManager;

        public UserNotifier(IUserNotificationManager userNotificationManager)
        {
            this.userNotificationManager = userNotificationManager;
        }

        [Function("UserNotifierHosted")]
        public async Task Run([TimerTrigger("0 */5 * * * *")] MyInfo myTimer, FunctionContext context)
        {
            var log = context.GetLogger("UserNotifier");
            log.LogInformation($"UserNotifier trigger function executed at: {DateTime.Now}");
            await userNotificationManager.RunAllNotificatons().ConfigureAwait(false);
        }
    }
}
