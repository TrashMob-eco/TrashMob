namespace TrashMob.Controllers.V2
{
    using System;
    using System.Threading;
    using System.Threading.Tasks;
    using Asp.Versioning;
    using Microsoft.AspNetCore.Cors;
    using Microsoft.AspNetCore.Http;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.Extensions.Logging;
    using TrashMob.Models.Extensions.V2;
    using TrashMob.Models.Poco.V2;
    using TrashMob.Shared.Extensions;
    using TrashMob.Shared.Managers.Interfaces;

    /// <summary>
    /// V2 controller for events with server-side pagination and filtering.
    /// </summary>
    [ApiController]
    [ApiVersion("2.0")]
    [EnableCors("_myAllowSpecificOrigins")]
    [Route("api/v{version:apiVersion}/events")]
    public class EventsV2Controller(
        IEventManager eventManager,
        ILogger<EventsV2Controller> logger) : ControllerBase
    {
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

            Guid? userId = User.Identity?.IsAuthenticated == true
                ? new Guid(HttpContext.Items["UserId"]?.ToString() ?? string.Empty)
                : null;

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
    }
}
