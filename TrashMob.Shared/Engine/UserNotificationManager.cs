namespace TrashMob.Shared.Engine
{
    using Microsoft.Extensions.Logging;
    using System.Threading.Tasks;
    using TrashMob.Models;
    using TrashMob.Shared.Managers.Interfaces;
    using TrashMob.Shared.Persistence.Interfaces;

    public class UserNotificationManager : IUserNotificationManager 
    {
        private readonly IEventManager eventManager;
        private readonly IKeyedManager<User> userManager;
        private readonly IEventAttendeeManager eventAttendeeManager;
        private readonly IKeyedManager<UserNotification> userNotificationManager;
        private readonly INonEventUserNotificationManager nonEventUserNotificationManager;
        private readonly IEmailSender emailSender;
        private readonly IEmailManager emailManager;
        private readonly IMapManager mapRepository;
        private readonly IBaseManager<EventSummary> eventSummaryManager;
        private readonly ILogger<UserNotificationManager> logger;

        public UserNotificationManager(IEventManager eventManager,
                                       IKeyedManager<User> userManager,
                                       IEventAttendeeManager eventAttendeeManager,
                                       IKeyedManager<UserNotification> userNotificationManager,
                                       INonEventUserNotificationManager nonEventUserNotificationManager,
                                       IEmailSender emailSender,
                                       IEmailManager emailManager,
                                       IMapManager mapRepository,
                                       IBaseManager<EventSummary> eventSummaryManager,
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
            this.logger = logger;
        }

        public async Task RunAllNotificatons()
        {
            logger.LogInformation("Starting RunAllNotifications");

            var eventSummaryAttendeeNotifier = new EventSummaryAttendeeNotifier(eventManager, userManager, eventAttendeeManager, userNotificationManager, nonEventUserNotificationManager, emailSender, emailManager, mapRepository, logger);
            await eventSummaryAttendeeNotifier.GenerateNotificationsAsync().ConfigureAwait(false);

            var eventSummaryHostReminderNotifier = new EventSummaryHostReminderNotifier(eventManager, userManager, eventAttendeeManager, userNotificationManager, nonEventUserNotificationManager, emailSender, emailManager, mapRepository, logger);
            await eventSummaryHostReminderNotifier.GenerateNotificationsAsync().ConfigureAwait(false);

            var eventSummaryHostWeekReminderNotifier = new EventSummaryHostWeekReminderNotifier(eventManager, userManager, eventAttendeeManager, userNotificationManager, nonEventUserNotificationManager, emailSender, emailManager, mapRepository, eventSummaryManager, logger);
            await eventSummaryHostWeekReminderNotifier.GenerateNotificationsAsync().ConfigureAwait(false);

            var upcomingEventAttendingThisWeekNotifier = new UpcomingEventAttendingThisWeekNotifier(eventManager, userManager, eventAttendeeManager, userNotificationManager, nonEventUserNotificationManager, emailSender, emailManager, mapRepository, logger);
            await upcomingEventAttendingThisWeekNotifier.GenerateNotificationsAsync().ConfigureAwait(false);

            var upcomingEventAttendingSoonNotifier = new UpcomingEventAttendingSoonNotifier(eventManager, userManager, eventAttendeeManager, userNotificationManager, nonEventUserNotificationManager, emailSender, emailManager, mapRepository, logger);
            await upcomingEventAttendingSoonNotifier.GenerateNotificationsAsync().ConfigureAwait(false);

            var upcomingEventHostingThisWeekNotifier = new UpcomingEventHostingThisWeekNotifier(eventManager, userManager, eventAttendeeManager, userNotificationManager, nonEventUserNotificationManager, emailSender, emailManager, mapRepository, logger);
            await upcomingEventHostingThisWeekNotifier.GenerateNotificationsAsync().ConfigureAwait(false);

            var upcomingEventHostingSoonNotifier = new UpcomingEventHostingSoonNotifier(eventManager, userManager, eventAttendeeManager, userNotificationManager, nonEventUserNotificationManager, emailSender, emailManager, mapRepository, logger);
            await upcomingEventHostingSoonNotifier.GenerateNotificationsAsync().ConfigureAwait(false);

            var upcomingEventsInYourAreaThisWeekNotifier = new UpcomingEventsInYourAreaThisWeekNotifier(eventManager, userManager, eventAttendeeManager, userNotificationManager, nonEventUserNotificationManager, emailSender, emailManager, mapRepository, logger);
            await upcomingEventsInYourAreaThisWeekNotifier.GenerateNotificationsAsync().ConfigureAwait(false);

            var upcomingEventsInYourAreaSoonNotifier = new UpcomingEventsInYourAreaSoonNotifier(eventManager, userManager, eventAttendeeManager, userNotificationManager, nonEventUserNotificationManager, emailSender, emailManager, mapRepository, logger);
            await upcomingEventsInYourAreaSoonNotifier.GenerateNotificationsAsync().ConfigureAwait(false);

            var userProfileLocationNotifier = new UserProfileLocationNotifier(eventManager, userManager, eventAttendeeManager, userNotificationManager, nonEventUserNotificationManager, emailSender, emailManager, mapRepository, logger);
            await userProfileLocationNotifier.GenerateNotificationsAsync().ConfigureAwait(false);

            logger.LogInformation("Completed RunAllNotifications");
        }
    }
}
