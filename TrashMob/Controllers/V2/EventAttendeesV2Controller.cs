namespace TrashMob.Controllers.V2
{
    using System;
    using System.Threading;
    using System.Threading.Tasks;
    using Asp.Versioning;
    using Microsoft.AspNetCore.Authorization;
    using Microsoft.AspNetCore.Cors;
    using Microsoft.AspNetCore.Http;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.Extensions.Logging;
    using TrashMob.Models.Extensions.V2;
    using TrashMob.Models.Poco.V2;
    using TrashMob.Security;
    using TrashMob.Shared.Extensions;
    using TrashMob.Shared.Managers.Interfaces;

    /// <summary>
    /// V2 controller for event attendees as a nested resource under events.
    /// </summary>
    [ApiController]
    [ApiVersion("2.0")]
    [EnableCors("_myAllowSpecificOrigins")]
    [Route("api/v{version:apiVersion}/events/{eventId}/attendees")]
    public class EventAttendeesV2Controller(
        IEventAttendeeManager eventAttendeeManager,
        ILogger<EventAttendeesV2Controller> logger) : ControllerBase
    {
        /// <summary>
        /// Gets the count of active attendees for an event.
        /// </summary>
        /// <param name="eventId">The event identifier.</param>
        /// <param name="cancellationToken">Cancellation token.</param>
        /// <returns>The attendee count.</returns>
        /// <response code="200">Returns the attendee count.</response>
        [HttpGet("count")]
        [ProducesResponseType(typeof(EventAttendeeCountDto), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> GetAttendeeCount(Guid eventId, CancellationToken cancellationToken)
        {
            logger.LogInformation("V2 GetAttendeeCount requested for Event={EventId}", eventId);

            var count = await eventAttendeeManager.GetActiveAttendeeCountAsync(eventId, cancellationToken);

            return Ok(new EventAttendeeCountDto
            {
                EventId = eventId,
                Count = count,
            });
        }

        /// <summary>
        /// Gets a paginated list of active attendees for an event. Requires authentication.
        /// </summary>
        /// <param name="eventId">The event identifier.</param>
        /// <param name="filter">Query parameters for pagination.</param>
        /// <param name="cancellationToken">Cancellation token.</param>
        /// <returns>A paginated list of event attendees with user info.</returns>
        /// <response code="200">Returns the paginated attendee list.</response>
        /// <response code="401">Authentication required.</response>
        [HttpGet]
        [Authorize(Policy = AuthorizationPolicyConstants.ValidUser)]
        [ProducesResponseType(typeof(PagedResponse<EventAttendeeDto>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> GetAttendees(
            Guid eventId,
            [FromQuery] QueryParameters filter,
            CancellationToken cancellationToken)
        {
            logger.LogInformation("V2 GetAttendees requested for Event={EventId}, Page={Page}, PageSize={PageSize}",
                eventId, filter.Page, filter.PageSize);

            var query = eventAttendeeManager.GetEventAttendeesQueryable(eventId);
            var result = await query.ToPagedAsync(filter, ea => ea.ToV2Dto(), cancellationToken);

            return Ok(result);
        }
    }
}
