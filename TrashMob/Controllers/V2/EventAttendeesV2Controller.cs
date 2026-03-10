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
    using TrashMob.Models.Extensions.V2;
    using TrashMob.Models.Poco;
    using TrashMob.Models.Poco.V2;
    using TrashMob.Security;
    using TrashMob.Shared;
    using TrashMob.Shared.Extensions;
    using TrashMob.Shared.Managers.Interfaces;
    using TrashMob.Shared.Poco;

    /// <summary>
    /// V2 controller for event attendees as a nested resource under events.
    /// </summary>
    [ApiController]
    [ApiVersion("2.0")]
    [EnableCors("_myAllowSpecificOrigins")]
    [Route("api/v{version:apiVersion}/events/{eventId}/attendees")]
    public class EventAttendeesV2Controller(
        IEventAttendeeManager eventAttendeeManager,
        IUserWaiverManager userWaiverManager,
        ILogger<EventAttendeesV2Controller> logger) : ControllerBase
    {
        private Guid UserId => new(HttpContext.Items["UserId"]?.ToString() ?? string.Empty);

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

        /// <summary>
        /// Adds a new event attendee. Requires all waivers to be signed.
        /// </summary>
        /// <param name="eventId">The event identifier.</param>
        /// <param name="dto">The event attendee DTO containing the UserId.</param>
        /// <param name="cancellationToken">Cancellation token.</param>
        /// <response code="200">Attendee registered.</response>
        /// <response code="400">Waivers required.</response>
        [HttpPost]
        [Authorize(Policy = AuthorizationPolicyConstants.ValidUser)]
        [RequiredScope(Constants.TrashMobWriteScope)]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(WaiverRequiredResponse), StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> AddEventAttendee(
            Guid eventId,
            EventAttendeeDto dto,
            CancellationToken cancellationToken)
        {
            logger.LogInformation("V2 AddEventAttendee Event={EventId}, User={UserId}", eventId, dto.UserId);

            var eventAttendee = new EventAttendee { EventId = eventId, UserId = dto.UserId };

            var hasValidWaiver = await userWaiverManager
                .HasValidWaiverForEventAsync(eventAttendee.UserId, eventId, cancellationToken);

            if (!hasValidWaiver)
            {
                var requiredWaivers = await userWaiverManager
                    .GetRequiredWaiversForEventAsync(eventAttendee.UserId, eventId, cancellationToken);

                return BadRequest(new WaiverRequiredResponse
                {
                    Message = "You must sign all required waivers before registering for this event.",
                    RequiredWaiverCount = requiredWaivers.Count(),
                    RequiredWaiverIds = requiredWaivers.Select(w => w.Id).ToList()
                });
            }

            await eventAttendeeManager.AddAsync(eventAttendee, UserId, cancellationToken);
            return Ok();
        }

        /// <summary>
        /// Removes an attendee from an event.
        /// </summary>
        /// <param name="eventId">The event ID.</param>
        /// <param name="userId">The user ID.</param>
        /// <param name="cancellationToken">Cancellation token.</param>
        /// <response code="204">Attendee removed.</response>
        [HttpDelete("{userId}")]
        [Authorize(Policy = AuthorizationPolicyConstants.ValidUser)]
        [RequiredScope(Constants.TrashMobWriteScope)]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        public async Task<IActionResult> DeleteEventAttendee(Guid eventId, Guid userId,
            CancellationToken cancellationToken)
        {
            logger.LogInformation("V2 DeleteEventAttendee Event={EventId}, User={UserId}", eventId, userId);

            await eventAttendeeManager.Delete(eventId, userId, cancellationToken);
            return NoContent();
        }

        /// <summary>
        /// Gets all event leads for an event.
        /// </summary>
        /// <param name="eventId">The event ID.</param>
        /// <param name="cancellationToken">Cancellation token.</param>
        /// <response code="200">Returns the event leads.</response>
        [HttpGet("leads")]
        [ProducesResponseType(typeof(IEnumerable<DisplayUser>), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetEventLeads(Guid eventId, CancellationToken cancellationToken)
        {
            logger.LogInformation("V2 GetEventLeads Event={EventId}", eventId);

            var result =
                (await eventAttendeeManager.GetEventLeadsAsync(eventId, cancellationToken))
                .Select(ea => ea.User.ToDisplayUser());
            return Ok(result);
        }

        /// <summary>
        /// Promotes an attendee to event lead. Only existing event leads can promote.
        /// </summary>
        /// <param name="eventId">The event ID.</param>
        /// <param name="userId">The user ID to promote.</param>
        /// <param name="cancellationToken">Cancellation token.</param>
        /// <response code="200">Returns the updated event attendee.</response>
        /// <response code="400">Operation not allowed.</response>
        /// <response code="403">User is not an event lead.</response>
        [HttpPut("{userId}/promote")]
        [Authorize(Policy = AuthorizationPolicyConstants.ValidUser)]
        [RequiredScope(Constants.TrashMobWriteScope)]
        [ProducesResponseType(typeof(EventAttendeeDto), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(string), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        public async Task<IActionResult> PromoteToLead(Guid eventId, Guid userId, CancellationToken cancellationToken)
        {
            logger.LogInformation("V2 PromoteToLead Event={EventId}, User={UserId}", eventId, userId);

            var isCurrentUserLead = await eventAttendeeManager.IsEventLeadAsync(eventId, UserId, cancellationToken);
            if (!isCurrentUserLead)
            {
                return Forbid();
            }

            try
            {
                var result = await eventAttendeeManager.PromoteToLeadAsync(eventId, userId, UserId, cancellationToken);
                return Ok(result.ToV2Dto());
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(ex.Message);
            }
        }

        /// <summary>
        /// Demotes an event lead to regular attendee. Only existing event leads can demote.
        /// </summary>
        /// <param name="eventId">The event ID.</param>
        /// <param name="userId">The user ID to demote.</param>
        /// <param name="cancellationToken">Cancellation token.</param>
        /// <response code="200">Returns the updated event attendee.</response>
        /// <response code="400">Operation not allowed.</response>
        /// <response code="403">User is not an event lead.</response>
        [HttpPut("{userId}/demote")]
        [Authorize(Policy = AuthorizationPolicyConstants.ValidUser)]
        [RequiredScope(Constants.TrashMobWriteScope)]
        [ProducesResponseType(typeof(EventAttendeeDto), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(string), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        public async Task<IActionResult> DemoteFromLead(Guid eventId, Guid userId, CancellationToken cancellationToken)
        {
            logger.LogInformation("V2 DemoteFromLead Event={EventId}, User={UserId}", eventId, userId);

            var isCurrentUserLead = await eventAttendeeManager.IsEventLeadAsync(eventId, UserId, cancellationToken);
            if (!isCurrentUserLead)
            {
                return Forbid();
            }

            try
            {
                var result = await eventAttendeeManager.DemoteFromLeadAsync(eventId, userId, UserId, cancellationToken);
                return Ok(result.ToV2Dto());
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(ex.Message);
            }
        }

        /// <summary>
        /// Verifies an attendee's waiver status. Only event leads or admins can check.
        /// </summary>
        /// <param name="eventId">The event ID.</param>
        /// <param name="userId">The user ID to check.</param>
        /// <param name="cancellationToken">Cancellation token.</param>
        /// <response code="200">Returns the waiver status.</response>
        /// <response code="403">User is not authorized.</response>
        [HttpGet("{userId}/waiver-status")]
        [Authorize(Policy = AuthorizationPolicyConstants.ValidUser)]
        [RequiredScope(Constants.TrashMobReadScope)]
        [ProducesResponseType(typeof(WaiverCheckResult), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        public async Task<IActionResult> VerifyAttendeeWaiverStatus(
            Guid eventId, Guid userId, CancellationToken cancellationToken)
        {
            logger.LogInformation("V2 VerifyWaiverStatus Event={EventId}, User={UserId}", eventId, userId);

            var isAdmin = User.IsInRole("Admin");
            var isEventLead = await eventAttendeeManager
                .IsEventLeadAsync(eventId, UserId, cancellationToken);

            if (!isAdmin && !isEventLead)
            {
                return Forbid();
            }

            var hasValidWaiver = await userWaiverManager
                .HasValidWaiverForEventAsync(userId, eventId, cancellationToken);

            return Ok(new WaiverCheckResult { HasValidWaiver = hasValidWaiver });
        }
    }
}
