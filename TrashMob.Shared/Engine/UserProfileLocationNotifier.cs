
namespace TrashMob.Shared.Engine
{
    using Microsoft.Extensions.Logging;
    using System.Threading;
    using System.Threading.Tasks;
    using System.Linq;
    using TrashMob.Shared.Persistence.Interfaces;
    using TrashMob.Models;
    using TrashMob.Shared.Managers.Interfaces;

    public class UserProfileLocationNotifier : NotificationEngineBase
    {
        protected override NotificationTypeEnum NotificationType => NotificationTypeEnum.UserProfileUpdateLocation;

        protected override int NumberOfHoursInWindow => 24;

        protected override string EmailSubject => "Set your User Location in TrashMob to get Upcoming Event Notifications!";

        public UserProfileLocationNotifier(IEventManager eventManager, 
                                           IKeyedManager<User> userManager, 
                                           IEventAttendeeManager eventAttendeeManager,
                                           IKeyedManager<UserNotification> userNotificationManager,
                                           IKeyedManager<NonEventUserNotification> nonEventUserNotificationManager,
                                           IEmailSender emailSender,
                                           IEmailManager emailManager,
                                           IMapManager mapRepository,
                                           ILogger logger) : 
            base(eventManager, userManager, eventAttendeeManager, userNotificationManager, nonEventUserNotificationManager, emailSender, emailManager, mapRepository, logger)
        {

        }

        public async Task GenerateNotificationsAsync(CancellationToken cancellationToken = default)
        {
            Logger.LogInformation("Generating Notifications for {0}", NotificationType);

            // Get list of all users
            var users = await UserManager.GetAsync(cancellationToken).ConfigureAwait(false);
            
            int notificationCounter = 0;

            Logger.LogInformation("Generating {0} Notifications for {1} total users", NotificationType, users.Count());

            // for each user
            foreach (var user in users)
            {
                if (await UserHasAlreadyReceivedNotification(user, cancellationToken).ConfigureAwait(false))
                {
                    continue;
                }

                // Notify users who have not set their home location
                if (user.Latitude == 0 && user.Longitude == 0)
                {
                    notificationCounter += await SendNotification(user, cancellationToken).ConfigureAwait(false);
                }
            }

            Logger.LogInformation("Generating {0} Total {1} Notifications", notificationCounter, NotificationType);
        }
    }
}
