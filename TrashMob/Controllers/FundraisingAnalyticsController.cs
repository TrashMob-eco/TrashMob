namespace TrashMob.Controllers
{
    using System;
    using System.Collections.Generic;
    using System.Text;
    using System.Threading;
    using System.Threading.Tasks;
    using Microsoft.AspNetCore.Authorization;
    using Microsoft.AspNetCore.Http;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.Identity.Web.Resource;
    using TrashMob.Models.Poco;
    using TrashMob.Security;
    using TrashMob.Shared;
    using TrashMob.Shared.Managers.Contacts;

    /// <summary>
    /// Controller for fundraising analytics, engagement scoring, and reporting (admin only).
    /// </summary>
    [Route("api/fundraising-analytics")]
    [Authorize(Policy = AuthorizationPolicyConstants.UserIsAdmin)]
    [RequiredScope(Constants.TrashMobReadScope)]
    public class FundraisingAnalyticsController(
        IFundraisingAnalyticsManager analyticsManager)
        : SecureController
    {
        /// <summary>
        /// Gets engagement scores for all active contacts.
        /// </summary>
        /// <param name="cancellationToken">Cancellation token.</param>
        [HttpGet("engagement-scores")]
        [ProducesResponseType(typeof(IEnumerable<ContactEngagementScore>), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetEngagementScores(CancellationToken cancellationToken)
        {
            var scores = await analyticsManager.GetEngagementScoresAsync(cancellationToken);
            return Ok(scores);
        }

        /// <summary>
        /// Gets the engagement score for a single contact.
        /// </summary>
        /// <param name="contactId">The contact ID.</param>
        /// <param name="cancellationToken">Cancellation token.</param>
        [HttpGet("engagement-scores/{contactId}")]
        [ProducesResponseType(typeof(ContactEngagementScore), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetEngagementScore(Guid contactId, CancellationToken cancellationToken)
        {
            var score = await analyticsManager.GetEngagementScoreAsync(contactId, cancellationToken);
            return Ok(score);
        }

        /// <summary>
        /// Gets the fundraising dashboard with aggregate metrics.
        /// </summary>
        /// <param name="cancellationToken">Cancellation token.</param>
        [HttpGet("dashboard")]
        [ProducesResponseType(typeof(FundraisingDashboard), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetDashboard(CancellationToken cancellationToken)
        {
            var dashboard = await analyticsManager.GetDashboardAsync(cancellationToken);
            return Ok(dashboard);
        }

        /// <summary>
        /// Gets contacts in the volunteer-to-donor pipeline (high engagement, no donations).
        /// </summary>
        /// <param name="cancellationToken">Cancellation token.</param>
        [HttpGet("volunteer-pipeline")]
        [ProducesResponseType(typeof(IEnumerable<ContactEngagementScore>), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetVolunteerPipeline(CancellationToken cancellationToken)
        {
            var pipeline = await analyticsManager.GetVolunteerToDonorPipelineAsync(cancellationToken);
            return Ok(pipeline);
        }

        /// <summary>
        /// Gets LYBUNT (Last Year But Unfortunately Not This Year) contacts.
        /// </summary>
        /// <param name="cancellationToken">Cancellation token.</param>
        [HttpGet("lybunt")]
        [ProducesResponseType(typeof(IEnumerable<ContactEngagementScore>), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetLybuntContacts(CancellationToken cancellationToken)
        {
            var lybunt = await analyticsManager.GetLybuntContactsAsync(cancellationToken);
            return Ok(lybunt);
        }

        /// <summary>
        /// Exports a donor report as CSV.
        /// </summary>
        /// <param name="cancellationToken">Cancellation token.</param>
        [HttpGet("export/donors")]
        [ProducesResponseType(typeof(FileResult), StatusCodes.Status200OK)]
        public async Task<IActionResult> ExportDonorReport(CancellationToken cancellationToken)
        {
            var csv = await analyticsManager.GenerateDonorReportCsvAsync(cancellationToken);
            var bytes = Encoding.UTF8.GetBytes(csv);
            var fileName = $"DonorReport_{DateTime.UtcNow:yyyyMMdd}.csv";

            return File(bytes, "text/csv", fileName);
        }

        /// <summary>
        /// Exports a fundraising summary as CSV.
        /// </summary>
        /// <param name="cancellationToken">Cancellation token.</param>
        [HttpGet("export/summary")]
        [ProducesResponseType(typeof(FileResult), StatusCodes.Status200OK)]
        public async Task<IActionResult> ExportFundraisingSummary(CancellationToken cancellationToken)
        {
            var csv = await analyticsManager.GenerateFundraisingSummaryCsvAsync(cancellationToken);
            var bytes = Encoding.UTF8.GetBytes(csv);
            var fileName = $"FundraisingSummary_{DateTime.UtcNow:yyyyMMdd}.csv";

            return File(bytes, "text/csv", fileName);
        }
    }
}
