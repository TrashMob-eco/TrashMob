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
        public static async Task Run([TimerTrigger("0 0 0 */1 * *")]TimerInfo myTimer, ILogger log)
        {
            log.LogInformation($"Getting Stats trigger function executed at: {DateTime.Now}");
            var connectionString = Environment.GetEnvironmentVariable("DBConnectionString");

            using (var conn = new SqlConnection(connectionString))
            {
                conn.Open();
                var sql = "SELECT count(*) FROM dbo.Users";
                var numberOfUsers = 0;

                using (var cmd = new SqlCommand(sql, conn))
                {
                    numberOfUsers = (int)await cmd.ExecuteScalarAsync().ConfigureAwait(false);

                    log.LogInformation("There are currently '{numberOfUsers}' Users.", numberOfUsers);
                }

                await AddSiteMetrics(log, conn, "TotalSiteUsers", numberOfUsers);               
            }
        }

        private static async Task AddSiteMetrics(ILogger log, SqlConnection conn, string metricType, long metricValue)
        {
            var id = Guid.NewGuid();
            var processedTime = DateTimeOffset.Now;
            var sql = "INSERT INTO dbo.SiteMetrics (id, processedtime, metricType, metricValue) VALUES (@id, @processedTime, @metricType, @metricValue)";
            using (var command = new SqlCommand(sql, conn))
            {
                command.Parameters.AddWithValue("@id", id);
                command.Parameters.AddWithValue("@processedTime", processedTime);
                command.Parameters.AddWithValue("@metricType", metricType);
                command.Parameters.AddWithValue("@metricValue", metricValue);

                int result = await command.ExecuteNonQueryAsync().ConfigureAwait(false);

                // Check Error
                if (result < 0)
                    log.LogError("Error inserting data into Database!");
            }
        }
    }
}
