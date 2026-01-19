namespace TrashMob.Shared.Managers.Interfaces
{
    using System.Threading;
    using System.Threading.Tasks;
    using TrashMob.Shared.Poco;

    /// <summary>
    /// Defines low-level email sending operations.
    /// </summary>
    public interface IEmailSender
    {
        /// <summary>
        /// Gets or sets the email service API key.
        /// </summary>
        public string ApiKey { get; set; }

        /// <summary>
        /// Sends a plain email.
        /// </summary>
        /// <param name="email">The email to send.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        public Task SendEmailAsync(Email email, CancellationToken cancellationToken);

        /// <summary>
        /// Sends a templated email using the email service's template system.
        /// </summary>
        /// <param name="email">The email with template information.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        public Task SendTemplatedEmailAsync(Email email, CancellationToken cancellationToken);
    }
}