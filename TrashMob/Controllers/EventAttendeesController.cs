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
        private readonly IUserWaiverManager userWaiverManager;

        public EventAttendeesController(
            IEventAttendeeManager eventAttendeeManager,
            IUserWaiverManager userWaiverManager)
        {
            this.eventAttendeeManager = eventAttendeeManager;
            this.userWaiverManager = userWaiverManager;
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
                (await eventAttendeeManager.GetByParentIdAsync(eventId, cancellationToken))
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
        [Authorize(Policy = AuthorizationPolicyConstants.ValidUser)]
        [RequiredScope(Constants.TrashMobWriteScope)]
        [ProducesResponseType(typeof(EventAttendee), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(void), StatusCodes.Status403Forbidden)]
        [ProducesResponseType(typeof(void), StatusCodes.Status404NotFound)]
        public async Task<IActionResult> UpdateEventAttendee(EventAttendee eventAttendee,
            CancellationToken cancellationToken)
        {
            if (!await IsAuthorizedAsync(eventAttendee, AuthorizationPolicyConstants.UserOwnsEntity))
            {
                return Forbid();
            }

            try
            {
                var updatedEventAttendee = await eventAttendeeManager
                    .UpdateAsync(eventAttendee, UserId, cancellationToken);
                TrackEvent(nameof(UpdateEventAttendee));

                return Ok(updatedEventAttendee);
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!await EventAttendeeExists(eventAttendee.EventId, eventAttendee.UserId, cancellationToken))
                {
                    return NotFound();
                }

                throw;
            }
        }

        /// <summary>
        /// Adds a new event attendee. Requires a valid user and all required waivers to be signed.
        /// </summary>
        /// <param name="eventAttendee">The event attendee to add.</param>
        /// <param name="cancellationToken">Cancellation token.</param>
        /// <returns>Ok if registration succeeds, BadRequest if waivers are required.</returns>
        [HttpPost]
        [Authorize(Policy = AuthorizationPolicyConstants.ValidUser)]
        [RequiredScope(Constants.TrashMobWriteScope)]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(WaiverRequiredResponse), StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> AddEventAttendee(EventAttendee eventAttendee,
            CancellationToken cancellationToken)
        {
            // Validate waiver compliance before registration
            var hasValidWaiver = await userWaiverManager
                .HasValidWaiverForEventAsync(eventAttendee.UserId, eventAttendee.EventId, cancellationToken);

            if (!hasValidWaiver)
            {
                var requiredWaivers = await userWaiverManager
                    .GetRequiredWaiversForEventAsync(eventAttendee.UserId, eventAttendee.EventId, cancellationToken);

                return BadRequest(new WaiverRequiredResponse
                {
                    Message = "You must sign all required waivers before registering for this event.",
                    RequiredWaiverCount = requiredWaivers.Count(),
                    RequiredWaiverIds = requiredWaivers.Select(w => w.Id).ToList()
                });
            }

            await eventAttendeeManager.AddAsync(eventAttendee, UserId, cancellationToken);
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
            await eventAttendeeManager.Delete(eventId, userId, cancellationToken);
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
                (await eventAttendeeManager.GetEventLeadsAsync(eventId, cancellationToken))
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
        [Authorize(Policy = AuthorizationPolicyConstants.ValidUser)]
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
        [Authorize(Policy = AuthorizationPolicyConstants.ValidUser)]
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

        /// <summary>
        /// Verifies an attendee's waiver status at check-in time.
        /// Only accessible by event leads or admins.
        /// </summary>
        /// <param name="eventId">The event ID.</param>
        /// <param name="userId">The user ID of the attendee to check.</param>
        /// <param name="cancellationToken">Cancellation token.</param>
        /// <returns>The waiver status for the attendee.</returns>
        [HttpGet("{eventId}/attendees/{userId}/waiver-status")]
        [Authorize(Policy = AuthorizationPolicyConstants.ValidUser)]
        [RequiredScope(Constants.TrashMobReadScope)]
        [ProducesResponseType(typeof(WaiverCheckResult), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        public async Task<IActionResult> VerifyAttendeeWaiverStatus(
            Guid eventId,
            Guid userId,
            CancellationToken cancellationToken = default)
        {
            // Check if caller is event lead or admin
            var isAdmin = User.IsInRole("Admin");
            var isEventLead = await eventAttendeeManager
                .IsEventLeadAsync(eventId, UserId, cancellationToken);

            if (!isAdmin && !isEventLead)
            {
                return Forbid();
            }

            var hasValidWaiver = await userWaiverManager
                .HasValidWaiverForEventAsync(userId, eventId, cancellationToken);

            TrackEvent(nameof(VerifyAttendeeWaiverStatus));

            return Ok(new WaiverCheckResult { HasValidWaiver = hasValidWaiver });
        }

        private async Task<bool> EventAttendeeExists(Guid eventId, Guid userId, CancellationToken cancellationToken)
        {
            var attendee = await eventAttendeeManager
                .GetAsync(ea => ea.EventId == eventId && ea.UserId == userId, cancellationToken);

            return attendee?.FirstOrDefault() != null;
        }
    }

    /// <summary>
    /// Response model when waiver signing is required before event registration.
    /// </summary>
    public class WaiverRequiredResponse
    {
        /// <summary>
        /// Gets or sets the error message.
        /// </summary>
        public string Message { get; set; }

        /// <summary>
        /// Gets or sets the count of required waivers that need to be signed.
        /// </summary>
        public int RequiredWaiverCount { get; set; }

        /// <summary>
        /// Gets or sets the IDs of the waiver versions that need to be signed.
        /// </summary>
        public List<Guid> RequiredWaiverIds { get; set; }
    }
}