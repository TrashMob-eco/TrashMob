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
    /// V2 controller for managing sponsors within a community's sponsored adoption program.
    /// </summary>
    [ApiController]
    [ApiVersion("2.0")]
    [EnableCors("_myAllowSpecificOrigins")]
    [Route("api/v{version:apiVersion}/communities/{partnerId}/sponsors")]
    public class CommunitySponsorsV2Controller(
        ISponsorManager sponsorManager,
        IKeyedManager<Partner> partnerManager,
        IImageManager imageManager,
        IAuthorizationService authorizationService,
        ILogger<CommunitySponsorsV2Controller> logger) : ControllerBase
    {
        private Guid UserId => new(HttpContext.Items["UserId"]?.ToString() ?? string.Empty);

        /// <summary>
        /// Gets all active sponsors for a community.
        /// </summary>
        /// <param name="partnerId">The community (partner) ID.</param>
        /// <param name="cancellationToken">Cancellation token.</param>
        /// <response code="200">Returns the sponsors.</response>
        /// <response code="403">Not authorized.</response>
        /// <response code="404">Partner not found.</response>
        [HttpGet]
        [Authorize(Policy = AuthorizationPolicyConstants.ValidUser)]
        [RequiredScope(Constants.TrashMobReadScope)]
        [ProducesResponseType(typeof(IEnumerable<SponsorDto>), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetSponsors(Guid partnerId, CancellationToken cancellationToken)
        {
            logger.LogInformation("V2 GetSponsors for Partner={PartnerId}", partnerId);

            var partner = await partnerManager.GetAsync(partnerId, cancellationToken);
            if (partner is null)
            {
                return NotFound();
            }

            if (!await IsAuthorizedAsync(partner, AuthorizationPolicyConstants.UserIsPartnerUserOrIsAdmin))
            {
                return Forbid();
            }

            var sponsors = await sponsorManager.GetByCommunityAsync(partnerId, cancellationToken);
            return Ok(sponsors.Select(s => s.ToV2Dto()));
        }

        /// <summary>
        /// Gets a single sponsor by ID.
        /// </summary>
        /// <param name="partnerId">The community (partner) ID.</param>
        /// <param name="sponsorId">The sponsor ID.</param>
        /// <param name="cancellationToken">Cancellation token.</param>
        /// <response code="200">Returns the sponsor.</response>
        /// <response code="403">Not authorized.</response>
        /// <response code="404">Partner or sponsor not found.</response>
        [HttpGet("{sponsorId}")]
        [Authorize(Policy = AuthorizationPolicyConstants.ValidUser)]
        [RequiredScope(Constants.TrashMobReadScope)]
        [ProducesResponseType(typeof(SponsorDto), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetSponsor(Guid partnerId, Guid sponsorId, CancellationToken cancellationToken)
        {
            logger.LogInformation("V2 GetSponsor for Partner={PartnerId}, Sponsor={SponsorId}", partnerId, sponsorId);

            var partner = await partnerManager.GetAsync(partnerId, cancellationToken);
            if (partner is null)
            {
                return NotFound();
            }

            if (!await IsAuthorizedAsync(partner, AuthorizationPolicyConstants.UserIsPartnerUserOrIsAdmin))
            {
                return Forbid();
            }

            var sponsor = await sponsorManager.GetAsync(sponsorId, cancellationToken);
            if (sponsor is null || sponsor.PartnerId != partnerId)
            {
                return NotFound();
            }

            return Ok(sponsor.ToV2Dto());
        }

        /// <summary>
        /// Creates a new sponsor for a community.
        /// </summary>
        /// <param name="partnerId">The community (partner) ID.</param>
        /// <param name="sponsorDto">The sponsor to create.</param>
        /// <param name="cancellationToken">Cancellation token.</param>
        /// <response code="201">Sponsor created.</response>
        /// <response code="400">Invalid data.</response>
        /// <response code="403">Not authorized.</response>
        /// <response code="404">Partner not found.</response>
        [HttpPost]
        [Authorize(Policy = AuthorizationPolicyConstants.ValidUser)]
        [RequiredScope(Constants.TrashMobWriteScope)]
        [ProducesResponseType(typeof(SponsorDto), StatusCodes.Status201Created)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
        public async Task<IActionResult> CreateSponsor(
            Guid partnerId,
            [FromBody] SponsorDto sponsorDto,
            CancellationToken cancellationToken)
        {
            logger.LogInformation("V2 CreateSponsor for Partner={PartnerId}", partnerId);

            var partner = await partnerManager.GetAsync(partnerId, cancellationToken);
            if (partner is null)
            {
                return NotFound();
            }

            if (!await IsAuthorizedAsync(partner, AuthorizationPolicyConstants.UserIsPartnerUserOrIsAdmin))
            {
                return Forbid();
            }

            var entity = sponsorDto.ToEntity();
            entity.PartnerId = partnerId;
            entity.CreatedByUserId = UserId;
            entity.LastUpdatedByUserId = UserId;

            var created = await sponsorManager.AddAsync(entity, UserId, cancellationToken);
            return CreatedAtAction(nameof(GetSponsor), new { partnerId, sponsorId = created.Id }, created.ToV2Dto());
        }

        /// <summary>
        /// Updates an existing sponsor.
        /// </summary>
        /// <param name="partnerId">The community (partner) ID.</param>
        /// <param name="sponsorId">The sponsor ID.</param>
        /// <param name="sponsorDto">The updated sponsor data.</param>
        /// <param name="cancellationToken">Cancellation token.</param>
        /// <response code="200">Returns the updated sponsor.</response>
        /// <response code="400">Invalid data.</response>
        /// <response code="403">Not authorized.</response>
        /// <response code="404">Partner or sponsor not found.</response>
        [HttpPut("{sponsorId}")]
        [Authorize(Policy = AuthorizationPolicyConstants.ValidUser)]
        [RequiredScope(Constants.TrashMobWriteScope)]
        [ProducesResponseType(typeof(SponsorDto), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
        public async Task<IActionResult> UpdateSponsor(
            Guid partnerId,
            Guid sponsorId,
            [FromBody] SponsorDto sponsorDto,
            CancellationToken cancellationToken)
        {
            logger.LogInformation("V2 UpdateSponsor for Partner={PartnerId}, Sponsor={SponsorId}", partnerId, sponsorId);

            var partner = await partnerManager.GetAsync(partnerId, cancellationToken);
            if (partner is null)
            {
                return NotFound();
            }

            if (!await IsAuthorizedAsync(partner, AuthorizationPolicyConstants.UserIsPartnerUserOrIsAdmin))
            {
                return Forbid();
            }

            var existing = await sponsorManager.GetAsync(sponsorId, cancellationToken);
            if (existing is null || existing.PartnerId != partnerId)
            {
                return NotFound();
            }

            var entity = sponsorDto.ToEntity();
            entity.Id = sponsorId;
            entity.PartnerId = partnerId;
            entity.LastUpdatedByUserId = UserId;

            var updated = await sponsorManager.UpdateAsync(entity, UserId, cancellationToken);
            return Ok(updated.ToV2Dto());
        }

        /// <summary>
        /// Deactivates a sponsor (soft delete).
        /// </summary>
        /// <param name="partnerId">The community (partner) ID.</param>
        /// <param name="sponsorId">The sponsor ID.</param>
        /// <param name="cancellationToken">Cancellation token.</param>
        /// <response code="204">Sponsor deactivated.</response>
        /// <response code="403">Not authorized.</response>
        /// <response code="404">Partner or sponsor not found.</response>
        [HttpDelete("{sponsorId}")]
        [Authorize(Policy = AuthorizationPolicyConstants.ValidUser)]
        [RequiredScope(Constants.TrashMobWriteScope)]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
        public async Task<IActionResult> DeactivateSponsor(Guid partnerId, Guid sponsorId, CancellationToken cancellationToken)
        {
            logger.LogInformation("V2 DeactivateSponsor for Partner={PartnerId}, Sponsor={SponsorId}", partnerId, sponsorId);

            var partner = await partnerManager.GetAsync(partnerId, cancellationToken);
            if (partner is null)
            {
                return NotFound();
            }

            if (!await IsAuthorizedAsync(partner, AuthorizationPolicyConstants.UserIsPartnerUserOrIsAdmin))
            {
                return Forbid();
            }

            var existing = await sponsorManager.GetAsync(sponsorId, cancellationToken);
            if (existing is null || existing.PartnerId != partnerId)
            {
                return NotFound();
            }

            existing.IsActive = false;
            existing.LastUpdatedByUserId = UserId;
            await sponsorManager.UpdateAsync(existing, UserId, cancellationToken);

            return NoContent();
        }

        /// <summary>
        /// Uploads a sponsor logo image (resized to 200x200).
        /// </summary>
        /// <param name="partnerId">The community (partner) ID.</param>
        /// <param name="sponsorId">The sponsor ID.</param>
        /// <param name="imageUpload">The image file.</param>
        /// <param name="cancellationToken">Cancellation token.</param>
        /// <response code="200">Returns the uploaded logo URL.</response>
        /// <response code="403">Not authorized.</response>
        /// <response code="404">Partner or sponsor not found.</response>
        [HttpPost("{sponsorId}/logo")]
        [Authorize(Policy = AuthorizationPolicyConstants.ValidUser)]
        [RequiredScope(Constants.TrashMobWriteScope)]
        [ProducesResponseType(typeof(object), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
        public async Task<IActionResult> UploadSponsorLogo(
            Guid partnerId,
            Guid sponsorId,
            [FromForm] ImageUpload imageUpload,
            CancellationToken cancellationToken)
        {
            logger.LogInformation("V2 UploadSponsorLogo for Partner={PartnerId}, Sponsor={SponsorId}", partnerId, sponsorId);

            var partner = await partnerManager.GetAsync(partnerId, cancellationToken);
            if (partner is null)
            {
                return NotFound();
            }

            if (!await IsAuthorizedAsync(partner, AuthorizationPolicyConstants.UserIsPartnerUserOrIsAdmin))
            {
                return Forbid();
            }

            var sponsor = await sponsorManager.GetAsync(sponsorId, cancellationToken);
            if (sponsor is null || sponsor.PartnerId != partnerId)
            {
                return NotFound();
            }

            imageUpload.ParentId = sponsorId;
            imageUpload.ImageType = ImageTypeEnum.SponsorLogo;
            var url = await imageManager.UploadImageWithSizeAsync(imageUpload, 200, 200);

            sponsor.LogoUrl = url;
            sponsor.LastUpdatedByUserId = UserId;
            await sponsorManager.UpdateAsync(sponsor, UserId, cancellationToken);

            return Ok(new { url });
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
