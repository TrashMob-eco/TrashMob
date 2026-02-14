namespace TrashMob.Shared.Engine
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading;
    using System.Threading.Tasks;
    using Microsoft.Extensions.Logging;
    using TrashMob.Models;
    using TrashMob.Shared.Managers.Interfaces;

    /// <summary>
    /// Base class for notification engines that notify users about events they are attending.
    /// </summary>
    public abstract class UpcomingEventAttendingBaseNotifier(
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
                var events = await EventManager.GetActiveEventsAsync(cancellationToken: cancellationToken).ConfigureAwait(false);

                // Get list of events user is already attending
                var eventsUserIsAttending = await EventAttendeeManager
                    .GetEventsUserIsAttendingAsync(user.Id, cancellationToken: cancellationToken).ConfigureAwait(false);

                // Limit the list of events to process to those in the next window UTC
                foreach (var mobEvent in events.Where(e =>
                             e.CreatedByUserId != user.Id &&
                             e.EventDate >= DateTimeOffset.UtcNow.AddHours(MinNumberOfHoursInWindow) &&
                             e.EventDate <= DateTimeOffset.UtcNow.AddHours(MaxNumberOfHoursInWindow)))
                {
                    // Verify that the user is attending the event.
                    if (!eventsUserIsAttending.Any(ea => ea.Id == mobEvent.Id))
                    {
                        continue;
                    }

                    if (await UserHasAlreadyReceivedNotification(user, mobEvent, cancellationToken)
                            .ConfigureAwait(false))
                    {
                        continue;
                    }

                    // Add to the event list to be sent
                    eventsToNotifyUserFor.Add(mobEvent);
                }

                // Populate email
                notificationCounter += await SendNotifications(user, eventsToNotifyUserFor, cancellationToken)
                    .ConfigureAwait(false);
            }

            Logger.LogInformation("Generating {NotificationCount} Total {NotificationType} Notifications", notificationCounter, NotificationType);
        }
    }
}