
namespace TrashMob.Shared.Engine
{
    using Microsoft.Extensions.Logging;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading;
    using System.Threading.Tasks;
    using TrashMob.Shared.Models;
    using TrashMob.Shared.Persistence;

    public class EventSummaryAttendeeNotifier : NotificationEngineBase, INotificationEngine
    {
        protected override NotificationTypeEnum NotificationType => NotificationTypeEnum.EventSummaryAttendee;

        protected override int NumberOfHoursInWindow => -24;

        protected override string EmailSubject => "Thank you for attending a TrashMob.eco event!";

        public EventSummaryAttendeeNotifier(IEventRepository eventRepository,
                                            IUserRepository userRepository,
                                            IEventAttendeeRepository eventAttendeeRepository,
                                            IUserNotificationRepository userNotificationRepository,
                                            IUserNotificationPreferenceRepository userNotificationPreferenceRepository,
                                            IEmailSender emailSender,
                                            IEmailManager emailManager,
                                            IMapRepository mapRepository,
                                            ILogger logger) :
            base(eventRepository, userRepository, eventAttendeeRepository, userNotificationRepository, userNotificationPreferenceRepository, emailSender, emailManager, mapRepository, logger)
        {
        }

        public async Task GenerateNotificationsAsync(CancellationToken cancellationToken = default)
        {
            Logger.LogInformation("Generating Notifications for {0}", NotificationType);

            // Get list of users who have notifications turned on for locations
            var users = await UserRepository.GetAllUsers(cancellationToken).ConfigureAwait(false);
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
                var events = await EventRepository.GetCompletedEvents(cancellationToken).ConfigureAwait(false);

                // Get list of events user has attended
                var eventsUserIsAttending = await EventAttendeeRepository.GetEventsUserIsAttending(user.Id, cancellationToken: cancellationToken).ConfigureAwait(false);

                // For all completed events where the user was not the lead
                foreach (var mobEvent in events.Where(e => e.CreatedByUserId != user.Id))
                {
                    // Did the user attend this event?
                    if (!eventsUserIsAttending.Any(ea => ea.Id == mobEvent.Id))
                    {
                        continue;
                    }

                    // Has the user already received the notification for this event?
                    if (await UserHasAlreadyReceivedNotification(user, mobEvent).ConfigureAwait(false))
                    {
                        continue;
                    }

                    // Add to the event list to be sent
                    eventsToNotifyUserFor.Add(mobEvent);
                }

                notificationCounter += await SendNotifications(user, eventsToNotifyUserFor, cancellationToken).ConfigureAwait(false);
            }

            Logger.LogInformation("Generating {0} Total {1} Notifications", notificationCounter, NotificationType);
        }
    }
}
