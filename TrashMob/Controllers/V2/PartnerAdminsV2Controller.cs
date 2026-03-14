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
    /// V2 controller for managing partner administrators.
    /// </summary>
    [ApiController]
    [ApiVersion("2.0")]
    [EnableCors("_myAllowSpecificOrigins")]
    [Route("api/v{version:apiVersion}/partner-admins")]
    public class PartnerAdminsV2Controller(
        IPartnerAdminManager partnerAdminManager,
        IKeyedManager<Partner> partnerManager,
        IAuthorizationService authorizationService,
        ILogger<PartnerAdminsV2Controller> logger) : ControllerBase
    {
        private Guid UserId => new(HttpContext.Items["UserId"]?.ToString() ?? string.Empty);

        /// <summary>
        /// Gets all administrator users for a partner.
        /// </summary>
        /// <param name="partnerId">The partner ID.</param>
        /// <param name="cancellationToken">Cancellation token.</param>
        /// <response code="200">Returns the partner admin users.</response>
        /// <response code="403">Not authorized.</response>
        /// <response code="404">Partner not found.</response>
        [HttpGet("{partnerId}")]
        [Authorize(Policy = AuthorizationPolicyConstants.ValidUser)]
        [ProducesResponseType(typeof(IEnumerable<UserDto>), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetAdmins(Guid partnerId, CancellationToken cancellationToken)
        {
            logger.LogInformation("V2 GetPartnerAdmins for Partner={PartnerId}", partnerId);

            var partner = await partnerManager.GetAsync(partnerId, cancellationToken);
            if (partner is null)
            {
                return NotFound();
            }

            if (!await IsAuthorizedAsync(partner, AuthorizationPolicyConstants.UserIsPartnerUserOrIsAdmin))
            {
                return Forbid();
            }

            var admins = await partnerAdminManager.GetAdminsForPartnerAsync(partnerId, cancellationToken);
            return Ok(admins.Select(u => u.ToV2Dto()));
        }

        /// <summary>
        /// Gets all partners that a user is an administrator of.
        /// </summary>
        /// <param name="userId">The user ID.</param>
        /// <param name="cancellationToken">Cancellation token.</param>
        /// <response code="200">Returns the partners the user administers.</response>
        /// <response code="401">Not authenticated.</response>
        [HttpGet("partners-for-user/{userId}")]
        [Authorize(Policy = AuthorizationPolicyConstants.ValidUser)]
        [ProducesResponseType(typeof(IEnumerable<PartnerDto>), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public async Task<IActionResult> GetPartnersForUser(Guid userId, CancellationToken cancellationToken)
        {
            logger.LogInformation("V2 GetPartnersForUser for User={UserId}", userId);

            var partners = await partnerAdminManager.GetPartnersByUserIdAsync(userId, cancellationToken);
            return Ok(partners.Select(p => p.ToV2Dto()));
        }

        /// <summary>
        /// Gets all partners that the current user is an administrator of.
        /// </summary>
        /// <param name="cancellationToken">Cancellation token.</param>
        /// <response code="200">Returns the partners the current user administers.</response>
        /// <response code="401">Not authenticated.</response>
        [HttpGet("my")]
        [Authorize(Policy = AuthorizationPolicyConstants.ValidUser)]
        [ProducesResponseType(typeof(IEnumerable<PartnerDto>), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public async Task<IActionResult> GetMyPartners(CancellationToken cancellationToken)
        {
            logger.LogInformation("V2 GetMyPartners for User={UserId}", UserId);

            var partners = await partnerAdminManager.GetPartnersByUserIdAsync(UserId, cancellationToken);
            return Ok(partners.Select(p => p.ToV2Dto()));
        }

        /// <summary>
        /// Gets a specific partner admin record.
        /// </summary>
        /// <param name="partnerId">The partner ID.</param>
        /// <param name="userId">The user ID.</param>
        /// <param name="cancellationToken">Cancellation token.</param>
        /// <response code="200">Returns the partner admin record.</response>
        /// <response code="403">Not authorized.</response>
        /// <response code="404">Partner or admin record not found.</response>
        [HttpGet("{partnerId}/{userId}")]
        [Authorize(Policy = AuthorizationPolicyConstants.ValidUser)]
        [ProducesResponseType(typeof(PartnerAdminDto), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetPartnerUser(Guid partnerId, Guid userId, CancellationToken cancellationToken)
        {
            logger.LogInformation("V2 GetPartnerUser for Partner={PartnerId}, User={UserId}", partnerId, userId);

            var partner = await partnerManager.GetAsync(partnerId, cancellationToken);
            if (partner is null)
            {
                return NotFound();
            }

            if (!await IsAuthorizedAsync(partner, AuthorizationPolicyConstants.UserIsPartnerUserOrIsAdmin))
            {
                return Forbid();
            }

            var admins = await partnerAdminManager.GetByParentIdAsync(partnerId, cancellationToken);
            var admin = admins.FirstOrDefault(a => a.UserId == userId);
            if (admin is null)
            {
                return NotFound();
            }

            return Ok(admin.ToV2Dto());
        }

        /// <summary>
        /// Adds a user as an administrator for a partner.
        /// </summary>
        /// <param name="partnerId">The partner ID.</param>
        /// <param name="userId">The user ID to add as admin.</param>
        /// <param name="cancellationToken">Cancellation token.</param>
        /// <response code="201">Partner admin created.</response>
        /// <response code="403">Not authorized.</response>
        /// <response code="404">Partner not found.</response>
        [HttpPost("{partnerId}/{userId}")]
        [Authorize(Policy = AuthorizationPolicyConstants.ValidUser)]
        [RequiredScope(Constants.TrashMobWriteScope)]
        [ProducesResponseType(typeof(PartnerAdminDto), StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
        public async Task<IActionResult> AddPartnerUser(Guid partnerId, Guid userId, CancellationToken cancellationToken)
        {
            logger.LogInformation("V2 AddPartnerUser for Partner={PartnerId}, User={UserId}", partnerId, userId);

            var partner = await partnerManager.GetAsync(partnerId, cancellationToken);
            if (partner is null)
            {
                return NotFound();
            }

            if (!await IsAuthorizedAsync(partner, AuthorizationPolicyConstants.UserIsPartnerUserOrIsAdmin))
            {
                return Forbid();
            }

            var entity = new PartnerAdmin
            {
                PartnerId = partnerId,
                UserId = userId,
            };

            var result = await partnerAdminManager.AddAsync(entity, UserId, cancellationToken);
            return StatusCode(StatusCodes.Status201Created, result.ToV2Dto());
        }

        /// <summary>
        /// Removes a user as an administrator for a partner.
        /// </summary>
        /// <param name="partnerId">The partner ID.</param>
        /// <param name="userId">The user ID to remove as admin.</param>
        /// <param name="cancellationToken">Cancellation token.</param>
        /// <response code="204">Partner admin removed.</response>
        /// <response code="403">Not authorized.</response>
        /// <response code="404">Partner not found.</response>
        [HttpDelete("{partnerId}/{userId}")]
        [Authorize(Policy = AuthorizationPolicyConstants.ValidUser)]
        [RequiredScope(Constants.TrashMobWriteScope)]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
        public async Task<IActionResult> RemovePartnerUser(Guid partnerId, Guid userId, CancellationToken cancellationToken)
        {
            logger.LogInformation("V2 RemovePartnerUser for Partner={PartnerId}, User={UserId}", partnerId, userId);

            var partner = await partnerManager.GetAsync(partnerId, cancellationToken);
            if (partner is null)
            {
                return NotFound();
            }

            if (!await IsAuthorizedAsync(partner, AuthorizationPolicyConstants.UserIsPartnerUserOrIsAdmin))
            {
                return Forbid();
            }

            await partnerAdminManager.Delete(partnerId, userId, cancellationToken);
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
