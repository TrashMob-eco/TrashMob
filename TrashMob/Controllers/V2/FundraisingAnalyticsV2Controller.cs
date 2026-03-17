namespace TrashMob.Controllers.V2
{
    using System;
    using System.Collections.Generic;
    using System.Text;
    using System.Threading;
    using System.Threading.Tasks;
    using Asp.Versioning;
    using Microsoft.AspNetCore.Authorization;
    using Microsoft.AspNetCore.Cors;
    using Microsoft.AspNetCore.Http;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.Extensions.Logging;
    using Microsoft.Identity.Web.Resource;
    using TrashMob.Models.Poco;
    using TrashMob.Security;
    using TrashMob.Shared;
    using TrashMob.Shared.Managers.Contacts;

    /// <summary>
    /// V2 admin controller for fundraising analytics and reporting.
    /// </summary>
    [ApiController]
    [ApiVersion("2.0")]
    [EnableCors("_myAllowSpecificOrigins")]
    [Route("api/v{version:apiVersion}/fundraising-analytics")]
    [Authorize(Policy = AuthorizationPolicyConstants.UserIsAdmin)]
    public class FundraisingAnalyticsV2Controller(
        IFundraisingAnalyticsManager analyticsManager,
        ILogger<FundraisingAnalyticsV2Controller> logger) : ControllerBase
    {
        private Guid UserId => Guid.TryParse(HttpContext.Items["UserId"]?.ToString(), out var parsedUserId) ? parsedUserId : Guid.Empty;

        /// <summary>
        /// Gets engagement scores for all active contacts.
        /// </summary>
        /// <param name="cancellationToken">Cancellation token.</param>
        /// <returns>A list of engagement scores.</returns>
        /// <response code="200">Returns engagement scores.</response>
        [HttpGet("engagement-scores")]
        [RequiredScope(Constants.TrashMobReadScope)]
        [ProducesResponseType(typeof(IEnumerable<ContactEngagementScore>), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetEngagementScores(CancellationToken cancellationToken)
        {
            logger.LogInformation("V2 GetEngagementScores by User={UserId}", UserId);

            var scores = await analyticsManager.GetEngagementScoresAsync(cancellationToken);

            return Ok(scores);
        }

        /// <summary>
        /// Gets the engagement score for a single contact.
        /// </summary>
        /// <param name="contactId">The contact identifier.</param>
        /// <param name="cancellationToken">Cancellation token.</param>
        /// <returns>The engagement score.</returns>
        /// <response code="200">Returns the engagement score.</response>
        [HttpGet("engagement-scores/{contactId}")]
        [RequiredScope(Constants.TrashMobReadScope)]
        [ProducesResponseType(typeof(ContactEngagementScore), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetEngagementScore(Guid contactId, CancellationToken cancellationToken)
        {
            logger.LogInformation("V2 GetEngagementScore Contact={ContactId} by User={UserId}", contactId, UserId);

            var score = await analyticsManager.GetEngagementScoreAsync(contactId, cancellationToken);

            return Ok(score);
        }

        /// <summary>
        /// Gets the fundraising dashboard with aggregate metrics.
        /// </summary>
        /// <param name="cancellationToken">Cancellation token.</param>
        /// <returns>The fundraising dashboard.</returns>
        /// <response code="200">Returns the dashboard.</response>
        [HttpGet("dashboard")]
        [RequiredScope(Constants.TrashMobReadScope)]
        [ProducesResponseType(typeof(FundraisingDashboard), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetDashboard(CancellationToken cancellationToken)
        {
            logger.LogInformation("V2 GetDashboard by User={UserId}", UserId);

            var dashboard = await analyticsManager.GetDashboardAsync(cancellationToken);

            return Ok(dashboard);
        }

        /// <summary>
        /// Gets contacts in the volunteer-to-donor pipeline (high engagement, no donations).
        /// </summary>
        /// <param name="cancellationToken">Cancellation token.</param>
        /// <returns>A list of pipeline contacts.</returns>
        /// <response code="200">Returns pipeline contacts.</response>
        [HttpGet("volunteer-pipeline")]
        [RequiredScope(Constants.TrashMobReadScope)]
        [ProducesResponseType(typeof(IEnumerable<ContactEngagementScore>), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetVolunteerPipeline(CancellationToken cancellationToken)
        {
            logger.LogInformation("V2 GetVolunteerPipeline by User={UserId}", UserId);

            var pipeline = await analyticsManager.GetVolunteerToDonorPipelineAsync(cancellationToken);

            return Ok(pipeline);
        }

        /// <summary>
        /// Gets LYBUNT (Last Year But Unfortunately Not This Year) contacts.
        /// </summary>
        /// <param name="cancellationToken">Cancellation token.</param>
        /// <returns>A list of LYBUNT contacts.</returns>
        /// <response code="200">Returns LYBUNT contacts.</response>
        [HttpGet("lybunt")]
        [RequiredScope(Constants.TrashMobReadScope)]
        [ProducesResponseType(typeof(IEnumerable<ContactEngagementScore>), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetLybuntContacts(CancellationToken cancellationToken)
        {
            logger.LogInformation("V2 GetLybuntContacts by User={UserId}", UserId);

            var contacts = await analyticsManager.GetLybuntContactsAsync(cancellationToken);

            return Ok(contacts);
        }

        /// <summary>
        /// Exports a donor report as CSV.
        /// </summary>
        /// <param name="cancellationToken">Cancellation token.</param>
        /// <returns>A CSV file containing the donor report.</returns>
        /// <response code="200">Returns the CSV file.</response>
        [HttpGet("export/donors")]
        [RequiredScope(Constants.TrashMobReadScope)]
        [ProducesResponseType(typeof(FileContentResult), StatusCodes.Status200OK)]
        public async Task<IActionResult> ExportDonorReport(CancellationToken cancellationToken)
        {
            logger.LogInformation("V2 ExportDonorReport by User={UserId}", UserId);

            var csv = await analyticsManager.GenerateDonorReportCsvAsync(cancellationToken);
            var bytes = Encoding.UTF8.GetBytes(csv);
            var filename = $"DonorReport_{DateTime.UtcNow:yyyyMMdd}.csv";

            return File(bytes, "text/csv", filename);
        }

        /// <summary>
        /// Exports a fundraising summary as CSV.
        /// </summary>
        /// <param name="cancellationToken">Cancellation token.</param>
        /// <returns>A CSV file containing the fundraising summary.</returns>
        /// <response code="200">Returns the CSV file.</response>
        [HttpGet("export/summary")]
        [RequiredScope(Constants.TrashMobReadScope)]
        [ProducesResponseType(typeof(FileContentResult), StatusCodes.Status200OK)]
        public async Task<IActionResult> ExportFundraisingSummary(CancellationToken cancellationToken)
        {
            logger.LogInformation("V2 ExportFundraisingSummary by User={UserId}", UserId);

            var csv = await analyticsManager.GenerateFundraisingSummaryCsvAsync(cancellationToken);
            var bytes = Encoding.UTF8.GetBytes(csv);
            var filename = $"FundraisingSummary_{DateTime.UtcNow:yyyyMMdd}.csv";

            return File(bytes, "text/csv", filename);
        }
    }
}
