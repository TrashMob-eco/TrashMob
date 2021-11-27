
namespace TrashMob.Shared.Engine
{
    using Microsoft.Extensions.Logging;
    using TrashMob.Shared.Persistence;

    public class UpcomingEventAttendingTodayNotifier : UpcomingEventAttendingBaseNotifier, INotificationEngine
    {
        protected override NotificationTypeEnum NotificationType => NotificationTypeEnum.UpcomingEventAttendingToday;

        protected override int NumberOfHoursInWindow => 24;

        protected override string EmailSubject => "You're attending a TrashMob.eco event today!";

        public UpcomingEventAttendingTodayNotifier(IEventRepository eventRepository,
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
