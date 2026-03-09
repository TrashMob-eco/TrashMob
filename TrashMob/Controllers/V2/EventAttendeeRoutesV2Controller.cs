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
    using TrashMob.Models.Extensions;
    using TrashMob.Models.Poco;
    using TrashMob.Security;
    using TrashMob.Shared;
    using TrashMob.Shared.Managers.Interfaces;

    /// <summary>
    /// V2 controller for managing event attendee routes.
    /// </summary>
    [ApiController]
    [ApiVersion("2.0")]
    [EnableCors("_myAllowSpecificOrigins")]
    [Route("api/v{version:apiVersion}/eventattendeeroutes")]
    public class EventAttendeeRoutesV2Controller(
        IEventAttendeeRouteManager eventAttendeeRouteManager,
        ILogger<EventAttendeeRoutesV2Controller> logger) : ControllerBase
    {
        private Guid UserId => new(HttpContext.Items["UserId"]?.ToString() ?? string.Empty);

        /// <summary>
        /// Gets event attendee routes for a specific event and user.
        /// </summary>
        /// <param name="eventId">The event ID.</param>
        /// <param name="userId">The user ID.</param>
        /// <param name="cancellationToken">Cancellation token.</param>
        /// <response code="200">Returns the routes for the event and user.</response>
        [HttpGet("{eventId}/{userId}")]
        [ProducesResponseType(typeof(IReadOnlyList<DisplayEventAttendeeRoute>), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetByEventAndUser(Guid eventId, Guid userId, CancellationToken cancellationToken)
        {
            logger.LogInformation("V2 GetEventAttendeeRoutes Event={EventId}, User={UserId}", eventId, userId);

            var result = (await eventAttendeeRouteManager.GetByParentIdAsync(eventId, cancellationToken))
                .Where(e => e.CreatedByUserId == userId)
                .Select(x => x.ToDisplayEventAttendeeRoute())
                .ToList();

            return Ok(result);
        }

        /// <summary>
        /// Gets display event attendee routes by event ID.
        /// </summary>
        /// <param name="eventId">The event ID.</param>
        /// <param name="cancellationToken">Cancellation token.</param>
        /// <response code="200">Returns the routes for the event.</response>
        [HttpGet("by-event/{eventId}")]
        [ProducesResponseType(typeof(IReadOnlyList<DisplayEventAttendeeRoute>), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetByEvent(Guid eventId, CancellationToken cancellationToken)
        {
            logger.LogInformation("V2 GetEventAttendeeRoutesByEvent Event={EventId}", eventId);

            var result = await eventAttendeeRouteManager.GetByParentIdAsync(eventId, cancellationToken);

            var currentUserId = User.Identity?.IsAuthenticated == true ? UserId : Guid.Empty;

            var displayRoutes = result
                .Where(r => r.PrivacyLevel != "Private" || r.UserId == currentUserId)
                .Select(x => x.ToDisplayEventAttendeeRoute())
                .ToList();

            return Ok(displayRoutes);
        }

        /// <summary>
        /// Gets a specific event attendee route by ID.
        /// </summary>
        /// <param name="id">The route ID.</param>
        /// <param name="cancellationToken">Cancellation token.</param>
        /// <response code="200">Returns the route.</response>
        /// <response code="404">Route not found.</response>
        [HttpGet("{id}")]
        [ProducesResponseType(typeof(IReadOnlyList<DisplayEventAttendeeRoute>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
        public async Task<IActionResult> Get(Guid id, CancellationToken cancellationToken)
        {
            logger.LogInformation("V2 GetEventAttendeeRoute Id={Id}", id);

            var result = await eventAttendeeRouteManager.GetAsync(x => x.Id == id, cancellationToken);

            if (result is null || !result.Any())
            {
                return NotFound();
            }

            var displayRoutes = result.Select(x => x.ToDisplayEventAttendeeRoute()).ToList();
            return Ok(displayRoutes);
        }

        /// <summary>
        /// Adds a new event attendee route.
        /// </summary>
        /// <param name="displayEventAttendeeRoute">The route to add.</param>
        /// <param name="cancellationToken">Cancellation token.</param>
        /// <response code="201">Route created.</response>
        [HttpPost]
        [Authorize(Policy = AuthorizationPolicyConstants.ValidUser)]
        [RequiredScope(Constants.TrashMobWriteScope)]
        [ProducesResponseType(StatusCodes.Status201Created)]
        public async Task<IActionResult> Add(
            DisplayEventAttendeeRoute displayEventAttendeeRoute,
            CancellationToken cancellationToken)
        {
            logger.LogInformation("V2 AddEventAttendeeRoute for Event={EventId}", displayEventAttendeeRoute.EventId);

            var eventAttendeeRoute = displayEventAttendeeRoute.ToEventAttendeeRoute();

            if (displayEventAttendeeRoute.SkipDefaultTrim)
            {
                eventAttendeeRoute.TrimStartMeters = -1;
                eventAttendeeRoute.TrimEndMeters = -1;
            }

            var result = await eventAttendeeRouteManager.AddAsync(eventAttendeeRoute, UserId, cancellationToken);

            return CreatedAtAction(nameof(Get), new { id = result.Id }, result);
        }

        /// <summary>
        /// Deletes an event attendee route.
        /// </summary>
        /// <param name="routeId">The route ID.</param>
        /// <param name="cancellationToken">Cancellation token.</param>
        /// <response code="204">Route deleted.</response>
        [HttpDelete("{routeId}")]
        [Authorize(Policy = AuthorizationPolicyConstants.ValidUser)]
        [RequiredScope(Constants.TrashMobWriteScope)]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        public async Task<IActionResult> Delete(Guid routeId, CancellationToken cancellationToken)
        {
            logger.LogInformation("V2 DeleteEventAttendeeRoute Route={RouteId}", routeId);

            await eventAttendeeRouteManager.DeleteAsync(routeId, cancellationToken);

            return NoContent();
        }
    }
}
