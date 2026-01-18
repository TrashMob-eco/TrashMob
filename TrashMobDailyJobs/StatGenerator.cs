namespace TrashMobDailyJobs
{
    using System;
    using System.Text;
    using System.Threading.Tasks;
    using Microsoft.Data.SqlClient;
    using Microsoft.Extensions.Logging;
    using TrashMob.Models;
    using TrashMob.Shared;
    using TrashMob.Shared.Managers;
    using TrashMob.Shared.Poco;

    public class StatGenerator
    {
        private readonly ILogger<StatGenerator> logger;

        public StatGenerator(ILogger<StatGenerator> logger)
        {
            this.logger = logger;
        }

        public async Task RunAsync()
        {
            logger.LogInformation("StatGenerator job started at: {Time}", DateTime.UtcNow);
            var connectionString = Environment.GetEnvironmentVariable("TMDBServerConnectionString");
            var sendGridApiKey = Environment.GetEnvironmentVariable("SendGridApiKey");
            var instanceName = Environment.GetEnvironmentVariable("InstanceName");

            if (sendGridApiKey == null)
            {
                logger.LogError("SendGrid API Key is not configured. Cannot send summary report email.");
                return;
            }

            if (instanceName == null)
            {
                logger.LogError("Instance Name is not configured. Cannot send summary report email.");
                return;
            }

            var siteStats = new SiteStats();

            using (var conn = new SqlConnection(connectionString))
            {
                await conn.OpenAsync();

                siteStats.UserCount = await CountUsers(conn);
                siteStats.EventCount = await CountEvents(conn);
                siteStats.AttendeeCount = await CountEventAttendees(conn);
                siteStats.FutureEventsCount = await CountFutureEvents(conn);
                siteStats.FutureEventAttendeesCount = await CountFutureEventAttendees(conn);
                siteStats.ContactRequestsCount = await CountContactRequests(conn);
                siteStats.BagsCount = await CountBags(conn);
                siteStats.MinutesCount = await CountMinutes(conn);
                siteStats.ActualAttendeesCount = await CountActualAttendees(conn);
                siteStats.LitterReportsCount = await CountLitterReports(conn);
                siteStats.NewLitterReportsCount = await CountNewLitterReports(conn);
                siteStats.CleanedLitterReportsCount = await CountCleanedLitterReports(conn);
            }

            await SendSummaryReport(siteStats, instanceName, sendGridApiKey);
            logger.LogInformation("StatGenerator job completed at: {Time}", DateTime.UtcNow);
        }

        private async Task<int> CountUsers(SqlConnection conn)
        {
            var sql = "SELECT count(*) FROM dbo.Users";
            var numberOfUsers = 0;

            using (var cmd = new SqlCommand(sql, conn))
            {
                var result = await cmd.ExecuteScalarAsync();
                numberOfUsers = result is DBNull or null ? 0 : Convert.ToInt32(result);
                logger.LogInformation("There are currently '{NumberOfUsers}' Users.", numberOfUsers);
            }

            await AddSiteMetrics(conn, "TotalSiteUsers", numberOfUsers);
            return numberOfUsers;
        }

        private async Task<int> CountEvents(SqlConnection conn)
        {
            var sql = "SELECT count(*) FROM dbo.Events where eventstatusid != 3";
            var numberOfEvents = 0;

            using (var cmd = new SqlCommand(sql, conn))
            {
                var result = await cmd.ExecuteScalarAsync();
                numberOfEvents = result is DBNull or null ? 0 : Convert.ToInt32(result);
                logger.LogInformation("There are currently '{NumberOfEvents}' Events.", numberOfEvents);
            }

            await AddSiteMetrics(conn, "TotalEvents", numberOfEvents);
            return numberOfEvents;
        }

        private async Task<int> CountBags(SqlConnection conn)
        {
            var sql = "SELECT sum(NumberOfBags) + sum(NumberOfBuckets)/3 FROM dbo.EventSummaries";
            var numberOfBags = 0;

            using (var cmd = new SqlCommand(sql, conn))
            {
                var result = await cmd.ExecuteScalarAsync();
                numberOfBags = result is DBNull or null ? 0 : Convert.ToInt32(result);
                logger.LogInformation("There are currently '{NumberOfBags}' Bags picked.", numberOfBags);
            }

            await AddSiteMetrics(conn, "TotalBags", numberOfBags);
            return numberOfBags;
        }

        private async Task<int> CountMinutes(SqlConnection conn)
        {
            var sql = "SELECT sum(DurationInMinutes * ActualNumberOfAttendees) FROM dbo.EventSummaries";
            var numberOfMinutes = 0;

            using (var cmd = new SqlCommand(sql, conn))
            {
                var result = await cmd.ExecuteScalarAsync();
                numberOfMinutes = result is DBNull or null ? 0 : Convert.ToInt32(result);
                logger.LogInformation("There are currently '{NumberOfMinutes}' minutes picked.", numberOfMinutes);
            }

            await AddSiteMetrics(conn, "TotalMinutes", numberOfMinutes);
            return numberOfMinutes;
        }

        private async Task<int> CountActualAttendees(SqlConnection conn)
        {
            var sql = "SELECT sum(ActualNumberOfAttendees) FROM dbo.EventSummaries";
            var numberOfAttendees = 0;

            using (var cmd = new SqlCommand(sql, conn))
            {
                var result = await cmd.ExecuteScalarAsync();
                numberOfAttendees = result is DBNull or null ? 0 : Convert.ToInt32(result);
                logger.LogInformation("There are currently '{NumberOfAttendees}' actual attendees.", numberOfAttendees);
            }

            await AddSiteMetrics(conn, "ActualAttendees", numberOfAttendees);
            return numberOfAttendees;
        }

        private async Task<int> CountFutureEvents(SqlConnection conn)
        {
            var sql = "SELECT count(*) FROM dbo.Events WHERE EventDate > GetDate() and eventstatusid != 3";
            var numberOfEvents = 0;

            using (var cmd = new SqlCommand(sql, conn))
            {
                var result = await cmd.ExecuteScalarAsync();
                numberOfEvents = result is DBNull or null ? 0 : Convert.ToInt32(result);
                logger.LogInformation("There are currently '{NumberOfEvents}' Future Events.", numberOfEvents);
            }

            await AddSiteMetrics(conn, "TotalFutureEvents", numberOfEvents);
            return numberOfEvents;
        }

        private async Task<int> CountEventAttendees(SqlConnection conn)
        {
            var sql = "SELECT count(*) FROM dbo.EventAttendees ea inner join dbo.Events e on ea.EventId = e.id WHERE eventstatusid != 3";
            var numberOfEventAttendees = 0;

            using (var cmd = new SqlCommand(sql, conn))
            {
                var result = await cmd.ExecuteScalarAsync();
                numberOfEventAttendees = result is DBNull or null ? 0 : Convert.ToInt32(result);
                logger.LogInformation("There are currently '{NumberOfEventAttendees}' EventAttendees.", numberOfEventAttendees);
            }

            await AddSiteMetrics(conn, "TotalEventAttendees", numberOfEventAttendees);
            return numberOfEventAttendees;
        }

        private async Task<int> CountFutureEventAttendees(SqlConnection conn)
        {
            var sql = "Select count(*) from dbo.EventAttendees ea inner join dbo.Events e on ea.EventId = e.id WHERE e.EventDate > GetDate() and eventstatusid != 3";
            var numberOfEventAttendees = 0;

            using (var cmd = new SqlCommand(sql, conn))
            {
                var result = await cmd.ExecuteScalarAsync();
                numberOfEventAttendees = result is DBNull or null ? 0 : Convert.ToInt32(result);
                logger.LogInformation("There are currently '{NumberOfEventAttendees}' EventAttendees.", numberOfEventAttendees);
            }

            await AddSiteMetrics(conn, "TotalFutureEventAttendees", numberOfEventAttendees);
            return numberOfEventAttendees;
        }

        private async Task<int> CountContactRequests(SqlConnection conn)
        {
            var sql = "SELECT count(*) FROM dbo.ContactRequests";
            var numberOfContactRequests = 0;

            using (var cmd = new SqlCommand(sql, conn))
            {
                var result = await cmd.ExecuteScalarAsync();
                numberOfContactRequests = result is DBNull or null ? 0 : Convert.ToInt32(result);
                logger.LogInformation("There are currently '{NumberOfContactRequests}' Contact Requests.", numberOfContactRequests);
            }

            await AddSiteMetrics(conn, "TotalContactRequests", numberOfContactRequests);
            return numberOfContactRequests;
        }

        private async Task<int> CountLitterReports(SqlConnection conn)
        {
            var sql = "SELECT count(*) FROM dbo.LitterReports";
            var numberOfLitterReports = 0;

            using (var cmd = new SqlCommand(sql, conn))
            {
                var result = await cmd.ExecuteScalarAsync();
                numberOfLitterReports = result is DBNull or null ? 0 : Convert.ToInt32(result);
                logger.LogInformation("There are currently '{NumberOfLitterReports}' Litter Reports.", numberOfLitterReports);
            }

            await AddSiteMetrics(conn, "TotalLitterReports", numberOfLitterReports);
            return numberOfLitterReports;
        }

        private async Task<int> CountNewLitterReports(SqlConnection conn)
        {
            var sql = "SELECT count(*) FROM dbo.LitterReports where LitterReportStatusId = " + LitterReportStatusEnum.New;
            var numberOfLitterReports = 0;

            using (var cmd = new SqlCommand(sql, conn))
            {
                var result = await cmd.ExecuteScalarAsync();
                numberOfLitterReports = result is DBNull or null ? 0 : Convert.ToInt32(result);
                logger.LogInformation("There are currently '{NumberOfLitterReports}' New Litter Reports.", numberOfLitterReports);
            }

            await AddSiteMetrics(conn, "TotalNewLitterReports", numberOfLitterReports);
            return numberOfLitterReports;
        }

        private async Task<int> CountCleanedLitterReports(SqlConnection conn)
        {
            var sql = "SELECT count(*) FROM dbo.LitterReports where LitterReportStatusId = " + LitterReportStatusEnum.Cleaned;
            var numberOfLitterReports = 0;

            using (var cmd = new SqlCommand(sql, conn))
            {
                var result = await cmd.ExecuteScalarAsync();
                numberOfLitterReports = result is DBNull or null ? 0 : Convert.ToInt32(result);
                logger.LogInformation("There are currently '{NumberOfLitterReports}' Cleaned Litter Reports.", numberOfLitterReports);
            }

            await AddSiteMetrics(conn, "TotalCleanedLitterReports", numberOfLitterReports);
            return numberOfLitterReports;
        }

        private async Task AddSiteMetrics(SqlConnection conn, string metricType, long metricValue)
        {
            var id = Guid.NewGuid();
            var processedTime = DateTimeOffset.Now;
            var sql = "INSERT INTO dbo.SiteMetrics (id, processedtime, metricType, metricValue) VALUES (@id, @processedTime, @metricType, @metricValue)";
            using var command = new SqlCommand(sql, conn);
            command.Parameters.AddWithValue("@id", id);
            command.Parameters.AddWithValue("@processedTime", processedTime);
            command.Parameters.AddWithValue("@metricType", metricType);
            command.Parameters.AddWithValue("@metricValue", metricValue);

            var result = await command.ExecuteNonQueryAsync();

            if (result < 0)
            {
                logger.LogError("Error inserting data into Database!");
            }
        }

        private static Task SendSummaryReport(SiteStats siteStats, string instanceName, string sendGridApiKey)
        {
            var sb = new StringBuilder();

            sb.AppendLine($"Summary Report for '{instanceName}'\n");
            sb.AppendLine($"Total Number of Users: {siteStats.UserCount}\n");
            sb.AppendLine($"Total Number of Events: {siteStats.EventCount}\n");
            sb.AppendLine($"Total Number of Attendees: {siteStats.AttendeeCount}\n");
            sb.AppendLine($"Total Number of Future Events: {siteStats.FutureEventsCount}\n");
            sb.AppendLine($"Total Number of Future Event Attendees: {siteStats.FutureEventAttendeesCount}\n");
            sb.AppendLine($"Total Number of Contact Requests: {siteStats.ContactRequestsCount}\n");
            sb.AppendLine($"Total Number of Bags Collected: {siteStats.BagsCount}\n");
            sb.AppendLine($"Total Number of Minutes: {siteStats.MinutesCount}\n");
            sb.AppendLine($"Total Number of Actual Attendees: {siteStats.ActualAttendeesCount}\n");
            sb.AppendLine($"Total Number of Litter Reports: {siteStats.LitterReportsCount}\n");
            sb.AppendLine($"Total Number of New Litter Reports: {siteStats.NewLitterReportsCount}\n");
            sb.AppendLine($"Total Number of Cleaned Litter Reports: {siteStats.CleanedLitterReportsCount}\n");
            sb.AppendLine("End Report.\n");

            var email = new Email
            {
                Subject = $"Summary Report for '{instanceName}'",
                Message = sb.ToString(),
            };

            email.Addresses.Add(new EmailAddress { Email = Constants.TrashMobEmailAddress, Name = Constants.TrashMobEmailName });

            var emailSender = new EmailSender { ApiKey = sendGridApiKey };
            return emailSender.SendEmailAsync(email);
        }
    }
}
