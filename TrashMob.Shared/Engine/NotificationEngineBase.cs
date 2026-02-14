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
    /// Base class for notification engines that send emails to users about events.
    /// </summary>
    public abstract class NotificationEngineBase
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="NotificationEngineBase"/> class.
        /// </summary>
        /// <param name="eventManager">Manager for event operations.</param>
        /// <param name="userManager">Manager for user operations.</param>
        /// <param name="eventAttendeeManager">Manager for event attendee operations.</param>
        /// <param name="userNotificationManager">Manager for user notification tracking.</param>
        /// <param name="nonEventUserNotificationManager">Manager for non-event user notifications.</param>
        /// <param name="emailSender">Service for sending emails.</param>
        /// <param name="emailManager">Manager for email operations.</param>
        /// <param name="mapRepository">Repository for map and location services.</param>
        /// <param name="logger">Logger instance.</param>
        public NotificationEngineBase(IEventManager eventManager,
            IKeyedManager<User> userManager,
            IEventAttendeeManager eventAttendeeManager,
            IKeyedManager<UserNotification> userNotificationManager,
            INonEventUserNotificationManager nonEventUserNotificationManager,
            IEmailSender emailSender,
            IEmailManager emailManager,
            IMapManager mapRepository,
            ILogger logger)
        {
            EventManager = eventManager;
            UserManager = userManager;
            EventAttendeeManager = eventAttendeeManager;
            UserNotificationManager = userNotificationManager;
            EmailSender = emailSender;
            EmailManager = emailManager;
            MapRepository = mapRepository;
            NonEventUserNotificationManager = nonEventUserNotificationManager;
            Logger = logger;

            // Set the Api Key Here
            EmailSender.ApiKey = Environment.GetEnvironmentVariable("SendGridApiKey");
        }

        /// <summary>
        /// Gets the event manager for event operations.
        /// </summary>
        protected IEventManager EventManager { get; }

        /// <summary>
        /// Gets the user manager for user operations.
        /// </summary>
        protected IKeyedManager<User> UserManager { get; }

        /// <summary>
        /// Gets the event attendee manager.
        /// </summary>
        protected IEventAttendeeManager EventAttendeeManager { get; }

        /// <summary>
        /// Gets the user notification manager for tracking sent notifications.
        /// </summary>
        protected IKeyedManager<UserNotification> UserNotificationManager { get; }

        /// <summary>
        /// Gets the manager for non-event user notifications.
        /// </summary>
        protected INonEventUserNotificationManager NonEventUserNotificationManager { get; }

        /// <summary>
        /// Gets the email manager for email operations.
        /// </summary>
        protected IEmailManager EmailManager { get; }

        /// <summary>
        /// Gets the email sender service.
        /// </summary>
        protected IEmailSender EmailSender { get; }

        /// <summary>
        /// Gets the map repository for location services.
        /// </summary>
        protected IMapManager MapRepository { get; }

        /// <summary>
        /// Gets the logger instance.
        /// </summary>
        public ILogger Logger { get; }

        /// <summary>
        /// Gets the type of notification this engine generates.
        /// </summary>
        protected abstract NotificationTypeEnum NotificationType { get; }

        /// <summary>
        /// Gets the minimum number of hours before an event to start sending notifications.
        /// </summary>
        protected abstract int MinNumberOfHoursInWindow { get; }

        /// <summary>
        /// Gets the maximum number of hours before an event to stop sending notifications.
        /// </summary>
        protected abstract int MaxNumberOfHoursInWindow { get; }

        /// <summary>
        /// Gets the email subject line for notifications.
        /// </summary>
        protected abstract string EmailSubject { get; }

        /// <summary>
        /// Gets the SendGrid API key.
        /// </summary>
        protected string SendGridApiKey { get; }

        /// <summary>
        /// Sends notifications to a user about multiple events.
        /// </summary>
        /// <param name="user">The user to notify.</param>
        /// <param name="eventsToNotifyUserFor">The events to include in the notification.</param>
        /// <param name="cancellationToken">Token to cancel the operation.</param>
        /// <returns>The number of notifications sent (1 if any were sent, 0 otherwise).</returns>
        protected async Task<int> SendNotifications(User user, IEnumerable<Event> eventsToNotifyUserFor,
            CancellationToken cancellationToken = default)
        {
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

                    await UserNotificationManager.AddAsync(userNotification, cancellationToken).ConfigureAwait(false);
                }

                var emailCopy = EmailManager.GetHtmlEmailCopy(NotificationType.ToString());

                foreach (var mobEvent in eventsToNotifyUserFor)
                {
                    Logger.LogInformation("Getting local event time for eventId: {eventId}", mobEvent.Id);

                    var localDate = await mobEvent.GetLocalEventTime(MapRepository).ConfigureAwait(false);

                    Logger.LogInformation("UTC event time for eventId ({eventId}): {eventDate}.", mobEvent.Id,
                        mobEvent.EventDate);
                    Logger.LogInformation("Local event time for eventId ({eventId}): {localDate}.", mobEvent.Id,
                        localDate);

                    // If the email has an event summary, add it here.
                    emailCopy = emailCopy.Replace("{eventSummaryUrl}", mobEvent.EventSummaryUrl());

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

                    Logger.LogInformation("Sending email to {Email}, Subject {Subject}", user.Email, EmailSubject);

                    await EmailManager.SendTemplatedEmailAsync(EmailSubject, SendGridEmailTemplateId.EventEmail,
                            SendGridEmailGroupId.EventRelated, dynamicTemplateData, recipients, CancellationToken.None)
                        .ConfigureAwait(false);
                }

                return 1;
            }

            return 0;
        }

        /// <summary>
        /// Sends a non-event notification to a user.
        /// </summary>
        /// <param name="user">The user to notify.</param>
        /// <param name="cancellationToken">Token to cancel the operation.</param>
        /// <returns>Always returns 1 indicating a notification was sent.</returns>
        protected async Task<int> SendNotification(User user, CancellationToken cancellationToken = default)
        {
            // Populate email
            var userNotification = new NonEventUserNotification
            {
                Id = Guid.NewGuid(),
                UserId = user.Id,
                SentDate = DateTimeOffset.UtcNow,
                UserNotificationTypeId = (int)NotificationType,
            };

            await NonEventUserNotificationManager.AddAsync(userNotification, cancellationToken).ConfigureAwait(false);

            var emailCopy = EmailManager.GetHtmlEmailCopy(NotificationType.ToString());

            var dynamicTemplateData = new
            {
                username = user.UserName,
                emailCopy,
                subject = EmailSubject,
            };

            List<EmailAddress> recipients =
            [
                new() { Name = user.UserName, Email = user.Email },
            ];

            Logger.LogInformation("Sending email to {Email}, Subject {Subject}", user.Email, EmailSubject);

            await EmailManager.SendTemplatedEmailAsync(EmailSubject, SendGridEmailTemplateId.GenericEmail,
                    SendGridEmailGroupId.General, dynamicTemplateData, recipients, CancellationToken.None)
                .ConfigureAwait(false);

            return 1;
        }

        /// <summary>
        /// Checks if a user has already received this notification type for a specific event.
        /// </summary>
        /// <param name="user">The user to check.</param>
        /// <param name="mobEvent">The event to check against.</param>
        /// <param name="cancellationToken">Token to cancel the operation.</param>
        /// <returns>True if the user has already received this notification type for the event.</returns>
        protected async Task<bool> UserHasAlreadyReceivedNotification(User user, Event mobEvent,
            CancellationToken cancellationToken = default)
        {
            // Get list of notification events user has already received for the event
            var notifications = await UserNotificationManager
                .GetCollectionAsync(user.Id, mobEvent.Id, cancellationToken).ConfigureAwait(false);

            // Verify that the user has not already received this type of notification for this event
            return notifications.Any(un => un.UserNotificationTypeId == (int)NotificationType);
        }

        /// <summary>
        /// Checks if a user has already received this non-event notification type.
        /// </summary>
        /// <param name="user">The user to check.</param>
        /// <param name="cancellationToken">Token to cancel the operation.</param>
        /// <returns>True if the user has already received this notification type.</returns>
        protected async Task<bool> UserHasAlreadyReceivedNotification(User user,
            CancellationToken cancellationToken = default)
        {
            // Get list of notification events user has already received for the event
            var notifications = await NonEventUserNotificationManager
                .GetByUserIdAsync(user.Id, (int)NotificationType, cancellationToken).ConfigureAwait(false);

            // Verify that the user has not already received this type of notification for this event
            return notifications.Any();
        }
    }
}