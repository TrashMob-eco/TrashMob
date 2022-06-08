
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
                                                        INonEventUserNotificationRepository nonEventUserNotificationRepository,
                                                        IEmailSender emailSender,
                                                        IEmailManager emailManager,
                                                        IMapRepository mapRepository,
                                                        ILogger logger) :
            base(eventRepository, userRepository, eventAttendeeRepository, userNotificationRepository, nonEventUserNotificationRepository, emailSender, emailManager, mapRepository, logger)
        {
        }
    }
}
