
namespace TrashMob.Shared.Persistence
{
    using Microsoft.Extensions.Configuration;
    using System.Threading;
    using System.Threading.Tasks;
    using TrashMob.Shared;

    public class EmailManager : IEmailManager
    {
        private readonly IConfiguration configuration;
        private readonly IEmailSender emailSender;

        public EmailManager(IConfiguration configuration, IEmailSender emailSender)
        {
            this.configuration = configuration;
            this.emailSender = emailSender;
        }

        public Task SendSystemEmail(Email email, CancellationToken cancellationToken = default)
        {
            var sendGridApiKey = configuration["sendGridApiKey"];

            return emailSender.SendEmailAsync(email, sendGridApiKey, cancellationToken);
        }
    }
}
