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

    /// <summary>
    /// V2 controller for attendee-submitted event metrics.
    /// </summary>
    [ApiController]
    [ApiVersion("2.0")]
    [EnableCors("_myAllowSpecificOrigins")]
    [Route("api/v{version:apiVersion}/events/{eventId}/attendee-metrics")]
    public class EventAttendeeMetricsV2Controller(
        IEventAttendeeMetricsManager metricsManager,
        IKeyedManager<Event> eventManager,
        IAuthorizationService authorizationService,
        ILogger<EventAttendeeMetricsV2Controller> logger) : ControllerBase
    {
        private Guid UserId => new(HttpContext.Items["UserId"]?.ToString() ?? string.Empty);

        /// <summary>
        /// Gets the current user's metrics submission for an event.
        /// </summary>
        /// <param name="eventId">The event ID.</param>
        /// <param name="cancellationToken">Cancellation token.</param>
        /// <response code="200">Returns the user's metrics.</response>
        /// <response code="404">No metrics found.</response>
        [HttpGet("my-metrics")]
        [Authorize(Policy = AuthorizationPolicyConstants.ValidUser)]
        [RequiredScope(Constants.TrashMobReadScope)]
        [ProducesResponseType(typeof(EventAttendeeMetricsDto), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetMyMetrics(Guid eventId, CancellationToken cancellationToken)
        {
            logger.LogInformation("V2 GetMyMetrics for Event={EventId}", eventId);

            var metrics = await metricsManager.GetMyMetricsAsync(eventId, UserId, cancellationToken);

            if (metrics is null)
            {
                return NotFound();
            }

            return Ok(metrics.ToV2Dto());
        }

        /// <summary>
        /// Submits or updates metrics for the current user.
        /// </summary>
        /// <param name="eventId">The event ID.</param>
        /// <param name="dto">The metrics DTO to submit.</param>
        /// <param name="cancellationToken">Cancellation token.</param>
        /// <response code="200">Returns the submitted metrics.</response>
        /// <response code="400">Validation error.</response>
        [HttpPost("my-metrics")]
        [Authorize(Policy = AuthorizationPolicyConstants.ValidUser)]
        [RequiredScope(Constants.TrashMobWriteScope)]
        [ProducesResponseType(typeof(EventAttendeeMetricsDto), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(string), StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> SubmitMyMetrics(
            Guid eventId,
            [FromBody] EventAttendeeMetricsDto dto,
            CancellationToken cancellationToken)
        {
            logger.LogInformation("V2 SubmitMyMetrics for Event={EventId}", eventId);

            var metrics = dto.ToEntity();
            metrics.EventId = eventId;

            var result = await metricsManager.SubmitMetricsAsync(eventId, UserId, metrics, cancellationToken);

            if (!result.IsSuccess)
            {
                return BadRequest(result.ErrorMessage);
            }

            return Ok(result.Data.ToV2Dto());
        }

        /// <summary>
        /// Gets the public metrics summary for an event.
        /// </summary>
        /// <param name="eventId">The event ID.</param>
        /// <param name="cancellationToken">Cancellation token.</param>
        /// <response code="200">Returns the public metrics summary.</response>
        /// <response code="404">Event not found.</response>
        [HttpGet("public")]
        [AllowAnonymous]
        [ProducesResponseType(typeof(EventMetricsPublicSummary), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetPublicMetrics(Guid eventId, CancellationToken cancellationToken)
        {
            logger.LogInformation("V2 GetPublicMetrics for Event={EventId}", eventId);

            var mobEvent = await eventManager.GetAsync(eventId, cancellationToken);
            if (mobEvent is null)
            {
                return NotFound();
            }

            var summary = await metricsManager.GetPublicMetricsSummaryAsync(eventId, cancellationToken);

            return Ok(summary);
        }

        /// <summary>
        /// Gets pending metrics submissions for an event. Requires event lead.
        /// </summary>
        /// <param name="eventId">The event ID.</param>
        /// <param name="cancellationToken">Cancellation token.</param>
        /// <response code="200">Returns the pending metrics.</response>
        /// <response code="403">Not authorized.</response>
        /// <response code="404">Event not found.</response>
        [HttpGet("pending")]
        [Authorize(Policy = AuthorizationPolicyConstants.ValidUser)]
        [RequiredScope(Constants.TrashMobReadScope)]
        [ProducesResponseType(typeof(IReadOnlyList<EventAttendeeMetricsDto>), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetPending(Guid eventId, CancellationToken cancellationToken)
        {
            logger.LogInformation("V2 GetPendingMetrics for Event={EventId}", eventId);

            var mobEvent = await eventManager.GetAsync(eventId, cancellationToken);
            if (mobEvent is null)
            {
                return NotFound();
            }

            if (!await IsAuthorizedAsync(mobEvent, AuthorizationPolicyConstants.UserIsEventLead))
            {
                return Forbid();
            }

            var pendingMetrics = await metricsManager.GetPendingByEventIdAsync(eventId, cancellationToken);
            var dtos = pendingMetrics.Select(m => m.ToV2Dto()).ToList();

            return Ok(dtos);
        }

        /// <summary>
        /// Approves all pending metrics submissions for an event. Requires event lead.
        /// </summary>
        /// <param name="eventId">The event ID.</param>
        /// <param name="cancellationToken">Cancellation token.</param>
        /// <response code="200">Returns the count of approved submissions.</response>
        /// <response code="403">Not authorized.</response>
        /// <response code="404">Event not found.</response>
        [HttpPost("approve-all")]
        [Authorize(Policy = AuthorizationPolicyConstants.ValidUser)]
        [RequiredScope(Constants.TrashMobWriteScope)]
        [ProducesResponseType(typeof(int), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
        public async Task<IActionResult> ApproveAll(Guid eventId, CancellationToken cancellationToken)
        {
            logger.LogInformation("V2 ApproveAllPendingMetrics for Event={EventId}", eventId);

            var mobEvent = await eventManager.GetAsync(eventId, cancellationToken);
            if (mobEvent is null)
            {
                return NotFound();
            }

            if (!await IsAuthorizedAsync(mobEvent, AuthorizationPolicyConstants.UserIsEventLead))
            {
                return Forbid();
            }

            var result = await metricsManager.ApproveAllPendingAsync(eventId, UserId, cancellationToken);

            if (!result.IsSuccess)
            {
                return BadRequest(result.ErrorMessage);
            }

            return Ok(result.Data);
        }

        /// <summary>
        /// Approves a metrics submission. Requires event lead.
        /// </summary>
        /// <param name="eventId">The event ID.</param>
        /// <param name="metricsId">The metrics ID.</param>
        /// <param name="cancellationToken">Cancellation token.</param>
        /// <response code="200">Returns the approved metrics.</response>
        /// <response code="403">Not authorized.</response>
        [HttpPost("{metricsId}/approve")]
        [Authorize(Policy = AuthorizationPolicyConstants.ValidUser)]
        [RequiredScope(Constants.TrashMobWriteScope)]
        [ProducesResponseType(typeof(EventAttendeeMetricsDto), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(string), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
        public async Task<IActionResult> Approve(Guid eventId, Guid metricsId, CancellationToken cancellationToken)
        {
            logger.LogInformation("V2 ApproveMetrics Metrics={MetricsId} for Event={EventId}", metricsId, eventId);

            var mobEvent = await eventManager.GetAsync(eventId, cancellationToken);
            if (mobEvent is null)
            {
                return NotFound();
            }

            if (!await IsAuthorizedAsync(mobEvent, AuthorizationPolicyConstants.UserIsEventLead))
            {
                return Forbid();
            }

            var result = await metricsManager.ApproveAsync(metricsId, UserId, cancellationToken);

            if (!result.IsSuccess)
            {
                return BadRequest(result.ErrorMessage);
            }

            return Ok(result.Data.ToV2Dto());
        }

        /// <summary>
        /// Rejects a metrics submission. Requires event lead.
        /// </summary>
        /// <param name="eventId">The event ID.</param>
        /// <param name="metricsId">The metrics ID.</param>
        /// <param name="request">The rejection request with reason.</param>
        /// <param name="cancellationToken">Cancellation token.</param>
        /// <response code="200">Returns the rejected metrics.</response>
        /// <response code="403">Not authorized.</response>
        [HttpPost("{metricsId}/reject")]
        [Authorize(Policy = AuthorizationPolicyConstants.ValidUser)]
        [RequiredScope(Constants.TrashMobWriteScope)]
        [ProducesResponseType(typeof(EventAttendeeMetricsDto), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(string), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
        public async Task<IActionResult> Reject(
            Guid eventId,
            Guid metricsId,
            [FromBody] RejectMetricsRequestDto request,
            CancellationToken cancellationToken)
        {
            logger.LogInformation("V2 RejectMetrics Metrics={MetricsId} for Event={EventId}", metricsId, eventId);

            var mobEvent = await eventManager.GetAsync(eventId, cancellationToken);
            if (mobEvent is null)
            {
                return NotFound();
            }

            if (!await IsAuthorizedAsync(mobEvent, AuthorizationPolicyConstants.UserIsEventLead))
            {
                return Forbid();
            }

            var result = await metricsManager.RejectAsync(metricsId, request.RejectionReason, UserId, cancellationToken);

            if (!result.IsSuccess)
            {
                return BadRequest(result.ErrorMessage);
            }

            return Ok(result.Data.ToV2Dto());
        }

        private async Task<bool> IsAuthorizedAsync(object resource, string policy)
        {
            if (User.Identity?.IsAuthenticated != true)
            {
                return false;
            }

            var authResult = await authorizationService.AuthorizeAsync(User, resource, policy);
            return authResult.Succeeded;
        }
    }
}
