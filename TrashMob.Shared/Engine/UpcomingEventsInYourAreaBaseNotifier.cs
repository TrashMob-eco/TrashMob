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

    public abstract class UpcomingEventsInYourAreaBaseNotifier : NotificationEngineBase, INotificationEngine
    {
        public UpcomingEventsInYourAreaBaseNotifier(IEventManager eventManager,
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
                // If the user has not set their latitude and longitude, skip user
                if (user.Latitude == 0 && user.Longitude == 0)
                {
                    continue;
                }

                var eventsToNotifyUserFor = new List<Event>();

                // Get list of active events
                var events = await EventManager.GetActiveEventsAsync(cancellationToken).ConfigureAwait(false);

                // Get list of events user is already attending
                var eventsUserIsAttending = await EventAttendeeManager.GetEventsUserIsAttendingAsync(user.Id, cancellationToken: cancellationToken).ConfigureAwait(false);

                // Limit the list of events to process to those in the next window UTC
                foreach (var mobEvent in events.Where(e => e.EventDate <= DateTimeOffset.UtcNow.AddHours(MaxNumberOfHoursInWindow) && e.EventDate > DateTimeOffset.UtcNow.AddHours(MinNumberOfHoursInWindow)))
                {
                    // Skip private events
                    if (!mobEvent.IsEventPublic)
                    {
                        continue;
                    }

                    // Verify that the user is not already attending the event. No need to remind them to attend
                    if (eventsUserIsAttending.Any(ea => ea.Id == mobEvent.Id))
                    {
                        continue;
                    }

                    // Only check distance if the user's country and region match the event
                    if (user.Country != mobEvent.Country || user.Region != mobEvent.Region)
                    {
                        continue;
                    }

                    // Get the distance from the User's home location to the event location
                    var userLocation = new Tuple<double, double>(user.Latitude.Value, user.Longitude.Value);
                    var eventLocation = new Tuple<double, double>(mobEvent.Latitude.Value, mobEvent.Longitude.Value);

                    var distance = await MapRepository.GetDistanceBetweenTwoPointsAsync(userLocation, eventLocation, user.PrefersMetric).ConfigureAwait(false);

                    // If the distance to the event is greater than the User's preference for distance, ignore it
                    if (distance > user.TravelLimitForLocalEvents)
                    {
                        continue;
                    }

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
