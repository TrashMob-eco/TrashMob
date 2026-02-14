namespace TrashMob.Shared.Engine
{
    using System.Threading.Tasks;
    using Microsoft.Extensions.Logging;
    using TrashMob.Models;
    using TrashMob.Shared.Managers.Interfaces;

    /// <summary>
    /// Manages and orchestrates all user notification engines.
    /// </summary>
    public class UserNotificationManager(
        IEventManager eventManager,
        IKeyedManager<User> userManager,
        IEventAttendeeManager eventAttendeeManager,
        IKeyedManager<UserNotification> userNotificationManager,
        INonEventUserNotificationManager nonEventUserNotificationManager,
        IEmailSender emailSender,
        IEmailManager emailManager,
        IMapManager mapRepository,
        IBaseManager<EventSummary> eventSummaryManager,
        ILitterReportManager litterReportManager,
        IUserWaiverManager userWaiverManager,
        ILogger<UserNotificationManager> logger) : IUserNotificationManager
    {

        /// <inheritdoc />
        public async Task RunAllNotifications()
        {
            logger.LogInformation("Starting RunAllNotifications");

            var eventSummaryAttendeeNotifier = new EventSummaryAttendeeNotifier(eventManager, userManager,
                eventAttendeeManager, userNotificationManager, nonEventUserNotificationManager, emailSender,
                emailManager, mapRepository, logger);
            await eventSummaryAttendeeNotifier.GenerateNotificationsAsync().ConfigureAwait(false);

            var eventSummaryHostReminderNotifier = new EventSummaryHostReminderNotifier(eventManager, userManager,
                eventAttendeeManager, userNotificationManager, nonEventUserNotificationManager, emailSender,
                emailManager, mapRepository, logger);
            await eventSummaryHostReminderNotifier.GenerateNotificationsAsync().ConfigureAwait(false);

            var eventSummaryHostWeekReminderNotifier = new EventSummaryHostWeekReminderNotifier(eventManager,
                userManager, eventAttendeeManager, userNotificationManager, nonEventUserNotificationManager,
                emailSender, emailManager, mapRepository, eventSummaryManager, logger);
            await eventSummaryHostWeekReminderNotifier.GenerateNotificationsAsync().ConfigureAwait(false);

            var upcomingEventAttendingThisWeekNotifier = new UpcomingEventAttendingThisWeekNotifier(eventManager,
                userManager, eventAttendeeManager, userNotificationManager, nonEventUserNotificationManager,
                emailSender, emailManager, mapRepository, logger);
            await upcomingEventAttendingThisWeekNotifier.GenerateNotificationsAsync().ConfigureAwait(false);

            var upcomingEventAttendingSoonNotifier = new UpcomingEventAttendingSoonNotifier(eventManager, userManager,
                eventAttendeeManager, userNotificationManager, nonEventUserNotificationManager, emailSender,
                emailManager, mapRepository, logger);
            await upcomingEventAttendingSoonNotifier.GenerateNotificationsAsync().ConfigureAwait(false);

            var upcomingEventHostingThisWeekNotifier = new UpcomingEventHostingThisWeekNotifier(eventManager,
                userManager, eventAttendeeManager, userNotificationManager, nonEventUserNotificationManager,
                emailSender, emailManager, mapRepository, logger);
            await upcomingEventHostingThisWeekNotifier.GenerateNotificationsAsync().ConfigureAwait(false);

            var upcomingEventHostingSoonNotifier = new UpcomingEventHostingSoonNotifier(eventManager, userManager,
                eventAttendeeManager, userNotificationManager, nonEventUserNotificationManager, emailSender,
                emailManager, mapRepository, logger);
            await upcomingEventHostingSoonNotifier.GenerateNotificationsAsync().ConfigureAwait(false);

            var upcomingEventsInYourAreaThisWeekNotifier = new UpcomingEventsInYourAreaThisWeekNotifier(eventManager,
                userManager, eventAttendeeManager, userNotificationManager, nonEventUserNotificationManager,
                emailSender, emailManager, mapRepository, logger);
            await upcomingEventsInYourAreaThisWeekNotifier.GenerateNotificationsAsync().ConfigureAwait(false);

            var upcomingEventsInYourAreaSoonNotifier = new UpcomingEventsInYourAreaSoonNotifier(eventManager,
                userManager, eventAttendeeManager, userNotificationManager, nonEventUserNotificationManager,
                emailSender, emailManager, mapRepository, logger);
            await upcomingEventsInYourAreaSoonNotifier.GenerateNotificationsAsync().ConfigureAwait(false);

            var userProfileLocationNotifier = new UserProfileLocationNotifier(eventManager, userManager,
                eventAttendeeManager, userNotificationManager, nonEventUserNotificationManager, emailSender,
                emailManager, mapRepository, logger);
            await userProfileLocationNotifier.GenerateNotificationsAsync().ConfigureAwait(false);

            var weeklyLitterReportsNotifier = new WeeklyLitterReportsInYourAreaNotifier(userManager,
                litterReportManager, nonEventUserNotificationManager, emailManager, mapRepository,
                logger);
            await weeklyLitterReportsNotifier.GenerateNotificationsAsync().ConfigureAwait(false);

            var waiverExpiringNotifier = new WaiverExpiringNotifier(eventManager, userManager,
                eventAttendeeManager, userNotificationManager, nonEventUserNotificationManager, emailSender,
                emailManager, mapRepository, userWaiverManager, logger);
            await waiverExpiringNotifier.GenerateNotificationsAsync().ConfigureAwait(false);

            logger.LogInformation("Completed RunAllNotifications");
        }
    }
}