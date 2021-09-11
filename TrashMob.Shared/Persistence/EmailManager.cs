
namespace TrashMob.Shared.Persistence
{
    using Microsoft.Extensions.Configuration;
    using Microsoft.Extensions.Logging;
    using System.Collections.Generic;
    using System.IO;
    using System.Reflection;
    using System.Threading;
    using System.Threading.Tasks;
    using TrashMob.Shared;
    using TrashMob.Shared.Engine;

    public class EmailManager : IEmailManager
    {
        private readonly IEmailSender emailSender;
        private readonly ILogger<EmailManager> logger;

        public EmailManager(IConfiguration configuration, IEmailSender emailSender, ILogger<EmailManager> logger)
        {
            this.emailSender = emailSender;
            this.logger = logger;
            this.emailSender.ApiKey = configuration["sendGridApiKey"];
        }

        /// <summary>
        /// Use this method to send an email using the generic template
        /// </summary>
        public async Task SendGenericSystemEmail(string subject, string message,  string htmlMessage, List<EmailAddress> recipients, CancellationToken cancellationToken = default)
        {
            var template = GetEmailTemplate(NotificationTypeEnum.Generic.ToString());
            var htmlTemplate = GetHtmlEmailTemplate(NotificationTypeEnum.Generic.ToString());

            foreach (var address in recipients)
            {
                var templatedEmail = template;

                templatedEmail = templatedEmail.Replace("{Title}", subject);
                templatedEmail = templatedEmail.Replace("{UserName}", address.Name);
                templatedEmail = templatedEmail.Replace("{Email}", address.Email);
                templatedEmail = templatedEmail.Replace("{Message}", message);

                var htmlTemplatedEmail = htmlTemplate;

                htmlTemplatedEmail = htmlTemplatedEmail.Replace("{Title}", subject);
                htmlTemplatedEmail = htmlTemplatedEmail.Replace("{UserName}", address.Name);
                htmlTemplatedEmail = htmlTemplatedEmail.Replace("{Email}", address.Email);
                htmlTemplatedEmail = htmlTemplatedEmail.Replace("{Message}", htmlMessage);

                var email = new Email
                {
                    Subject = subject,
                    Message = templatedEmail,
                    HtmlMessage = htmlTemplatedEmail,
                };

                email.Addresses.Add(address);

                await emailSender.SendEmailAsync(email, cancellationToken).ConfigureAwait(false);
            }
        }

        /// <summary>
        /// Use this method to send an email which has already been templated
        /// </summary>
        public async Task SendSystemEmail(string subject, string message, string htmlMessage, List<EmailAddress> recipients, CancellationToken cancellationToken = default)
        {
            var email = new Email
            {
                Subject = subject,
                Message = message,
                HtmlMessage = htmlMessage,
            };

            email.Addresses.AddRange(recipients);

            await emailSender.SendEmailAsync(email, cancellationToken).ConfigureAwait(false);
        }

        public string GetEmailTemplate(string notificationType)
        {
            var assembly = Assembly.GetExecutingAssembly();
            var resourceName = string.Format("TrashMob.Shared.Engine.EmailTemplates.{0}.html", notificationType);
            logger.LogInformation("Getting email template: {0}", resourceName);
            string result;

            using (Stream stream = assembly.GetManifestResourceStream(resourceName))
            using (StreamReader reader = new StreamReader(stream))
            {
                result = reader.ReadToEnd();
            }

            return result;
        }

        public string GetHtmlEmailTemplate(string notificationType)
        {
            var assembly = Assembly.GetExecutingAssembly();
            var resourceName = string.Format("TrashMob.Shared.Engine.EmailTemplates.{0}.txt", notificationType);
            logger.LogInformation("Getting email template: {0}", resourceName);
            string result;

            using (Stream stream = assembly.GetManifestResourceStream(resourceName))
            using (StreamReader reader = new StreamReader(stream))
            {
                result = reader.ReadToEnd();
            }

            return result;
        }
    }
}
