namespace TrashMob.Controllers
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading;
    using System.Threading.Tasks;
    using Microsoft.AspNetCore.Authorization;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.Identity.Web.Resource;
    using TrashMob.Models;
    using TrashMob.Models.Extensions;
    using TrashMob.Models.Poco;
    using TrashMob.Security;
    using TrashMob.Shared;
    using TrashMob.Shared.Managers.Interfaces;

    [Route("api/eventattendeeroutes")]
    public class EventAttendeeRoutesController(IEventAttendeeRouteManager eventAttendeeRouteManager) : SecureController
    {
        private readonly IEventAttendeeRouteManager eventAttendeeRouteManager = eventAttendeeRouteManager;

        /// <summary>
        /// Gets a list of event attendee routes for a specific event and user.
        /// </summary>
        /// <param name="eventId">The event ID.</param>
        /// <param name="userId">The user ID.</param>
        /// <param name="cancellationToken">Cancellation token.</param>
        [HttpGet("{eventId}/{userId}")]
        [ProducesResponseType(typeof(IEnumerable<EventAttendeeRoute>), 200)]
        public async Task<IActionResult> GetEventAttendeeRoutes(Guid eventId, Guid userId, CancellationToken cancellationToken)
        {
            var result = (await eventAttendeeRouteManager.GetByParentIdAsync(eventId, cancellationToken).ConfigureAwait(false)).Where(e => e.CreatedByUserId == userId);

            TelemetryClient.TrackEvent(nameof(GetEventAttendeeRoutes));
            return Ok(result);
        }

        /// <summary>
        /// Gets a list of display event attendee routes by event ID.
        /// </summary>
        /// <param name="eventId">The event ID.</param>
        /// <param name="cancellationToken">Cancellation token.</param>
        [HttpGet("byeventid/{eventId}")]
        [ProducesResponseType(typeof(IEnumerable<DisplayEventAttendeeRoute>), 200)]
        public async Task<IActionResult> GetEventAttendeeRoutesByEventId(Guid eventId, CancellationToken cancellationToken)
        {
            var result = await eventAttendeeRouteManager.GetByParentIdAsync(eventId, cancellationToken).ConfigureAwait(false);

            var displayEventAttendeeRoutes = result.Select(x => x.ToDisplayEventAttendeeRoute()).ToList();

            TelemetryClient.TrackEvent(nameof(GetEventAttendeeRoutes));
            return Ok(displayEventAttendeeRoutes);
        }

        /// <summary>
        /// Gets a list of event attendee routes by user ID.
        /// </summary>
        /// <param name="userId">The user ID.</param>
        /// <param name="cancellationToken">Cancellation token.</param>
        [HttpGet("byuserid/{userId}")]
        [ProducesResponseType(typeof(IEnumerable<EventAttendeeRoute>), 200)]
        public async Task<IActionResult> GetEventAttendeeRoutesByUserId(Guid userId, CancellationToken cancellationToken)
        {
            var result = await eventAttendeeRouteManager.GetByCreatedUserIdAsync(userId, cancellationToken).ConfigureAwait(false);

            TelemetryClient.TrackEvent(nameof(GetEventAttendeeRoutes));
            return Ok(result);
        }

        /// <summary>
        /// Gets a specific event attendee route by ID.
        /// </summary>
        /// <param name="id">The route ID.</param>
        /// <param name="cancellationToken">Cancellation token.</param>
        /// <remarks>Returns an enumarable of event attendee routes for the route ID.</remarks>
        [HttpGet("{id}")]
        [ProducesResponseType(typeof(IEnumerable<EventAttendeeRoute>), 200)]
        [ProducesResponseType(typeof(void), 404)]
        public async Task<IActionResult> GetEventAttendeeRoute(Guid id, CancellationToken cancellationToken)
        {
            var result = await eventAttendeeRouteManager.GetAsync(x => x.Id == id, cancellationToken).ConfigureAwait(false);

            if (result == null || result.Count() == 0)
            {
                return NotFound();
            }

            TelemetryClient.TrackEvent(nameof(GetEventAttendeeRoutes));
            return Ok(result);
        }

        /// <summary>
        /// Updates an existing event attendee route.
        /// </summary>
        /// <param name="displayEventAttendeeRoute">The event attendee route to update.</param>
        /// <param name="cancellationToken">Cancellation token.</param>
        /// <remarks>The updated event attendee route.</remarks>
        [HttpPut]
        [RequiredScope(Constants.TrashMobWriteScope)]
        [ProducesResponseType(typeof(EventAttendeeRoute), 200)]
        [ProducesResponseType(typeof(void), 403)]
        public async Task<IActionResult> UpdateEventAttendeeRoute(DisplayEventAttendeeRoute displayEventAttendeeRoute,
            CancellationToken cancellationToken)
        {
            var authResult = await AuthorizationService.AuthorizeAsync(User, displayEventAttendeeRoute,
                AuthorizationPolicyConstants.UserOwnsEntity);

            if (!User.Identity.IsAuthenticated || !authResult.Succeeded)
            {
                return Forbid();
            }

            var eventAttendeeRoute = displayEventAttendeeRoute.ToEventAttendeeRoute();

            var updatedEventAttendeeRoute = await eventAttendeeRouteManager
                .UpdateAsync(eventAttendeeRoute, UserId, cancellationToken).ConfigureAwait(false);
            TelemetryClient.TrackEvent(nameof(UpdateEventAttendeeRoute));

            return Ok(updatedEventAttendeeRoute);
        }

        /// <summary>
        /// Adds a new event attendee route.
        /// </summary>
        /// <param name="displayEventAttendeeRoute">The event attendee route to add.</param>
        /// <param name="cancellationToken">Cancellation token.</param>
        [HttpPost]
        [Authorize(Policy = AuthorizationPolicyConstants.ValidUser)]
        [RequiredScope(Constants.TrashMobWriteScope)]
        [ProducesResponseType(typeof(void), 200)]
        public async Task<IActionResult> AddEventAttendeeRoute(DisplayEventAttendeeRoute displayEventAttendeeRoute,
            CancellationToken cancellationToken)
        {
            var eventAttendeeRoute = displayEventAttendeeRoute.ToEventAttendeeRoute();

            await eventAttendeeRouteManager.AddAsync(eventAttendeeRoute, UserId, cancellationToken)
                .ConfigureAwait(false);
            TelemetryClient.TrackEvent(nameof(AddEventAttendeeRoute));
            return Ok();
        }

        /// <summary>
        /// Deletes an event attendee route.
        /// </summary>
        /// <param name="routeId">The route ID.</param>
        /// <param name="cancellationToken">Cancellation token.</param>
        [HttpDelete("{routeId}")]
        // Todo: Tighten this down
        [Authorize(Policy = AuthorizationPolicyConstants.ValidUser)]
        [RequiredScope(Constants.TrashMobWriteScope)]
        [ProducesResponseType(typeof(void), 204)]
        public async Task<IActionResult> DeleteEventAttendeeRoute(Guid routeId,
            CancellationToken cancellationToken)
        {
            await eventAttendeeRouteManager.DeleteAsync(routeId, cancellationToken).ConfigureAwait(false);
            TelemetryClient.TrackEvent(nameof(DeleteEventAttendeeRoute));

            return new NoContentResult();
        }
    }
}