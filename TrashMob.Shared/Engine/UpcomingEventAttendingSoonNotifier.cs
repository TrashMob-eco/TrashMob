
namespace TrashMob.Shared.Engine
{
    using Microsoft.Extensions.Logging;
    using TrashMob.Models;
    using TrashMob.Shared.Managers.Interfaces;
    using TrashMob.Shared.Persistence.Interfaces;

    public class UpcomingEventAttendingSoonNotifier : UpcomingEventAttendingBaseNotifier, INotificationEngine
    {
        protected override NotificationTypeEnum NotificationType => NotificationTypeEnum.UpcomingEventAttendingSoon;

        protected override int NumberOfHoursInWindow => 24;

        protected override string EmailSubject => "You're attending a TrashMob.eco event soon!";

        public UpcomingEventAttendingSoonNotifier(IEventManager eventManager,
                                                  IKeyedManager<User> userManager, 
                                                  IEventAttendeeManager eventAttendeeManager,
                                                  IKeyedManager<UserNotification> userNotificationManager,
                                                  IKeyedManager<NonEventUserNotification> nonEventUserNotificationManager,
                                                  IEmailSender emailSender,
                                                  IEmailManager emailManager,
                                                  IMapManager mapRepository,
                                                  ILogger logger) :
            base(eventManager, userManager, eventAttendeeManager, userNotificationManager, nonEventUserNotificationManager, emailSender, emailManager, mapRepository, logger)
        {
        }
    }
}
