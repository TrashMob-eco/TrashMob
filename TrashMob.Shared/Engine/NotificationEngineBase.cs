namespace TrashMob.Shared.Engine
{
    using Microsoft.Extensions.Logging;
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using System.Reflection;
    using System.Threading;
    using System.Threading.Tasks;
    using TrashMob.Shared.Models;
    using TrashMob.Shared.Persistence;

    public abstract class NotificationEngineBase
    {
        protected IEventRepository EventRepository { get; }

        protected IUserRepository UserRepository { get; }

        protected IEventAttendeeRepository EventAttendeeRepository { get; }

        protected IUserNotificationRepository UserNotificationRepository { get; }

        public IUserNotificationPreferenceRepository UserNotificationPreferenceRepository { get; }

        protected IEmailSender EmailSender { get; }

        protected IMapRepository MapRepository { get; }
        public ILogger Logger { get; }

        protected abstract NotificationTypeEnum NotificationType { get; }

        protected abstract int NumberOfHoursInWindow { get; }

        protected abstract string EmailSubject { get; }

        protected string SendGridApiKey { get; }

        public NotificationEngineBase(IEventRepository eventRepository,
                                      IUserRepository userRepository,
                                      IEventAttendeeRepository eventAttendeeRepository,
                                      IUserNotificationRepository userNotificationRepository,
                                      IUserNotificationPreferenceRepository userNotificationPreferenceRepository,
                                      IEmailSender emailSender,
                                      IMapRepository mapRepository, 
                                      ILogger logger)
        {
            EventRepository = eventRepository;
            UserRepository = userRepository;
            EventAttendeeRepository = eventAttendeeRepository;
            UserNotificationRepository = userNotificationRepository;
            UserNotificationPreferenceRepository = userNotificationPreferenceRepository;
            EmailSender = emailSender;
            MapRepository = mapRepository;
            Logger = logger;
        }

        public string GetEmailTemplate()
        {
            var assembly = Assembly.GetExecutingAssembly();
            var resourceName = string.Format("TrashMob.Shared.Engine.EmailTemplates.{0}.html", NotificationType);
            Logger.LogInformation("Getting email template: {0}", resourceName);
            string result;

            using (Stream stream = assembly.GetManifestResourceStream(resourceName))
            using (StreamReader reader = new StreamReader(stream))
            {
                result = reader.ReadToEnd();
            }

            return result;
        }

        protected async Task<bool> IsOptedOut(User user)
        {
            if (user.IsOptedOutOfAllEmails)
            {
                return true;
            }

            var userNotificationPreferences = await UserNotificationPreferenceRepository.GetUserNotificationPreferences(user.Id).ConfigureAwait(false);

            if (userNotificationPreferences.Any(unp => unp.UserNotificationTypeId == (int)NotificationType && unp.IsOptedOut))
            {
                return true;
            }

            return false;
        }

        protected async Task<int> SendNotifications(User user, IEnumerable<Event> eventsToNotifyUserFor, CancellationToken cancellationToken)
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
                return 1;
            }

            return 0;
        }

        protected async Task<bool> UserHasAlreadyReceivedNotification(User user, Event mobEvent)
        {
            // Get list of notification events user has already received for the event
            var notifications = await UserNotificationRepository.GetUserNotifications(user.Id, mobEvent.Id).ConfigureAwait(false);

            // Verify that the user has not already received this type of notification for this event
            return notifications.Any(un => un.UserNotificationTypeId == (int)NotificationType);
        }
    }
}
