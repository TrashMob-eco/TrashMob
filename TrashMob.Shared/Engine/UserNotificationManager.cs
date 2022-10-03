namespace TrashMob.Shared.Engine
{
    using Microsoft.Extensions.Logging;
    using System.Threading.Tasks;
    using TrashMob.Models;
    using TrashMob.Shared.Managers.Interfaces;
    using TrashMob.Shared.Persistence.Interfaces;

    public class UserNotificationManager : IUserNotificationManager 
    {
        private readonly IEventRepository eventRepository;
        private readonly IKeyedManager<User> userManager;
        private readonly IEventAttendeeRepository eventAttendeeRepository;
        private readonly IKeyedManager<UserNotification> userNotificationManager;
        private readonly IKeyedManager<NonEventUserNotification> nonEventUserNotificationManager;
        private readonly IEmailSender emailSender;
        private readonly IEmailManager emailManager;
        private readonly IMapRepository mapRepository;
        private readonly IEventSummaryRepository eventSummaryRepository;
        private readonly ILogger<UserNotificationManager> logger;

        public UserNotificationManager(IEventRepository eventRepository,
                                       IKeyedManager<User> userManager,
                                       IEventAttendeeRepository eventAttendeeRepository,
                                       IKeyedManager<UserNotification> userNotificationManager,
                                       IKeyedManager<NonEventUserNotification> nonEventUserNotificationManager,
                                       IEmailSender emailSender,
                                       IEmailManager emailManager,
                                       IMapRepository mapRepository,
                                       IEventSummaryRepository eventSummaryRepository,
                                       ILogger<UserNotificationManager> logger)
        {
            this.eventRepository = eventRepository;
            this.userManager = userManager;
            this.eventAttendeeRepository = eventAttendeeRepository;
            this.userNotificationManager = userNotificationManager;
            this.nonEventUserNotificationManager = nonEventUserNotificationManager;
            this.emailSender = emailSender;
            this.emailManager = emailManager;
            this.mapRepository = mapRepository;
            this.eventSummaryRepository = eventSummaryRepository;
            this.logger = logger;
        }

        public async Task RunAllNotificatons()
        {
            logger.LogInformation("Starting RunAllNotifications");

            var eventSummaryAttendeeNotifier = new EventSummaryAttendeeNotifier(eventRepository, userManager, eventAttendeeRepository, userNotificationManager, nonEventUserNotificationManager, emailSender, emailManager, mapRepository, logger);
            await eventSummaryAttendeeNotifier.GenerateNotificationsAsync().ConfigureAwait(false);

            var eventSummaryHostReminderNotifier = new EventSummaryHostReminderNotifier(eventRepository, userManager, eventAttendeeRepository, userNotificationManager, nonEventUserNotificationManager, emailSender, emailManager, mapRepository, logger);
            await eventSummaryHostReminderNotifier.GenerateNotificationsAsync().ConfigureAwait(false);

            var eventSummaryHostWeekReminderNotifier = new EventSummaryHostWeekReminderNotifier(eventRepository, userManager, eventAttendeeRepository, userNotificationManager, nonEventUserNotificationManager, emailSender, emailManager, mapRepository, eventSummaryRepository, logger);
            await eventSummaryHostWeekReminderNotifier.GenerateNotificationsAsync().ConfigureAwait(false);

            var upcomingEventAttendingThisWeekNotifier = new UpcomingEventAttendingThisWeekNotifier(eventRepository, userManager, eventAttendeeRepository, userNotificationManager, nonEventUserNotificationManager, emailSender, emailManager, mapRepository, logger);
            await upcomingEventAttendingThisWeekNotifier.GenerateNotificationsAsync().ConfigureAwait(false);

            var upcomingEventAttendingSoonNotifier = new UpcomingEventAttendingSoonNotifier(eventRepository, userManager, eventAttendeeRepository, userNotificationManager, nonEventUserNotificationManager, emailSender, emailManager, mapRepository, logger);
            await upcomingEventAttendingSoonNotifier.GenerateNotificationsAsync().ConfigureAwait(false);

            var upcomingEventHostingThisWeekNotifier = new UpcomingEventHostingThisWeekNotifier(eventRepository, userManager, eventAttendeeRepository, userNotificationManager, nonEventUserNotificationManager, emailSender, emailManager, mapRepository, logger);
            await upcomingEventHostingThisWeekNotifier.GenerateNotificationsAsync().ConfigureAwait(false);

            var upcomingEventHostingSoonNotifier = new UpcomingEventHostingSoonNotifier(eventRepository, userManager, eventAttendeeRepository, userNotificationManager, nonEventUserNotificationManager, emailSender, emailManager, mapRepository, logger);
            await upcomingEventHostingSoonNotifier.GenerateNotificationsAsync().ConfigureAwait(false);

            var upcomingEventsInYourAreaThisWeekNotifier = new UpcomingEventsInYourAreaThisWeekNotifier(eventRepository, userManager, eventAttendeeRepository, userNotificationManager, nonEventUserNotificationManager, emailSender, emailManager, mapRepository, logger);
            await upcomingEventsInYourAreaThisWeekNotifier.GenerateNotificationsAsync().ConfigureAwait(false);

            var upcomingEventsInYourAreaSoonNotifier = new UpcomingEventsInYourAreaSoonNotifier(eventRepository, userManager, eventAttendeeRepository, userNotificationManager, nonEventUserNotificationManager, emailSender, emailManager, mapRepository, logger);
            await upcomingEventsInYourAreaSoonNotifier.GenerateNotificationsAsync().ConfigureAwait(false);

            var userProfileLocationNotifier = new UserProfileLocationNotifier(eventRepository, userManager, eventAttendeeRepository, userNotificationManager, nonEventUserNotificationManager, emailSender, emailManager, mapRepository, logger);
            await userProfileLocationNotifier.GenerateNotificationsAsync().ConfigureAwait(false);

            logger.LogInformation("Completed RunAllNotifications");
        }
    }
}
