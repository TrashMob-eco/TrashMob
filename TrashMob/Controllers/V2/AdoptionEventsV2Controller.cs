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
    using TrashMob.Models.Extensions.V2;
    using TrashMob.Models.Poco.V2;
    using TrashMob.Security;
    using TrashMob.Shared;
    using TrashMob.Shared.Managers.Interfaces;

    /// <summary>
    /// V2 controller for managing event links to team adoptions.
    /// </summary>
    [ApiController]
    [ApiVersion("2.0")]
    [EnableCors("_myAllowSpecificOrigins")]
    [Route("api/v{version:apiVersion}/adoptions/{adoptionId}/events")]
    public class AdoptionEventsV2Controller(
        ITeamAdoptionEventManager adoptionEventManager,
        ITeamAdoptionManager adoptionManager,
        ITeamMemberManager teamMemberManager,
        ILogger<AdoptionEventsV2Controller> logger) : ControllerBase
    {
        private Guid UserId => new(HttpContext.Items["UserId"]?.ToString() ?? string.Empty);

        /// <summary>
        /// Gets all events linked to an adoption.
        /// </summary>
        /// <param name="adoptionId">The adoption ID.</param>
        /// <param name="cancellationToken">Cancellation token.</param>
        [HttpGet]
        [Authorize(Policy = AuthorizationPolicyConstants.ValidUser)]
        [RequiredScope(Constants.TrashMobReadScope)]
        [ProducesResponseType(typeof(IEnumerable<TeamAdoptionEventDto>), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetLinkedEvents(Guid adoptionId, CancellationToken cancellationToken)
        {
            logger.LogInformation("V2 GetLinkedEvents for Adoption={AdoptionId}", adoptionId);

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
            return Ok(events.Select(e => e.ToV2Dto()));
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
        [ProducesResponseType(typeof(TeamAdoptionEventDto), StatusCodes.Status201Created)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
        public async Task<IActionResult> LinkEvent(
            Guid adoptionId,
            Guid eventId,
            [FromBody] LinkEventRequest request,
            CancellationToken cancellationToken)
        {
            logger.LogInformation("V2 LinkEvent Adoption={AdoptionId}, Event={EventId}", adoptionId, eventId);

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

            return CreatedAtAction(nameof(GetLinkedEvents), new { adoptionId }, result.Data.ToV2Dto());
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
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
        public async Task<IActionResult> UnlinkEvent(
            Guid adoptionId,
            Guid adoptionEventId,
            CancellationToken cancellationToken)
        {
            logger.LogInformation("V2 UnlinkEvent Adoption={AdoptionId}, AdoptionEvent={AdoptionEventId}", adoptionId, adoptionEventId);

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
}
