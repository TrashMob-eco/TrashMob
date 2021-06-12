using System;
using System.Data.SqlClient;
using System.Threading.Tasks;
using Microsoft.Azure.WebJobs;
using Microsoft.Extensions.Logging;

namespace TrashMobNotifier
{
    public static class StatGenerator
    {
        [FunctionName("GetStats")]
        public static async Task Run([TimerTrigger("0 */1 * * * *")]TimerInfo myTimer, ILogger log)
        {
            log.LogInformation($"Getting Stats trigger function executed at: {DateTime.Now}");
            var connectionString = Environment.GetEnvironmentVariable("DBConnectionString");

            using (var conn = new SqlConnection(connectionString))
            {
                conn.Open();
                var sql = "SELECT count * from dbo.Users";

                using (var cmd = new SqlCommand(sql, conn))
                {
                    var result = await cmd.ExecuteScalarAsync();

                    log.LogInformation("There are currently '{numberOfUsers}' Users.", result);
                }
            }
        }
    }
}
