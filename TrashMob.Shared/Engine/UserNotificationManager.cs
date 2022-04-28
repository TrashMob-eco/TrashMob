namespace TrashMob.Shared.Engine
{
    using Microsoft.Extensions.Logging;
    using System.Threading.Tasks;
    using TrashMob.Shared.Persistence;

    public class UserNotificationManager : IUserNotificationManager 
    {
        private readonly IEventRepository eventRepository;
        private readonly IUserRepository userRepository;
        private readonly IEventAttendeeRepository eventAttendeeRepository;
        private readonly IUserNotificationRepository userNotificationRepository;
        private readonly IUserNotificationPreferenceRepository userNotificationPreferenceRepository;
        private readonly IEmailSender emailSender;
        private readonly IEmailManager emailManager;
        private readonly IMapRepository mapRepository;
        private readonly ILogger<UserNotificationManager> logger;

        public UserNotificationManager(IEventRepository eventRepository,
                                       IUserRepository userRepository,
                                       IEventAttendeeRepository eventAttendeeRepository,
                                       IUserNotificationRepository userNotificationRepository,
                                       IUserNotificationPreferenceRepository userNotificationPreferenceRepository,
                                       IEmailSender emailSender,
                                       IEmailManager emailManager,
                                       IMapRepository mapRepository,
                                       ILogger<UserNotificationManager> logger)
        {
            this.eventRepository = eventRepository;
            this.userRepository = userRepository;
            this.eventAttendeeRepository = eventAttendeeRepository;
            this.userNotificationRepository = userNotificationRepository;
            this.userNotificationPreferenceRepository = userNotificationPreferenceRepository;
            this.emailSender = emailSender;
            this.emailManager = emailManager;
            this.mapRepository = mapRepository;
            this.logger = logger;
        }

        public async Task RunAllNotificatons()
        {
            logger.LogInformation("Starting RunAllNotifications");

            var eventSummaryAttendeeNotifier = new EventSummaryAttendeeNotifier(eventRepository, userRepository, eventAttendeeRepository, userNotificationRepository, userNotificationPreferenceRepository, emailSender, emailManager, mapRepository, logger);
            await eventSummaryAttendeeNotifier.GenerateNotificationsAsync().ConfigureAwait(false);

            var eventSummaryHostReminderNotifier = new EventSummaryHostReminderNotifier(eventRepository, userRepository, eventAttendeeRepository, userNotificationRepository, userNotificationPreferenceRepository, emailSender, emailManager, mapRepository, logger);
            await eventSummaryHostReminderNotifier.GenerateNotificationsAsync().ConfigureAwait(false);

            var upcomingEventAttendingThisWeekNotifier = new UpcomingEventAttendingThisWeekNotifier(eventRepository, userRepository, eventAttendeeRepository, userNotificationRepository, userNotificationPreferenceRepository, emailSender, emailManager, mapRepository, logger);
            await upcomingEventAttendingThisWeekNotifier.GenerateNotificationsAsync().ConfigureAwait(false);

            var upcomingEventAttendingSoonNotifier = new UpcomingEventAttendingSoonNotifier(eventRepository, userRepository, eventAttendeeRepository, userNotificationRepository, userNotificationPreferenceRepository, emailSender, emailManager, mapRepository, logger);
            await upcomingEventAttendingSoonNotifier.GenerateNotificationsAsync().ConfigureAwait(false);

            var upcomingEventHostingThisWeekNotifier = new UpcomingEventHostingThisWeekNotifier(eventRepository, userRepository, eventAttendeeRepository, userNotificationRepository, userNotificationPreferenceRepository, emailSender, emailManager, mapRepository, logger);
            await upcomingEventHostingThisWeekNotifier.GenerateNotificationsAsync().ConfigureAwait(false);

            var upcomingEventHostingSoonNotifier = new UpcomingEventHostingSoonNotifier(eventRepository, userRepository, eventAttendeeRepository, userNotificationRepository, userNotificationPreferenceRepository, emailSender, emailManager, mapRepository, logger);
            await upcomingEventHostingSoonNotifier.GenerateNotificationsAsync().ConfigureAwait(false);

            var upcomingEventsInYourAreaThisWeekNotifier = new UpcomingEventsInYourAreaThisWeekNotifier(eventRepository, userRepository, eventAttendeeRepository, userNotificationRepository, userNotificationPreferenceRepository, emailSender, emailManager, mapRepository, logger);
            await upcomingEventsInYourAreaThisWeekNotifier.GenerateNotificationsAsync().ConfigureAwait(false);

            var upcomingEventsInYourAreaSoonNotifier = new UpcomingEventsInYourAreaSoonNotifier(eventRepository, userRepository, eventAttendeeRepository, userNotificationRepository, userNotificationPreferenceRepository, emailSender, emailManager, mapRepository, logger);
            await upcomingEventsInYourAreaSoonNotifier.GenerateNotificationsAsync().ConfigureAwait(false);

            logger.LogInformation("Completed RunAllNotifications");
        }
    }
}
