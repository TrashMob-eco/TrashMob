namespace TrashMob.Shared.Engine
{
    using Microsoft.Extensions.Logging;
    using TrashMob.Models;
    using TrashMob.Shared.Managers.Interfaces;

    /// <summary>
    /// Notification engine that notifies users about events in their area happening within the next 24 hours.
    /// </summary>
    public class UpcomingEventsInYourAreaSoonNotifier(
        IEventManager eventManager,
        IKeyedManager<User> userManager,
        IEventAttendeeManager eventAttendeeManager,
        IKeyedManager<UserNotification> userNotificationManager,
        INonEventUserNotificationManager nonEventUserNotificationManager,
        IEmailSender emailSender,
        IEmailManager emailManager,
        IMapManager mapRepository,
        ILogger logger)
        : UpcomingEventsInYourAreaBaseNotifier(eventManager, userManager, eventAttendeeManager, userNotificationManager,
            nonEventUserNotificationManager, emailSender, emailManager, mapRepository, logger), INotificationEngine
    {

        /// <inheritdoc />
        protected override NotificationTypeEnum NotificationType => NotificationTypeEnum.UpcomingEventsInYourAreaSoon;

        /// <inheritdoc />
        protected override string EmailSubject => "Upcoming TrashMob.eco events in your area soon!";

        /// <inheritdoc />
        protected override int MaxNumberOfHoursInWindow => 24;

        /// <inheritdoc />
        protected override int MinNumberOfHoursInWindow => 1;
    }
}