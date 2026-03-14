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
    /// V2 controller for managing partner social media accounts.
    /// </summary>
    [ApiController]
    [ApiVersion("2.0")]
    [EnableCors("_myAllowSpecificOrigins")]
    [Route("api/v{version:apiVersion}/partner-social-media-accounts")]
    public class PartnerSocialMediaAccountsV2Controller(
        IPartnerSocialMediaAccountManager partnerSocialMediaAccountManager,
        IKeyedManager<Partner> partnerManager,
        IAuthorizationService authorizationService,
        ILogger<PartnerSocialMediaAccountsV2Controller> logger) : ControllerBase
    {
        private Guid UserId => new(HttpContext.Items["UserId"]?.ToString() ?? string.Empty);

        /// <summary>
        /// Gets all social media accounts for a partner.
        /// </summary>
        /// <param name="partnerId">The partner ID.</param>
        /// <param name="cancellationToken">Cancellation token.</param>
        /// <response code="200">Returns the partner social media accounts.</response>
        /// <response code="403">Not authorized.</response>
        /// <response code="404">Partner not found.</response>
        [HttpGet("by-partner/{partnerId}")]
        [Authorize(Policy = AuthorizationPolicyConstants.ValidUser)]
        [ProducesResponseType(typeof(IEnumerable<PartnerSocialMediaAccountDto>), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetByPartner(Guid partnerId, CancellationToken cancellationToken)
        {
            logger.LogInformation("V2 GetPartnerSocialMediaAccounts for Partner={PartnerId}", partnerId);

            var partner = await partnerManager.GetAsync(partnerId, cancellationToken);
            if (partner is null)
            {
                return NotFound();
            }

            if (!await IsAuthorizedAsync(partner, AuthorizationPolicyConstants.UserIsPartnerUserOrIsAdmin))
            {
                return Forbid();
            }

            var accounts = await partnerSocialMediaAccountManager.GetByParentIdAsync(partnerId, cancellationToken);
            return Ok(accounts.Select(a => a.ToV2Dto()));
        }

        /// <summary>
        /// Gets a single partner social media account by ID.
        /// </summary>
        /// <param name="id">The social media account ID.</param>
        /// <param name="cancellationToken">Cancellation token.</param>
        /// <response code="200">Returns the partner social media account.</response>
        /// <response code="401">Not authenticated.</response>
        /// <response code="404">Social media account not found.</response>
        [HttpGet("{id}")]
        [Authorize(Policy = AuthorizationPolicyConstants.ValidUser)]
        [ProducesResponseType(typeof(PartnerSocialMediaAccountDto), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
        public async Task<IActionResult> Get(Guid id, CancellationToken cancellationToken)
        {
            logger.LogInformation("V2 GetPartnerSocialMediaAccount for Id={AccountId}", id);

            var account = await partnerSocialMediaAccountManager.GetAsync(id, cancellationToken);
            if (account is null)
            {
                return NotFound();
            }

            return Ok(account.ToV2Dto());
        }

        /// <summary>
        /// Adds a new partner social media account.
        /// </summary>
        /// <param name="dto">The social media account to add.</param>
        /// <param name="cancellationToken">Cancellation token.</param>
        /// <response code="200">Returns the created social media account.</response>
        /// <response code="403">Not authorized.</response>
        /// <response code="404">Partner not found.</response>
        [HttpPost]
        [Authorize(Policy = AuthorizationPolicyConstants.ValidUser)]
        [RequiredScope(Constants.TrashMobWriteScope)]
        [ProducesResponseType(typeof(PartnerSocialMediaAccountDto), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
        public async Task<IActionResult> Add(PartnerSocialMediaAccountDto dto, CancellationToken cancellationToken)
        {
            logger.LogInformation("V2 AddPartnerSocialMediaAccount for Partner={PartnerId}", dto.PartnerId);

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
            var result = await partnerSocialMediaAccountManager.AddAsync(entity, UserId, cancellationToken);
            return Ok(result.ToV2Dto());
        }

        /// <summary>
        /// Updates an existing partner social media account.
        /// </summary>
        /// <param name="dto">The social media account to update.</param>
        /// <param name="cancellationToken">Cancellation token.</param>
        /// <response code="200">Returns the updated social media account.</response>
        /// <response code="403">Not authorized.</response>
        /// <response code="404">Partner not found.</response>
        [HttpPut]
        [Authorize(Policy = AuthorizationPolicyConstants.ValidUser)]
        [RequiredScope(Constants.TrashMobWriteScope)]
        [ProducesResponseType(typeof(PartnerSocialMediaAccountDto), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
        public async Task<IActionResult> Update(PartnerSocialMediaAccountDto dto, CancellationToken cancellationToken)
        {
            logger.LogInformation("V2 UpdatePartnerSocialMediaAccount for Id={AccountId}", dto.Id);

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
            var result = await partnerSocialMediaAccountManager.UpdateAsync(entity, UserId, cancellationToken);
            return Ok(result.ToV2Dto());
        }

        /// <summary>
        /// Deletes a partner social media account.
        /// </summary>
        /// <param name="id">The social media account ID.</param>
        /// <param name="cancellationToken">Cancellation token.</param>
        /// <response code="204">Social media account deleted.</response>
        /// <response code="403">Not authorized.</response>
        /// <response code="404">Social media account or partner not found.</response>
        [HttpDelete("{id}")]
        [Authorize(Policy = AuthorizationPolicyConstants.ValidUser)]
        [RequiredScope(Constants.TrashMobWriteScope)]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
        public async Task<IActionResult> Delete(Guid id, CancellationToken cancellationToken)
        {
            logger.LogInformation("V2 DeletePartnerSocialMediaAccount for Id={AccountId}", id);

            var account = await partnerSocialMediaAccountManager.GetAsync(id, cancellationToken);
            if (account is null)
            {
                return NotFound();
            }

            var partner = await partnerManager.GetAsync(account.PartnerId, cancellationToken);
            if (partner is null)
            {
                return NotFound();
            }

            if (!await IsAuthorizedAsync(partner, AuthorizationPolicyConstants.UserIsPartnerUserOrIsAdmin))
            {
                return Forbid();
            }

            await partnerSocialMediaAccountManager.DeleteAsync(id, cancellationToken);
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
