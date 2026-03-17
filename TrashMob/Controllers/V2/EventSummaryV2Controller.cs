namespace TrashMob.Controllers.V2
{
    using System;
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
    using TrashMob.Models.Extensions.V2;
    using TrashMob.Models.Poco.V2;
    using TrashMob.Security;
    using TrashMob.Shared;
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
        IKeyedManager<Event> eventManager,
        IAuthorizationService authorizationService,
        ILogger<EventSummaryV2Controller> logger) : ControllerBase
    {
        private Guid UserId => Guid.TryParse(HttpContext.Items["UserId"]?.ToString(), out var parsedUserId) ? parsedUserId : Guid.Empty;

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

        /// <summary>
        /// Adds a new event summary. Only event leads can add.
        /// </summary>
        /// <param name="eventId">The event identifier.</param>
        /// <param name="dto">The event summary DTO to add.</param>
        /// <param name="cancellationToken">Cancellation token.</param>
        /// <response code="201">Event summary created.</response>
        /// <response code="403">User is not an event lead.</response>
        [HttpPost]
        [Authorize(Policy = AuthorizationPolicyConstants.ValidUser)]
        [RequiredScope(Constants.TrashMobWriteScope)]
        [ProducesResponseType(typeof(EventSummaryDto), StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        public async Task<IActionResult> AddEventSummary(Guid eventId, EventSummaryDto dto,
            CancellationToken cancellationToken)
        {
            logger.LogInformation("V2 AddEventSummary Event={EventId}", eventId);

            var eventSummary = dto.ToEntity();
            eventSummary.EventId = eventId;

            if (!await IsAuthorizedAsync(eventSummary, AuthorizationPolicyConstants.UserIsEventLead))
            {
                return Forbid();
            }

            var result = await eventSummaryManager.AddAsync(eventSummary, UserId, cancellationToken);

            return CreatedAtAction(nameof(GetEventSummary), new { eventId }, result.ToV2Dto());
        }

        /// <summary>
        /// Updates an existing event summary. Only event leads can update.
        /// </summary>
        /// <param name="eventId">The event identifier.</param>
        /// <param name="dto">The event summary DTO to update.</param>
        /// <param name="cancellationToken">Cancellation token.</param>
        /// <response code="200">Returns the updated event summary.</response>
        /// <response code="403">User is not an event lead.</response>
        [HttpPut]
        [Authorize(Policy = AuthorizationPolicyConstants.ValidUser)]
        [RequiredScope(Constants.TrashMobWriteScope)]
        [ProducesResponseType(typeof(EventSummaryDto), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        public async Task<IActionResult> UpdateEventSummary(Guid eventId, EventSummaryDto dto,
            CancellationToken cancellationToken)
        {
            logger.LogInformation("V2 UpdateEventSummary Event={EventId}", eventId);

            var eventSummary = dto.ToEntity();
            eventSummary.EventId = eventId;

            if (!await IsAuthorizedAsync(eventSummary, AuthorizationPolicyConstants.UserIsEventLead))
            {
                return Forbid();
            }

            var updatedEvent = await eventSummaryManager.UpdateAsync(eventSummary, UserId, cancellationToken);
            return Ok(updatedEvent.ToV2Dto());
        }

        /// <summary>
        /// Deletes an event summary. Only event leads can delete.
        /// </summary>
        /// <param name="eventId">The event identifier.</param>
        /// <param name="cancellationToken">Cancellation token.</param>
        /// <response code="204">Event summary deleted.</response>
        /// <response code="403">User is not an event lead.</response>
        [HttpDelete]
        [Authorize(Policy = AuthorizationPolicyConstants.ValidUser)]
        [RequiredScope(Constants.TrashMobWriteScope)]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        public async Task<IActionResult> DeleteEventSummary(Guid eventId, CancellationToken cancellationToken)
        {
            logger.LogInformation("V2 DeleteEventSummary Event={EventId}", eventId);

            var mobEvent = await eventManager.GetAsync(eventId, cancellationToken);

            if (!await IsAuthorizedAsync(mobEvent, AuthorizationPolicyConstants.UserIsEventLead))
            {
                return Forbid();
            }

            await eventSummaryManager.DeleteAsync(eventId, cancellationToken);
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
