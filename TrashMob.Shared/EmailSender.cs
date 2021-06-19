namespace TrashMob.Shared
{
    using SendGrid;
    using SendGrid.Helpers.Mail;
    using System;
    using System.Collections.Generic;
    using System.Threading;
    using System.Threading.Tasks;

    public class EmailSender : IEmailSender
    {
        public async Task SendEmailAsync(Email email, string apiKey, CancellationToken cancellationToken = default)
        {
            // To not send emails from dev environments, don't store an apikey password in the local secrets
            if (string.IsNullOrWhiteSpace(apiKey) || apiKey == "x")
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
                var client = new SendGridClient(apiKey);
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
