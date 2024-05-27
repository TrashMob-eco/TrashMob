namespace TrashMob.Shared.Engine
{
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading;
    using System.Threading.Tasks;
    using Microsoft.Extensions.Logging;
    using TrashMob.Models;
    using TrashMob.Shared.Managers.Interfaces;

    public class EventSummaryAttendeeNotifier : NotificationEngineBase, INotificationEngine
    {
        public EventSummaryAttendeeNotifier(IEventManager eventManager,
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

        protected override NotificationTypeEnum NotificationType => NotificationTypeEnum.EventSummaryAttendee;

        protected override int MaxNumberOfHoursInWindow => -24;

        protected override int MinNumberOfHoursInWindow => -5;

        protected override string EmailSubject => "Thank you for attending a TrashMob.eco event!";

        public async Task GenerateNotificationsAsync(CancellationToken cancellationToken = default)
        {
            Logger.LogInformation("Generating Notifications for {0}", NotificationType);

            // Get list of users who have notifications turned on for locations
            var users = await UserManager.GetAsync(cancellationToken).ConfigureAwait(false);
            var notificationCounter = 0;

            Logger.LogInformation("Generating {0} Notifications for {1} total users", NotificationType, users.Count());

            // for each user
            foreach (var user in users)
            {
                var eventsToNotifyUserFor = new List<Event>();

                // Get list of active events
                var events = await EventManager.GetCompletedEventsAsync(cancellationToken).ConfigureAwait(false);

                // Get list of events user has attended
                var eventsUserIsAttending = await EventAttendeeManager
                    .GetEventsUserIsAttendingAsync(user.Id, cancellationToken: cancellationToken).ConfigureAwait(false);

                // For all completed events where the user was not the lead
                foreach (var mobEvent in events.Where(e => e.CreatedByUserId != user.Id))
                {
                    // Did the user attend this event?
                    if (!eventsUserIsAttending.Any(ea => ea.Id == mobEvent.Id))
                    {
                        continue;
                    }

                    // Has the user already received the notification for this event?
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

            Logger.LogInformation("Generating {0} Total {1} Notifications", notificationCounter, NotificationType);
        }
    }
}