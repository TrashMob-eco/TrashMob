
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
        public async Task SendGenericSystemEmail(string subject, string message, List<EmailAddress> recipients, CancellationToken cancellationToken = default)
        {
            var template = GetEmailTemplate(NotificationTypeEnum.Generic.ToString());

            foreach (var address in recipients)
            {
                var templatedEmail = template;

                templatedEmail.Replace("{Title}", subject);
                templatedEmail.Replace("{UserName}", address.Name);
                templatedEmail.Replace("{Email}", address.Email);
                templatedEmail.Replace("{Message}", message);

                var email = new Email
                {
                    Subject = subject,
                    Message = templatedEmail,
                };

                email.Addresses.Add(address);

                await emailSender.SendEmailAsync(email, cancellationToken);
            }
        }

        /// <summary>
        /// Use this method to send an email which has already been templated
        /// </summary>
        public async Task SendSystemEmail(string subject, string message, List<EmailAddress> recipients, CancellationToken cancellationToken = default)
        {
            var email = new Email
            {
                Subject = subject,
                Message = message,
            };

            email.Addresses.AddRange(recipients);

            await emailSender.SendEmailAsync(email, cancellationToken);
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
    }
}
