namespace TrashMob.Shared.Engine
{
    using Microsoft.Extensions.Logging;
    using TrashMob.Models;
    using TrashMob.Shared.Managers.Interfaces;

    /// <summary>
    /// Notification engine that reminds event hosts about events they are hosting within the next 24 hours.
    /// </summary>
    public class UpcomingEventHostingSoonNotifier : UpcomingEventHostingBaseNotifier, INotificationEngine
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="UpcomingEventHostingSoonNotifier"/> class.
        /// </summary>
        public UpcomingEventHostingSoonNotifier(IEventManager eventManager,
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
        protected override NotificationTypeEnum NotificationType => NotificationTypeEnum.UpcomingEventHostingSoon;

        /// <inheritdoc />
        protected override int MaxNumberOfHoursInWindow => 24;

        /// <inheritdoc />
        protected override int MinNumberOfHoursInWindow => 1;

        /// <inheritdoc />
        protected override string EmailSubject => "You're hosting a TrashMob.eco event soon!";
    }
}