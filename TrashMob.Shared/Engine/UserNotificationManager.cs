namespace TrashMob.Shared.Engine
{
    using Microsoft.Extensions.Logging;
    using System.Threading.Tasks;
    using TrashMob.Shared.Persistence;

    public class UserNotificationManager
    {
        private readonly IEventRepository eventRepository;
        private readonly IUserRepository userRepository;
        private readonly IEventAttendeeRepository eventAttendeeRepository;
        private readonly IUserNotificationRepository userNotificationRepository;
        private readonly IUserNotificationPreferenceRepository userNotificationPreferenceRepository;
        private readonly IEmailSender emailSender;
        private readonly IMapRepository mapRepository;
        private readonly ILogger logger;

        public UserNotificationManager(IEventRepository eventRepository,
                                       IUserRepository userRepository,
                                       IEventAttendeeRepository eventAttendeeRepository,
                                       IUserNotificationRepository userNotificationRepository,
                                       IUserNotificationPreferenceRepository userNotificationPreferenceRepository,
                                       IEmailSender emailSender,
                                       IMapRepository mapRepository,
                                       ILogger logger)
        {
            this.eventRepository = eventRepository;
            this.userRepository = userRepository;
            this.eventAttendeeRepository = eventAttendeeRepository;
            this.userNotificationRepository = userNotificationRepository;
            this.userNotificationPreferenceRepository = userNotificationPreferenceRepository;
            this.emailSender = emailSender;
            this.mapRepository = mapRepository;
            this.logger = logger;
        }

        public async Task RunAllNotificatons()
        {
            logger.LogInformation("Starting RunAllNotifications");

            var eventSummaryAttendeeNotifier = new EventSummaryAttendeeNotifier(eventRepository, userRepository, eventAttendeeRepository, userNotificationRepository, userNotificationPreferenceRepository, emailSender, mapRepository, logger);
            await eventSummaryAttendeeNotifier.GenerateNotificationsAsync().ConfigureAwait(false);

            var eventSummaryHostReminderNotifier = new EventSummaryHostReminderNotifier(eventRepository, userRepository, eventAttendeeRepository, userNotificationRepository, userNotificationPreferenceRepository, emailSender, mapRepository, logger);
            await eventSummaryHostReminderNotifier.GenerateNotificationsAsync().ConfigureAwait(false);

            var upcomingEventAttendingThisWeekNotifier = new UpcomingEventAttendingThisWeekNotifier(eventRepository, userRepository, eventAttendeeRepository, userNotificationRepository, userNotificationPreferenceRepository, emailSender, mapRepository, logger);
            await upcomingEventAttendingThisWeekNotifier.GenerateNotificationsAsync().ConfigureAwait(false);

            var upcomingEventAttendingTodayNotifier = new UpcomingEventAttendingTodayNotifier(eventRepository, userRepository, eventAttendeeRepository, userNotificationRepository, userNotificationPreferenceRepository, emailSender, mapRepository, logger);
            await upcomingEventAttendingTodayNotifier.GenerateNotificationsAsync().ConfigureAwait(false);

            var upcomingEventHostingThisWeekNotifier = new UpcomingEventHostingThisWeekNotifier(eventRepository, userRepository, eventAttendeeRepository, userNotificationRepository, userNotificationPreferenceRepository, emailSender, mapRepository, logger);
            await upcomingEventHostingThisWeekNotifier.GenerateNotificationsAsync().ConfigureAwait(false);

            var upcomingEventHostingTodayNotifier = new UpcomingEventHostingTodayNotifier(eventRepository, userRepository, eventAttendeeRepository, userNotificationRepository, userNotificationPreferenceRepository, emailSender, mapRepository, logger);
            await upcomingEventHostingTodayNotifier.GenerateNotificationsAsync().ConfigureAwait(false);

            var upcomingEventsInYourAreaThisWeekNotifier = new UpcomingEventsInYourAreaThisWeekNotifier(eventRepository, userRepository, eventAttendeeRepository, userNotificationRepository, userNotificationPreferenceRepository, emailSender, mapRepository, logger);
            await upcomingEventsInYourAreaThisWeekNotifier.GenerateNotificationsAsync().ConfigureAwait(false);

            var upcomingEventsInYourAreaTodayNotifier = new UpcomingEventsInYourAreaTodayNotifier(eventRepository, userRepository, eventAttendeeRepository, userNotificationRepository, userNotificationPreferenceRepository, emailSender, mapRepository, logger);
            await upcomingEventsInYourAreaTodayNotifier.GenerateNotificationsAsync().ConfigureAwait(false);

            logger.LogInformation("Completed RunAllNotifications");
        }
    }
}
