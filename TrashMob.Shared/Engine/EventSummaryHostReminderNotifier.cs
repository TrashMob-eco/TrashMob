
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

    public class EventSummaryHostReminderNotifier : NotificationEngineBase, INotificationEngine
    {
        protected override NotificationTypeEnum NotificationType => NotificationTypeEnum.EventSummaryHostReminder;

        protected override int MaxNumberOfHoursInWindow => -5;

        protected override int MinNumberOfHoursInWindow => -2;

        protected override string EmailSubject => "Your TrashMob.eco event has completed. We'd love to know how it went!";

        public EventSummaryHostReminderNotifier(IEventManager eventManager,
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
                var events = await EventManager.GetCompletedEventsAsync(cancellationToken).ConfigureAwait(false);

                foreach (var mobEvent in events.Where(e => e.CreatedByUserId == user.Id))
                {
                    if (await UserHasAlreadyReceivedNotification(user, mobEvent, cancellationToken).ConfigureAwait(false))
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
