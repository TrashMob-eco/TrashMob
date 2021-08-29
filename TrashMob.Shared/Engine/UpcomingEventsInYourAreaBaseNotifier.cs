
namespace TrashMob.Shared.Engine
{
    using Microsoft.Extensions.Logging;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading;
    using System.Threading.Tasks;
    using TrashMob.Shared.Models;
    using TrashMob.Shared.Persistence;

    public abstract class UpcomingEventsInYourAreaBaseNotifier : NotificationEngineBase, INotificationEngine
    {
        public UpcomingEventsInYourAreaBaseNotifier(IEventRepository eventRepository,
                                                     IUserRepository userRepository,
                                                     IEventAttendeeRepository eventAttendeeRepository,
                                                     IUserNotificationRepository userNotificationRepository,
                                                     IUserNotificationPreferenceRepository userNotificationPreferenceRepository,
                                                     IEmailSender emailSender,
                                                     IMapRepository mapRepository,
                                                     ILogger logger) :
            base(eventRepository, userRepository, eventAttendeeRepository, userNotificationRepository, userNotificationPreferenceRepository, emailSender, mapRepository, logger)
        {
        }

        public async Task GenerateNotificationsAsync(CancellationToken cancellationToken = default)
        {
            Logger.LogInformation("Generating Notifications for {0}", NotificationType);

            // Get list of users who have notifications turned on for locations
            var users = await UserRepository.GetAllUsers().ConfigureAwait(false);
            int notificationCounter = 0;

            Logger.LogInformation("Generating {0} Notifications for {1} total users", NotificationType, users.Count());

            // for each user
            foreach (var user in users)
            {
                if (await IsOptedOut(user).ConfigureAwait(false))
                {
                    continue;
                }

                // If the user has not set their latitude and longitude, skip user
                if (user.Latitude == 0 && user.Longitude == 0)
                {
                    continue;
                }

                var eventsToNotifyUserFor = new List<Event>();

                // Get list of active events
                var events = await EventRepository.GetActiveEvents().ConfigureAwait(false);

                // Get list of events user is already attending
                var eventsUserIsAttending = await EventAttendeeRepository.GetEventsUserIsAttending(user.Id).ConfigureAwait(false);

                // Limit the list of events to process to those in the next window UTC
                foreach (var mobEvent in events.Where(e => e.EventDate <= DateTimeOffset.UtcNow.AddHours(NumberOfHoursInWindow)))
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

                    var distance = await MapRepository.GetDistanceBetweenTwoPoints(userLocation, eventLocation, user.PrefersMetric).ConfigureAwait(false);

                    // If the distance to the event is greater than the User's preference for distance, ignore it
                    if (distance > user.TravelLimitForLocalEvents)
                    {
                        continue;
                    }

                    if (await UserHasAlreadyReceivedNotification(user, mobEvent).ConfigureAwait(false))
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
