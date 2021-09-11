namespace TrashMobJobs
{
    using System;
    using System.Data.SqlClient;
    using System.Text;
    using System.Threading.Tasks;
    using Microsoft.Azure.Functions.Worker;
    using Microsoft.Extensions.Logging;
    using TrashMob.Shared;

    public class StatGenerator
    {
        // Runs once a day at 1 AM Z
        [Function("GetStatsHosted")]
        public async Task Run([TimerTrigger("0 0 0 */1 * *")] MyInfo myTimer, FunctionContext context)
        {
            var log = context.GetLogger("HttpExample");
            log.LogInformation($"Getting Stats trigger function executed at: {DateTime.Now}");
            var connectionString = Environment.GetEnvironmentVariable("TMDBServerConnectionString");
            var sendGridApiKey = Environment.GetEnvironmentVariable("SendGridApiKey");
            var instanceName = Environment.GetEnvironmentVariable("InstanceName");

            var siteStats = new SiteStats();

            using (var conn = new SqlConnection(connectionString))
            {
                conn.Open();

                siteStats.UserCount = await CountUsers(log, conn).ConfigureAwait(false);
                siteStats.EventCount = await CountEvents(log, conn).ConfigureAwait(false);
                siteStats.AttendeeCount = await CountEventAttendees(log, conn).ConfigureAwait(false);
                siteStats.FutureEventsCount = await CountFutureEvents(log, conn).ConfigureAwait(false);
                siteStats.FutureEventAttendeesCount = await CountFutureEventAttendees(log, conn).ConfigureAwait(false);
                siteStats.ContactRequestsCount = await CountContactRequests(log, conn).ConfigureAwait(false);
            }

            await SendSummaryReport(siteStats, instanceName, sendGridApiKey).ConfigureAwait(false);
        }

        private static async Task<int> CountUsers(ILogger log, SqlConnection conn)
        {
            var sql = "SELECT count(*) FROM dbo.Users";
            var numberOfUsers = 0;

            using (var cmd = new SqlCommand(sql, conn))
            {
                numberOfUsers = (int)await cmd.ExecuteScalarAsync().ConfigureAwait(false);

                log.LogInformation("There are currently '{numberOfUsers}' Users.", numberOfUsers);
            }

            await AddSiteMetrics(log, conn, "TotalSiteUsers", numberOfUsers).ConfigureAwait(false);

            return numberOfUsers;
        }

        private static async Task<int> CountEvents(ILogger log, SqlConnection conn)
        {
            var sql = "SELECT count(*) FROM dbo.Events";
            var numberOfEvents = 0;

            using (var cmd = new SqlCommand(sql, conn))
            {
                numberOfEvents = (int)await cmd.ExecuteScalarAsync().ConfigureAwait(false);

                log.LogInformation("There are currently '{numberOfEvents}' Events.", numberOfEvents);
            }

            await AddSiteMetrics(log, conn, "TotalEvents", numberOfEvents).ConfigureAwait(false);

            return numberOfEvents;
        }

        private static async Task<int> CountFutureEvents(ILogger log, SqlConnection conn)
        {
            var sql = "SELECT count(*) FROM dbo.Events WHERE EventDate > GetDate()";
            var numberOfEvents = 0;

            using (var cmd = new SqlCommand(sql, conn))
            {
                numberOfEvents = (int)await cmd.ExecuteScalarAsync().ConfigureAwait(false);

                log.LogInformation("There are currently '{numberOfEvents}' Future Events.", numberOfEvents);
            }

            await AddSiteMetrics(log, conn, "TotalFutureEvents", numberOfEvents).ConfigureAwait(false);

            return numberOfEvents;
        }

        private static async Task<int> CountEventAttendees(ILogger log, SqlConnection conn)
        {
            var sql = "SELECT count(*) FROM dbo.EventAttendees";
            var numberOfEventAttendees = 0;

            using (var cmd = new SqlCommand(sql, conn))
            {
                numberOfEventAttendees = (int)await cmd.ExecuteScalarAsync().ConfigureAwait(false);

                log.LogInformation("There are currently '{numberOfEventAttendees}' EventAttendees.", numberOfEventAttendees);
            }

            await AddSiteMetrics(log, conn, "TotalEventAttendees", numberOfEventAttendees).ConfigureAwait(false);

            return numberOfEventAttendees;
        }

        private static async Task<int> CountFutureEventAttendees(ILogger log, SqlConnection conn)
        {
            var sql = "  Select count(*) from dbo.EventAttendees ea inner join dbo.Events e on ea.EventId = e.id  WHERE e.EventDate > GetDate() ";
            var numberOfEventAttendees = 0;

            using (var cmd = new SqlCommand(sql, conn))
            {
                numberOfEventAttendees = (int)await cmd.ExecuteScalarAsync().ConfigureAwait(false);

                log.LogInformation("There are currently '{numberOfEventAttendees}' EventAttendees.", numberOfEventAttendees);
            }

            await AddSiteMetrics(log, conn, "TotalFutureEventAttendees", numberOfEventAttendees).ConfigureAwait(false);

            return numberOfEventAttendees;
        }

        private static async Task<int> CountContactRequests(ILogger log, SqlConnection conn)
        {
            var sql = "SELECT count(*) FROM dbo.ContactRequests";
            var numberOfContactRequests = 0;

            using (var cmd = new SqlCommand(sql, conn))
            {
                numberOfContactRequests = (int)await cmd.ExecuteScalarAsync().ConfigureAwait(false);

                log.LogInformation("There are currently '{numberOfContactRequests}' Future Events.", numberOfContactRequests);
            }

            await AddSiteMetrics(log, conn, "TotalContactRequests", numberOfContactRequests).ConfigureAwait(false);

            return numberOfContactRequests;
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

        private static Task SendSummaryReport(SiteStats siteStats, string instanceName, string sendGridApiKey)
        {
            var sb = new StringBuilder();

            sb.AppendLine($"Summary Report for '{instanceName}'");
            sb.AppendLine($"Total Number of Users: {siteStats.UserCount}");
            sb.AppendLine($"Total Number of Events: {siteStats.EventCount}");
            sb.AppendLine($"Total Number of Attendees: {siteStats.AttendeeCount}");
            sb.AppendLine($"Total Number of Future Events: {siteStats.FutureEventsCount}");
            sb.AppendLine($"Total Number of Future Event Attendees: {siteStats.FutureEventAttendeesCount}");
            sb.AppendLine($"Total Number of Contact Requests: {siteStats.ContactRequestsCount}");

            var email = new Email
            {
                Subject = $"Summary Report for '{instanceName}'",
                Message = sb.ToString()
            };

            email.Addresses.Add(new EmailAddress() { Email = Constants.TrashMobEmailAddress, Name = Constants.TrashMobEmailName });

            var emailSender = new EmailSender() { ApiKey = sendGridApiKey };
            return emailSender.SendEmailAsync(email);
        }
    }

    public class SiteStats
    {
        public int UserCount { get; set; }

        public int EventCount { get; set; }

        public int AttendeeCount { get; set; }

        public int FutureEventsCount { get; set; }

        public int FutureEventAttendeesCount { get; set; }

        public int ContactRequestsCount { get; set; }
    }
}
