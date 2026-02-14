namespace TrashMob.Controllers
{
    using System;
    using System.Threading;
    using System.Threading.Tasks;
    using Microsoft.AspNetCore.Authorization;
    using Microsoft.AspNetCore.Http;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.Identity.Web.Resource;
    using TrashMob.Models;
    using TrashMob.Security;
    using TrashMob.Shared;
    using TrashMob.Shared.Managers.Interfaces;
    using TrashMob.Shared.Poco;

    /// <summary>
    /// Controller for attendee-submitted event metrics.
    /// </summary>
    [Route("api/events/{eventId}/attendee-metrics")]
    public class EventAttendeeMetricsController(
        IEventAttendeeMetricsManager metricsManager,
        IKeyedManager<Event> eventManager)
        : SecureController
    {

        /// <summary>
        /// Gets the current user's metrics submission for an event.
        /// </summary>
        /// <param name="eventId">The event ID.</param>
        /// <param name="cancellationToken">Cancellation token.</param>
        [HttpGet("my-metrics")]
        [Authorize(Policy = AuthorizationPolicyConstants.ValidUser)]
        [RequiredScope(Constants.TrashMobReadScope)]
        [ProducesResponseType(typeof(EventAttendeeMetrics), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetMyMetrics(Guid eventId, CancellationToken cancellationToken)
        {
            var metrics = await metricsManager.GetMyMetricsAsync(eventId, UserId, cancellationToken);

            if (metrics == null)
            {
                return NotFound();
            }

            return Ok(metrics);
        }

        /// <summary>
        /// Submits or updates metrics for the current user.
        /// </summary>
        /// <param name="eventId">The event ID.</param>
        /// <param name="metrics">The metrics to submit.</param>
        /// <param name="cancellationToken">Cancellation token.</param>
        [HttpPost("my-metrics")]
        [Authorize(Policy = AuthorizationPolicyConstants.ValidUser)]
        [RequiredScope(Constants.TrashMobWriteScope)]
        [ProducesResponseType(typeof(EventAttendeeMetrics), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(EventAttendeeMetrics), StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> SubmitMyMetrics(
            Guid eventId,
            [FromBody] EventAttendeeMetrics metrics,
            CancellationToken cancellationToken)
        {
            var result = await metricsManager.SubmitMetricsAsync(eventId, UserId, metrics, cancellationToken);

            if (!result.IsSuccess)
            {
                return BadRequest(result.ErrorMessage);
            }

            return Ok(result.Data);
        }

        /// <summary>
        /// Gets all metrics submissions for an event. Requires event lead authorization.
        /// </summary>
        /// <param name="eventId">The event ID.</param>
        /// <param name="cancellationToken">Cancellation token.</param>
        [HttpGet]
        [Authorize(Policy = AuthorizationPolicyConstants.ValidUser)]
        [RequiredScope(Constants.TrashMobReadScope)]
        [ProducesResponseType(typeof(EventAttendeeMetrics[]), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetAllMetrics(Guid eventId, CancellationToken cancellationToken)
        {
            var mobEvent = await eventManager.GetAsync(eventId, cancellationToken);
            if (mobEvent == null)
            {
                return NotFound();
            }

            if (!await IsAuthorizedAsync(mobEvent, AuthorizationPolicyConstants.UserIsEventLead))
            {
                return Forbid();
            }

            var allMetrics = await metricsManager.GetByEventIdAsync(eventId, cancellationToken);
            return Ok(allMetrics);
        }

        /// <summary>
        /// Gets pending metrics submissions for an event. Requires event lead authorization.
        /// </summary>
        /// <param name="eventId">The event ID.</param>
        /// <param name="cancellationToken">Cancellation token.</param>
        [HttpGet("pending")]
        [Authorize(Policy = AuthorizationPolicyConstants.ValidUser)]
        [RequiredScope(Constants.TrashMobReadScope)]
        [ProducesResponseType(typeof(EventAttendeeMetrics[]), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetPendingMetrics(Guid eventId, CancellationToken cancellationToken)
        {
            var mobEvent = await eventManager.GetAsync(eventId, cancellationToken);
            if (mobEvent == null)
            {
                return NotFound();
            }

            if (!await IsAuthorizedAsync(mobEvent, AuthorizationPolicyConstants.UserIsEventLead))
            {
                return Forbid();
            }

            var pendingMetrics = await metricsManager.GetPendingByEventIdAsync(eventId, cancellationToken);
            return Ok(pendingMetrics);
        }

        /// <summary>
        /// Approves a metrics submission. Requires event lead authorization.
        /// </summary>
        /// <param name="eventId">The event ID.</param>
        /// <param name="metricsId">The metrics ID to approve.</param>
        /// <param name="cancellationToken">Cancellation token.</param>
        [HttpPost("{metricsId}/approve")]
        [Authorize(Policy = AuthorizationPolicyConstants.ValidUser)]
        [RequiredScope(Constants.TrashMobWriteScope)]
        [ProducesResponseType(typeof(EventAttendeeMetrics), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> ApproveMetrics(
            Guid eventId,
            Guid metricsId,
            CancellationToken cancellationToken)
        {
            var mobEvent = await eventManager.GetAsync(eventId, cancellationToken);
            if (mobEvent == null)
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

            return Ok(result.Data);
        }

        /// <summary>
        /// Rejects a metrics submission. Requires event lead authorization.
        /// </summary>
        /// <param name="eventId">The event ID.</param>
        /// <param name="metricsId">The metrics ID to reject.</param>
        /// <param name="request">The rejection request with reason.</param>
        /// <param name="cancellationToken">Cancellation token.</param>
        [HttpPost("{metricsId}/reject")]
        [Authorize(Policy = AuthorizationPolicyConstants.ValidUser)]
        [RequiredScope(Constants.TrashMobWriteScope)]
        [ProducesResponseType(typeof(EventAttendeeMetrics), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> RejectMetrics(
            Guid eventId,
            Guid metricsId,
            [FromBody] RejectMetricsRequest request,
            CancellationToken cancellationToken)
        {
            var mobEvent = await eventManager.GetAsync(eventId, cancellationToken);
            if (mobEvent == null)
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

            return Ok(result.Data);
        }

        /// <summary>
        /// Adjusts and approves a metrics submission. Requires event lead authorization.
        /// </summary>
        /// <param name="eventId">The event ID.</param>
        /// <param name="metricsId">The metrics ID to adjust.</param>
        /// <param name="request">The adjustment request with new values and reason.</param>
        /// <param name="cancellationToken">Cancellation token.</param>
        [HttpPut("{metricsId}/adjust")]
        [Authorize(Policy = AuthorizationPolicyConstants.ValidUser)]
        [RequiredScope(Constants.TrashMobWriteScope)]
        [ProducesResponseType(typeof(EventAttendeeMetrics), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> AdjustMetrics(
            Guid eventId,
            Guid metricsId,
            [FromBody] AdjustMetricsRequest request,
            CancellationToken cancellationToken)
        {
            var mobEvent = await eventManager.GetAsync(eventId, cancellationToken);
            if (mobEvent == null)
            {
                return NotFound();
            }

            if (!await IsAuthorizedAsync(mobEvent, AuthorizationPolicyConstants.UserIsEventLead))
            {
                return Forbid();
            }

            var adjustedValues = new EventAttendeeMetrics
            {
                AdjustedBagsCollected = request.AdjustedBagsCollected,
                AdjustedPickedWeight = request.AdjustedPickedWeight,
                AdjustedPickedWeightUnitId = request.AdjustedPickedWeightUnitId,
                AdjustedDurationMinutes = request.AdjustedDurationMinutes
            };

            var result = await metricsManager.AdjustAsync(
                metricsId,
                adjustedValues,
                request.AdjustmentReason,
                UserId,
                cancellationToken);

            if (!result.IsSuccess)
            {
                return BadRequest(result.ErrorMessage);
            }

            return Ok(result.Data);
        }

        /// <summary>
        /// Approves all pending metrics submissions for an event. Requires event lead authorization.
        /// </summary>
        /// <param name="eventId">The event ID.</param>
        /// <param name="cancellationToken">Cancellation token.</param>
        [HttpPost("approve-all")]
        [Authorize(Policy = AuthorizationPolicyConstants.ValidUser)]
        [RequiredScope(Constants.TrashMobWriteScope)]
        [ProducesResponseType(typeof(int), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> ApproveAllPending(Guid eventId, CancellationToken cancellationToken)
        {
            var mobEvent = await eventManager.GetAsync(eventId, cancellationToken);
            if (mobEvent == null)
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
        /// Calculates the totals from approved metrics submissions for an event. Requires event lead authorization.
        /// </summary>
        /// <param name="eventId">The event ID.</param>
        /// <param name="cancellationToken">Cancellation token.</param>
        [HttpGet("totals")]
        [Authorize(Policy = AuthorizationPolicyConstants.ValidUser)]
        [RequiredScope(Constants.TrashMobReadScope)]
        [ProducesResponseType(typeof(AttendeeMetricsTotals), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetTotals(Guid eventId, CancellationToken cancellationToken)
        {
            var mobEvent = await eventManager.GetAsync(eventId, cancellationToken);
            if (mobEvent == null)
            {
                return NotFound();
            }

            if (!await IsAuthorizedAsync(mobEvent, AuthorizationPolicyConstants.UserIsEventLead))
            {
                return Forbid();
            }

            var totals = await metricsManager.CalculateTotalsAsync(eventId, cancellationToken);
            return Ok(totals);
        }

        /// <summary>
        /// Gets the public metrics summary for an event, including approved contributor breakdown.
        /// This endpoint is publicly accessible and returns only approved metrics from users who opted into public visibility.
        /// </summary>
        /// <param name="eventId">The event ID.</param>
        /// <param name="cancellationToken">Cancellation token.</param>
        [HttpGet("public")]
        [AllowAnonymous]
        [ProducesResponseType(typeof(EventMetricsPublicSummary), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetPublicMetrics(Guid eventId, CancellationToken cancellationToken)
        {
            var mobEvent = await eventManager.GetAsync(eventId, cancellationToken);
            if (mobEvent == null)
            {
                return NotFound();
            }

            var summary = await metricsManager.GetPublicMetricsSummaryAsync(eventId, cancellationToken);
            return Ok(summary);
        }
    }

    /// <summary>
    /// Request model for rejecting a metrics submission.
    /// </summary>
    public class RejectMetricsRequest
    {
        /// <summary>
        /// Gets or sets the reason for rejection.
        /// </summary>
        public string RejectionReason { get; set; }
    }

    /// <summary>
    /// Request model for adjusting a metrics submission.
    /// </summary>
    public class AdjustMetricsRequest
    {
        /// <summary>
        /// Gets or sets the adjusted bags collected value.
        /// </summary>
        public int? AdjustedBagsCollected { get; set; }

        /// <summary>
        /// Gets or sets the adjusted weight value.
        /// </summary>
        public decimal? AdjustedPickedWeight { get; set; }

        /// <summary>
        /// Gets or sets the adjusted weight unit ID.
        /// </summary>
        public int? AdjustedPickedWeightUnitId { get; set; }

        /// <summary>
        /// Gets or sets the adjusted duration in minutes.
        /// </summary>
        public int? AdjustedDurationMinutes { get; set; }

        /// <summary>
        /// Gets or sets the reason for adjustment.
        /// </summary>
        public string AdjustmentReason { get; set; }
    }
}
