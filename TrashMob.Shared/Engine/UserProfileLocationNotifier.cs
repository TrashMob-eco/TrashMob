namespace TrashMob.Shared.Engine
{
    using System.Linq;
    using System.Threading;
    using System.Threading.Tasks;
    using Microsoft.Extensions.Logging;
    using TrashMob.Models;
    using TrashMob.Shared.Managers.Interfaces;

    public class UserProfileLocationNotifier : NotificationEngineBase
    {
        public UserProfileLocationNotifier(IEventManager eventManager,
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

        protected override NotificationTypeEnum NotificationType => NotificationTypeEnum.UserProfileUpdateLocation;

        protected override int MaxNumberOfHoursInWindow => 24;

        protected override int MinNumberOfHoursInWindow => 2;

        protected override string EmailSubject =>
            "Set your User Location in TrashMob to get Upcoming Event Notifications!";

        public async Task GenerateNotificationsAsync(CancellationToken cancellationToken = default)
        {
            Logger.LogInformation("Generating Notifications for {0}", NotificationType);

            // Get list of all users
            var users = await UserManager.GetAsync(cancellationToken).ConfigureAwait(false);

            var notificationCounter = 0;

            Logger.LogInformation("Generating {0} Notifications for {1} total users", NotificationType, users.Count());

            // for each user
            foreach (var user in users)
            {
                if (await UserHasAlreadyReceivedNotification(user, cancellationToken).ConfigureAwait(false))
                {
                    Logger.LogInformation("User {0} has already received notification {1}. Skipping.", user.Id,
                        NotificationType);
                    continue;
                }

                // Notify users who have not set their home location
                if (user.Latitude == 0 && user.Longitude == 0)
                {
                    if (user.Id.ToString() == "2A648BA2-854C-4949-BB36-64D90E15B8CA")
                    {
                        Logger.LogInformation("User {0} has not set their location. Notifying.", user.Id);
                        notificationCounter += await SendNotification(user, cancellationToken).ConfigureAwait(false);
                    }
                    else
                    {
                        Logger.LogInformation("User {0} has not set their location. We're in debug mode so skipping.",
                            user.Id);
                    }
                }
            }

            Logger.LogInformation("Generating {0} Total {1} Notifications", notificationCounter, NotificationType);
        }
    }
}