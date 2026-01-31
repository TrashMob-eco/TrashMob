namespace TrashMob.Shared.Engine
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading;
    using System.Threading.Tasks;
    using Microsoft.Extensions.Logging;
    using TrashMob.Models;
    using TrashMob.Shared.Managers.Interfaces;
    using TrashMob.Shared.Poco;

    /// <summary>
    /// Notification engine that sends weekly digest emails about new litter reports in users' geographic area.
    /// This notifier should only run once per week (e.g., on Mondays).
    /// </summary>
    public class WeeklyLitterReportsInYourAreaNotifier : INotificationEngine
    {
        private readonly IEmailManager emailManager;
        private readonly IEmailSender emailSender;
        private readonly ILitterReportManager litterReportManager;
        private readonly ILogger logger;
        private readonly IMapManager mapRepository;
        private readonly INonEventUserNotificationManager nonEventUserNotificationManager;
        private readonly IKeyedManager<User> userManager;

        /// <summary>
        /// Initializes a new instance of the <see cref="WeeklyLitterReportsInYourAreaNotifier"/> class.
        /// </summary>
        public WeeklyLitterReportsInYourAreaNotifier(
            IKeyedManager<User> userManager,
            ILitterReportManager litterReportManager,
            INonEventUserNotificationManager nonEventUserNotificationManager,
            IEmailSender emailSender,
            IEmailManager emailManager,
            IMapManager mapRepository,
            ILogger logger)
        {
            this.userManager = userManager;
            this.litterReportManager = litterReportManager;
            this.nonEventUserNotificationManager = nonEventUserNotificationManager;
            this.emailSender = emailSender;
            this.emailManager = emailManager;
            this.mapRepository = mapRepository;
            this.logger = logger;
        }

        /// <inheritdoc />
        public async Task GenerateNotificationsAsync(CancellationToken cancellationToken = default)
        {
            // Only run on Mondays to avoid spamming users
            if (DateTime.UtcNow.DayOfWeek != DayOfWeek.Monday)
            {
                logger.LogInformation("Skipping WeeklyLitterReportsInYourAreaNotifier - not Monday");
                return;
            }

            logger.LogInformation("Generating Weekly Litter Report Digest Notifications");

            var users = await userManager.GetAsync(cancellationToken).ConfigureAwait(false);
            var notificationCounter = 0;

            // Get litter reports created in the last 7 days
            var recentReports = await litterReportManager.GetNewLitterReportsAsync(cancellationToken).ConfigureAwait(false);
            var oneWeekAgo = DateTime.UtcNow.AddDays(-7);
            recentReports = recentReports.Where(r => r.CreatedDate >= oneWeekAgo).ToList();

            if (!recentReports.Any())
            {
                logger.LogInformation("No new litter reports in the last 7 days");
                return;
            }

            logger.LogInformation("Found {ReportCount} new litter reports in the last 7 days", recentReports.Count());

            foreach (var user in users)
            {
                // Skip if user has no location set
                if (user.Latitude == 0 && user.Longitude == 0)
                {
                    continue;
                }

                // Check if user already received this notification this week
                var existingNotifications = await nonEventUserNotificationManager
                    .GetByUserIdAsync(user.Id, (int)NotificationTypeEnum.WeeklyLitterReportDigest, cancellationToken)
                    .ConfigureAwait(false);

                if (existingNotifications.Any())
                {
                    continue;
                }

                var nearbyReports = new List<LitterReport>();

                foreach (var report in recentReports)
                {
                    var firstImage = report.LitterImages?.FirstOrDefault();
                    if (firstImage == null || !firstImage.Latitude.HasValue || !firstImage.Longitude.HasValue)
                    {
                        continue;
                    }

                    // Check if report is in user's region
                    if (!string.IsNullOrEmpty(user.Country) && !string.IsNullOrEmpty(firstImage.Country) &&
                        user.Country != firstImage.Country)
                    {
                        continue;
                    }

                    if (!string.IsNullOrEmpty(user.Region) && !string.IsNullOrEmpty(firstImage.Region) &&
                        user.Region != firstImage.Region)
                    {
                        continue;
                    }

                    // Calculate distance
                    var userLocation = new Tuple<double, double>(user.Latitude.Value, user.Longitude.Value);
                    var reportLocation = new Tuple<double, double>(firstImage.Latitude.Value, firstImage.Longitude.Value);

                    var distance = await mapRepository
                        .GetDistanceBetweenTwoPointsAsync(userLocation, reportLocation, user.PrefersMetric)
                        .ConfigureAwait(false);

                    if (distance <= user.TravelLimitForLocalEvents)
                    {
                        nearbyReports.Add(report);
                    }
                }

                if (nearbyReports.Any())
                {
                    await SendDigestNotification(user, nearbyReports, cancellationToken).ConfigureAwait(false);
                    notificationCounter++;
                }
            }

            logger.LogInformation("Sent {NotificationCount} Weekly Litter Report Digest notifications", notificationCounter);
        }

        private async Task SendDigestNotification(User user, List<LitterReport> reports, CancellationToken cancellationToken)
        {
            try
            {
                if (string.IsNullOrEmpty(user.Email))
                {
                    return;
                }

                const string baseUrl = "https://www.trashmob.eco";

                var reportListHtml = string.Join("", reports.Take(5).Select(r =>
                {
                    var firstImage = r.LitterImages?.FirstOrDefault();
                    var location = firstImage != null
                        ? $"{firstImage.City}, {firstImage.Region}"
                        : "Unknown location";
                    var url = $"{baseUrl}/litterreports/{r.Id}";
                    return $"<li><a href=\"{url}\">{r.Name ?? "Untitled Report"}</a> - {location}</li>";
                }));

                var emailCopy = emailManager.GetHtmlEmailCopy(NotificationTypeEnum.WeeklyLitterReportDigest.ToString());
                emailCopy = emailCopy.Replace("{ReportList}", $"<ul>{reportListHtml}</ul>");
                emailCopy = emailCopy.Replace("{LitterReportsUrl}", $"{baseUrl}/litterreports");
                emailCopy = emailCopy.Replace("{CreateEventUrl}", $"{baseUrl}/events/create");

                var subject = $"Weekly Digest: {reports.Count} new litter report(s) in your area";

                var recipients = new List<EmailAddress>
                {
                    new() { Name = user.UserName, Email = user.Email },
                };

                var dynamicTemplateData = new
                {
                    username = user.UserName,
                    emailCopy,
                    subject,
                };

                await emailManager.SendTemplatedEmailAsync(subject, SendGridEmailTemplateId.GenericEmail,
                        SendGridEmailGroupId.LitterReportRelated, dynamicTemplateData, recipients, cancellationToken)
                    .ConfigureAwait(false);

                // Record notification sent
                await nonEventUserNotificationManager.AddAsync(
                    new NonEventUserNotification
                    {
                        Id = Guid.NewGuid(),
                        UserId = user.Id,
                        UserNotificationTypeId = (int)NotificationTypeEnum.WeeklyLitterReportDigest,
                        SentDate = DateTimeOffset.UtcNow,
                    },
                    cancellationToken).ConfigureAwait(false);

                logger.LogInformation("Sent weekly litter report digest to user {UserId} with {ReportCount} reports",
                    user.Id, reports.Count);
            }
            catch (Exception ex)
            {
                logger.LogWarning(ex, "Failed to send weekly litter report digest to user {UserId}", user.Id);
            }
        }
    }
}
