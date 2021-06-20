namespace TrashMob.Shared.Engine
{
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

        public UserNotificationManager(IEventRepository eventRepository,
                                       IUserRepository userRepository,
                                       IEventAttendeeRepository eventAttendeeRepository,
                                       IUserNotificationRepository userNotificationRepository,
                                       IUserNotificationPreferenceRepository userNotificationPreferenceRepository,
                                       IEmailSender emailSender)
        {
            this.eventRepository = eventRepository;
            this.userRepository = userRepository;
            this.eventAttendeeRepository = eventAttendeeRepository;
            this.userNotificationRepository = userNotificationRepository;
            this.userNotificationPreferenceRepository = userNotificationPreferenceRepository;
            this.emailSender = emailSender;
        }

        public async Task RunAllNotificatons()
        {
            var eventSummaryAttendeeNotifier = new EventSummaryAttendeeNotifier(eventRepository, userRepository, eventAttendeeRepository, userNotificationRepository, userNotificationPreferenceRepository, emailSender);
            await eventSummaryAttendeeNotifier.GenerateNotificationsAsync().ConfigureAwait(false);

            var eventSummaryHostReminderNotifier = new EventSummaryHostReminderNotifier(eventRepository, userRepository, eventAttendeeRepository, userNotificationRepository, userNotificationPreferenceRepository, emailSender);
            await eventSummaryHostReminderNotifier.GenerateNotificationsAsync().ConfigureAwait(false);

            var upcomingEventAttendingThisWeekNotifier = new UpcomingEventAttendingThisWeekNotifier(eventRepository, userRepository, eventAttendeeRepository, userNotificationRepository, userNotificationPreferenceRepository, emailSender);
            await upcomingEventAttendingThisWeekNotifier.GenerateNotificationsAsync().ConfigureAwait(false);

            var upcomingEventAttendingTodayNotifier = new UpcomingEventAttendingTodayNotifier(eventRepository, userRepository, eventAttendeeRepository, userNotificationRepository, userNotificationPreferenceRepository, emailSender);
            await upcomingEventAttendingTodayNotifier.GenerateNotificationsAsync().ConfigureAwait(false);

            var upcomingEventHostingThisWeekNotifier = new UpcomingEventHostingThisWeekNotifier(eventRepository, userRepository, eventAttendeeRepository, userNotificationRepository, userNotificationPreferenceRepository, emailSender);
            await upcomingEventHostingThisWeekNotifier.GenerateNotificationsAsync().ConfigureAwait(false);

            var upcomingEventHostingTodayNotifier = new UpcomingEventHostingTodayNotifier(eventRepository, userRepository, eventAttendeeRepository, userNotificationRepository, userNotificationPreferenceRepository, emailSender);
            await upcomingEventHostingTodayNotifier.GenerateNotificationsAsync().ConfigureAwait(false);

            var upcomingEventsInYourAreaThisWeekNotifier = new UpcomingEventsInYourAreaThisWeekNotifier(eventRepository, userRepository, eventAttendeeRepository, userNotificationRepository, userNotificationPreferenceRepository, emailSender);
            await upcomingEventsInYourAreaThisWeekNotifier.GenerateNotificationsAsync().ConfigureAwait(false);

            var upcomingEventsInYourAreaTodayNotifier = new UpcomingEventsInYourAreaTodayNotifier(eventRepository, userRepository, eventAttendeeRepository, userNotificationRepository, userNotificationPreferenceRepository, emailSender);
            await upcomingEventsInYourAreaTodayNotifier.GenerateNotificationsAsync().ConfigureAwait(false);
        }
    }
}
