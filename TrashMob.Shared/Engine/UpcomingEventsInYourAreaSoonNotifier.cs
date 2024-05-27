namespace TrashMob.Shared.Engine
{
    using Microsoft.Extensions.Logging;
    using TrashMob.Models;
    using TrashMob.Shared.Managers.Interfaces;

    public class UpcomingEventsInYourAreaSoonNotifier : UpcomingEventsInYourAreaBaseNotifier, INotificationEngine
    {
        public UpcomingEventsInYourAreaSoonNotifier(IEventManager eventManager,
            IKeyedManager<User> userManager,
            IEventAttendeeManager eventAttendeeManager,
            IKeyedManager<UserNotification> userNotificationManager,
            INonEventUserNotificationManager nonEventUserNotificationManager,
            IEmailSender emailSender,
            IEmailManager emailManager,
            IMapManager mapRepository,
            ILogger logger) :
            base(eventManager, userManager, eventAttendeeManager, userNotificationManager,
                nonEventUserNotificationManager, emailSender, emailManager, mapRepository, logger)
        {
        }

        protected override NotificationTypeEnum NotificationType => NotificationTypeEnum.UpcomingEventsInYourAreaSoon;

        protected override string EmailSubject => "Upcoming TrashMob.eco events in your area soon!";

        protected override int MaxNumberOfHoursInWindow => 24;

        protected override int MinNumberOfHoursInWindow => 1;
    }
}