namespace TrashMob.Shared.Engine
{
    using Microsoft.Extensions.Logging;
    using TrashMob.Models;
    using TrashMob.Shared.Managers.Interfaces;

    /// <summary>
    /// Notification engine that notifies users about events in their area happening within the next week (2-7 days).
    /// </summary>
    public class UpcomingEventsInYourAreaThisWeekNotifier(
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
        protected override NotificationTypeEnum NotificationType =>
            NotificationTypeEnum.UpcomingEventsInYourAreaThisWeek;

        /// <inheritdoc />
        protected override int MaxNumberOfHoursInWindow => 7 * 24;

        /// <inheritdoc />
        protected override int MinNumberOfHoursInWindow => 2 * 24;

        /// <inheritdoc />
        protected override string EmailSubject => "Upcoming TrashMob.eco events in your area this week!";
    }
}