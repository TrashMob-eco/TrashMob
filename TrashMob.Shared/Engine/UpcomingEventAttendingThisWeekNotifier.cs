
namespace TrashMob.Shared.Engine
{
    using Microsoft.Extensions.Logging;
    using TrashMob.Models;
    using TrashMob.Shared.Managers.Interfaces;
    using TrashMob.Shared.Persistence.Interfaces;

    public class UpcomingEventAttendingThisWeekNotifier : UpcomingEventAttendingBaseNotifier, INotificationEngine
    {
        protected override NotificationTypeEnum NotificationType { get { return NotificationTypeEnum.UpcomingEventAttendingThisWeek; } }

        protected override int MaxNumberOfHoursInWindow => 7 * 24;

        protected override int MinNumberOfHoursInWindow => 2 * 24;

        protected override string EmailSubject => "You're attending a TrashMob.eco event this week!";

        public UpcomingEventAttendingThisWeekNotifier(IEventManager eventManager, 
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
