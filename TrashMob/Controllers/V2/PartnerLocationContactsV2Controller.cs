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
    /// V2 controller for managing partner location contacts.
    /// </summary>
    [ApiController]
    [ApiVersion("2.0")]
    [EnableCors("_myAllowSpecificOrigins")]
    [Route("api/v{version:apiVersion}/partner-location-contacts")]
    public class PartnerLocationContactsV2Controller(
        IPartnerLocationManager partnerLocationManager,
        IPartnerLocationContactManager partnerLocationContactManager,
        IAuthorizationService authorizationService,
        ILogger<PartnerLocationContactsV2Controller> logger) : ControllerBase
    {
        private Guid UserId => Guid.TryParse(HttpContext.Items["UserId"]?.ToString(), out var parsedUserId) ? parsedUserId : Guid.Empty;

        /// <summary>
        /// Gets all contacts for a partner location.
        /// </summary>
        /// <param name="partnerLocationId">The partner location ID.</param>
        /// <param name="cancellationToken">Cancellation token.</param>
        /// <response code="200">Returns the partner location contacts.</response>
        [HttpGet("by-location/{partnerLocationId}")]
        [Authorize(Policy = AuthorizationPolicyConstants.ValidUser)]
        [ProducesResponseType(typeof(IEnumerable<PartnerLocationContactDto>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> GetByLocation(Guid partnerLocationId, CancellationToken cancellationToken)
        {
            logger.LogInformation("V2 GetPartnerLocationContacts for Location={PartnerLocationId}", partnerLocationId);

            var results = await partnerLocationContactManager.GetByParentIdAsync(partnerLocationId, cancellationToken);

            return Ok(results.Select(c => c.ToV2Dto()));
        }

        /// <summary>
        /// Gets a single partner location contact by its identifier.
        /// </summary>
        /// <param name="id">The partner location contact ID.</param>
        /// <param name="cancellationToken">Cancellation token.</param>
        /// <response code="200">Returns the partner location contact.</response>
        /// <response code="404">Contact not found.</response>
        [HttpGet("{id}")]
        [Authorize(Policy = AuthorizationPolicyConstants.ValidUser)]
        [ProducesResponseType(typeof(PartnerLocationContactDto), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> Get(Guid id, CancellationToken cancellationToken)
        {
            logger.LogInformation("V2 GetPartnerLocationContact for Id={ContactId}", id);

            var contact = await partnerLocationContactManager.GetAsync(id, cancellationToken);

            if (contact is null)
            {
                return NotFound();
            }

            return Ok(contact.ToV2Dto());
        }

        /// <summary>
        /// Creates a new partner location contact.
        /// </summary>
        /// <param name="dto">The partner location contact to create.</param>
        /// <param name="cancellationToken">Cancellation token.</param>
        /// <response code="201">Contact created.</response>
        /// <response code="403">Not authorized.</response>
        /// <response code="404">Partner location not found.</response>
        [HttpPost]
        [Authorize(Policy = AuthorizationPolicyConstants.ValidUser)]
        [RequiredScope(Constants.TrashMobWriteScope)]
        [ProducesResponseType(typeof(PartnerLocationContactDto), StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> Add(PartnerLocationContactDto dto, CancellationToken cancellationToken)
        {
            logger.LogInformation("V2 AddPartnerLocationContact for Location={PartnerLocationId}", dto.PartnerLocationId);

            var partner = await partnerLocationManager.GetPartnerForLocationAsync(dto.PartnerLocationId, cancellationToken);

            if (partner is null)
            {
                return NotFound();
            }

            if (!await IsAuthorizedAsync(partner, AuthorizationPolicyConstants.UserIsPartnerUserOrIsAdmin))
            {
                return Forbid();
            }

            var entity = dto.ToEntity();
            var result = await partnerLocationContactManager.AddAsync(entity, UserId, cancellationToken);

            return CreatedAtAction(nameof(Get), new { id = result.Id }, result.ToV2Dto());
        }

        /// <summary>
        /// Updates an existing partner location contact.
        /// </summary>
        /// <param name="dto">The partner location contact to update.</param>
        /// <param name="cancellationToken">Cancellation token.</param>
        /// <response code="200">Contact updated.</response>
        /// <response code="403">Not authorized.</response>
        [HttpPut]
        [Authorize(Policy = AuthorizationPolicyConstants.ValidUser)]
        [RequiredScope(Constants.TrashMobWriteScope)]
        [ProducesResponseType(typeof(PartnerLocationContactDto), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> Update(PartnerLocationContactDto dto, CancellationToken cancellationToken)
        {
            logger.LogInformation("V2 UpdatePartnerLocationContact for Id={ContactId}", dto.Id);

            var partner = await partnerLocationManager.GetPartnerForLocationAsync(dto.PartnerLocationId, cancellationToken);

            if (partner is null)
            {
                return NotFound();
            }

            if (!await IsAuthorizedAsync(partner, AuthorizationPolicyConstants.UserIsPartnerUserOrIsAdmin))
            {
                return Forbid();
            }

            var entity = dto.ToEntity();
            var result = await partnerLocationContactManager.UpdateAsync(entity, UserId, cancellationToken);

            return Ok(result.ToV2Dto());
        }

        /// <summary>
        /// Deletes a partner location contact.
        /// </summary>
        /// <param name="id">The partner location contact ID.</param>
        /// <param name="cancellationToken">Cancellation token.</param>
        /// <response code="204">Contact deleted.</response>
        /// <response code="403">Not authorized.</response>
        [HttpDelete("{id}")]
        [Authorize(Policy = AuthorizationPolicyConstants.ValidUser)]
        [RequiredScope(Constants.TrashMobWriteScope)]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> Delete(Guid id, CancellationToken cancellationToken)
        {
            logger.LogInformation("V2 DeletePartnerLocationContact for Id={ContactId}", id);

            var partner = await partnerLocationContactManager.GetPartnerForLocationContact(id, cancellationToken);

            if (partner is null)
            {
                return NotFound();
            }

            if (!await IsAuthorizedAsync(partner, AuthorizationPolicyConstants.UserIsPartnerUserOrIsAdmin))
            {
                return Forbid();
            }

            await partnerLocationContactManager.DeleteAsync(id, cancellationToken);

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
