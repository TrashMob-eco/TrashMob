namespace TrashMob.Shared
{
    using System.Threading;
    using System.Threading.Tasks;

    public interface IEmailSender
    {
        public Task SendEmailAsync(Email email, string apiKey, CancellationToken cancellationToken = default);
    }
}
