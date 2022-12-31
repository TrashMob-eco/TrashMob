
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

        public UpcomingEventsInYourAreaThisWeekNotifier(IEventManager eventManager, 
                                                        IKeyedManager<User> userManager, 
                                                        IEventAttendeeManager eventAttendeeManager,
                                                        IKeyedManager<UserNotification> userNotificationManager,
                                                        INonEventUserNotificationManager nonEventUserNotificationManager,
                                                        IEmailSender emailSender,
                                                        IEmailManager emailManager,
                                                        IMapManager mapRepository,
                                                        ILogger logger) :
            base(eventManager, userManager, eventAttendeeManager, userNotificationManager, nonEventUserNotificationManager, emailSender, emailManager, mapRepository, logger)
        {
        }
    }
}
