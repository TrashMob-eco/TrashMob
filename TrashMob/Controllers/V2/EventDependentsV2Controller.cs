namespace TrashMob.Controllers.V2
{
    using System;
    using System.Collections.Generic;
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
    using TrashMob.Security;
    using TrashMob.Shared;
    using TrashMob.Shared.Managers.Interfaces;

    /// <summary>
    /// V2 controller for managing dependents registered for events.
    /// </summary>
    [ApiController]
    [ApiVersion("2.0")]
    [EnableCors("_myAllowSpecificOrigins")]
    [Route("api/v{version:apiVersion}/events/{eventId}/dependents")]
    public class EventDependentsV2Controller(
        IEventDependentManager eventDependentManager,
        IEventAttendeeManager eventAttendeeManager,
        ILogger<EventDependentsV2Controller> logger) : ControllerBase
    {
        private Guid UserId => new(HttpContext.Items["UserId"]?.ToString() ?? string.Empty);

        /// <summary>
        /// Registers dependents for an event.
        /// </summary>
        /// <param name="eventId">The event ID.</param>
        /// <param name="request">The registration request containing dependent IDs.</param>
        /// <param name="cancellationToken">Cancellation token.</param>
        /// <response code="200">Returns the created registrations.</response>
        /// <response code="400">Invalid request.</response>
        [HttpPost]
        [Authorize(Policy = AuthorizationPolicyConstants.ValidUser)]
        [RequiredScope(Constants.TrashMobWriteScope)]
        [ProducesResponseType(typeof(IEnumerable<EventDependent>), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> RegisterDependents(
            Guid eventId, RegisterEventDependentsRequest request, CancellationToken cancellationToken)
        {
            logger.LogInformation("V2 RegisterDependents Event={EventId} Count={Count}", eventId, request.DependentIds?.Count ?? 0);

            var result = await eventDependentManager.RegisterDependentsAsync(
                eventId, request.DependentIds, UserId, cancellationToken);

            if (!result.IsSuccess)
            {
                return BadRequest(result.ErrorMessage);
            }

            return Ok(result.Data);
        }

        /// <summary>
        /// Gets all dependents registered for an event (event leads only).
        /// </summary>
        /// <param name="eventId">The event ID.</param>
        /// <param name="cancellationToken">Cancellation token.</param>
        /// <response code="200">Returns the list of event dependents.</response>
        /// <response code="403">User is not an event lead.</response>
        [HttpGet]
        [Authorize(Policy = AuthorizationPolicyConstants.ValidUser)]
        [RequiredScope(Constants.TrashMobReadScope)]
        [ProducesResponseType(typeof(IEnumerable<EventDependent>), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        public async Task<IActionResult> GetEventDependents(Guid eventId, CancellationToken cancellationToken)
        {
            logger.LogInformation("V2 GetEventDependents Event={EventId}", eventId);

            var isLead = await eventAttendeeManager.IsEventLeadAsync(eventId, UserId, cancellationToken);
            if (!isLead)
            {
                return Forbid();
            }

            var dependents = await eventDependentManager.GetByEventIdAsync(eventId, cancellationToken);
            return Ok(dependents);
        }

        /// <summary>
        /// Gets the count of dependents registered for an event.
        /// </summary>
        /// <param name="eventId">The event ID.</param>
        /// <param name="cancellationToken">Cancellation token.</param>
        /// <response code="200">Returns the dependent count.</response>
        [HttpGet("count")]
        [Authorize(Policy = AuthorizationPolicyConstants.ValidUser)]
        [RequiredScope(Constants.TrashMobReadScope)]
        [ProducesResponseType(typeof(int), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetDependentCount(Guid eventId, CancellationToken cancellationToken)
        {
            logger.LogInformation("V2 GetDependentCount Event={EventId}", eventId);

            var count = await eventDependentManager.GetDependentCountAsync(eventId, cancellationToken);
            return Ok(count);
        }

        /// <summary>
        /// Unregisters a dependent from an event.
        /// </summary>
        /// <param name="eventId">The event ID.</param>
        /// <param name="dependentId">The dependent ID to unregister.</param>
        /// <param name="cancellationToken">Cancellation token.</param>
        /// <response code="204">Dependent unregistered successfully.</response>
        /// <response code="404">Registration not found.</response>
        [HttpDelete("{dependentId}")]
        [Authorize(Policy = AuthorizationPolicyConstants.ValidUser)]
        [RequiredScope(Constants.TrashMobWriteScope)]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> UnregisterDependent(
            Guid eventId, Guid dependentId, CancellationToken cancellationToken)
        {
            logger.LogInformation("V2 UnregisterDependent Event={EventId} Dependent={DependentId}", eventId, dependentId);

            var result = await eventDependentManager.UnregisterDependentAsync(
                eventId, dependentId, UserId, cancellationToken);

            if (result == 0)
            {
                return NotFound();
            }

            return NoContent();
        }

        /// <summary>
        /// Request model for registering dependents for an event.
        /// </summary>
        public class RegisterEventDependentsRequest
        {
            /// <summary>
            /// Gets or sets the IDs of the dependents to register.
            /// </summary>
            public List<Guid> DependentIds { get; set; } = [];
        }
    }
}
