namespace TrashMob.Shared.Engine
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading;
    using System.Threading.Tasks;
    using Microsoft.Extensions.Logging;
    using TrashMob.Models;
    using TrashMob.Models.Extensions;
    using TrashMob.Shared.Extensions;
    using TrashMob.Shared.Managers.Interfaces;
    using TrashMob.Shared.Poco;

    /// <summary>
    /// Base class for notification engines that notify team members about upcoming team-only events.
    /// </summary>
    public abstract class UpcomingTeamEventsBaseNotifier(
        IEventManager eventManager,
        IKeyedManager<User> userManager,
        IEventAttendeeManager eventAttendeeManager,
        IKeyedManager<UserNotification> userNotificationManager,
        INonEventUserNotificationManager nonEventUserNotificationManager,
        IEmailSender emailSender,
        IEmailManager emailManager,
        IMapManager mapRepository,
        ITeamMemberManager teamMemberManager,
        ILogger logger)
        : NotificationEngineBase(eventManager, userManager, eventAttendeeManager, userNotificationManager,
            nonEventUserNotificationManager, emailSender, emailManager, mapRepository, logger), INotificationEngine
    {

        /// <inheritdoc />
        public async Task GenerateNotificationsAsync(CancellationToken cancellationToken = default)
        {
            Logger.LogInformation("Generating Notifications for {NotificationType}", NotificationType);

            // Get all active team-only events
            var teamEvents = await EventManager.GetActiveTeamEventsAsync(cancellationToken).ConfigureAwait(false);
            var notificationCounter = 0;

            // Filter to events in the time window
            var eventsInWindow = teamEvents.Where(e =>
                e.EventDate <= DateTimeOffset.UtcNow.AddHours(MaxNumberOfHoursInWindow) &&
                e.EventDate > DateTimeOffset.UtcNow.AddHours(MinNumberOfHoursInWindow));

            Logger.LogInformation("Found {EventCount} team events in window for {NotificationType}",
                eventsInWindow.Count(), NotificationType);

            foreach (var mobEvent in eventsInWindow)
            {
                if (mobEvent.TeamId == null)
                {
                    continue;
                }

                // Get all members of the event's team
                var teamMembers = await teamMemberManager
                    .GetByTeamIdAsync(mobEvent.TeamId.Value, cancellationToken).ConfigureAwait(false);

                var teamName = mobEvent.Team?.Name ?? "your team";

                foreach (var teamMember in teamMembers)
                {
                    // Get user details
                    var user = await UserManager.GetAsync(teamMember.UserId, cancellationToken).ConfigureAwait(false);

                    if (user == null)
                    {
                        continue;
                    }

                    // Check if user is already attending — no need to remind them
                    var eventsUserIsAttending = await EventAttendeeManager
                        .GetEventsUserIsAttendingAsync(user.Id, cancellationToken: cancellationToken)
                        .ConfigureAwait(false);

                    if (eventsUserIsAttending.Any(ea => ea.Id == mobEvent.Id))
                    {
                        continue;
                    }

                    // Check if user has already received this notification for this event
                    if (await UserHasAlreadyReceivedNotification(user, mobEvent, cancellationToken)
                            .ConfigureAwait(false))
                    {
                        continue;
                    }

                    // Send notification
                    notificationCounter += await SendTeamEventNotification(user, mobEvent, teamName,
                        cancellationToken).ConfigureAwait(false);
                }
            }

            Logger.LogInformation("Sent {NotificationCount} total {NotificationType} notifications",
                notificationCounter, NotificationType);
        }

        private async Task<int> SendTeamEventNotification(User user, Event mobEvent, string teamName,
            CancellationToken cancellationToken)
        {
            // Record notification in database to prevent duplicates
            var userNotification = new UserNotification
            {
                Id = Guid.NewGuid(),
                EventId = mobEvent.Id,
                UserId = user.Id,
                SentDate = DateTimeOffset.UtcNow,
                UserNotificationTypeId = (int)NotificationType,
            };

            await UserNotificationManager.AddAsync(userNotification, cancellationToken).ConfigureAwait(false);

            var emailCopy = EmailManager.GetHtmlEmailCopy(NotificationType.ToString());
            emailCopy = emailCopy.Replace("{teamName}", teamName);
            emailCopy = emailCopy.Replace("{eventSummaryUrl}", mobEvent.EventSummaryUrl());

            Logger.LogInformation("Getting local event time for eventId: {eventId}", mobEvent.Id);

            var localDate = await mobEvent.GetLocalEventTime(MapRepository).ConfigureAwait(false);

            Logger.LogInformation("UTC event time for eventId ({eventId}): {eventDate}.", mobEvent.Id,
                mobEvent.EventDate);
            Logger.LogInformation("Local event time for eventId ({eventId}): {localDate}.", mobEvent.Id,
                localDate);

            var dynamicTemplateData = new
            {
                username = user.UserName,
                eventName = mobEvent.Name,
                eventDate = localDate.Date,
                eventTime = localDate.Time,
                eventAddress = mobEvent.EventAddress(),
                emailCopy,
                subject = EmailSubject,
                eventDetailsUrl = mobEvent.EventDetailsUrl(),
                eventSummaryUrl = mobEvent.EventSummaryUrl(),
                googleMapsUrl = mobEvent.GoogleMapsUrl(),
            };

            List<EmailAddress> recipients =
            [
                new() { Name = user.UserName, Email = user.Email },
            ];

            Logger.LogInformation("Sending team event email to {Email}, Subject {Subject}", user.Email,
                EmailSubject);

            await EmailManager.SendTemplatedEmailAsync(EmailSubject, SendGridEmailTemplateId.EventEmail,
                    SendGridEmailGroupId.EventRelated, dynamicTemplateData, recipients, CancellationToken.None)
                .ConfigureAwait(false);

            return 1;
        }
    }
}
