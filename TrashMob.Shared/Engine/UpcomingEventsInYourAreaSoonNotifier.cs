
namespace TrashMob.Shared.Engine
{
    using Microsoft.Extensions.Logging;
    using TrashMob.Models;
    using TrashMob.Shared.Managers.Interfaces;
    using TrashMob.Shared.Persistence.Interfaces;

    public class UpcomingEventsInYourAreaSoonNotifier : UpcomingEventsInYourAreaBaseNotifier, INotificationEngine
    {
        protected override NotificationTypeEnum NotificationType => NotificationTypeEnum.UpcomingEventsInYourAreaSoon;

        protected override string EmailSubject => "Upcoming TrashMob.eco events in your area soon!";

        protected override int NumberOfHoursInWindow => 24;

        public UpcomingEventsInYourAreaSoonNotifier(IEventRepository eventRepository, 
                                                    IKeyedManager<User> userManager, 
                                                    IEventAttendeeRepository eventAttendeeRepository,
                                                    IKeyedManager<UserNotification> userNotificationManager,
                                                    IKeyedManager<NonEventUserNotification> nonEventUserNotificationManager,
                                                    IEmailSender emailSender,
                                                    IEmailManager emailManager,
                                                    IMapRepository mapRepository,
                                                    ILogger logger) :
            base(eventRepository, userManager, eventAttendeeRepository, userNotificationManager, nonEventUserNotificationManager, emailSender, emailManager, mapRepository, logger)
        {
        }
    }
}
