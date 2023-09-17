
namespace TrashMob.Shared.Engine
{
    using Microsoft.Extensions.Logging;
    using TrashMob.Models;
    using TrashMob.Shared.Managers.Interfaces;

    public class UpcomingEventHostingSoonNotifier : UpcomingEventHostingBaseNotifier, INotificationEngine
    {
        protected override NotificationTypeEnum NotificationType => NotificationTypeEnum.UpcomingEventHostingSoon;

        protected override int MaxNumberOfHoursInWindow => 24;

        protected override int MinNumberOfHoursInWindow => 1;

        protected override string EmailSubject => "You're hosting a TrashMob.eco event soon!";

        public UpcomingEventHostingSoonNotifier(IEventManager eventManager, 
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
