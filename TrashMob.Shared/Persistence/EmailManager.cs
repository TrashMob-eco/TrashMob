
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
            this.emailSender.ApiKey = configuration["sendGridApiKey"];
        }

        public Task SendSystemEmail(Email email, CancellationToken cancellationToken = default)
        {
            
            return emailSender.SendEmailAsync(email, cancellationToken);
        }
    }
}
