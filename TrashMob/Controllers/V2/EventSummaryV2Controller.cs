namespace TrashMob.Controllers.V2
{
    using System;
    using System.Linq;
    using System.Threading;
    using System.Threading.Tasks;
    using Asp.Versioning;
    using Microsoft.AspNetCore.Cors;
    using Microsoft.AspNetCore.Http;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.Extensions.Logging;
    using TrashMob.Models;
    using TrashMob.Models.Extensions.V2;
    using TrashMob.Models.Poco.V2;
    using TrashMob.Shared.Managers.Interfaces;

    /// <summary>
    /// V2 controller for event summaries as a nested resource under events.
    /// </summary>
    [ApiController]
    [ApiVersion("2.0")]
    [EnableCors("_myAllowSpecificOrigins")]
    [Route("api/v{version:apiVersion}/events/{eventId}/summary")]
    public class EventSummaryV2Controller(
        IEventSummaryManager eventSummaryManager,
        ILogger<EventSummaryV2Controller> logger) : ControllerBase
    {
        /// <summary>
        /// Gets the post-completion summary for an event.
        /// Returns an empty summary with just the EventId if none exists yet.
        /// </summary>
        /// <param name="eventId">The event identifier.</param>
        /// <param name="cancellationToken">Cancellation token.</param>
        /// <returns>The event summary.</returns>
        /// <response code="200">Returns the event summary.</response>
        [HttpGet]
        [ProducesResponseType(typeof(EventSummaryDto), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> GetEventSummary(Guid eventId, CancellationToken cancellationToken)
        {
            logger.LogInformation("V2 GetEventSummary requested for Event={EventId}", eventId);

            var eventSummary =
                (await eventSummaryManager.GetAsync(es => es.EventId == eventId, cancellationToken)).FirstOrDefault();

            if (eventSummary is null)
            {
                return Ok(new EventSummaryDto { EventId = eventId });
            }

            return Ok(eventSummary.ToV2Dto());
        }
    }
}
