
namespace TrashMob.Shared.Engine
{
    using Microsoft.Extensions.Logging;
    using TrashMob.Models;
    using TrashMob.Shared.Managers.Interfaces;
    using TrashMob.Shared.Persistence.Interfaces;

    public class UpcomingEventsInYourAreaThisWeekNotifier : UpcomingEventsInYourAreaBaseNotifier, INotificationEngine
    {
        protected override NotificationTypeEnum NotificationType => NotificationTypeEnum.UpcomingEventsInYourAreaThisWeek;

        protected override int NumberOfHoursInWindow => 7 * 24;

        protected override string EmailSubject => "Upcoming TrashMob.eco events in your area this week!";

        public UpcomingEventsInYourAreaThisWeekNotifier(IEventRepository eventRepository, 
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
