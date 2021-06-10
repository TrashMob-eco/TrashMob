using System;
using Microsoft.Azure.WebJobs;
using Microsoft.Extensions.Logging;

namespace TrashMobNotifier
{
    public static class StatGenerator
    {
        [FunctionName("GetStats")]
        public static void Run([TimerTrigger("0 0 0 1 * *")]TimerInfo myTimer, ILogger log)
        {
            log.LogInformation($"C# Timer trigger function executed at: {DateTime.Now}");
        }
    }
}
