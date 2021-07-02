namespace TrashMobJobs
{
    using System;
    using System.Threading.Tasks;
    using Microsoft.Azure.Functions.Worker;
    using Microsoft.Extensions.Logging;
    using TrashMob.Shared;

    public class UserNotifier
    {
        [Function("UserNotifierHosted")]
        public async Task Run([TimerTrigger("0 0 * * * *")] MyInfo myTimer, FunctionContext context, IUserNotificationManager userNotificationManager)
        {
            var log = context.GetLogger("UserNotifier");
            log.LogInformation($"UserNotifier trigger function executed at: {DateTime.Now}");
            var connectionString = Environment.GetEnvironmentVariable("DBConnectionString");
            var sendGridApiKey = Environment.GetEnvironmentVariable("SendGridApiKey");
            var instanceName = Environment.GetEnvironmentVariable("InstanceName");
            var azureMapsKey = Environment.GetEnvironmentVariable("AzureMapsKey");

            await userNotificationManager.RunAllNotificatons().ConfigureAwait(false);
        }
    }
}
