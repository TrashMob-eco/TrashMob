namespace TrashMob.Controllers
{
    using System;
    using System.Collections.Generic;
    using System.Threading;
    using System.Threading.Tasks;
    using Microsoft.AspNetCore.Http;
    using Microsoft.AspNetCore.Mvc;
    using TrashMob.Models.Poco;
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
                .GetAnonymizedRoutesForEventAsync(eventId, cancellationToken)
                .ConfigureAwait(false);

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
                .GetEventRouteStatsAsync(eventId, cancellationToken)
                .ConfigureAwait(false);

            TrackEvent(nameof(GetEventRouteStats));
            return Ok(stats);
        }
    }
}
