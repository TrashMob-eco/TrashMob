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
        public static async Task Run([TimerTrigger("0 0 0 */1 * *")] TimerInfo myTimer, ILogger log)
        {
            log.LogInformation($"Getting Stats trigger function executed at: {DateTime.Now}");
            var connectionString = Environment.GetEnvironmentVariable("DBConnectionString");

            using (var conn = new SqlConnection(connectionString))
            {
                conn.Open();
                await CountUsers(log, conn);
                await CountEvents(log, conn);
                await CountEventAttendees(log, conn);
                await CountFutureEvents(log, conn);
                await CountFutureEventAttendees(log, conn);
                await CountContactRequests(log, conn);
            }
        }

        private static async Task CountUsers(ILogger log, SqlConnection conn)
        {
            var sql = "SELECT count(*) FROM dbo.Users";
            var numberOfUsers = 0;

            using (var cmd = new SqlCommand(sql, conn))
            {
                numberOfUsers = (int)await cmd.ExecuteScalarAsync().ConfigureAwait(false);

                log.LogInformation("There are currently '{numberOfUsers}' Users.", numberOfUsers);
            }

            await AddSiteMetrics(log, conn, "TotalSiteUsers", numberOfUsers);
        }

        private static async Task CountEvents(ILogger log, SqlConnection conn)
        {
            var sql = "SELECT count(*) FROM dbo.Events";
            var numberOfEvents = 0;

            using (var cmd = new SqlCommand(sql, conn))
            {
                numberOfEvents = (int)await cmd.ExecuteScalarAsync().ConfigureAwait(false);

                log.LogInformation("There are currently '{numberOfEvents}' Events.", numberOfEvents);
            }

            await AddSiteMetrics(log, conn, "TotalEvents", numberOfEvents);
        }

        private static async Task CountFutureEvents(ILogger log, SqlConnection conn)
        {
            var sql = "SELECT count(*) FROM dbo.Events WHERE EventDate > GetDate()";
            var numberOfEvents = 0;

            using (var cmd = new SqlCommand(sql, conn))
            {
                numberOfEvents = (int)await cmd.ExecuteScalarAsync().ConfigureAwait(false);

                log.LogInformation("There are currently '{numberOfEvents}' Future Events.", numberOfEvents);
            }

            await AddSiteMetrics(log, conn, "TotalFutureEvents", numberOfEvents);
        }

        private static async Task CountEventAttendees(ILogger log, SqlConnection conn)
        {
            var sql = "SELECT count(*) FROM dbo.EventAttendees";
            var numberOfEventAttendees = 0;

            using (var cmd = new SqlCommand(sql, conn))
            {
                numberOfEventAttendees = (int)await cmd.ExecuteScalarAsync().ConfigureAwait(false);

                log.LogInformation("There are currently '{numberOfEventAttendees}' EventAttendees.", numberOfEventAttendees);
            }

            await AddSiteMetrics(log, conn, "TotalEventAttendees", numberOfEventAttendees);
        }

        private static async Task CountFutureEventAttendees(ILogger log, SqlConnection conn)
        {
            var sql = "  Select count(*) from dbo.EventAttendees ea inner join dbo.Events e on ea.EventId = e.id  WHERE e.EventDate > GetDate() ";
            var numberOfEventAttendees = 0;

            using (var cmd = new SqlCommand(sql, conn))
            {
                numberOfEventAttendees = (int)await cmd.ExecuteScalarAsync().ConfigureAwait(false);

                log.LogInformation("There are currently '{numberOfEventAttendees}' EventAttendees.", numberOfEventAttendees);
            }

            await AddSiteMetrics(log, conn, "TotalFutureEventAttendees", numberOfEventAttendees);
        }

        private static async Task CountContactRequests(ILogger log, SqlConnection conn)
        {
            var sql = "SELECT count(*) FROM dbo.ContactRequests";
            var numberOfContactRequests = 0;

            using (var cmd = new SqlCommand(sql, conn))
            {
                numberOfContactRequests = (int)await cmd.ExecuteScalarAsync().ConfigureAwait(false);

                log.LogInformation("There are currently '{numberOfContactRequests}' Future Events.", numberOfContactRequests);
            }

            await AddSiteMetrics(log, conn, "TotalContactRequests", numberOfContactRequests);
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
