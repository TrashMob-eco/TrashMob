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
    /// V2 controller for managing sponsored adoptions within a community.
    /// </summary>
    [ApiController]
    [ApiVersion("2.0")]
    [EnableCors("_myAllowSpecificOrigins")]
    [Route("api/v{version:apiVersion}/communities/{partnerId}/sponsored-adoptions")]
    public class CommunitySponsoredAdoptionsV2Controller(
        ISponsoredAdoptionManager adoptionManager,
        IKeyedManager<Partner> partnerManager,
        IAuthorizationService authorizationService,
        ILogger<CommunitySponsoredAdoptionsV2Controller> logger) : ControllerBase
    {
        private Guid UserId => new(HttpContext.Items["UserId"]?.ToString() ?? string.Empty);

        /// <summary>
        /// Gets all sponsored adoptions for a community.
        /// </summary>
        /// <param name="partnerId">The community (partner) ID.</param>
        /// <param name="cancellationToken">Cancellation token.</param>
        /// <response code="200">Returns the sponsored adoptions.</response>
        /// <response code="403">Not authorized.</response>
        /// <response code="404">Partner not found.</response>
        [HttpGet]
        [Authorize(Policy = AuthorizationPolicyConstants.ValidUser)]
        [RequiredScope(Constants.TrashMobReadScope)]
        [ProducesResponseType(typeof(IEnumerable<SponsoredAdoptionDto>), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetSponsoredAdoptions(Guid partnerId, CancellationToken cancellationToken)
        {
            logger.LogInformation("V2 GetSponsoredAdoptions for Partner={PartnerId}", partnerId);

            var partner = await partnerManager.GetAsync(partnerId, cancellationToken);
            if (partner is null)
            {
                return NotFound();
            }

            if (!await IsAuthorizedAsync(partner, AuthorizationPolicyConstants.UserIsPartnerUserOrIsAdmin))
            {
                return Forbid();
            }

            var adoptions = await adoptionManager.GetByCommunityAsync(partnerId, cancellationToken);
            return Ok(adoptions.Select(a => a.ToV2Dto()));
        }

        /// <summary>
        /// Gets a single sponsored adoption by ID.
        /// </summary>
        /// <param name="partnerId">The community (partner) ID.</param>
        /// <param name="id">The sponsored adoption ID.</param>
        /// <param name="cancellationToken">Cancellation token.</param>
        /// <response code="200">Returns the sponsored adoption.</response>
        /// <response code="403">Not authorized.</response>
        /// <response code="404">Partner or adoption not found.</response>
        [HttpGet("{id}")]
        [Authorize(Policy = AuthorizationPolicyConstants.ValidUser)]
        [RequiredScope(Constants.TrashMobReadScope)]
        [ProducesResponseType(typeof(SponsoredAdoptionDto), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetSponsoredAdoption(Guid partnerId, Guid id, CancellationToken cancellationToken)
        {
            logger.LogInformation("V2 GetSponsoredAdoption for Partner={PartnerId}, Id={AdoptionId}", partnerId, id);

            var partner = await partnerManager.GetAsync(partnerId, cancellationToken);
            if (partner is null)
            {
                return NotFound();
            }

            if (!await IsAuthorizedAsync(partner, AuthorizationPolicyConstants.UserIsPartnerUserOrIsAdmin))
            {
                return Forbid();
            }

            var adoption = await adoptionManager.GetAsync(id, cancellationToken);
            if (adoption is null)
            {
                return NotFound();
            }

            return Ok(adoption.ToV2Dto());
        }

        /// <summary>
        /// Creates a new sponsored adoption.
        /// </summary>
        /// <param name="partnerId">The community (partner) ID.</param>
        /// <param name="adoptionDto">The sponsored adoption to create.</param>
        /// <param name="cancellationToken">Cancellation token.</param>
        /// <response code="201">Sponsored adoption created.</response>
        /// <response code="400">Invalid data.</response>
        /// <response code="403">Not authorized.</response>
        /// <response code="404">Partner not found.</response>
        [HttpPost]
        [Authorize(Policy = AuthorizationPolicyConstants.ValidUser)]
        [RequiredScope(Constants.TrashMobWriteScope)]
        [ProducesResponseType(typeof(SponsoredAdoptionDto), StatusCodes.Status201Created)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
        public async Task<IActionResult> CreateSponsoredAdoption(
            Guid partnerId,
            [FromBody] SponsoredAdoptionDto adoptionDto,
            CancellationToken cancellationToken)
        {
            logger.LogInformation("V2 CreateSponsoredAdoption for Partner={PartnerId}", partnerId);

            var partner = await partnerManager.GetAsync(partnerId, cancellationToken);
            if (partner is null)
            {
                return NotFound();
            }

            if (!await IsAuthorizedAsync(partner, AuthorizationPolicyConstants.UserIsPartnerUserOrIsAdmin))
            {
                return Forbid();
            }

            var entity = adoptionDto.ToEntity();
            entity.CreatedByUserId = UserId;
            entity.LastUpdatedByUserId = UserId;

            var created = await adoptionManager.AddAsync(entity, UserId, cancellationToken);
            return CreatedAtAction(nameof(GetSponsoredAdoption), new { partnerId, id = created.Id }, created.ToV2Dto());
        }

        /// <summary>
        /// Updates an existing sponsored adoption.
        /// </summary>
        /// <param name="partnerId">The community (partner) ID.</param>
        /// <param name="id">The sponsored adoption ID.</param>
        /// <param name="adoptionDto">The updated sponsored adoption data.</param>
        /// <param name="cancellationToken">Cancellation token.</param>
        /// <response code="200">Returns the updated sponsored adoption.</response>
        /// <response code="400">Invalid data.</response>
        /// <response code="403">Not authorized.</response>
        /// <response code="404">Partner or adoption not found.</response>
        [HttpPut("{id}")]
        [Authorize(Policy = AuthorizationPolicyConstants.ValidUser)]
        [RequiredScope(Constants.TrashMobWriteScope)]
        [ProducesResponseType(typeof(SponsoredAdoptionDto), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
        public async Task<IActionResult> UpdateSponsoredAdoption(
            Guid partnerId,
            Guid id,
            [FromBody] SponsoredAdoptionDto adoptionDto,
            CancellationToken cancellationToken)
        {
            logger.LogInformation("V2 UpdateSponsoredAdoption for Partner={PartnerId}, Id={AdoptionId}", partnerId, id);

            var partner = await partnerManager.GetAsync(partnerId, cancellationToken);
            if (partner is null)
            {
                return NotFound();
            }

            if (!await IsAuthorizedAsync(partner, AuthorizationPolicyConstants.UserIsPartnerUserOrIsAdmin))
            {
                return Forbid();
            }

            var existing = await adoptionManager.GetAsync(id, cancellationToken);
            if (existing is null)
            {
                return NotFound();
            }

            var entity = adoptionDto.ToEntity();
            entity.Id = id;
            entity.LastUpdatedByUserId = UserId;

            var updated = await adoptionManager.UpdateAsync(entity, UserId, cancellationToken);
            return Ok(updated.ToV2Dto());
        }

        /// <summary>
        /// Gets compliance statistics for a community's sponsored adoption program.
        /// </summary>
        /// <param name="partnerId">The community (partner) ID.</param>
        /// <param name="cancellationToken">Cancellation token.</param>
        /// <response code="200">Returns the compliance statistics.</response>
        /// <response code="403">Not authorized.</response>
        /// <response code="404">Partner not found.</response>
        [HttpGet("compliance")]
        [Authorize(Policy = AuthorizationPolicyConstants.ValidUser)]
        [RequiredScope(Constants.TrashMobReadScope)]
        [ProducesResponseType(typeof(SponsoredAdoptionComplianceStats), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetComplianceStats(Guid partnerId, CancellationToken cancellationToken)
        {
            logger.LogInformation("V2 GetComplianceStats for Partner={PartnerId}", partnerId);

            var partner = await partnerManager.GetAsync(partnerId, cancellationToken);
            if (partner is null)
            {
                return NotFound();
            }

            if (!await IsAuthorizedAsync(partner, AuthorizationPolicyConstants.UserIsPartnerUserOrIsAdmin))
            {
                return Forbid();
            }

            var stats = await adoptionManager.GetComplianceByCommunityAsync(partnerId, cancellationToken);
            return Ok(stats);
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
