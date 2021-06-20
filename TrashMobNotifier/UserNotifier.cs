using System;
using System.Data.SqlClient;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Azure.WebJobs;
using Microsoft.Extensions.Logging;
using TrashMob.Shared;

namespace TrashMobNotifier
{
    public static class UserNotifier
    {
        [FunctionName("NotifyUsers")]
        public static async Task Run([TimerTrigger("0 0 0 */1 * *")] TimerInfo myTimer, ILogger log, IUserNotificationManager userNotificationManager)
        {
            log.LogInformation($"NotifyUsers trigger function executed at: {DateTime.Now}");
            var connectionString = Environment.GetEnvironmentVariable("DBConnectionString");
            var sendGridApiKey = Environment.GetEnvironmentVariable("SendGridApiKey");
            var instanceName = Environment.GetEnvironmentVariable("InstanceName");

            
        }
    }
}
