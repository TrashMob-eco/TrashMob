
namespace TrashMob.Shared.Engine
{
    using Microsoft.Extensions.Logging;
    using TrashMob.Shared.Persistence;

    public class UpcomingEventsInYourAreaThisWeekNotifier : UpcomingEventsInYourAreaBaseNotifier, INotificationEngine
    {
        protected override NotificationTypeEnum NotificationType => NotificationTypeEnum.UpcomingEventsInYourAreaThisWeek;

        protected override int NumberOfHoursInWindow => 7 * 24;

        protected override string EmailSubject => "Upcoming TrashMob.eco events in your area this week!";

        public UpcomingEventsInYourAreaThisWeekNotifier(IEventRepository eventRepository, 
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
