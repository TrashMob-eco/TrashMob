namespace TrashMob.Shared.Managers.Contacts
{
    using System;
    using System.Collections.Generic;
    using System.Threading;
    using System.Threading.Tasks;

    /// <summary>
    /// Manages sending donation-related emails (thank-you, receipts, appeals).
    /// </summary>
    public interface IDonationEmailManager
    {
        /// <summary>
        /// Sends a thank-you email for a donation and logs it as a ContactNote.
        /// </summary>
        Task SendThankYouAsync(Guid donationId, Guid userId, CancellationToken cancellationToken = default);

        /// <summary>
        /// Sends a tax receipt email with PDF attachment for a donation and logs it as a ContactNote.
        /// </summary>
        Task SendReceiptAsync(Guid donationId, Guid userId, CancellationToken cancellationToken = default);

        /// <summary>
        /// Sends a fundraising appeal email to a single contact and logs it as a ContactNote.
        /// </summary>
        Task SendAppealAsync(Guid contactId, string subject, string body, Guid userId, CancellationToken cancellationToken = default);

        /// <summary>
        /// Sends a fundraising appeal email to multiple contacts and logs each as a ContactNote.
        /// </summary>
        /// <returns>A summary with sent count and any errors.</returns>
        Task<BulkAppealResult> SendBulkAppealAsync(IEnumerable<Guid> contactIds, string subject, string body, Guid userId, CancellationToken cancellationToken = default);
    }

    /// <summary>
    /// Result of a bulk appeal send operation.
    /// </summary>
    public class BulkAppealResult
    {
        /// <summary>
        /// Gets or sets the number of appeals successfully sent.
        /// </summary>
        public int SentCount { get; set; }

        /// <summary>
        /// Gets or sets the number of appeals that failed.
        /// </summary>
        public int FailedCount { get; set; }

        /// <summary>
        /// Gets or sets the number of contacts skipped (no email address).
        /// </summary>
        public int SkippedCount { get; set; }
    }
}
