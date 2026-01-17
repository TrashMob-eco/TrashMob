namespace TrashMobJobs
{
    using System;
    using System.Text;
    using System.Threading;
    using System.Threading.Tasks;
    using Microsoft.Data.SqlClient;
    using Microsoft.Extensions.Hosting;
    using Microsoft.Extensions.Logging;
    using TrashMob.Models;
    using TrashMob.Shared;
    using TrashMob.Shared.Managers;
    using TrashMob.Shared.Poco;

    public class StatGeneratorWorker : BackgroundService
    {
        private readonly ILogger<StatGeneratorWorker> logger;

        public StatGeneratorWorker(ILogger<StatGeneratorWorker> logger)
        {
            this.logger = logger;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            await RunAsync(stoppingToken);
        }

        private async Task RunAsync(CancellationToken cancellationToken)
        {
            logger.LogInformation("Getting Stats trigger function executed at: {Time}", DateTime.UtcNow);
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
                await conn.OpenAsync(cancellationToken);

                siteStats.UserCount = await CountUsers(conn, cancellationToken);
                siteStats.EventCount = await CountEvents(conn, cancellationToken);
                siteStats.AttendeeCount = await CountEventAttendees(conn, cancellationToken);
                siteStats.FutureEventsCount = await CountFutureEvents(conn, cancellationToken);
                siteStats.FutureEventAttendeesCount = await CountFutureEventAttendees(conn, cancellationToken);
                siteStats.ContactRequestsCount = await CountContactRequests(conn, cancellationToken);
                siteStats.BagsCount = await CountBags(conn, cancellationToken);
                siteStats.MinutesCount = await CountMinutes(conn, cancellationToken);
                siteStats.ActualAttendeesCount = await CountActualAttendees(conn, cancellationToken);
                siteStats.LitterReportsCount = await CountLitterReports(conn, cancellationToken);
                siteStats.NewLitterReportsCount = await CountNewLitterReports(conn, cancellationToken);
                siteStats.CleanedLitterReportsCount = await CountCleanedLitterReports(conn, cancellationToken);
            }

            await SendSummaryReport(siteStats, instanceName, sendGridApiKey);
        }

        private async Task<int> CountUsers(SqlConnection conn, CancellationToken cancellationToken)
        {
            var sql = "SELECT count(*) FROM dbo.Users";
            var numberOfUsers = 0;

            using (var cmd = new SqlCommand(sql, conn))
            {
                numberOfUsers = (int)await cmd.ExecuteScalarAsync(cancellationToken);
                logger.LogInformation("There are currently '{NumberOfUsers}' Users.", numberOfUsers);
            }

            await AddSiteMetrics(conn, "TotalSiteUsers", numberOfUsers, cancellationToken);
            return numberOfUsers;
        }

        private async Task<int> CountEvents(SqlConnection conn, CancellationToken cancellationToken)
        {
            var sql = "SELECT count(*) FROM dbo.Events where eventstatusid != 3";
            var numberOfEvents = 0;

            using (var cmd = new SqlCommand(sql, conn))
            {
                numberOfEvents = (int)await cmd.ExecuteScalarAsync(cancellationToken);
                logger.LogInformation("There are currently '{NumberOfEvents}' Events.", numberOfEvents);
            }

            await AddSiteMetrics(conn, "TotalEvents", numberOfEvents, cancellationToken);
            return numberOfEvents;
        }

        private async Task<int> CountBags(SqlConnection conn, CancellationToken cancellationToken)
        {
            var sql = "SELECT sum(NumberOfBags) + sum(NumberOfBuckets)/3 FROM dbo.EventSummaries";
            var numberOfBags = 0;

            using (var cmd = new SqlCommand(sql, conn))
            {
                numberOfBags = (int)await cmd.ExecuteScalarAsync(cancellationToken);
                logger.LogInformation("There are currently '{NumberOfBags}' Bags picked.", numberOfBags);
            }

            await AddSiteMetrics(conn, "TotalBags", numberOfBags, cancellationToken);
            return numberOfBags;
        }

        private async Task<int> CountMinutes(SqlConnection conn, CancellationToken cancellationToken)
        {
            var sql = "SELECT sum(DurationInMinutes * ActualNumberOfAttendees) FROM dbo.EventSummaries";
            var numberOfMinutes = 0;

            using (var cmd = new SqlCommand(sql, conn))
            {
                numberOfMinutes = (int)await cmd.ExecuteScalarAsync(cancellationToken);
                logger.LogInformation("There are currently '{NumberOfMinutes}' minutes picked.", numberOfMinutes);
            }

            await AddSiteMetrics(conn, "TotalMinutes", numberOfMinutes, cancellationToken);
            return numberOfMinutes;
        }

        private async Task<int> CountActualAttendees(SqlConnection conn, CancellationToken cancellationToken)
        {
            var sql = "SELECT sum(ActualNumberOfAttendees) FROM dbo.EventSummaries";
            var numberOfAttendees = 0;

            using (var cmd = new SqlCommand(sql, conn))
            {
                numberOfAttendees = (int)await cmd.ExecuteScalarAsync(cancellationToken);
                logger.LogInformation("There are currently '{NumberOfAttendees}' actual attendees.", numberOfAttendees);
            }

            await AddSiteMetrics(conn, "ActualAttendees", numberOfAttendees, cancellationToken);
            return numberOfAttendees;
        }

        private async Task<int> CountFutureEvents(SqlConnection conn, CancellationToken cancellationToken)
        {
            var sql = "SELECT count(*) FROM dbo.Events WHERE EventDate > GetDate() and eventstatusid != 3";
            var numberOfEvents = 0;

            using (var cmd = new SqlCommand(sql, conn))
            {
                numberOfEvents = (int)await cmd.ExecuteScalarAsync(cancellationToken);
                logger.LogInformation("There are currently '{NumberOfEvents}' Future Events.", numberOfEvents);
            }

            await AddSiteMetrics(conn, "TotalFutureEvents", numberOfEvents, cancellationToken);
            return numberOfEvents;
        }

        private async Task<int> CountEventAttendees(SqlConnection conn, CancellationToken cancellationToken)
        {
            var sql = "SELECT count(*) FROM dbo.EventAttendees ea inner join dbo.Events e on ea.EventId = e.id WHERE eventstatusid != 3";
            var numberOfEventAttendees = 0;

            using (var cmd = new SqlCommand(sql, conn))
            {
                numberOfEventAttendees = (int)await cmd.ExecuteScalarAsync(cancellationToken);
                logger.LogInformation("There are currently '{NumberOfEventAttendees}' EventAttendees.", numberOfEventAttendees);
            }

            await AddSiteMetrics(conn, "TotalEventAttendees", numberOfEventAttendees, cancellationToken);
            return numberOfEventAttendees;
        }

        private async Task<int> CountFutureEventAttendees(SqlConnection conn, CancellationToken cancellationToken)
        {
            var sql = "Select count(*) from dbo.EventAttendees ea inner join dbo.Events e on ea.EventId = e.id WHERE e.EventDate > GetDate() and eventstatusid != 3";
            var numberOfEventAttendees = 0;

            using (var cmd = new SqlCommand(sql, conn))
            {
                numberOfEventAttendees = (int)await cmd.ExecuteScalarAsync(cancellationToken);
                logger.LogInformation("There are currently '{NumberOfEventAttendees}' EventAttendees.", numberOfEventAttendees);
            }

            await AddSiteMetrics(conn, "TotalFutureEventAttendees", numberOfEventAttendees, cancellationToken);
            return numberOfEventAttendees;
        }

        private async Task<int> CountContactRequests(SqlConnection conn, CancellationToken cancellationToken)
        {
            var sql = "SELECT count(*) FROM dbo.ContactRequests";
            var numberOfContactRequests = 0;

            using (var cmd = new SqlCommand(sql, conn))
            {
                numberOfContactRequests = (int)await cmd.ExecuteScalarAsync(cancellationToken);
                logger.LogInformation("There are currently '{NumberOfContactRequests}' Contact Requests.", numberOfContactRequests);
            }

            await AddSiteMetrics(conn, "TotalContactRequests", numberOfContactRequests, cancellationToken);
            return numberOfContactRequests;
        }

        private async Task<int> CountLitterReports(SqlConnection conn, CancellationToken cancellationToken)
        {
            var sql = "SELECT count(*) FROM dbo.LitterReports";
            var numberOfLitterReports = 0;

            using (var cmd = new SqlCommand(sql, conn))
            {
                numberOfLitterReports = (int)await cmd.ExecuteScalarAsync(cancellationToken);
                logger.LogInformation("There are currently '{NumberOfLitterReports}' Litter Reports.", numberOfLitterReports);
            }

            await AddSiteMetrics(conn, "TotalLitterReports", numberOfLitterReports, cancellationToken);
            return numberOfLitterReports;
        }

        private async Task<int> CountNewLitterReports(SqlConnection conn, CancellationToken cancellationToken)
        {
            var sql = "SELECT count(*) FROM dbo.LitterReports where LitterReportStatusId = " + LitterReportStatusEnum.New;
            var numberOfLitterReports = 0;

            using (var cmd = new SqlCommand(sql, conn))
            {
                numberOfLitterReports = (int)await cmd.ExecuteScalarAsync(cancellationToken);
                logger.LogInformation("There are currently '{NumberOfLitterReports}' New Litter Reports.", numberOfLitterReports);
            }

            await AddSiteMetrics(conn, "TotalNewLitterReports", numberOfLitterReports, cancellationToken);
            return numberOfLitterReports;
        }

        private async Task<int> CountCleanedLitterReports(SqlConnection conn, CancellationToken cancellationToken)
        {
            var sql = "SELECT count(*) FROM dbo.LitterReports where LitterReportStatusId = " + LitterReportStatusEnum.Cleaned;
            var numberOfLitterReports = 0;

            using (var cmd = new SqlCommand(sql, conn))
            {
                numberOfLitterReports = (int)await cmd.ExecuteScalarAsync(cancellationToken);
                logger.LogInformation("There are currently '{NumberOfLitterReports}' Cleaned Litter Reports.", numberOfLitterReports);
            }

            await AddSiteMetrics(conn, "TotalCleanedLitterReports", numberOfLitterReports, cancellationToken);
            return numberOfLitterReports;
        }

        private async Task AddSiteMetrics(SqlConnection conn, string metricType, long metricValue, CancellationToken cancellationToken)
        {
            var id = Guid.NewGuid();
            var processedTime = DateTimeOffset.Now;
            var sql = "INSERT INTO dbo.SiteMetrics (id, processedtime, metricType, metricValue) VALUES (@id, @processedTime, @metricType, @metricValue)";
            using var command = new SqlCommand(sql, conn);
            command.Parameters.AddWithValue("@id", id);
            command.Parameters.AddWithValue("@processedTime", processedTime);
            command.Parameters.AddWithValue("@metricType", metricType);
            command.Parameters.AddWithValue("@metricValue", metricValue);

            var result = await command.ExecuteNonQueryAsync(cancellationToken);

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