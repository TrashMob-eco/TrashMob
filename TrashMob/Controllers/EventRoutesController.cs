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
    using TrashMob.Models.Poco;
    using TrashMob.Security;
    using TrashMob.Shared;
    using TrashMob.Shared.Managers.Interfaces;

    [Route("api/events")]
    public class EventRoutesController(IEventAttendeeRouteManager eventAttendeeRouteManager) : SecureController
    {
        private readonly IEventAttendeeRouteManager eventAttendeeRouteManager = eventAttendeeRouteManager;

        /// <summary>
        /// Gets anonymized routes for an event (no user identity).
        /// </summary>
        /// <param name="eventId">The event ID.</param>
        /// <param name="cancellationToken">Cancellation token.</param>
        [HttpGet("{eventId}/routes")]
        [ProducesResponseType(typeof(IEnumerable<DisplayAnonymizedRoute>), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetEventRoutes(Guid eventId, CancellationToken cancellationToken)
        {
            var routes = await eventAttendeeRouteManager
                .GetAnonymizedRoutesForEventAsync(eventId, cancellationToken);

            TrackEvent(nameof(GetEventRoutes));
            return Ok(routes);
        }

        /// <summary>
        /// Gets aggregated route statistics for an event.
        /// </summary>
        /// <param name="eventId">The event ID.</param>
        /// <param name="cancellationToken">Cancellation token.</param>
        [HttpGet("{eventId}/routes/stats")]
        [ProducesResponseType(typeof(DisplayEventRouteStats), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetEventRouteStats(Guid eventId, CancellationToken cancellationToken)
        {
            var stats = await eventAttendeeRouteManager
                .GetEventRouteStatsAsync(eventId, cancellationToken);

            TrackEvent(nameof(GetEventRouteStats));
            return Ok(stats);
        }

        /// <summary>
        /// Gets route aggregate data for pre-filling an event summary.
        /// </summary>
        /// <param name="eventId">The event ID.</param>
        /// <param name="weightUnitId">Target weight unit (1=lb, 2=kg). Defaults to 1.</param>
        /// <param name="cancellationToken">Cancellation token.</param>
        [HttpGet("{eventId}/routes/summary-prefill")]
        [Authorize(Policy = AuthorizationPolicyConstants.ValidUser)]
        [RequiredScope(Constants.TrashMobReadScope)]
        [ProducesResponseType(typeof(EventSummaryPrefill), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetEventSummaryPrefill(Guid eventId,
            [FromQuery] int weightUnitId = 1, CancellationToken cancellationToken = default)
        {
            var prefill = await eventAttendeeRouteManager
                .GetEventSummaryPrefillAsync(eventId, weightUnitId, cancellationToken);

            TrackEvent(nameof(GetEventSummaryPrefill));
            return Ok(prefill);
        }
    }
}
