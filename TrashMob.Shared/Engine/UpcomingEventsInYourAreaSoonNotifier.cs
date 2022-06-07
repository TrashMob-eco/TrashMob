
namespace TrashMob.Shared.Engine
{
    using Microsoft.Extensions.Logging;
    using TrashMob.Shared.Persistence;

    public class UpcomingEventsInYourAreaSoonNotifier : UpcomingEventsInYourAreaBaseNotifier, INotificationEngine
    {
        protected override NotificationTypeEnum NotificationType => NotificationTypeEnum.UpcomingEventsInYourAreaSoon;

        protected override string EmailSubject => "Upcoming TrashMob.eco events in your area soon!";

        protected override int NumberOfHoursInWindow => 24;

        public UpcomingEventsInYourAreaSoonNotifier(IEventRepository eventRepository, 
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
