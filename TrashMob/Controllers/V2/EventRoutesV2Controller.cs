namespace TrashMob.Controllers.V2
{
    using System;
    using System.Collections.Generic;
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
    using TrashMob.Shared.Managers.Interfaces;

    /// <summary>
    /// V2 controller for event-level route aggregation.
    /// </summary>
    [ApiController]
    [ApiVersion("2.0")]
    [EnableCors("_myAllowSpecificOrigins")]
    [Route("api/v{version:apiVersion}/events/{eventId}/routes")]
    public class EventRoutesV2Controller(
        IEventAttendeeRouteManager eventAttendeeRouteManager,
        ILogger<EventRoutesV2Controller> logger) : ControllerBase
    {
        /// <summary>
        /// Gets anonymized routes for an event (no user identity).
        /// </summary>
        /// <param name="eventId">The event ID.</param>
        /// <param name="cancellationToken">Cancellation token.</param>
        /// <response code="200">Returns the anonymized routes.</response>
        [HttpGet]
        [ProducesResponseType(typeof(IReadOnlyList<DisplayAnonymizedRoute>), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetRoutes(Guid eventId, CancellationToken cancellationToken)
        {
            logger.LogInformation("V2 GetEventRoutes for Event={EventId}", eventId);

            var routes = await eventAttendeeRouteManager.GetAnonymizedRoutesForEventAsync(eventId, cancellationToken);

            return Ok(routes);
        }

        /// <summary>
        /// Gets aggregated route statistics for an event.
        /// </summary>
        /// <param name="eventId">The event ID.</param>
        /// <param name="cancellationToken">Cancellation token.</param>
        /// <response code="200">Returns the route statistics.</response>
        [HttpGet("stats")]
        [ProducesResponseType(typeof(DisplayEventRouteStats), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetStats(Guid eventId, CancellationToken cancellationToken)
        {
            logger.LogInformation("V2 GetEventRouteStats for Event={EventId}", eventId);

            var stats = await eventAttendeeRouteManager.GetEventRouteStatsAsync(eventId, cancellationToken);

            return Ok(stats);
        }

        /// <summary>
        /// Gets route aggregate data for pre-filling an event summary.
        /// </summary>
        /// <param name="eventId">The event ID.</param>
        /// <param name="weightUnitId">Target weight unit (1=lb, 2=kg). Defaults to 1.</param>
        /// <param name="cancellationToken">Cancellation token.</param>
        /// <response code="200">Returns the summary prefill data.</response>
        [HttpGet("summary-prefill")]
        [Authorize(Policy = AuthorizationPolicyConstants.ValidUser)]
        [RequiredScope(Constants.TrashMobReadScope)]
        [ProducesResponseType(typeof(EventSummaryPrefill), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetSummaryPrefill(
            Guid eventId,
            [FromQuery] int weightUnitId = 1,
            CancellationToken cancellationToken = default)
        {
            logger.LogInformation("V2 GetEventSummaryPrefill for Event={EventId}", eventId);

            var prefill = await eventAttendeeRouteManager.GetEventSummaryPrefillAsync(eventId, weightUnitId, cancellationToken);

            return Ok(prefill);
        }
    }
}
