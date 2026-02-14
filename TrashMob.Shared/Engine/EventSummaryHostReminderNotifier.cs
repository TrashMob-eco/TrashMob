namespace TrashMob.Shared.Engine
{
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading;
    using System.Threading.Tasks;
    using Microsoft.Extensions.Logging;
    using TrashMob.Models;
    using TrashMob.Shared.Managers.Interfaces;

    /// <summary>
    /// Notification engine that reminds event hosts to complete their event summary after an event.
    /// </summary>
    public class EventSummaryHostReminderNotifier(
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
            nonEventUserNotificationManager, emailSender, emailManager, mapRepository, logger), INotificationEngine
    {

        protected override NotificationTypeEnum NotificationType => NotificationTypeEnum.EventSummaryHostReminder;

        protected override int MaxNumberOfHoursInWindow => -5;

        protected override int MinNumberOfHoursInWindow => -2;

        protected override string EmailSubject =>
            "Your TrashMob.eco event has completed. We'd love to know how it went!";

        /// <inheritdoc />
        public async Task GenerateNotificationsAsync(CancellationToken cancellationToken = default)
        {
            Logger.LogInformation("Generating Notifications for {NotificationType}", NotificationType);

            // Get list of users who have notifications turned on for locations
            var users = await UserManager.GetAsync(cancellationToken).ConfigureAwait(false);
            var notificationCounter = 0;

            Logger.LogInformation("Generating {NotificationType} Notifications for {UserCount} total users", NotificationType, users.Count());

            // for each user
            foreach (var user in users)
            {
                List<Event> eventsToNotifyUserFor = [];

                // Get list of active events
                var events = await EventManager.GetCompletedEventsAsync(cancellationToken).ConfigureAwait(false);

                foreach (var mobEvent in events.Where(e => e.CreatedByUserId == user.Id))
                {
                    if (await UserHasAlreadyReceivedNotification(user, mobEvent, cancellationToken)
                            .ConfigureAwait(false))
                    {
                        continue;
                    }

                    // Add to the event list to be sent
                    eventsToNotifyUserFor.Add(mobEvent);
                }

                notificationCounter += await SendNotifications(user, eventsToNotifyUserFor, cancellationToken)
                    .ConfigureAwait(false);
            }

            Logger.LogInformation("Generating {NotificationCount} Total {NotificationType} Notifications", notificationCounter, NotificationType);
        }
    }
}