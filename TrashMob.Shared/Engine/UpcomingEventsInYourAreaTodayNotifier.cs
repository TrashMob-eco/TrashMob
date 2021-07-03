
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

    public class UpcomingEventsInYourAreaTodayNotifier : NotificationEngineBase, INotificationEngine
    {
        protected override NotificationTypeEnum NotificationType => NotificationTypeEnum.UpcomingEventsInYourAreaToday;

        protected override string EmailSubject => "Upcoming TrashMob.eco events in your area today!";

        protected override int NumberOfHoursInWindow => 24;

        public UpcomingEventsInYourAreaTodayNotifier(IEventRepository eventRepository, 
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

                var eventsToNotifyUserFor = new List<Event>();

                // Get list of active events
                var events = await EventRepository.GetActiveEvents().ConfigureAwait(false);

                // Get list of events user is already attending
                var eventsUserIsAttending = await EventAttendeeRepository.GetEventsUserIsAttending(user.Id).ConfigureAwait(false);

                // Limit the list of events to process to those in the next 48 hours UTC
                foreach (var mobEvent in events.Where(e => e.EventDate <= DateTimeOffset.UtcNow.AddHours(NumberOfHoursInWindow)))
                {
                    // Verify that the user is not already attending the event. No need to remind them to attend
                    if (eventsUserIsAttending.Any(ea => ea.Id == mobEvent.Id))
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

                    // Get list of notification events user has already received for the event
                    var notifications = await UserNotificationRepository.GetUserNotifications(user.Id, mobEvent.Id).ConfigureAwait(false);

                    // Verify that the user has not already received this type of notification for this event
                    if (notifications.Any(un => un.UserNotificationTypeId == (int)NotificationType))
                    {
                        continue;
                    }

                    // Add to the event list to be sent
                    eventsToNotifyUserFor.Add(mobEvent);
                }

                // Populate email
                if (eventsToNotifyUserFor.Any())
                {
                    // Update the database first so that a user is not notified multiple times
                    foreach (var mobEvent in eventsToNotifyUserFor)
                    {
                        var userNotification = new UserNotification
                        {
                            Id = Guid.NewGuid(),
                            EventId = mobEvent.Id,
                            UserId = user.Id,
                            SentDate = DateTimeOffset.UtcNow,
                            UserNotificationTypeId = (int)NotificationType,
                        };

                        await UserNotificationRepository.AddUserNotification(userNotification).ConfigureAwait(false);
                    }

                    var emailTemplate = GetEmailTemplate();
                    var content = EmailFormatter.PopulateTemplate(emailTemplate, user, eventsToNotifyUserFor);
                    var email = new Email();
                    email.Addresses.Add(new EmailAddress() { Email = user.Email, Name = $"{user.GivenName} {user.SurName}" });
                    email.Subject = EmailSubject;
                    email.Message = content;

                    // send email
                    await EmailSender.SendEmailAsync(email, cancellationToken);
                    notificationCounter++;
                }
            }

            Logger.LogInformation("Generating {0} Total {1} Notifications", notificationCounter, NotificationType);
        }
    }
}
