﻿
namespace TrashMob.Shared.Engine
{
    using Microsoft.Extensions.Logging;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading;
    using System.Threading.Tasks;
    using TrashMob.Models;
    using TrashMob.Shared.Managers.Interfaces;
    using TrashMob.Shared.Persistence.Interfaces;

    public abstract class UpcomingEventHostingBaseNotifier : NotificationEngineBase, INotificationEngine
    {
        public UpcomingEventHostingBaseNotifier(IEventManager eventManager,
                                                IKeyedManager<User> userManager,
                                                IEventAttendeeManager eventAttendeeManager,
                                                IKeyedManager<UserNotification> userNotificationManager,
                                                INonEventUserNotificationManager nonEventUserNotificationManager,
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

            // Get list of users who have notifications turned on for locations
            var users = await UserManager.GetAsync(cancellationToken).ConfigureAwait(false);
            int notificationCounter = 0;

            Logger.LogInformation("Generating {0} Notifications for {1} total users", NotificationType, users.Count());

            // for each user
            foreach (var user in users)
            {
                var eventsToNotifyUserFor = new List<Event>();

                // Get list of active events
                var events = await EventManager.GetActiveEventsAsync(cancellationToken).ConfigureAwait(false);

                // Limit the list of events to process to those in the next window UTC
                foreach (var mobEvent in events.Where(e => e.CreatedByUserId == user.Id && e.EventDate >= DateTimeOffset.UtcNow.AddHours(MinNumberOfHoursInWindow) && e.EventDate <= DateTimeOffset.UtcNow.AddHours(MaxNumberOfHoursInWindow)))
                {
                    if (await UserHasAlreadyReceivedNotification(user, mobEvent, cancellationToken).ConfigureAwait(false))
                    { 
                        continue;
                    }

                    // Add to the event list to be sent
                    eventsToNotifyUserFor.Add(mobEvent);
                }

                // Populate email
                notificationCounter += await SendNotifications(user, eventsToNotifyUserFor, cancellationToken).ConfigureAwait(false);
            }

            Logger.LogInformation("Generating {0} Total {1} Notifications", notificationCounter, NotificationType);
        }
    }
}
