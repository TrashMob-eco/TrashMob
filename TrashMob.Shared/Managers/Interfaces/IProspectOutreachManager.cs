namespace TrashMob.Shared.Managers.Interfaces
{
    using System;
    using System.Collections.Generic;
    using System.Threading;
    using System.Threading.Tasks;
    using TrashMob.Models;
    using TrashMob.Models.Poco;

    /// <summary>
    /// Manages outreach email operations for community prospects including preview, send, batch, and cadence follow-ups.
    /// </summary>
    public interface IProspectOutreachManager
    {
        /// <summary>
        /// Generates a preview of the next outreach email for a prospect without sending.
        /// </summary>
        Task<OutreachPreview> PreviewOutreachAsync(Guid prospectId, CancellationToken cancellationToken = default);

        /// <summary>
        /// Generates and sends an outreach email to a prospect.
        /// </summary>
        Task<OutreachSendResult> SendOutreachAsync(Guid prospectId, Guid userId, CancellationToken cancellationToken = default);

        /// <summary>
        /// Sends outreach emails to multiple prospects.
        /// </summary>
        Task<BatchOutreachResult> SendBatchOutreachAsync(List<Guid> prospectIds, Guid userId, CancellationToken cancellationToken = default);

        /// <summary>
        /// Gets the outreach email history for a prospect.
        /// </summary>
        Task<IEnumerable<ProspectOutreachEmail>> GetOutreachHistoryAsync(Guid prospectId, CancellationToken cancellationToken = default);

        /// <summary>
        /// Processes prospects that are due for follow-up emails based on the cadence schedule.
        /// Called by the hourly job.
        /// </summary>
        Task<int> ProcessDueFollowUpsAsync(CancellationToken cancellationToken = default);

        /// <summary>
        /// Gets the current outreach configuration settings.
        /// </summary>
        OutreachSettings GetOutreachSettings();
    }
}
