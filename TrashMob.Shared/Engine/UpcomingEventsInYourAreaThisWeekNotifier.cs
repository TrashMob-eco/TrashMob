namespace TrashMob.Shared.Engine
{
    using Microsoft.Extensions.Logging;
    using TrashMob.Models;
    using TrashMob.Shared.Managers.Interfaces;

    public class UpcomingEventsInYourAreaThisWeekNotifier : UpcomingEventsInYourAreaBaseNotifier, INotificationEngine
    {
        public UpcomingEventsInYourAreaThisWeekNotifier(IEventManager eventManager,
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

        protected override NotificationTypeEnum NotificationType =>
            NotificationTypeEnum.UpcomingEventsInYourAreaThisWeek;

        protected override int MaxNumberOfHoursInWindow => 7 * 24;

        protected override int MinNumberOfHoursInWindow => 2 * 24;

        protected override string EmailSubject => "Upcoming TrashMob.eco events in your area this week!";
    }
}