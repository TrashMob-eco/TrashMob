
namespace TrashMob.Shared.Engine
{
    using Microsoft.Extensions.Logging;
    using TrashMob.Shared.Persistence;

    public class UpcomingEventsInYourAreaTodayNotifier : UpcomingEventsInYourAreaBaseNotifier, INotificationEngine
    {
        protected override NotificationTypeEnum NotificationType => NotificationTypeEnum.UpcomingEventsInYourAreaToday;

        protected override string EmailSubject => "Upcoming TrashMob.eco events in your area today!";

        protected override int NumberOfHoursInWindow => 24;

        public UpcomingEventsInYourAreaTodayNotifier(IEventRepository eventRepository, 
                                                     IUserRepository userRepository, 
                                                     IEventAttendeeRepository eventAttendeeRepository, 
                                                     IUserNotificationRepository userNotificationRepository,
                                                     IUserNotificationPreferenceRepository userNotificationPreferenceRepository,
                                                     IEmailSender emailSender,
                                                     IMapRepository mapRepository,
                                                     ILogger logger) : 
            base(eventRepository, userRepository, eventAttendeeRepository, userNotificationRepository, userNotificationPreferenceRepository, emailSender, mapRepository, logger)
        {
        }
    }
}
