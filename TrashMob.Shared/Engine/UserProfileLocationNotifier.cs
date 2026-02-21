namespace TrashMob.Shared.Engine
{
    using System.Linq;
    using System.Threading;
    using System.Threading.Tasks;
    using Microsoft.Extensions.Logging;
    using TrashMob.Models;
    using TrashMob.Shared.Managers.Interfaces;

    /// <summary>
    /// Notification engine that reminds users to set their location in their profile to receive event notifications.
    /// </summary>
    public class UserProfileLocationNotifier(
        IEventManager eventManager,
        IKeyedManager<User> userManager,
        IEventAttendeeManager eventAttendeeManager,
        IKeyedManager<UserNotification> userNotificationManager,
        INonEventUserNotificationManager nonEventUserNotificationManager,
        IEmailSender emailSender,
        IEmailManager emailManager,
        IMapManager mapRepository,
        ILogger logger)
        : NotificationEngineBase(eventManager, userManager, eventAttendeeManager, userNotificationManager,
            nonEventUserNotificationManager, emailSender, emailManager, mapRepository, logger)
    {

        /// <inheritdoc />
        protected override NotificationTypeEnum NotificationType => NotificationTypeEnum.UserProfileUpdateLocation;

        /// <inheritdoc />
        protected override int MaxNumberOfHoursInWindow => 24;

        /// <inheritdoc />
        protected override int MinNumberOfHoursInWindow => 2;

        /// <inheritdoc />
        protected override string EmailSubject =>
            "Set your User Location in TrashMob to get Upcoming Event Notifications!";

        /// <summary>
        /// Generates notifications for users who have not set their profile location.
        /// </summary>
        /// <param name="cancellationToken">Token to cancel the operation.</param>
        /// <returns>A task representing the asynchronous operation.</returns>
        public async Task GenerateNotificationsAsync(CancellationToken cancellationToken = default)
        {
            Logger.LogInformation("Generating Notifications for {NotificationType}", NotificationType);

            // Get list of all users
            var users = await UserManager.GetAsync(cancellationToken).ConfigureAwait(false);

            var notificationCounter = 0;

            Logger.LogInformation("Generating {NotificationType} Notifications for {UserCount} total users", NotificationType, users.Count());

            // for each user
            foreach (var user in users)
            {
                if (await UserHasAlreadyReceivedNotification(user, cancellationToken).ConfigureAwait(false))
                {
                    Logger.LogInformation("User {UserId} has already received notification {NotificationType}. Skipping.", user.Id,
                        NotificationType);
                    continue;
                }

                // Notify users who have not set their home location
                if (user.Latitude == 0 && user.Longitude == 0)
                {
                    if (user.Id.ToString() == "2A648BA2-854C-4949-BB36-64D90E15B8CA")
                    {
                        Logger.LogInformation("User {UserId} has not set their location. Notifying.", user.Id);
                        notificationCounter += await SendNotification(user, cancellationToken).ConfigureAwait(false);
                    }
                    else
                    {
                        Logger.LogInformation("User {UserId} has not set their location. We're in debug mode so skipping.",
                            user.Id);
                    }
                }
            }

            Logger.LogInformation("Generating {NotificationCount} Total {NotificationType} Notifications", notificationCounter, NotificationType);
        }
    }
}