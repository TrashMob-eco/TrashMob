namespace TrashMob.Shared.Managers.Interfaces
{
    using System;
    using System.Collections.Generic;
    using System.Threading;
    using System.Threading.Tasks;
    using TrashMob.Models;

    /// <summary>
    /// Defines operations for managing newsletters.
    /// </summary>
    public interface INewsletterManager : IKeyedManager<Newsletter>
    {
        /// <summary>
        /// Gets all newsletters with optional filtering by status.
        /// </summary>
        /// <param name="status">Optional status filter.</param>
        /// <param name="cancellationToken">Cancellation token.</param>
        /// <returns>Collection of newsletters.</returns>
        Task<IEnumerable<Newsletter>> GetNewslettersAsync(string status = null, CancellationToken cancellationToken = default);

        /// <summary>
        /// Gets newsletters scheduled to be sent.
        /// </summary>
        /// <param name="beforeDate">Get newsletters scheduled before this date.</param>
        /// <param name="cancellationToken">Cancellation token.</param>
        /// <returns>Collection of scheduled newsletters.</returns>
        Task<IEnumerable<Newsletter>> GetScheduledNewslettersAsync(DateTimeOffset beforeDate, CancellationToken cancellationToken = default);

        /// <summary>
        /// Schedules a newsletter for sending.
        /// </summary>
        /// <param name="newsletterId">The newsletter ID.</param>
        /// <param name="scheduledDate">When to send the newsletter.</param>
        /// <param name="userId">The user performing the action.</param>
        /// <param name="cancellationToken">Cancellation token.</param>
        /// <returns>The updated newsletter.</returns>
        Task<Newsletter> ScheduleNewsletterAsync(Guid newsletterId, DateTimeOffset scheduledDate, Guid userId, CancellationToken cancellationToken = default);

        /// <summary>
        /// Sends a newsletter immediately.
        /// </summary>
        /// <param name="newsletterId">The newsletter ID.</param>
        /// <param name="userId">The user performing the action.</param>
        /// <param name="cancellationToken">Cancellation token.</param>
        /// <returns>The updated newsletter.</returns>
        Task<Newsletter> SendNewsletterAsync(Guid newsletterId, Guid userId, CancellationToken cancellationToken = default);

        /// <summary>
        /// Gets recipients for a newsletter based on targeting.
        /// </summary>
        /// <param name="newsletter">The newsletter.</param>
        /// <param name="cancellationToken">Cancellation token.</param>
        /// <returns>Collection of user IDs who should receive the newsletter.</returns>
        Task<IEnumerable<User>> GetRecipientsAsync(Newsletter newsletter, CancellationToken cancellationToken = default);

        /// <summary>
        /// Updates newsletter statistics.
        /// </summary>
        /// <param name="newsletterId">The newsletter ID.</param>
        /// <param name="deliveredCount">Number delivered.</param>
        /// <param name="openCount">Number opened.</param>
        /// <param name="clickCount">Number clicked.</param>
        /// <param name="bounceCount">Number bounced.</param>
        /// <param name="unsubscribeCount">Number unsubscribed.</param>
        /// <param name="cancellationToken">Cancellation token.</param>
        /// <returns>Task.</returns>
        Task UpdateStatisticsAsync(Guid newsletterId, int deliveredCount, int openCount, int clickCount, int bounceCount, int unsubscribeCount, CancellationToken cancellationToken = default);

        /// <summary>
        /// Sends a test email for a newsletter to specified addresses.
        /// </summary>
        /// <param name="newsletterId">The newsletter ID.</param>
        /// <param name="testEmails">List of email addresses to send test to.</param>
        /// <param name="cancellationToken">Cancellation token.</param>
        /// <returns>Task.</returns>
        Task SendTestEmailAsync(Guid newsletterId, IEnumerable<string> testEmails, CancellationToken cancellationToken = default);

        /// <summary>
        /// Processes newsletters that are in 'Sending' status by actually sending the emails.
        /// </summary>
        /// <param name="cancellationToken">Cancellation token.</param>
        /// <returns>Number of newsletters processed.</returns>
        Task<int> ProcessSendingNewslettersAsync(CancellationToken cancellationToken = default);

        /// <summary>
        /// Processes scheduled newsletters that are due to be sent.
        /// </summary>
        /// <param name="cancellationToken">Cancellation token.</param>
        /// <returns>Number of newsletters started.</returns>
        Task<int> ProcessScheduledNewslettersAsync(CancellationToken cancellationToken = default);
    }
}
