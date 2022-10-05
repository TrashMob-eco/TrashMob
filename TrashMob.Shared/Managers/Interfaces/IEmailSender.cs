namespace TrashMob.Shared.Managers.Interfaces
{
    using System.Threading;
    using System.Threading.Tasks;
    using TrashMob.Shared.Poco;

    public interface IEmailSender
    {
        public Task SendEmailAsync(Email email, CancellationToken cancellationToken = default);

        public Task SendTemplatedEmailAsync(Email email, CancellationToken cancellationToken = default);

        public string ApiKey { get; set; }
    }
}
