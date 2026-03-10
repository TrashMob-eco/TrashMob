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
    using Microsoft.EntityFrameworkCore;
    using Microsoft.Extensions.Logging;
    using Microsoft.Identity.Web.Resource;
    using TrashMob.Models;
    using TrashMob.Models.Extensions.V2;
    using TrashMob.Models.Poco;
    using TrashMob.Models.Poco.V2;
    using TrashMob.Security;
    using TrashMob.Shared;
    using TrashMob.Shared.Extensions;
    using TrashMob.Shared.Managers.Interfaces;
    using TrashMob.Shared.Poco;

    /// <summary>
    /// V2 controller for events with server-side pagination and filtering.
    /// </summary>
    [ApiController]
    [ApiVersion("2.0")]
    [EnableCors("_myAllowSpecificOrigins")]
    [Route("api/v{version:apiVersion}/events")]
    public class EventsV2Controller(
        IEventManager eventManager,
        IEventAttendeeManager eventAttendeeManager,
        IAuthorizationService authorizationService,
        ILogger<EventsV2Controller> logger) : ControllerBase
    {
        private Guid UserId => new(HttpContext.Items["UserId"]?.ToString() ?? string.Empty);

        /// <summary>
        /// Gets a paginated list of events with optional filtering.
        /// </summary>
        /// <param name="filter">Query parameters for pagination and filtering.</param>
        /// <param name="cancellationToken">Cancellation token.</param>
        /// <returns>A paginated list of events.</returns>
        /// <response code="200">Returns the paginated event list.</response>
        [HttpGet]
        [ProducesResponseType(typeof(PagedResponse<EventDto>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> GetEvents(
            [FromQuery] EventQueryParameters filter,
            CancellationToken cancellationToken)
        {
            logger.LogInformation("V2 GetEvents requested with Page={Page}, PageSize={PageSize}",
                filter.Page, filter.PageSize);

            Guid? userId = User.Identity?.IsAuthenticated == true ? UserId : (Guid?)null;

            var query = await eventManager.GetFilteredEventsQueryableAsync(filter, userId, cancellationToken);
            var result = await query.ToPagedAsync(filter, e => e.ToV2Dto(), cancellationToken);

            return Ok(result);
        }

        /// <summary>
        /// Gets a single event by its identifier.
        /// </summary>
        /// <param name="id">The event identifier.</param>
        /// <param name="cancellationToken">Cancellation token.</param>
        /// <returns>The event details.</returns>
        /// <response code="200">Returns the event.</response>
        /// <response code="404">Event not found.</response>
        [HttpGet("{id}")]
        [ProducesResponseType(typeof(EventDto), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> GetEvent(Guid id, CancellationToken cancellationToken)
        {
            logger.LogInformation("V2 GetEvent requested for {EventId}", id);

            var mobEvent = await eventManager.GetAsync(id, cancellationToken);

            if (mobEvent is null)
            {
                return NotFound();
            }

            return Ok(mobEvent.ToV2Dto());
        }

        /// <summary>
        /// Gets events for a specific user (created + attending).
        /// </summary>
        /// <param name="userId">The user ID.</param>
        /// <param name="futureEventsOnly">When true, only return future events.</param>
        /// <param name="cancellationToken">Cancellation token.</param>
        /// <response code="200">Returns the user's events.</response>
        [HttpGet("userevents/{userId}/{futureEventsOnly}")]
        [Authorize(Policy = AuthorizationPolicyConstants.ValidUser)]
        [RequiredScope(Constants.TrashMobReadScope)]
        [ProducesResponseType(typeof(IEnumerable<Event>), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetUserEvents(Guid userId, bool futureEventsOnly,
            CancellationToken cancellationToken)
        {
            logger.LogInformation("V2 GetUserEvents User={UserId}, FutureOnly={FutureOnly}", userId, futureEventsOnly);

            var result1 = await eventManager.GetUserEventsAsync(userId, futureEventsOnly, cancellationToken);
            var result2 = await eventAttendeeManager
                .GetEventsUserIsAttendingAsync(userId, futureEventsOnly, cancellationToken);

            var allResults = result1.Union(result2, new EventComparer());
            return Ok(allResults);
        }

        /// <summary>
        /// Gets events a specific user is attending.
        /// </summary>
        /// <param name="userId">The user ID.</param>
        /// <param name="cancellationToken">Cancellation token.</param>
        /// <response code="200">Returns events the user is attending.</response>
        [HttpGet("eventsuserisattending/{userId}")]
        [Authorize(Policy = AuthorizationPolicyConstants.ValidUser)]
        [RequiredScope(Constants.TrashMobReadScope)]
        [ProducesResponseType(typeof(IEnumerable<Event>), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetEventsUserIsAttending(Guid userId, CancellationToken cancellationToken)
        {
            logger.LogInformation("V2 GetEventsUserIsAttending User={UserId}", userId);

            var result = await eventAttendeeManager
                .GetEventsUserIsAttendingAsync(userId, cancellationToken: cancellationToken);
            return Ok(result);
        }

        /// <summary>
        /// Gets event locations within a specific time range.
        /// </summary>
        /// <param name="startTime">The start time (ISO 8601).</param>
        /// <param name="endTime">The end time (ISO 8601).</param>
        /// <param name="cancellationToken">Cancellation token.</param>
        /// <response code="200">Returns event locations in the time range.</response>
        [HttpGet("locationsbytimerange")]
        [ProducesResponseType(typeof(IEnumerable<Location>), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetEventLocationsByTimeRange(
            [FromQuery] DateTimeOffset? startTime,
            [FromQuery] DateTimeOffset? endTime,
            CancellationToken cancellationToken)
        {
            logger.LogInformation("V2 GetEventLocationsByTimeRange Start={Start}, End={End}", startTime, endTime);

            var result = await eventManager.GetEventLocationsByTimeRangeAsync(startTime, endTime, cancellationToken);
            return Ok(result);
        }

        /// <summary>
        /// Gets a paginated list of filtered events.
        /// </summary>
        /// <param name="filter">The event filter.</param>
        /// <param name="cancellationToken">Cancellation token.</param>
        /// <response code="200">Returns a paginated list of filtered events.</response>
        [HttpPost("pagedfilteredevents")]
        [ProducesResponseType(typeof(PaginatedList<Event>), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetPagedFilteredEvents(
            [FromBody] EventFilter filter,
            CancellationToken cancellationToken)
        {
            logger.LogInformation("V2 GetPagedFilteredEvents");

            Guid? userId = User.Identity?.IsAuthenticated == true ? UserId : (Guid?)null;
            var result = await eventManager.GetFilteredEventsAsync(filter, userId, cancellationToken);

            if (filter.PageSize is not null)
            {
                var pagedResults = PaginatedList<Event>.Create(
                    result.OrderByDescending(e => e.EventDate).AsQueryable(),
                    filter.PageIndex.GetValueOrDefault(0),
                    filter.PageSize.GetValueOrDefault(10));
                return Ok(pagedResults);
            }

            return Ok(result);
        }

        /// <summary>
        /// Gets a paginated list of events for a user (created + attending).
        /// </summary>
        /// <param name="filter">The event filter.</param>
        /// <param name="userId">The user ID.</param>
        /// <param name="cancellationToken">Cancellation token.</param>
        /// <response code="200">Returns a paginated list of user events.</response>
        [HttpPost("pageduserevents/{userId}")]
        [Authorize(Policy = AuthorizationPolicyConstants.ValidUser)]
        [RequiredScope(Constants.TrashMobReadScope)]
        [ProducesResponseType(typeof(PaginatedList<Event>), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetPagedUserEvents(
            [FromBody] EventFilter filter,
            Guid userId,
            CancellationToken cancellationToken)
        {
            logger.LogInformation("V2 GetPagedUserEvents User={UserId}", userId);

            var result1 = await eventManager.GetUserEventsAsync(filter, userId, cancellationToken);
            var result2 = await eventAttendeeManager
                .GetEventsUserIsAttendingAsync(filter, userId, cancellationToken);

            var allResults = result1.Union(result2, new EventComparer());

            if (filter.PageSize is not null)
            {
                var pagedResults = PaginatedList<Event>.Create(
                    allResults.OrderByDescending(e => e.EventDate).AsQueryable(),
                    filter.PageIndex.GetValueOrDefault(0),
                    filter.PageSize.GetValueOrDefault(10));
                return Ok(pagedResults);
            }

            return Ok(allResults);
        }

        /// <summary>
        /// Adds a new event.
        /// </summary>
        /// <param name="mobEvent">The event to add.</param>
        /// <param name="cancellationToken">Cancellation token.</param>
        /// <response code="201">Event created.</response>
        [HttpPost]
        [Authorize(Policy = AuthorizationPolicyConstants.ValidUser)]
        [RequiredScope(Constants.TrashMobWriteScope)]
        [ProducesResponseType(typeof(Event), StatusCodes.Status201Created)]
        public async Task<IActionResult> AddEvent(Event mobEvent, CancellationToken cancellationToken)
        {
            logger.LogInformation("V2 AddEvent Name={Name}", mobEvent.Name);

            var newEvent = await eventManager.AddAsync(mobEvent, UserId, cancellationToken);
            return CreatedAtAction(nameof(GetEvent), new { id = newEvent.Id }, newEvent);
        }

        /// <summary>
        /// Updates an existing event. Only event leads can update.
        /// </summary>
        /// <param name="mobEvent">The event to update.</param>
        /// <param name="cancellationToken">Cancellation token.</param>
        /// <response code="200">Returns the updated event.</response>
        /// <response code="403">User is not an event lead.</response>
        /// <response code="404">Event not found.</response>
        [HttpPut]
        [Authorize(Policy = AuthorizationPolicyConstants.ValidUser)]
        [RequiredScope(Constants.TrashMobWriteScope)]
        [ProducesResponseType(typeof(Event), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> UpdateEvent(Event mobEvent, CancellationToken cancellationToken)
        {
            logger.LogInformation("V2 UpdateEvent Event={EventId}", mobEvent.Id);

            if (!await IsAuthorizedAsync(mobEvent, AuthorizationPolicyConstants.UserIsEventLead))
            {
                return Forbid();
            }

            try
            {
                var updatedEvent = await eventManager.UpdateAsync(mobEvent, UserId, cancellationToken);
                return Ok(updatedEvent);
            }
            catch (DbUpdateConcurrencyException)
            {
                var existing = await eventManager.GetAsync(mobEvent.Id, cancellationToken);
                if (existing is null)
                {
                    return NotFound();
                }

                throw;
            }
        }

        /// <summary>
        /// Deletes (cancels) an event. Only event leads or admins can delete.
        /// </summary>
        /// <param name="eventCancellationRequest">The cancellation request with event ID and reason.</param>
        /// <param name="cancellationToken">Cancellation token.</param>
        /// <response code="204">Event deleted.</response>
        /// <response code="403">User is not authorized.</response>
        [HttpDelete]
        [Authorize(Policy = AuthorizationPolicyConstants.ValidUser)]
        [RequiredScope(Constants.TrashMobWriteScope)]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        public async Task<IActionResult> DeleteEvent(
            EventCancellationRequest eventCancellationRequest,
            CancellationToken cancellationToken)
        {
            logger.LogInformation("V2 DeleteEvent Event={EventId}", eventCancellationRequest.EventId);

            var mobEvent = await eventManager.GetAsync(eventCancellationRequest.EventId, cancellationToken);

            if (!await IsAuthorizedAsync(mobEvent, AuthorizationPolicyConstants.UserIsEventLeadOrIsAdmin))
            {
                return Forbid();
            }

            await eventManager.DeleteAsync(eventCancellationRequest.EventId,
                eventCancellationRequest.CancellationReason, UserId, cancellationToken);

            return NoContent();
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
