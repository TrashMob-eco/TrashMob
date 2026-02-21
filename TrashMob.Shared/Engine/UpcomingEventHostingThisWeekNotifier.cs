namespace TrashMob.Shared.Engine
{
    using Microsoft.Extensions.Logging;
    using TrashMob.Models;
    using TrashMob.Shared.Managers.Interfaces;

    /// <summary>
    /// Notification engine that reminds event hosts about events they are hosting within the next week (2-7 days).
    /// </summary>
    public class UpcomingEventHostingThisWeekNotifier(
        IEventManager eventManager,
        IKeyedManager<User> userManager,
        IEventAttendeeManager eventAttendeeManager,
        IKeyedManager<UserNotification> userNotificationManager,
        INonEventUserNotificationManager nonEventUserNotificationManager,
        IEmailSender emailSender,
        IEmailManager emailManager,
        IMapManager mapRepository,
        ILogger logger)
        : UpcomingEventHostingBaseNotifier(eventManager, userManager, eventAttendeeManager, userNotificationManager,
            nonEventUserNotificationManager, emailSender, emailManager, mapRepository, logger), INotificationEngine
    {

        /// <inheritdoc />
        protected override NotificationTypeEnum NotificationType => NotificationTypeEnum.UpcomingEventHostingThisWeek;

        /// <inheritdoc />
        protected override int MaxNumberOfHoursInWindow => 7 * 24;

        /// <inheritdoc />
        protected override int MinNumberOfHoursInWindow => 2 * 24;

        /// <inheritdoc />
        protected override string EmailSubject => "You're hosting a TrashMob.eco event this week!";
    }
}