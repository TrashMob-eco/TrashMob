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
    using TrashMob.Models.Poco.V2;
    using TrashMob.Security;
    using TrashMob.Shared;
    using TrashMob.Shared.Managers.Interfaces;

    /// <summary>
    /// V2 controller for managing partner contacts.
    /// </summary>
    [ApiController]
    [ApiVersion("2.0")]
    [EnableCors("_myAllowSpecificOrigins")]
    [Route("api/v{version:apiVersion}/partner-contacts")]
    public class PartnerContactsV2Controller(
        IKeyedManager<Partner> partnerManager,
        IPartnerContactManager partnerContactManager,
        IAuthorizationService authorizationService,
        ILogger<PartnerContactsV2Controller> logger) : ControllerBase
    {
        private Guid UserId => Guid.TryParse(HttpContext.Items["UserId"]?.ToString(), out var parsedUserId) ? parsedUserId : Guid.Empty;

        /// <summary>
        /// Gets all contacts for a partner.
        /// </summary>
        /// <param name="partnerId">The partner ID.</param>
        /// <param name="cancellationToken">Cancellation token.</param>
        /// <response code="200">Returns the partner contacts.</response>
        /// <response code="401">Not authenticated.</response>
        [HttpGet("by-partner/{partnerId}")]
        [Authorize(Policy = AuthorizationPolicyConstants.ValidUser)]
        [ProducesResponseType(typeof(IEnumerable<PartnerContactDto>), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public async Task<IActionResult> GetByPartner(Guid partnerId, CancellationToken cancellationToken)
        {
            logger.LogInformation("V2 GetPartnerContacts for Partner={PartnerId}", partnerId);

            var contacts = await partnerContactManager.GetByParentIdAsync(partnerId, cancellationToken);
            return Ok(contacts.Select(c => c.ToV2Dto()));
        }

        /// <summary>
        /// Gets a single partner contact by ID.
        /// </summary>
        /// <param name="id">The partner contact ID.</param>
        /// <param name="cancellationToken">Cancellation token.</param>
        /// <response code="200">Returns the partner contact.</response>
        /// <response code="401">Not authenticated.</response>
        /// <response code="404">Partner contact not found.</response>
        [HttpGet("{id}")]
        [Authorize(Policy = AuthorizationPolicyConstants.ValidUser)]
        [ProducesResponseType(typeof(PartnerContactDto), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
        public async Task<IActionResult> Get(Guid id, CancellationToken cancellationToken)
        {
            logger.LogInformation("V2 GetPartnerContact for Id={PartnerContactId}", id);

            var contact = await partnerContactManager.GetAsync(id, cancellationToken);
            if (contact is null)
            {
                return NotFound();
            }

            return Ok(contact.ToV2Dto());
        }

        /// <summary>
        /// Adds a new partner contact.
        /// </summary>
        /// <param name="dto">The partner contact to add.</param>
        /// <param name="cancellationToken">Cancellation token.</param>
        /// <response code="200">Returns the created partner contact.</response>
        /// <response code="403">Not authorized.</response>
        /// <response code="404">Partner not found.</response>
        [HttpPost]
        [Authorize(Policy = AuthorizationPolicyConstants.ValidUser)]
        [RequiredScope(Constants.TrashMobWriteScope)]
        [ProducesResponseType(typeof(PartnerContactDto), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
        public async Task<IActionResult> Add(PartnerContactDto dto, CancellationToken cancellationToken)
        {
            logger.LogInformation("V2 AddPartnerContact for Partner={PartnerId}", dto.PartnerId);

            var partner = await partnerManager.GetAsync(dto.PartnerId, cancellationToken);
            if (partner is null)
            {
                return NotFound();
            }

            if (!await IsAuthorizedAsync(partner, AuthorizationPolicyConstants.UserIsPartnerUserOrIsAdmin))
            {
                return Forbid();
            }

            var entity = dto.ToEntity();
            var result = await partnerContactManager.AddAsync(entity, UserId, cancellationToken);
            return Ok(result.ToV2Dto());
        }

        /// <summary>
        /// Updates an existing partner contact.
        /// </summary>
        /// <param name="dto">The partner contact to update.</param>
        /// <param name="cancellationToken">Cancellation token.</param>
        /// <response code="200">Returns the updated partner contact.</response>
        /// <response code="403">Not authorized.</response>
        /// <response code="404">Partner not found.</response>
        [HttpPut]
        [Authorize(Policy = AuthorizationPolicyConstants.ValidUser)]
        [RequiredScope(Constants.TrashMobWriteScope)]
        [ProducesResponseType(typeof(PartnerContactDto), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
        public async Task<IActionResult> Update(PartnerContactDto dto, CancellationToken cancellationToken)
        {
            logger.LogInformation("V2 UpdatePartnerContact for Id={PartnerContactId}", dto.Id);

            var partner = await partnerManager.GetAsync(dto.PartnerId, cancellationToken);
            if (partner is null)
            {
                return NotFound();
            }

            if (!await IsAuthorizedAsync(partner, AuthorizationPolicyConstants.UserIsPartnerUserOrIsAdmin))
            {
                return Forbid();
            }

            var entity = dto.ToEntity();
            var result = await partnerContactManager.UpdateAsync(entity, UserId, cancellationToken);
            return Ok(result.ToV2Dto());
        }

        /// <summary>
        /// Deletes a partner contact.
        /// </summary>
        /// <param name="id">The partner contact ID.</param>
        /// <param name="cancellationToken">Cancellation token.</param>
        /// <response code="204">Partner contact deleted.</response>
        /// <response code="403">Not authorized.</response>
        /// <response code="404">Partner not found.</response>
        [HttpDelete("{id}")]
        [Authorize(Policy = AuthorizationPolicyConstants.ValidUser)]
        [RequiredScope(Constants.TrashMobWriteScope)]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
        public async Task<IActionResult> Delete(Guid id, CancellationToken cancellationToken)
        {
            logger.LogInformation("V2 DeletePartnerContact for Id={PartnerContactId}", id);

            var partner = await partnerContactManager.GetPartnerForContact(id, cancellationToken);
            if (partner is null)
            {
                return NotFound();
            }

            if (!await IsAuthorizedAsync(partner, AuthorizationPolicyConstants.UserIsPartnerUserOrIsAdmin))
            {
                return Forbid();
            }

            await partnerContactManager.DeleteAsync(id, cancellationToken);
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
