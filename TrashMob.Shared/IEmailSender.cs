namespace TrashMob.Shared
{
    using System.Threading;
    using System.Threading.Tasks;

    public interface IEmailSender
    {
        public Task SendEmailAsync(Email email, CancellationToken cancellationToken = default);

        public Task SendTemplatedEmailAsync(Email email, CancellationToken cancellationToken = default);

        public string ApiKey { get; set; }
    }
}
