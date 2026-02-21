namespace TrashMob.Shared.Engine
{
    using System.Linq;
    using System.Threading;
    using System.Threading.Tasks;
    using Microsoft.Extensions.Logging;
    using TrashMob.Models;
    using TrashMob.Shared.Managers.Interfaces;

    /// <summary>
    /// Notification engine that reminds users when their waivers are expiring.
    /// </summary>
    public class WaiverExpiringNotifier(
        IEventManager eventManager,
        IKeyedManager<User> userManager,
        IEventAttendeeManager eventAttendeeManager,
        IKeyedManager<UserNotification> userNotificationManager,
        INonEventUserNotificationManager nonEventUserNotificationManager,
        IEmailSender emailSender,
        IEmailManager emailManager,
        IMapManager mapRepository,
        IUserWaiverManager userWaiverManager,
        ILogger logger)
        : NotificationEngineBase(eventManager, userManager, eventAttendeeManager, userNotificationManager,
            nonEventUserNotificationManager, emailSender, emailManager, mapRepository, logger)
    {
        private const int DaysBeforeExpiry = 30;

        /// <inheritdoc />
        protected override NotificationTypeEnum NotificationType => NotificationTypeEnum.WaiverExpiringReminder;

        /// <inheritdoc />
        protected override int MaxNumberOfHoursInWindow => 24;

        /// <inheritdoc />
        protected override int MinNumberOfHoursInWindow => 2;

        /// <inheritdoc />
        protected override string EmailSubject => "Your TrashMob.eco Waiver is Expiring Soon!";

        /// <summary>
        /// Generates notifications for users whose waivers are expiring soon.
        /// </summary>
        /// <param name="cancellationToken">Token to cancel the operation.</param>
        /// <returns>A task representing the asynchronous operation.</returns>
        public async Task GenerateNotificationsAsync(CancellationToken cancellationToken = default)
        {
            Logger.LogInformation("Generating Notifications for {NotificationType}", NotificationType);

            // Get users with waivers expiring in the next 30 days
            var usersWithExpiringWaivers = await userWaiverManager
                .GetUsersWithExpiringWaiversAsync(DaysBeforeExpiry, cancellationToken)
                .ConfigureAwait(false);

            var notificationCounter = 0;

            Logger.LogInformation(
                "Generating {NotificationType} Notifications for {Count} users with expiring waivers",
                NotificationType,
                usersWithExpiringWaivers.Count());

            foreach (var user in usersWithExpiringWaivers)
            {
                // Check if user has already received this notification this year
                if (await UserHasAlreadyReceivedNotification(user, cancellationToken).ConfigureAwait(false))
                {
                    Logger.LogInformation(
                        "User {UserId} has already received notification {NotificationType}. Skipping.",
                        user.Id,
                        NotificationType);
                    continue;
                }

                Logger.LogInformation(
                    "Sending waiver expiring notification to user {UserId}.",
                    user.Id);

                notificationCounter += await SendNotification(user, cancellationToken).ConfigureAwait(false);
            }

            Logger.LogInformation(
                "Generated {Count} Total {NotificationType} Notifications",
                notificationCounter,
                NotificationType);
        }
    }
}
