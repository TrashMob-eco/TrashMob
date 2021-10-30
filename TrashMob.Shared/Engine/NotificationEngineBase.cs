namespace TrashMob.Shared.Engine
{
    using Microsoft.Extensions.Logging;
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using System.Reflection;
    using System.Text;
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

            // Set the Api Key Here
            EmailSender.ApiKey = Environment.GetEnvironmentVariable("SendGridApiKey");
        }

        public string GetEmailTemplate()
        {
            var assembly = Assembly.GetExecutingAssembly();
            var resourceName = string.Format("TrashMob.Shared.Engine.EmailTemplates.{0}.txt", NotificationType);
            Logger.LogInformation("Getting email template: {0}", resourceName);
            string result;

            using (Stream stream = assembly.GetManifestResourceStream(resourceName))
            using (StreamReader reader = new StreamReader(stream))
            {
                result = reader.ReadToEnd();
            }

            return result;
        }

        public string GetHtmlEmailTemplate()
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
                var content = await PopulateTemplate(emailTemplate, user, eventsToNotifyUserFor).ConfigureAwait(false);
                var htmlEmailTemplate = GetHtmlEmailTemplate();
                var htmlContent = await PopulateTemplate(htmlEmailTemplate, user, eventsToNotifyUserFor).ConfigureAwait(false);
                var email = new Email();
                email.Addresses.Add(new EmailAddress() { Email = user.Email, Name = $"{user.GivenName} {user.SurName}" });
                email.Subject = EmailSubject;
                email.Message = content;
                email.HtmlMessage = htmlContent;

                Logger.LogInformation("Sending email to {0}, Subject {0}", email.Addresses[0].Email, email.Subject);

                // send email
                await EmailSender.SendEmailAsync(email, cancellationToken).ConfigureAwait(false);

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

        public async Task<string> PopulateTemplate(string template, User user, Event mobEvent)
        {
            var localTime = await MapRepository.GetTimeForPoint(new Tuple<double, double>(mobEvent.Latitude.Value, mobEvent.Longitude.Value), mobEvent.EventDate).ConfigureAwait(false);
            var populatedTemplate = template;
            populatedTemplate.Replace("{UserName}", user.UserName);
            populatedTemplate.Replace("{EventName}", mobEvent.Name);
            populatedTemplate.Replace("{EventDate}", localTime ?? mobEvent.EventDate.ToString("o"));
            populatedTemplate.Replace("{EventStreet}", mobEvent.StreetAddress);
            populatedTemplate.Replace("{EventCity}", mobEvent.City);
            populatedTemplate.Replace("{EventRegion}", mobEvent.Region);
            populatedTemplate.Replace("{EventCountry}", mobEvent.Country);
            var summaryLink = $"<a target='_blank' href='https://www.trashmob.eco/eventsummary/{mobEvent.Id}'>Event Summary</a>";
            populatedTemplate.Replace("{EventSummaryLink}", summaryLink);
            var detailsLink = $"<a target='_blank' href='https://www.trashmob.eco/eventdetails/{mobEvent.Id}'>Event Details</a>";
            populatedTemplate.Replace("{EventDetailsLink}", detailsLink);
            return populatedTemplate;
        }

        public async Task<string> PopulateTemplate(string template, User user, IEnumerable<Event> mobEvents)
        {
            var populatedTemplate = template;
            populatedTemplate.Replace("{UserName}", user.UserName);

            var eventGrid = new StringBuilder();
            eventGrid.AppendLine("<table>");
            eventGrid.AppendLine("<th>");
            eventGrid.AppendLine("<td>");
            eventGrid.AppendLine("Event Name");
            eventGrid.AppendLine("</td>");
            eventGrid.AppendLine("<td>");
            eventGrid.AppendLine("Event Date");
            eventGrid.AppendLine("</td>");
            eventGrid.AppendLine("<td>");
            eventGrid.AppendLine("Event Address");
            eventGrid.AppendLine("</td>");
            eventGrid.AppendLine("<td>");
            eventGrid.AppendLine("Event City");
            eventGrid.AppendLine("</td>");
            eventGrid.AppendLine("<td>");
            eventGrid.AppendLine("Event Region");
            eventGrid.AppendLine("</td>");
            eventGrid.AppendLine("<td>");
            eventGrid.AppendLine("Event Country");
            eventGrid.AppendLine("</td>");
            eventGrid.AppendLine("</th>");

            foreach (var mobEvent in mobEvents)
            {
                var localTime = await MapRepository.GetTimeForPoint(new Tuple<double, double>(mobEvent.Latitude.Value, mobEvent.Longitude.Value), mobEvent.EventDate).ConfigureAwait(false);
                eventGrid.AppendLine("<tr>");
                eventGrid.AppendLine("<td>");
                var link = $"<a target='_blank' href='https://www.trashmob.eco/eventdetails/{mobEvent.Id}'>{mobEvent.Name}</a>";
                eventGrid.AppendLine(link);
                eventGrid.AppendLine("</td>");
                eventGrid.AppendLine("<td>");
                populatedTemplate.Replace("{EventDate}", localTime ?? mobEvent.EventDate.ToString("o"));
                eventGrid.AppendLine("</td>");
                eventGrid.AppendLine("<td>");
                eventGrid.AppendLine(mobEvent.StreetAddress);
                eventGrid.AppendLine("</td>");
                eventGrid.AppendLine("<td>");
                eventGrid.AppendLine(mobEvent.City);
                eventGrid.AppendLine("</td>");
                eventGrid.AppendLine("<td>");
                eventGrid.AppendLine(mobEvent.Region);
                eventGrid.AppendLine("</td>");
                eventGrid.AppendLine("<td>");
                eventGrid.AppendLine(mobEvent.Country);
                eventGrid.AppendLine("</td>");
                eventGrid.AppendLine("</tr>");
            }
            
            eventGrid.AppendLine("</table>");

            populatedTemplate = populatedTemplate.Replace("{EventGrid}", eventGrid.ToString());

            return populatedTemplate;
        }
    }
}
