
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

    public class EventSummaryHostWeekReminderNotifier : NotificationEngineBase, INotificationEngine
    {
        private readonly IEventSummaryRepository eventSummaryRepository;

        protected override NotificationTypeEnum NotificationType => NotificationTypeEnum.EventSummaryHostWeekReminder;

        protected override int NumberOfHoursInWindow => -4;

        protected override string EmailSubject => "Thanks for leading a TrashMob.eco event! We'd love to know how it went!";

        public EventSummaryHostWeekReminderNotifier(IEventRepository eventRepository, 
                                                IUserRepository userRepository, 
                                                IEventAttendeeRepository eventAttendeeRepository,
                                                IUserNotificationRepository userNotificationRepository,
                                                INonEventUserNotificationRepository nonEventUserNotificationRepository,
                                                IEmailSender emailSender,
                                                IEmailManager emailManager,
                                                IMapRepository mapRepository,
                                                IEventSummaryRepository eventSummaryRepository,
                                                ILogger logger) :
            base(eventRepository, userRepository, eventAttendeeRepository, userNotificationRepository, nonEventUserNotificationRepository, emailSender, emailManager, mapRepository, logger)
        {
            this.eventSummaryRepository = eventSummaryRepository;
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
                var eventsToNotifyUserFor = new List<Event>();

                // Get list of completed events
                var events = await EventRepository.GetCompletedEvents(cancellationToken).ConfigureAwait(false);

                foreach (var mobEvent in events.Where(e => e.CreatedByUserId == user.Id))
                {
                    if (await UserHasAlreadyReceivedNotification(user, mobEvent).ConfigureAwait(false))
                    {
                        continue;
                    }

                    // Only send an email for this event if the event was at least 7 days before
                    if (mobEvent.EventDate > DateTime.UtcNow.AddDays(-7))
                    {
                        continue;
                    }

                    var eventSummary = await eventSummaryRepository.GetEventSummary(mobEvent.Id, cancellationToken).ConfigureAwait(false);

                    // Only send an email if the summary has not been completed.
                    if (eventSummary == null)
                    {
                        // Add to the event list to be sent
                        eventsToNotifyUserFor.Add(mobEvent);
                    }
                }

                notificationCounter += await SendNotifications(user, eventsToNotifyUserFor, cancellationToken).ConfigureAwait(false);
            }

            Logger.LogInformation("Generating {0} Total {1} Notifications", notificationCounter, NotificationType);
        }
    }
}
