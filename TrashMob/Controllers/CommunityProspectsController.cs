namespace TrashMob.Controllers
{
    using System;
    using System.Collections.Generic;
    using System.Threading;
    using System.Threading.Tasks;
    using Microsoft.AspNetCore.Authorization;
    using Microsoft.AspNetCore.Http;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.Identity.Web.Resource;
    using TrashMob.Models;
    using TrashMob.Models.Poco;
    using TrashMob.Security;
    using TrashMob.Shared;
    using TrashMob.Shared.Managers.Interfaces;
    using TrashMob.Shared.Managers.Prospects;

    /// <summary>
    /// Controller for community prospect management (admin only).
    /// </summary>
    [Route("api/communityprospects")]
    [Authorize(Policy = AuthorizationPolicyConstants.UserIsAdmin)]
    [RequiredScope(Constants.TrashMobWriteScope)]
    public class CommunityProspectsController(
        ICommunityProspectManager communityProspectManager,
        IProspectActivityManager prospectActivityManager,
        IClaudeDiscoveryService claudeDiscoveryService,
        IProspectScoringManager prospectScoringManager,
        ICsvImportManager csvImportManager,
        IProspectOutreachManager prospectOutreachManager,
        IPipelineAnalyticsManager pipelineAnalyticsManager,
        IProspectConversionManager prospectConversionManager)
        : SecureController
    {
        /// <summary>
        /// Gets all community prospects, optionally filtered by pipeline stage or search term.
        /// </summary>
        /// <param name="stage">Optional pipeline stage filter.</param>
        /// <param name="search">Optional search term.</param>
        /// <param name="cancellationToken">Cancellation token.</param>
        [HttpGet]
        [ProducesResponseType(typeof(IEnumerable<CommunityProspect>), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetAll(
            [FromQuery] int? stage,
            [FromQuery] string search,
            CancellationToken cancellationToken)
        {
            IEnumerable<CommunityProspect> prospects;

            if (!string.IsNullOrWhiteSpace(search))
            {
                prospects = await communityProspectManager.SearchAsync(search, cancellationToken);
            }
            else if (stage.HasValue)
            {
                prospects = await communityProspectManager.GetByPipelineStageAsync(stage.Value, cancellationToken);
            }
            else
            {
                prospects = await communityProspectManager.GetAsync(cancellationToken);
            }

            return Ok(prospects);
        }

        /// <summary>
        /// Gets a community prospect by ID.
        /// </summary>
        /// <param name="id">The prospect ID.</param>
        /// <param name="cancellationToken">Cancellation token.</param>
        [HttpGet("{id}")]
        [ProducesResponseType(typeof(CommunityProspect), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetById(Guid id, CancellationToken cancellationToken)
        {
            var prospect = await communityProspectManager.GetAsync(id, cancellationToken);

            if (prospect == null)
            {
                return NotFound();
            }

            return Ok(prospect);
        }

        /// <summary>
        /// Creates a new community prospect.
        /// </summary>
        /// <param name="prospect">The prospect to create.</param>
        /// <param name="cancellationToken">Cancellation token.</param>
        [HttpPost]
        [ProducesResponseType(typeof(CommunityProspect), StatusCodes.Status200OK)]
        public async Task<IActionResult> Create(CommunityProspect prospect, CancellationToken cancellationToken)
        {
            var result = await communityProspectManager.AddAsync(prospect, UserId, cancellationToken);
            return Ok(result);
        }

        /// <summary>
        /// Updates a community prospect.
        /// </summary>
        /// <param name="prospect">The updated prospect data.</param>
        /// <param name="cancellationToken">Cancellation token.</param>
        [HttpPut]
        [ProducesResponseType(typeof(CommunityProspect), StatusCodes.Status200OK)]
        public async Task<IActionResult> Update(CommunityProspect prospect, CancellationToken cancellationToken)
        {
            var result = await communityProspectManager.UpdateAsync(prospect, UserId, cancellationToken);
            return Ok(result);
        }

        /// <summary>
        /// Deletes a community prospect.
        /// </summary>
        /// <param name="id">The prospect ID to delete.</param>
        /// <param name="cancellationToken">Cancellation token.</param>
        [HttpDelete("{id}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> Delete(Guid id, CancellationToken cancellationToken)
        {
            await communityProspectManager.DeleteAsync(id, cancellationToken);
            return NoContent();
        }

        /// <summary>
        /// Updates the pipeline stage of a community prospect.
        /// </summary>
        /// <param name="id">The prospect ID.</param>
        /// <param name="request">The new pipeline stage.</param>
        /// <param name="cancellationToken">Cancellation token.</param>
        [HttpPut("{id}/stage")]
        [ProducesResponseType(typeof(CommunityProspect), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> UpdateStage(Guid id, [FromBody] UpdateStageRequest request, CancellationToken cancellationToken)
        {
            var result = await communityProspectManager.UpdatePipelineStageAsync(id, request.Stage, UserId, cancellationToken);

            if (result == null)
            {
                return NotFound();
            }

            return Ok(result);
        }

        /// <summary>
        /// Gets all activities for a community prospect.
        /// </summary>
        /// <param name="id">The prospect ID.</param>
        /// <param name="cancellationToken">Cancellation token.</param>
        [HttpGet("{id}/activities")]
        [ProducesResponseType(typeof(IEnumerable<ProspectActivity>), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetActivities(Guid id, CancellationToken cancellationToken)
        {
            var activities = await prospectActivityManager.GetByProspectIdAsync(id, cancellationToken);
            return Ok(activities);
        }

        /// <summary>
        /// Creates a new activity for a community prospect.
        /// </summary>
        /// <param name="id">The prospect ID.</param>
        /// <param name="activity">The activity to create.</param>
        /// <param name="cancellationToken">Cancellation token.</param>
        [HttpPost("{id}/activities")]
        [ProducesResponseType(typeof(ProspectActivity), StatusCodes.Status200OK)]
        public async Task<IActionResult> CreateActivity(Guid id, ProspectActivity activity, CancellationToken cancellationToken)
        {
            activity.ProspectId = id;
            var result = await prospectActivityManager.AddAsync(activity, UserId, cancellationToken);
            return Ok(result);
        }

        /// <summary>
        /// Discovers new community prospects using AI.
        /// </summary>
        /// <param name="request">The discovery request with optional prompt or location.</param>
        /// <param name="cancellationToken">Cancellation token.</param>
        [HttpPost("discover")]
        [ProducesResponseType(typeof(DiscoveryResult), StatusCodes.Status200OK)]
        public async Task<IActionResult> Discover(DiscoveryRequest request, CancellationToken cancellationToken)
        {
            var result = await claudeDiscoveryService.DiscoverProspectsAsync(request, cancellationToken);
            return Ok(result);
        }

        /// <summary>
        /// Gets the FitScore breakdown for a specific prospect.
        /// </summary>
        /// <param name="id">The prospect ID.</param>
        /// <param name="cancellationToken">Cancellation token.</param>
        [HttpGet("{id}/score")]
        [ProducesResponseType(typeof(FitScoreBreakdown), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetScoreBreakdown(Guid id, CancellationToken cancellationToken)
        {
            var breakdown = await prospectScoringManager.CalculateFitScoreAsync(id, cancellationToken);

            if (breakdown == null)
            {
                return NotFound();
            }

            return Ok(breakdown);
        }

        /// <summary>
        /// Recalculates FitScores for all prospects.
        /// </summary>
        /// <param name="cancellationToken">Cancellation token.</param>
        [HttpPost("rescore")]
        [ProducesResponseType(typeof(int), StatusCodes.Status200OK)]
        public async Task<IActionResult> RescoreAll(CancellationToken cancellationToken)
        {
            var count = await prospectScoringManager.RecalculateAllScoresAsync(UserId, cancellationToken);
            return Ok(count);
        }

        /// <summary>
        /// Gets geographic areas with events but no active community partner.
        /// </summary>
        /// <param name="cancellationToken">Cancellation token.</param>
        [HttpGet("gaps")]
        [ProducesResponseType(typeof(IEnumerable<GeographicGap>), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetGeographicGaps(CancellationToken cancellationToken)
        {
            var gaps = await prospectScoringManager.GetGeographicGapsAsync(cancellationToken);
            return Ok(gaps);
        }

        /// <summary>
        /// Imports community prospects from a CSV file.
        /// </summary>
        /// <param name="file">The CSV file to import.</param>
        /// <param name="cancellationToken">Cancellation token.</param>
        [HttpPost("import")]
        [ProducesResponseType(typeof(CsvImportResult), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> ImportCsv(IFormFile file, CancellationToken cancellationToken)
        {
            if (file == null || file.Length == 0)
            {
                return BadRequest("No file provided.");
            }

            if (!file.FileName.EndsWith(".csv", StringComparison.OrdinalIgnoreCase))
            {
                return BadRequest("Only .csv files are supported.");
            }

            using var stream = file.OpenReadStream();
            var result = await csvImportManager.ImportProspectsAsync(stream, UserId, cancellationToken);
            return Ok(result);
        }
        /// <summary>
        /// Generates a preview of the next outreach email for a prospect without sending.
        /// </summary>
        /// <param name="id">The prospect ID.</param>
        /// <param name="cancellationToken">Cancellation token.</param>
        [HttpPost("{id}/outreach/preview")]
        [ProducesResponseType(typeof(OutreachPreview), StatusCodes.Status200OK)]
        public async Task<IActionResult> PreviewOutreach(Guid id, CancellationToken cancellationToken)
        {
            var preview = await prospectOutreachManager.PreviewOutreachAsync(id, cancellationToken);
            return Ok(preview);
        }

        /// <summary>
        /// Sends an outreach email to a prospect.
        /// </summary>
        /// <param name="id">The prospect ID.</param>
        /// <param name="cancellationToken">Cancellation token.</param>
        [HttpPost("{id}/outreach")]
        [ProducesResponseType(typeof(OutreachSendResult), StatusCodes.Status200OK)]
        public async Task<IActionResult> SendOutreach(Guid id, CancellationToken cancellationToken)
        {
            var result = await prospectOutreachManager.SendOutreachAsync(id, UserId, cancellationToken);
            return Ok(result);
        }

        /// <summary>
        /// Gets the outreach email history for a prospect.
        /// </summary>
        /// <param name="id">The prospect ID.</param>
        /// <param name="cancellationToken">Cancellation token.</param>
        [HttpGet("{id}/outreach/history")]
        [ProducesResponseType(typeof(IEnumerable<ProspectOutreachEmail>), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetOutreachHistory(Guid id, CancellationToken cancellationToken)
        {
            var history = await prospectOutreachManager.GetOutreachHistoryAsync(id, cancellationToken);
            return Ok(history);
        }

        /// <summary>
        /// Sends outreach emails to multiple prospects.
        /// </summary>
        /// <param name="request">The batch outreach request.</param>
        /// <param name="cancellationToken">Cancellation token.</param>
        [HttpPost("outreach/batch")]
        [ProducesResponseType(typeof(BatchOutreachResult), StatusCodes.Status200OK)]
        public async Task<IActionResult> SendBatchOutreach(BatchOutreachRequest request, CancellationToken cancellationToken)
        {
            var result = await prospectOutreachManager.SendBatchOutreachAsync(request.ProspectIds, UserId, cancellationToken);
            return Ok(result);
        }

        /// <summary>
        /// Gets the current outreach configuration settings.
        /// </summary>
        [HttpGet("outreach/settings")]
        [ProducesResponseType(typeof(OutreachSettings), StatusCodes.Status200OK)]
        public IActionResult GetOutreachSettings()
        {
            var settings = prospectOutreachManager.GetOutreachSettings();
            return Ok(settings);
        }

        /// <summary>
        /// Gets pipeline analytics including funnel metrics, outreach effectiveness, and conversion rates.
        /// </summary>
        /// <param name="cancellationToken">Cancellation token.</param>
        [HttpGet("analytics")]
        [ProducesResponseType(typeof(PipelineAnalytics), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetAnalytics(CancellationToken cancellationToken)
        {
            var analytics = await pipelineAnalyticsManager.GetAnalyticsAsync(cancellationToken);
            return Ok(analytics);
        }

        /// <summary>
        /// Converts a prospect to a partner organization.
        /// </summary>
        /// <param name="id">The prospect ID.</param>
        /// <param name="request">The conversion request with partner type and email options.</param>
        /// <param name="cancellationToken">Cancellation token.</param>
        [HttpPost("{id}/convert")]
        [ProducesResponseType(typeof(ProspectConversionResult), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> ConvertToPartner(Guid id, [FromBody] ProspectConversionRequest request, CancellationToken cancellationToken)
        {
            request.ProspectId = id;
            var result = await prospectConversionManager.ConvertToPartnerAsync(request, UserId, cancellationToken);

            if (!result.Success && result.ErrorMessage?.Contains("not found") == true)
            {
                return NotFound();
            }

            return Ok(result);
        }
    }

    /// <summary>
    /// Request body for updating a prospect's pipeline stage.
    /// </summary>
    public class UpdateStageRequest
    {
        /// <summary>
        /// Gets or sets the new pipeline stage value.
        /// </summary>
        public int Stage { get; set; }
    }
}
