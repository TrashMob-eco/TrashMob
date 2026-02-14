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
    /// Controller for managing event links to team adoptions.
    /// </summary>
    [Route("api/adoptions/{adoptionId}/events")]
    public class AdoptionEventsController(
        ITeamAdoptionEventManager adoptionEventManager,
        ITeamAdoptionManager adoptionManager,
        ITeamMemberManager teamMemberManager)
        : SecureController
    {

        /// <summary>
        /// Gets all events linked to an adoption.
        /// </summary>
        /// <param name="adoptionId">The adoption ID.</param>
        /// <param name="cancellationToken">Cancellation token.</param>
        [HttpGet]
        [Authorize(Policy = AuthorizationPolicyConstants.ValidUser)]
        [RequiredScope(Constants.TrashMobReadScope)]
        [ProducesResponseType(typeof(IEnumerable<TeamAdoptionEvent>), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetLinkedEvents(Guid adoptionId, CancellationToken cancellationToken)
        {
            var adoption = await adoptionManager.GetAsync(adoptionId, cancellationToken);
            if (adoption is null)
            {
                return NotFound();
            }

            // Only team members can view adoption events
            var isMember = await teamMemberManager.IsMemberAsync(adoption.TeamId, UserId, cancellationToken);
            if (!isMember)
            {
                return Forbid();
            }

            var events = await adoptionEventManager.GetByAdoptionIdAsync(adoptionId, cancellationToken);
            return Ok(events);
        }

        /// <summary>
        /// Links an event to an adoption. Only team leads can link events.
        /// </summary>
        /// <param name="adoptionId">The adoption ID.</param>
        /// <param name="eventId">The event ID to link.</param>
        /// <param name="request">Optional request body with notes.</param>
        /// <param name="cancellationToken">Cancellation token.</param>
        [HttpPost("{eventId}")]
        [Authorize(Policy = AuthorizationPolicyConstants.ValidUser)]
        [RequiredScope(Constants.TrashMobWriteScope)]
        [ProducesResponseType(typeof(TeamAdoptionEvent), StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> LinkEvent(
            Guid adoptionId,
            Guid eventId,
            [FromBody] LinkEventRequest request,
            CancellationToken cancellationToken)
        {
            var adoption = await adoptionManager.GetAsync(adoptionId, cancellationToken);
            if (adoption is null)
            {
                return NotFound();
            }

            // Only team leads can link events to adoptions
            var isLead = await teamMemberManager.IsTeamLeadAsync(adoption.TeamId, UserId, cancellationToken);
            if (!isLead)
            {
                return Forbid();
            }

            var result = await adoptionEventManager.LinkEventAsync(
                adoptionId,
                eventId,
                request?.Notes,
                UserId,
                cancellationToken);

            if (!result.IsSuccess)
            {
                return BadRequest(result.ErrorMessage);
            }

            return CreatedAtAction(nameof(GetLinkedEvents), new { adoptionId }, result.Data);
        }

        /// <summary>
        /// Unlinks an event from an adoption. Only team leads can unlink events.
        /// </summary>
        /// <param name="adoptionId">The adoption ID.</param>
        /// <param name="adoptionEventId">The adoption event link ID to remove.</param>
        /// <param name="cancellationToken">Cancellation token.</param>
        [HttpDelete("{adoptionEventId}")]
        [Authorize(Policy = AuthorizationPolicyConstants.ValidUser)]
        [RequiredScope(Constants.TrashMobWriteScope)]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> UnlinkEvent(
            Guid adoptionId,
            Guid adoptionEventId,
            CancellationToken cancellationToken)
        {
            var adoption = await adoptionManager.GetAsync(adoptionId, cancellationToken);
            if (adoption is null)
            {
                return NotFound();
            }

            // Only team leads can unlink events from adoptions
            var isLead = await teamMemberManager.IsTeamLeadAsync(adoption.TeamId, UserId, cancellationToken);
            if (!isLead)
            {
                return Forbid();
            }

            var result = await adoptionEventManager.UnlinkEventAsync(adoptionEventId, UserId, cancellationToken);

            if (!result.IsSuccess)
            {
                return BadRequest(result.ErrorMessage);
            }

            return NoContent();
        }
    }

    /// <summary>
    /// Request model for linking an event to an adoption.
    /// </summary>
    public class LinkEventRequest
    {
        /// <summary>
        /// Gets or sets optional notes about why this event is linked.
        /// </summary>
        public string Notes { get; set; }
    }
}
