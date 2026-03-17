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
    using TrashMob.Shared.Poco;

    /// <summary>
    /// V2 controller for sponsor adoption reports — viewing adoptions and their cleanup logs.
    /// </summary>
    [ApiController]
    [ApiVersion("2.0")]
    [EnableCors("_myAllowSpecificOrigins")]
    [Route("api/v{version:apiVersion}/sponsors/{sponsorId}/adoptions")]
    public class SponsorReportsV2Controller(
        ISponsoredAdoptionManager adoptionManager,
        IProfessionalCleanupLogManager logManager,
        ISponsorManager sponsorManager,
        IKeyedManager<Partner> partnerManager,
        IAuthorizationService authorizationService,
        ILogger<SponsorReportsV2Controller> logger) : ControllerBase
    {
        private Guid UserId => Guid.TryParse(HttpContext.Items["UserId"]?.ToString(), out var parsedUserId) ? parsedUserId : Guid.Empty;

        /// <summary>
        /// Gets all sponsored adoptions for a sponsor.
        /// </summary>
        /// <param name="sponsorId">The sponsor ID.</param>
        /// <param name="cancellationToken">Cancellation token.</param>
        /// <response code="200">Returns the sponsored adoptions.</response>
        /// <response code="403">Not authorized.</response>
        /// <response code="404">Sponsor or partner not found.</response>
        [HttpGet]
        [Authorize(Policy = AuthorizationPolicyConstants.ValidUser)]
        [RequiredScope(Constants.TrashMobReadScope)]
        [ProducesResponseType(typeof(IEnumerable<SponsoredAdoptionDto>), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetAdoptions(Guid sponsorId, CancellationToken cancellationToken)
        {
            logger.LogInformation("V2 GetAdoptions for Sponsor={SponsorId}", sponsorId);

            var sponsor = await sponsorManager.GetAsync(sponsorId, cancellationToken);
            if (sponsor is null)
            {
                return NotFound();
            }

            var partner = await partnerManager.GetAsync(sponsor.PartnerId, cancellationToken);
            if (partner is null)
            {
                return NotFound();
            }

            if (!await IsAuthorizedAsync(partner, AuthorizationPolicyConstants.UserIsPartnerUserOrIsAdmin))
            {
                return Forbid();
            }

            var adoptions = await adoptionManager.GetBySponsorIdAsync(sponsorId, cancellationToken);
            return Ok(adoptions.Select(a => a.ToV2Dto()));
        }

        /// <summary>
        /// Gets cleanup log reports for a specific sponsored adoption.
        /// </summary>
        /// <param name="sponsorId">The sponsor ID.</param>
        /// <param name="adoptionId">The sponsored adoption ID.</param>
        /// <param name="cancellationToken">Cancellation token.</param>
        /// <response code="200">Returns the cleanup logs.</response>
        /// <response code="403">Not authorized.</response>
        /// <response code="404">Sponsor, partner, or adoption not found.</response>
        [HttpGet("{adoptionId}/reports")]
        [Authorize(Policy = AuthorizationPolicyConstants.ValidUser)]
        [RequiredScope(Constants.TrashMobReadScope)]
        [ProducesResponseType(typeof(IEnumerable<ProfessionalCleanupLogDto>), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetAdoptionReports(Guid sponsorId, Guid adoptionId, CancellationToken cancellationToken)
        {
            logger.LogInformation("V2 GetAdoptionReports for Sponsor={SponsorId}, Adoption={AdoptionId}", sponsorId, adoptionId);

            var sponsor = await sponsorManager.GetAsync(sponsorId, cancellationToken);
            if (sponsor is null)
            {
                return NotFound();
            }

            var partner = await partnerManager.GetAsync(sponsor.PartnerId, cancellationToken);
            if (partner is null)
            {
                return NotFound();
            }

            if (!await IsAuthorizedAsync(partner, AuthorizationPolicyConstants.UserIsPartnerUserOrIsAdmin))
            {
                return Forbid();
            }

            var adoption = await adoptionManager.GetAsync(adoptionId, cancellationToken);
            if (adoption is null || adoption.SponsorId != sponsorId)
            {
                return NotFound();
            }

            var logs = await logManager.GetBySponsoredAdoptionIdAsync(adoptionId, cancellationToken);
            return Ok(logs.Select(l => l.ToV2Dto()));
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
