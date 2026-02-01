namespace TrashMob.Shared.Engine
{
    using System.Threading.Tasks;
    using Microsoft.Extensions.Logging;
    using TrashMob.Models;
    using TrashMob.Shared.Managers.Interfaces;

    /// <summary>
    /// Manages and orchestrates all user notification engines.
    /// </summary>
    public class UserNotificationManager : IUserNotificationManager
    {
        private readonly IEmailManager emailManager;
        private readonly IEmailSender emailSender;
        private readonly IEventAttendeeManager eventAttendeeManager;
        private readonly IEventManager eventManager;
        private readonly IBaseManager<EventSummary> eventSummaryManager;
        private readonly ILitterReportManager litterReportManager;
        private readonly ILogger<UserNotificationManager> logger;
        private readonly IMapManager mapRepository;
        private readonly INonEventUserNotificationManager nonEventUserNotificationManager;
        private readonly IKeyedManager<User> userManager;
        private readonly IKeyedManager<UserNotification> userNotificationManager;

        /// <summary>
        /// Initializes a new instance of the <see cref="UserNotificationManager"/> class.
        /// </summary>
        /// <param name="eventManager">Manager for event operations.</param>
        /// <param name="userManager">Manager for user operations.</param>
        /// <param name="eventAttendeeManager">Manager for event attendee operations.</param>
        /// <param name="userNotificationManager">Manager for user notification tracking.</param>
        /// <param name="nonEventUserNotificationManager">Manager for non-event notifications.</param>
        /// <param name="emailSender">Service for sending emails.</param>
        /// <param name="emailManager">Manager for email operations.</param>
        /// <param name="mapRepository">Repository for map services.</param>
        /// <param name="eventSummaryManager">Manager for event summaries.</param>
        /// <param name="litterReportManager">Manager for litter report operations.</param>
        /// <param name="logger">Logger instance.</param>
        public UserNotificationManager(IEventManager eventManager,
            IKeyedManager<User> userManager,
            IEventAttendeeManager eventAttendeeManager,
            IKeyedManager<UserNotification> userNotificationManager,
            INonEventUserNotificationManager nonEventUserNotificationManager,
            IEmailSender emailSender,
            IEmailManager emailManager,
            IMapManager mapRepository,
            IBaseManager<EventSummary> eventSummaryManager,
            ILitterReportManager litterReportManager,
            ILogger<UserNotificationManager> logger)
        {
            this.eventManager = eventManager;
            this.userManager = userManager;
            this.eventAttendeeManager = eventAttendeeManager;
            this.userNotificationManager = userNotificationManager;
            this.nonEventUserNotificationManager = nonEventUserNotificationManager;
            this.emailSender = emailSender;
            this.emailManager = emailManager;
            this.mapRepository = mapRepository;
            this.eventSummaryManager = eventSummaryManager;
            this.litterReportManager = litterReportManager;
            this.logger = logger;
        }

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
                litterReportManager, nonEventUserNotificationManager, emailSender, emailManager, mapRepository,
                logger);
            await weeklyLitterReportsNotifier.GenerateNotificationsAsync().ConfigureAwait(false);

            logger.LogInformation("Completed RunAllNotifications");
        }
    }
}