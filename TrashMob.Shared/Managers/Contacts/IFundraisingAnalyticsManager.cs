namespace TrashMob.Shared.Managers.Contacts
{
    using System;
    using System.Collections.Generic;
    using System.Threading;
    using System.Threading.Tasks;
    using TrashMob.Models.Poco;

    /// <summary>
    /// Computes engagement scores, donor lifecycle stages, and fundraising dashboard metrics.
    /// All data is computed on-the-fly from existing entities (no stored scores).
    /// </summary>
    public interface IFundraisingAnalyticsManager
    {
        /// <summary>
        /// Gets engagement scores for all active contacts.
        /// </summary>
        Task<IEnumerable<ContactEngagementScore>> GetEngagementScoresAsync(CancellationToken cancellationToken = default);

        /// <summary>
        /// Gets the engagement score for a single contact.
        /// </summary>
        Task<ContactEngagementScore> GetEngagementScoreAsync(Guid contactId, CancellationToken cancellationToken = default);

        /// <summary>
        /// Gets the fundraising dashboard with aggregate metrics.
        /// </summary>
        Task<FundraisingDashboard> GetDashboardAsync(CancellationToken cancellationToken = default);

        /// <summary>
        /// Gets contacts in the volunteer-to-donor pipeline (high engagement, no donations).
        /// </summary>
        Task<IEnumerable<ContactEngagementScore>> GetVolunteerToDonorPipelineAsync(CancellationToken cancellationToken = default);

        /// <summary>
        /// Gets LYBUNT (Last Year But Unfortunately Not This Year) contacts.
        /// </summary>
        Task<IEnumerable<ContactEngagementScore>> GetLybuntContactsAsync(CancellationToken cancellationToken = default);

        /// <summary>
        /// Generates CSV content for the donor report.
        /// </summary>
        Task<string> GenerateDonorReportCsvAsync(CancellationToken cancellationToken = default);

        /// <summary>
        /// Generates CSV content for the fundraising summary report.
        /// </summary>
        Task<string> GenerateFundraisingSummaryCsvAsync(CancellationToken cancellationToken = default);
    }
}
