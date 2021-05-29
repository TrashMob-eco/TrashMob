
namespace TrashMob.Persistence
{
    using Microsoft.Extensions.Configuration;
    using SendGrid;
    using SendGrid.Helpers.Mail;
    using System;
    using System.Collections.Generic;
    using System.Threading;
    using System.Threading.Tasks;
    using TrashMob.Common;
    using TrashMob.Poco;

    public class EmailManager : IEmailManager
    {
        private readonly IConfiguration configuration;

        public EmailManager(IConfiguration configuration)
        {
            this.configuration = configuration;
        }

        public async Task SendSystemEmail(Email email, CancellationToken cancellationToken = default)
        {
            var sendGridApiKey = configuration["sendGridApiKey"];

            // To not send emails from dev environments, don't store an apikey password in the local secrets
            if (string.IsNullOrWhiteSpace(sendGridApiKey) || sendGridApiKey == "x")
            {
                return;
            }

            var from = new SendGrid.Helpers.Mail.EmailAddress(Constants.TrashMobEmailAddress, Constants.TrashMobEmailName);

            var tos = new List<SendGrid.Helpers.Mail.EmailAddress>();
            foreach (var address in email.Addresses)
            {
                tos.Add(new SendGrid.Helpers.Mail.EmailAddress(address.Email, address.Name));
            }

            var body = email.Message;

            try
            {
                var client = new SendGridClient(sendGridApiKey);
                var message = MailHelper.CreateSingleEmailToMultipleRecipients(from, tos, email.Subject, body, "");
                var response = await client.SendEmailAsync(message, cancellationToken);
                Console.WriteLine(response);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
            }
        }
    }
}
