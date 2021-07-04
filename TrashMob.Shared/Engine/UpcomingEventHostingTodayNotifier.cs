
namespace TrashMob.Shared.Engine
{
    using Microsoft.Extensions.Logging;
    using TrashMob.Shared.Persistence;

    public class UpcomingEventHostingTodayNotifier : UpcomingEventHostingBaseNotifier, INotificationEngine
    {
        protected override NotificationTypeEnum NotificationType => NotificationTypeEnum.UpcomingEventHostingToday;

        protected override int NumberOfHoursInWindow => 24;

        protected override string EmailSubject => "You're hosting a TrashMob.eco event today!";

        public UpcomingEventHostingTodayNotifier(IEventRepository eventRepository, 
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
