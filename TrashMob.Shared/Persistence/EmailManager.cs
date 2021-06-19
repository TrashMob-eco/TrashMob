
namespace TrashMob.Shared.Persistence
{
    using Microsoft.Extensions.Configuration;
    using System.Threading;
    using System.Threading.Tasks;
    using TrashMob.Shared;

    public class EmailManager : IEmailManager
    {
        private readonly IConfiguration configuration;

        public EmailManager(IConfiguration configuration)
        {
            this.configuration = configuration;
        }

        public Task SendSystemEmail(Email email, CancellationToken cancellationToken = default)
        {
            var sendGridApiKey = configuration["sendGridApiKey"];

            var emailSender = new EmailSender();
            return emailSender.SendEmailAsync(email, sendGridApiKey, cancellationToken);
        }
    }
}
