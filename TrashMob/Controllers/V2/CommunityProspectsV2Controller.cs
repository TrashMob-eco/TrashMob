namespace TrashMob.Controllers.V2
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading;
    using System.Threading.Tasks;
    using Asp.Versioning;
    using Microsoft.AspNetCore.Authorization;
    using Microsoft.AspNetCore.Cors;
    using Microsoft.AspNetCore.Http;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.Extensions.Logging;
    using Microsoft.Identity.Web.Resource;
    using TrashMob.Models;
    using TrashMob.Models.Extensions.V2;
    using TrashMob.Models.Poco;
    using TrashMob.Models.Poco.V2;
    using TrashMob.Security;
    using TrashMob.Shared;
    using TrashMob.Shared.Managers.Interfaces;
    using TrashMob.Shared.Managers.Prospects;

    /// <summary>
    /// V2 controller for community prospect management (admin only).
    /// </summary>
    [ApiController]
    [ApiVersion("2.0")]
    [EnableCors("_myAllowSpecificOrigins")]
    [Route("api/v{version:apiVersion}/community-prospects")]
    [Authorize(Policy = AuthorizationPolicyConstants.UserIsAdmin)]
    [RequiredScope(Constants.TrashMobWriteScope)]
    public class CommunityProspectsV2Controller(
        ICommunityProspectManager communityProspectManager,
        IProspectActivityManager prospectActivityManager,
        IClaudeDiscoveryService claudeDiscoveryService,
        IProspectScoringManager prospectScoringManager,
        ICsvImportManager csvImportManager,
        IProspectOutreachManager prospectOutreachManager,
        IPipelineAnalyticsManager pipelineAnalyticsManager,
        IProspectConversionManager prospectConversionManager,
        ILogger<CommunityProspectsV2Controller> logger) : ControllerBase
    {
        private Guid UserId => Guid.TryParse(HttpContext.Items["UserId"]?.ToString(), out var parsedUserId) ? parsedUserId : Guid.Empty;

        /// <summary>
        /// Gets all community prospects, optionally filtered by pipeline stage or search term.
        /// </summary>
        /// <param name="stage">Optional pipeline stage filter.</param>
        /// <param name="search">Optional search term.</param>
        /// <param name="cancellationToken">Cancellation token.</param>
        /// <response code="200">Returns the prospect list.</response>
        [HttpGet]
        [ProducesResponseType(typeof(IEnumerable<CommunityProspectDto>), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetAll(
            [FromQuery] int? stage,
            [FromQuery] string search,
            CancellationToken cancellationToken)
        {
            logger.LogInformation("V2 GetAll prospects with stage={Stage}, search={Search}", stage, search);

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

            var dtos = prospects.Select(p => p.ToV2Dto()).ToList();
            return Ok(dtos);
        }

        /// <summary>
        /// Gets a community prospect by ID.
        /// </summary>
        /// <param name="id">The prospect ID.</param>
        /// <param name="cancellationToken">Cancellation token.</param>
        /// <response code="200">Returns the prospect.</response>
        /// <response code="404">Prospect not found.</response>
        [HttpGet("{id}")]
        [ProducesResponseType(typeof(CommunityProspectDto), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetById(Guid id, CancellationToken cancellationToken)
        {
            var prospect = await communityProspectManager.GetAsync(id, cancellationToken);

            if (prospect is null)
            {
                return NotFound();
            }

            return Ok(prospect.ToV2Dto());
        }

        /// <summary>
        /// Creates a new community prospect.
        /// </summary>
        /// <param name="dto">The prospect data.</param>
        /// <param name="cancellationToken">Cancellation token.</param>
        /// <response code="201">Prospect created.</response>
        [HttpPost]
        [ProducesResponseType(typeof(CommunityProspectDto), StatusCodes.Status201Created)]
        public async Task<IActionResult> Create([FromBody] CommunityProspectDto dto, CancellationToken cancellationToken)
        {
            logger.LogInformation("V2 Create prospect {Name}", dto.Name);

            var entity = dto.ToEntity();
            var result = await communityProspectManager.AddAsync(entity, UserId, cancellationToken);

            return CreatedAtAction(nameof(GetById), new { id = result.Id }, result.ToV2Dto());
        }

        /// <summary>
        /// Updates a community prospect.
        /// </summary>
        /// <param name="dto">The updated prospect data.</param>
        /// <param name="cancellationToken">Cancellation token.</param>
        /// <response code="200">Prospect updated.</response>
        [HttpPut]
        [ProducesResponseType(typeof(CommunityProspectDto), StatusCodes.Status200OK)]
        public async Task<IActionResult> Update([FromBody] CommunityProspectDto dto, CancellationToken cancellationToken)
        {
            logger.LogInformation("V2 Update prospect {ProspectId}", dto.Id);

            var entity = dto.ToEntity();
            var result = await communityProspectManager.UpdateAsync(entity, UserId, cancellationToken);

            return Ok(result.ToV2Dto());
        }

        /// <summary>
        /// Deletes a community prospect.
        /// </summary>
        /// <param name="id">The prospect ID to delete.</param>
        /// <param name="cancellationToken">Cancellation token.</param>
        /// <response code="204">Prospect deleted.</response>
        /// <response code="404">Prospect not found.</response>
        [HttpDelete("{id}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> Delete(Guid id, CancellationToken cancellationToken)
        {
            logger.LogInformation("V2 Delete prospect {ProspectId}", id);

            await communityProspectManager.DeleteAsync(id, cancellationToken);
            return NoContent();
        }

        /// <summary>
        /// Updates the pipeline stage of a community prospect.
        /// </summary>
        /// <param name="id">The prospect ID.</param>
        /// <param name="request">The new pipeline stage.</param>
        /// <param name="cancellationToken">Cancellation token.</param>
        /// <response code="200">Stage updated.</response>
        /// <response code="404">Prospect not found.</response>
        [HttpPut("{id}/stage")]
        [ProducesResponseType(typeof(CommunityProspectDto), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> UpdateStage(Guid id, [FromBody] UpdateStageRequest request, CancellationToken cancellationToken)
        {
            logger.LogInformation("V2 UpdateStage prospect {ProspectId} to stage {Stage}", id, request.Stage);

            var result = await communityProspectManager.UpdatePipelineStageAsync(id, request.Stage, UserId, cancellationToken);

            if (result is null)
            {
                return NotFound();
            }

            return Ok(result.ToV2Dto());
        }

        /// <summary>
        /// Gets all activities for a community prospect.
        /// </summary>
        /// <param name="id">The prospect ID.</param>
        /// <param name="cancellationToken">Cancellation token.</param>
        /// <response code="200">Returns the activity list.</response>
        [HttpGet("{id}/activities")]
        [ProducesResponseType(typeof(IEnumerable<ProspectActivityDto>), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetActivities(Guid id, CancellationToken cancellationToken)
        {
            var activities = await prospectActivityManager.GetByProspectIdAsync(id, cancellationToken);
            var dtos = activities.Select(a => a.ToV2Dto()).ToList();

            return Ok(dtos);
        }

        /// <summary>
        /// Creates a new activity for a community prospect.
        /// </summary>
        /// <param name="id">The prospect ID.</param>
        /// <param name="dto">The activity data.</param>
        /// <param name="cancellationToken">Cancellation token.</param>
        /// <response code="201">Activity created.</response>
        [HttpPost("{id}/activities")]
        [ProducesResponseType(typeof(ProspectActivityDto), StatusCodes.Status201Created)]
        public async Task<IActionResult> CreateActivity(Guid id, [FromBody] ProspectActivityDto dto, CancellationToken cancellationToken)
        {
            logger.LogInformation("V2 CreateActivity for prospect {ProspectId}", id);

            var entity = dto.ToEntity();
            entity.ProspectId = id;
            var result = await prospectActivityManager.AddAsync(entity, UserId, cancellationToken);

            return CreatedAtAction(nameof(GetActivities), new { id }, result.ToV2Dto());
        }

        /// <summary>
        /// Discovers new community prospects using AI.
        /// </summary>
        /// <param name="request">The discovery request with optional prompt or location.</param>
        /// <param name="cancellationToken">Cancellation token.</param>
        /// <response code="200">Returns the discovery results.</response>
        [HttpPost("discover")]
        [ProducesResponseType(typeof(DiscoveryResult), StatusCodes.Status200OK)]
        public async Task<IActionResult> Discover([FromBody] DiscoveryRequest request, CancellationToken cancellationToken)
        {
            logger.LogInformation("V2 Discover prospects");

            var result = await claudeDiscoveryService.DiscoverProspectsAsync(request, cancellationToken);
            return Ok(result);
        }

        /// <summary>
        /// Gets the FitScore breakdown for a specific prospect.
        /// </summary>
        /// <param name="id">The prospect ID.</param>
        /// <param name="cancellationToken">Cancellation token.</param>
        /// <response code="200">Returns the score breakdown.</response>
        /// <response code="404">Prospect not found.</response>
        [HttpGet("{id}/score")]
        [ProducesResponseType(typeof(FitScoreBreakdown), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetScoreBreakdown(Guid id, CancellationToken cancellationToken)
        {
            var breakdown = await prospectScoringManager.CalculateFitScoreAsync(id, cancellationToken);

            if (breakdown is null)
            {
                return NotFound();
            }

            return Ok(breakdown);
        }

        /// <summary>
        /// Recalculates FitScores for all prospects.
        /// </summary>
        /// <param name="cancellationToken">Cancellation token.</param>
        /// <response code="200">Returns the number of prospects updated.</response>
        [HttpPost("rescore")]
        [ProducesResponseType(typeof(int), StatusCodes.Status200OK)]
        public async Task<IActionResult> RescoreAll(CancellationToken cancellationToken)
        {
            logger.LogInformation("V2 RescoreAll prospects by user {UserId}", UserId);

            var count = await prospectScoringManager.RecalculateAllScoresAsync(UserId, cancellationToken);
            return Ok(count);
        }

        /// <summary>
        /// Gets geographic areas with events but no active community partner.
        /// </summary>
        /// <param name="cancellationToken">Cancellation token.</param>
        /// <response code="200">Returns the geographic gaps.</response>
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
        /// <response code="200">Returns the import results.</response>
        /// <response code="400">Invalid file.</response>
        [HttpPost("import")]
        [ProducesResponseType(typeof(CsvImportResult), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> ImportCsv(IFormFile file, CancellationToken cancellationToken)
        {
            if (file is null || file.Length == 0)
            {
                return Problem("No file provided.", statusCode: StatusCodes.Status400BadRequest);
            }

            if (!file.FileName.EndsWith(".csv", StringComparison.OrdinalIgnoreCase))
            {
                return Problem("Only .csv files are supported.", statusCode: StatusCodes.Status400BadRequest);
            }

            logger.LogInformation("V2 ImportCsv: {FileName} ({Size} bytes)", file.FileName, file.Length);

            using var stream = file.OpenReadStream();
            var result = await csvImportManager.ImportProspectsAsync(stream, UserId, cancellationToken);

            return Ok(result);
        }

        /// <summary>
        /// Generates a preview of the next outreach email for a prospect without sending.
        /// </summary>
        /// <param name="id">The prospect ID.</param>
        /// <param name="cancellationToken">Cancellation token.</param>
        /// <response code="200">Returns the outreach preview.</response>
        [HttpPost("{id}/outreach/preview")]
        [ProducesResponseType(typeof(OutreachPreview), StatusCodes.Status200OK)]
        public async Task<IActionResult> PreviewOutreach(Guid id, CancellationToken cancellationToken)
        {
            logger.LogInformation("V2 PreviewOutreach for prospect {ProspectId}", id);

            var preview = await prospectOutreachManager.PreviewOutreachAsync(id, cancellationToken);
            return Ok(preview);
        }

        /// <summary>
        /// Sends an outreach email to a prospect. Optionally accepts custom subject and body to override AI-generated content.
        /// </summary>
        /// <param name="id">The prospect ID.</param>
        /// <param name="request">Optional custom email content.</param>
        /// <param name="cancellationToken">Cancellation token.</param>
        /// <response code="200">Returns the send result.</response>
        [HttpPost("{id}/outreach")]
        [ProducesResponseType(typeof(OutreachSendResult), StatusCodes.Status200OK)]
        public async Task<IActionResult> SendOutreach(Guid id, [FromBody] OutreachSendRequest request, CancellationToken cancellationToken)
        {
            logger.LogInformation("V2 SendOutreach to prospect {ProspectId}", id);

            var result = await prospectOutreachManager.SendOutreachAsync(id, UserId, request, cancellationToken);
            return Ok(result);
        }

        /// <summary>
        /// Gets the outreach email history for a prospect.
        /// </summary>
        /// <param name="id">The prospect ID.</param>
        /// <param name="cancellationToken">Cancellation token.</param>
        /// <response code="200">Returns the outreach history.</response>
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
        /// <response code="200">Returns the batch send results.</response>
        [HttpPost("outreach/batch")]
        [ProducesResponseType(typeof(BatchOutreachResult), StatusCodes.Status200OK)]
        public async Task<IActionResult> SendBatchOutreach([FromBody] BatchOutreachRequest request, CancellationToken cancellationToken)
        {
            logger.LogInformation("V2 SendBatchOutreach for {Count} prospects", request.ProspectIds.Count);

            var result = await prospectOutreachManager.SendBatchOutreachAsync(request.ProspectIds, UserId, cancellationToken);
            return Ok(result);
        }

        /// <summary>
        /// Gets the current outreach configuration settings.
        /// </summary>
        /// <response code="200">Returns the outreach settings.</response>
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
        /// <response code="200">Returns the analytics data.</response>
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
        /// <response code="200">Returns the conversion result.</response>
        /// <response code="404">Prospect not found.</response>
        [HttpPost("{id}/convert")]
        [ProducesResponseType(typeof(ProspectConversionResult), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> ConvertToPartner(Guid id, [FromBody] ProspectConversionRequest request, CancellationToken cancellationToken)
        {
            logger.LogInformation("V2 ConvertToPartner for prospect {ProspectId}", id);

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
    /// Request body for updating a prospect's pipeline stage (V2).
    /// </summary>
    public class UpdateStageRequest
    {
        /// <summary>
        /// Gets or sets the new pipeline stage value.
        /// </summary>
        public int Stage { get; set; }
    }
}
