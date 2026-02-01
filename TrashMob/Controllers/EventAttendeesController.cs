namespace TrashMob.Controllers
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading;
    using System.Threading.Tasks;
    using Microsoft.AspNetCore.Authorization;
    using Microsoft.AspNetCore.Http;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.EntityFrameworkCore;
    using Microsoft.Identity.Web.Resource;
    using TrashMob.Models;
    using TrashMob.Models.Poco;
    using TrashMob.Security;
    using TrashMob.Shared;
    using TrashMob.Shared.Managers.Interfaces;
    using TrashMob.Shared.Poco;

    [Route("api/eventattendees")]
    public class EventAttendeesController : SecureController
    {
        private readonly IEventAttendeeManager eventAttendeeManager;

        public EventAttendeesController(IEventAttendeeManager eventAttendeeManager)
        {
            this.eventAttendeeManager = eventAttendeeManager;
        }

        /// <summary>
        /// Gets all attendees for a given event.
        /// </summary>
        /// <param name="eventId">The event ID.</param>
        /// <param name="cancellationToken">Cancellation token.</param>
        [HttpGet("{eventId}")]
        [ProducesResponseType(typeof(IEnumerable<DisplayUser>), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetEventAttendees(Guid eventId, CancellationToken cancellationToken)
        {
            var result =
                (await eventAttendeeManager.GetByParentIdAsync(eventId, cancellationToken).ConfigureAwait(false))
                .Select(u => u.User.ToDisplayUser());
            TrackEvent(nameof(GetEventAttendees));
            return Ok(result);
        }

        /// <summary>
        /// Updates an event attendee. Requires write scope.
        /// </summary>
        /// <param name="eventAttendee">The event attendee to update.</param>
        /// <param name="cancellationToken">Cancellation token.</param>
        /// <remarks>Returns the updated event attendee.</remarks>
        [HttpPut]
        [RequiredScope(Constants.TrashMobWriteScope)]
        [ProducesResponseType(typeof(EventAttendee), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(void), StatusCodes.Status403Forbidden)]
        [ProducesResponseType(typeof(void), StatusCodes.Status404NotFound)]
        public async Task<IActionResult> UpdateEventAttendee(EventAttendee eventAttendee,
            CancellationToken cancellationToken)
        {
            var authResult =
                await AuthorizationService.AuthorizeAsync(User, eventAttendee,
                    AuthorizationPolicyConstants.UserOwnsEntity);

            if (!User.Identity.IsAuthenticated || !authResult.Succeeded)
            {
                return Forbid();
            }

            try
            {
                var updatedEventAttendee = await eventAttendeeManager
                    .UpdateAsync(eventAttendee, UserId, cancellationToken).ConfigureAwait(false);
                TrackEvent(nameof(UpdateEventAttendee));

                return Ok(updatedEventAttendee);
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!await EventAttendeeExists(eventAttendee.EventId, eventAttendee.UserId, cancellationToken)
                        .ConfigureAwait(false))
                {
                    return NotFound();
                }

                throw;
            }
        }

        /// <summary>
        /// Adds a new event attendee. Requires a valid user.
        /// </summary>
        /// <param name="eventAttendee">The event attendee to add.</param>
        /// <param name="cancellationToken">Cancellation token.</param>
        [HttpPost]
        [Authorize(Policy = AuthorizationPolicyConstants.ValidUser)]
        [RequiredScope(Constants.TrashMobWriteScope)]
        [ProducesResponseType(200)]
        public async Task<IActionResult> AddEventAttendee(EventAttendee eventAttendee,
            CancellationToken cancellationToken)
        {
            await eventAttendeeManager.AddAsync(eventAttendee, UserId, cancellationToken).ConfigureAwait(false);
            TrackEvent(nameof(AddEventAttendee));
            return Ok();
        }

        /// <summary>
        /// Deletes an event attendee. Requires a valid user.
        /// </summary>
        /// <param name="eventId">The event ID.</param>
        /// <param name="userId">The user ID.</param>
        /// <param name="cancellationToken">Cancellation token.</param>
        [HttpDelete("{eventId}/{userId}")]
        // Todo: Tighten this down
        [Authorize(Policy = AuthorizationPolicyConstants.ValidUser)]
        [RequiredScope(Constants.TrashMobWriteScope)]
        [ProducesResponseType(204)]
        public async Task<IActionResult> DeleteEventAttendee(Guid eventId, Guid userId,
            CancellationToken cancellationToken)
        {
            await eventAttendeeManager.Delete(eventId, userId, cancellationToken).ConfigureAwait(false);
            TrackEvent(nameof(DeleteEventAttendee));

            return new NoContentResult();
        }

        /// <summary>
        /// Gets all event leads for a given event.
        /// </summary>
        /// <param name="eventId">The event ID.</param>
        /// <param name="cancellationToken">Cancellation token.</param>
        [HttpGet("{eventId}/leads")]
        [ProducesResponseType(typeof(IEnumerable<DisplayUser>), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetEventLeads(Guid eventId, CancellationToken cancellationToken)
        {
            var result =
                (await eventAttendeeManager.GetEventLeadsAsync(eventId, cancellationToken).ConfigureAwait(false))
                .Select(ea => ea.User.ToDisplayUser());
            TrackEvent(nameof(GetEventLeads));
            return Ok(result);
        }

        /// <summary>
        /// Promotes an attendee to event lead status. Requires write scope.
        /// Only existing event leads can promote other attendees.
        /// Maximum 5 co-leads per event.
        /// </summary>
        /// <param name="eventId">The event ID.</param>
        /// <param name="userId">The user ID of the attendee to promote.</param>
        /// <param name="cancellationToken">Cancellation token.</param>
        [HttpPut("{eventId}/{userId}/promote")]
        [RequiredScope(Constants.TrashMobWriteScope)]
        [ProducesResponseType(typeof(EventAttendee), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(string), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(void), StatusCodes.Status403Forbidden)]
        public async Task<IActionResult> PromoteToLead(Guid eventId, Guid userId, CancellationToken cancellationToken)
        {
            // Check if current user is an event lead
            var isCurrentUserLead = await eventAttendeeManager.IsEventLeadAsync(eventId, UserId, cancellationToken);
            if (!isCurrentUserLead)
            {
                return Forbid();
            }

            try
            {
                var result = await eventAttendeeManager.PromoteToLeadAsync(eventId, userId, UserId, cancellationToken);
                TrackEvent(nameof(PromoteToLead));
                return Ok(result);
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(ex.Message);
            }
        }

        /// <summary>
        /// Demotes an event lead back to regular attendee status. Requires write scope.
        /// Only existing event leads can demote other leads.
        /// Cannot demote the last remaining lead.
        /// </summary>
        /// <param name="eventId">The event ID.</param>
        /// <param name="userId">The user ID of the lead to demote.</param>
        /// <param name="cancellationToken">Cancellation token.</param>
        [HttpPut("{eventId}/{userId}/demote")]
        [RequiredScope(Constants.TrashMobWriteScope)]
        [ProducesResponseType(typeof(EventAttendee), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(string), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(void), StatusCodes.Status403Forbidden)]
        public async Task<IActionResult> DemoteFromLead(Guid eventId, Guid userId, CancellationToken cancellationToken)
        {
            // Check if current user is an event lead
            var isCurrentUserLead = await eventAttendeeManager.IsEventLeadAsync(eventId, UserId, cancellationToken);
            if (!isCurrentUserLead)
            {
                return Forbid();
            }

            try
            {
                var result = await eventAttendeeManager.DemoteFromLeadAsync(eventId, userId, UserId, cancellationToken);
                TrackEvent(nameof(DemoteFromLead));
                return Ok(result);
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(ex.Message);
            }
        }

        private async Task<bool> EventAttendeeExists(Guid eventId, Guid userId, CancellationToken cancellationToken)
        {
            var attendee = await eventAttendeeManager
                .GetAsync(ea => ea.EventId == eventId && ea.UserId == userId, cancellationToken).ConfigureAwait(false);

            return attendee?.FirstOrDefault() != null;
        }
    }
}