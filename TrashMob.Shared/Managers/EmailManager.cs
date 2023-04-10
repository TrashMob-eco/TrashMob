
namespace TrashMob.Shared.Managers
{
    using Microsoft.EntityFrameworkCore.Query.Internal;
    using Microsoft.Extensions.Configuration;
    using Microsoft.Extensions.Logging;
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using System.Reflection;
    using System.Threading;
    using System.Threading.Tasks;
    using TrashMob.Shared.Engine;
    using TrashMob.Shared.Managers.Interfaces;
    using TrashMob.Shared.Poco;

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

        public string GetEmailTemplate(string notificationType)
        {
            var assembly = Assembly.GetExecutingAssembly();
            var resourceName = string.Format("TrashMob.Shared.Engine.EmailTemplates.{0}.txt", notificationType);
            logger.LogInformation("Getting email template: {resourceName}", resourceName);
            string result;

            using (Stream stream = assembly.GetManifestResourceStream(resourceName))
            using (StreamReader reader = new(stream))
            {
                result = reader.ReadToEnd();
            }

            return result;
        }

        public string GetHtmlEmailTemplate(string notificationType)
        {
            var assembly = Assembly.GetExecutingAssembly();
            var resourceName = string.Format("TrashMob.Shared.Engine.EmailTemplates.{0}.html", notificationType);
            logger.LogInformation("Getting email template: {resourceName}", resourceName);
            string result;

            using (Stream stream = assembly.GetManifestResourceStream(resourceName))
            using (StreamReader reader = new(stream))
            {
                result = reader.ReadToEnd();
            }

            return result;
        }

        public string GetHtmlEmailCopy(string notificationType)
        {
            var assembly = Assembly.GetExecutingAssembly();
            var resourceName = string.Format("TrashMob.Shared.Engine.EmailCopy.{0}.html", notificationType);
            logger.LogInformation("Getting email copy: {resourceName}", resourceName);
            string result;

            using (Stream stream = assembly.GetManifestResourceStream(resourceName))
            using (StreamReader reader = new(stream))
            {
                result = reader.ReadToEnd();
            }

            return result;
        }

        public async Task SendTemplatedEmailAsync(string subject, string templateId, int groupId, object dynamicTemplateData, List<EmailAddress> recipients, CancellationToken cancellationToken = default)
        {
            var email = new Email
            {
                Subject = subject,
                DynamicTemplateData = dynamicTemplateData,
                TemplateId = templateId,
                GroupId = groupId,
            };

            email.Addresses.AddRange(recipients);

            await emailSender.SendTemplatedEmailAsync(email, cancellationToken).ConfigureAwait(false);
        }

        public Task<IEnumerable<EmailTemplate>> GetEmailTemplatesAsync(CancellationToken cancellationToken)
        {
            var emailTemplates = new List<EmailTemplate>();
            foreach (var notificationType in Enum.GetValues(typeof(NotificationTypeEnum)))
            {
                var content = GetHtmlEmailCopy(notificationType.ToString());

                var template = new EmailTemplate
                {
                    Name = notificationType.ToString(),
                    Content = content,
                };

                emailTemplates.Add(template);
            }

            return Task.FromResult(emailTemplates.AsEnumerable());
        }
    }
}
