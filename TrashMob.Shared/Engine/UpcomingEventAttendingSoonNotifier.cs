namespace TrashMob.Shared.Engine
{
    using Microsoft.Extensions.Logging;
    using TrashMob.Models;
    using TrashMob.Shared.Managers.Interfaces;

    /// <summary>
    /// Notification engine that reminds users about events they are attending within the next 24 hours.
    /// </summary>
    public class UpcomingEventAttendingSoonNotifier : UpcomingEventAttendingBaseNotifier, INotificationEngine
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="UpcomingEventAttendingSoonNotifier"/> class.
        /// </summary>
        public UpcomingEventAttendingSoonNotifier(IEventManager eventManager,
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

        /// <inheritdoc />
        protected override NotificationTypeEnum NotificationType => NotificationTypeEnum.UpcomingEventAttendingSoon;

        /// <inheritdoc />
        protected override int MaxNumberOfHoursInWindow => 24;

        /// <inheritdoc />
        protected override int MinNumberOfHoursInWindow => 1;

        /// <inheritdoc />
        protected override string EmailSubject => "You're attending a TrashMob.eco event soon!";
    }
}