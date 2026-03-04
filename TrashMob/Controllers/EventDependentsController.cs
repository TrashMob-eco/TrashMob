namespace TrashMob.Controllers
{
    using System;
    using System.Collections.Generic;
    using System.Threading;
    using System.Threading.Tasks;
    using Microsoft.AspNetCore.Authorization;
    using Microsoft.AspNetCore.Http;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.Identity.Web.Resource;
    using TrashMob.Models;
    using TrashMob.Security;
    using TrashMob.Shared;
    using TrashMob.Shared.Managers.Interfaces;

    /// <summary>
    /// Controller for managing dependents registered for events.
    /// </summary>
    [Route("api/events/{eventId}/dependents")]
    public class EventDependentsController(
        IEventDependentManager eventDependentManager,
        IEventAttendeeManager eventAttendeeManager)
        : SecureController
    {
        /// <summary>
        /// Registers dependents for an event.
        /// </summary>
        /// <param name="eventId">The event ID.</param>
        /// <param name="request">The registration request containing dependent IDs.</param>
        /// <param name="cancellationToken">Cancellation token.</param>
        /// <returns>The created event dependent registrations.</returns>
        [HttpPost]
        [Authorize(Policy = AuthorizationPolicyConstants.ValidUser)]
        [RequiredScope(Constants.TrashMobWriteScope)]
        [ProducesResponseType(typeof(IEnumerable<EventDependent>), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> RegisterDependents(
            Guid eventId, RegisterEventDependentsRequest request, CancellationToken cancellationToken)
        {
            var result = await eventDependentManager.RegisterDependentsAsync(
                eventId, request.DependentIds, UserId, cancellationToken);

            if (!result.IsSuccess)
            {
                return BadRequest(result.ErrorMessage);
            }

            TrackEvent(nameof(RegisterDependents));
            return Ok(result.Data);
        }

        /// <summary>
        /// Gets all dependents registered for an event (event leads only).
        /// </summary>
        /// <param name="eventId">The event ID.</param>
        /// <param name="cancellationToken">Cancellation token.</param>
        /// <returns>List of event dependents.</returns>
        [HttpGet]
        [Authorize(Policy = AuthorizationPolicyConstants.ValidUser)]
        [RequiredScope(Constants.TrashMobReadScope)]
        [ProducesResponseType(typeof(IEnumerable<EventDependent>), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        public async Task<IActionResult> GetEventDependents(Guid eventId, CancellationToken cancellationToken)
        {
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
        /// <returns>The dependent count.</returns>
        [HttpGet("count")]
        [Authorize(Policy = AuthorizationPolicyConstants.ValidUser)]
        [RequiredScope(Constants.TrashMobReadScope)]
        [ProducesResponseType(typeof(int), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetDependentCount(Guid eventId, CancellationToken cancellationToken)
        {
            var count = await eventDependentManager.GetDependentCountAsync(eventId, cancellationToken);
            return Ok(count);
        }

        /// <summary>
        /// Unregisters a dependent from an event.
        /// </summary>
        /// <param name="eventId">The event ID.</param>
        /// <param name="dependentId">The dependent ID to unregister.</param>
        /// <param name="cancellationToken">Cancellation token.</param>
        /// <returns>No content on success.</returns>
        [HttpDelete("{dependentId}")]
        [Authorize(Policy = AuthorizationPolicyConstants.ValidUser)]
        [RequiredScope(Constants.TrashMobWriteScope)]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> UnregisterDependent(
            Guid eventId, Guid dependentId, CancellationToken cancellationToken)
        {
            var result = await eventDependentManager.UnregisterDependentAsync(
                eventId, dependentId, UserId, cancellationToken);

            if (result == 0)
            {
                return NotFound();
            }

            TrackEvent(nameof(UnregisterDependent));
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
            public List<Guid> DependentIds { get; set; }
        }
    }
}
