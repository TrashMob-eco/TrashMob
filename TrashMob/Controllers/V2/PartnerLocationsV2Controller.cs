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
    /// V2 controller for managing partner locations.
    /// </summary>
    [ApiController]
    [ApiVersion("2.0")]
    [EnableCors("_myAllowSpecificOrigins")]
    [Route("api/v{version:apiVersion}/partner-locations")]
    public class PartnerLocationsV2Controller(
        IPartnerLocationManager partnerLocationManager,
        IKeyedManager<Partner> partnerManager,
        IAuthorizationService authorizationService,
        ILogger<PartnerLocationsV2Controller> logger) : ControllerBase
    {
        private Guid UserId => Guid.TryParse(HttpContext.Items["UserId"]?.ToString(), out var parsedUserId) ? parsedUserId : Guid.Empty;

        /// <summary>
        /// Gets all locations for a partner.
        /// </summary>
        /// <param name="partnerId">The partner ID.</param>
        /// <param name="cancellationToken">Cancellation token.</param>
        /// <response code="200">Returns the partner locations.</response>
        [HttpGet("by-partner/{partnerId}")]
        [Authorize(Policy = AuthorizationPolicyConstants.ValidUser)]
        [ProducesResponseType(typeof(IEnumerable<PartnerLocationDto>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> GetByPartner(Guid partnerId, CancellationToken cancellationToken)
        {
            logger.LogInformation("V2 GetPartnerLocationsByPartner for Partner={PartnerId}", partnerId);

            var results = await partnerLocationManager.GetByParentIdAsync(partnerId, cancellationToken);

            return Ok(results.Select(pl => pl.ToV2Dto()));
        }

        /// <summary>
        /// Gets a single partner location by its identifier.
        /// </summary>
        /// <param name="id">The partner location ID.</param>
        /// <param name="cancellationToken">Cancellation token.</param>
        /// <response code="200">Returns the partner location.</response>
        /// <response code="404">Partner location not found.</response>
        [HttpGet("{id}")]
        [Authorize(Policy = AuthorizationPolicyConstants.ValidUser)]
        [ProducesResponseType(typeof(PartnerLocationDto), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> Get(Guid id, CancellationToken cancellationToken)
        {
            logger.LogInformation("V2 GetPartnerLocation for Id={PartnerLocationId}", id);

            var partnerLocation = await partnerLocationManager.GetAsync(id, cancellationToken);

            if (partnerLocation is null)
            {
                return NotFound();
            }

            return Ok(partnerLocation.ToV2Dto());
        }

        /// <summary>
        /// Creates a new partner location.
        /// </summary>
        /// <param name="dto">The partner location to create.</param>
        /// <param name="cancellationToken">Cancellation token.</param>
        /// <response code="201">Partner location created.</response>
        /// <response code="400">Invalid input.</response>
        /// <response code="403">Not authorized.</response>
        [HttpPost]
        [Authorize(Policy = AuthorizationPolicyConstants.ValidUser)]
        [RequiredScope(Constants.TrashMobWriteScope)]
        [ProducesResponseType(typeof(PartnerLocationDto), StatusCodes.Status201Created)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> Add(PartnerLocationDto dto, CancellationToken cancellationToken)
        {
            logger.LogInformation("V2 AddPartnerLocation for Partner={PartnerId}", dto.PartnerId);

            if (dto.PartnerId == Guid.Empty)
            {
                return BadRequest(new ProblemDetails
                {
                    Title = "Invalid request",
                    Detail = "PartnerId is required.",
                    Status = StatusCodes.Status400BadRequest,
                });
            }

            var partner = await partnerManager.GetAsync(dto.PartnerId, cancellationToken);

            if (partner is null)
            {
                return NotFound(new ProblemDetails
                {
                    Title = "Not found",
                    Detail = "Partner not found.",
                    Status = StatusCodes.Status404NotFound,
                });
            }

            if (!await IsAuthorizedAsync(partner, AuthorizationPolicyConstants.UserIsPartnerUserOrIsAdmin))
            {
                return Forbid();
            }

            var entity = dto.ToEntity();
            var result = await partnerLocationManager.AddAsync(entity, UserId, cancellationToken);

            return CreatedAtAction(nameof(Get), new { id = result.Id }, result.ToV2Dto());
        }

        /// <summary>
        /// Updates an existing partner location.
        /// </summary>
        /// <param name="dto">The partner location to update.</param>
        /// <param name="cancellationToken">Cancellation token.</param>
        /// <response code="200">Partner location updated.</response>
        /// <response code="403">Not authorized.</response>
        [HttpPut]
        [Authorize(Policy = AuthorizationPolicyConstants.ValidUser)]
        [RequiredScope(Constants.TrashMobWriteScope)]
        [ProducesResponseType(typeof(PartnerLocationDto), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> Update(PartnerLocationDto dto, CancellationToken cancellationToken)
        {
            logger.LogInformation("V2 UpdatePartnerLocation for Id={PartnerLocationId}", dto.Id);

            var partner = await partnerLocationManager.GetPartnerForLocationAsync(dto.Id, cancellationToken);

            if (partner is null)
            {
                return NotFound();
            }

            if (!await IsAuthorizedAsync(partner, AuthorizationPolicyConstants.UserIsPartnerUserOrIsAdmin))
            {
                return Forbid();
            }

            var entity = dto.ToEntity();
            var result = await partnerLocationManager.UpdateAsync(entity, UserId, cancellationToken);

            return Ok(result.ToV2Dto());
        }

        /// <summary>
        /// Deletes a partner location.
        /// </summary>
        /// <param name="id">The partner location ID.</param>
        /// <param name="cancellationToken">Cancellation token.</param>
        /// <response code="204">Partner location deleted.</response>
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
            logger.LogInformation("V2 DeletePartnerLocation for Id={PartnerLocationId}", id);

            var partner = await partnerLocationManager.GetPartnerForLocationAsync(id, cancellationToken);

            if (partner is null)
            {
                return NotFound();
            }

            if (!await IsAuthorizedAsync(partner, AuthorizationPolicyConstants.UserIsPartnerUserOrIsAdmin))
            {
                return Forbid();
            }

            await partnerLocationManager.DeleteAsync(id, cancellationToken);

            return NoContent();
        }

        /// <summary>
        /// Finds partners with locations near a specified point.
        /// </summary>
        /// <param name="latitude">The latitude of the search center.</param>
        /// <param name="longitude">The longitude of the search center.</param>
        /// <param name="radiusMiles">The search radius in miles (default: 25).</param>
        /// <param name="cancellationToken">Cancellation token.</param>
        /// <response code="200">Returns the nearby partners.</response>
        /// <response code="400">Invalid coordinates or radius.</response>
        [AllowAnonymous]
        [HttpGet("nearby")]
        [ProducesResponseType(typeof(IEnumerable<PartnerDto>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> GetNearby(
            [FromQuery] double latitude,
            [FromQuery] double longitude,
            [FromQuery] double radiusMiles = 25,
            CancellationToken cancellationToken = default)
        {
            logger.LogInformation("V2 GetNearbyPartners at Lat={Latitude}, Lon={Longitude}, Radius={RadiusMiles}",
                latitude, longitude, radiusMiles);

            if (latitude < -90 || latitude > 90)
            {
                return BadRequest(new ProblemDetails
                {
                    Title = "Invalid request",
                    Detail = "Latitude must be between -90 and 90.",
                    Status = StatusCodes.Status400BadRequest,
                });
            }

            if (longitude < -180 || longitude > 180)
            {
                return BadRequest(new ProblemDetails
                {
                    Title = "Invalid request",
                    Detail = "Longitude must be between -180 and 180.",
                    Status = StatusCodes.Status400BadRequest,
                });
            }

            if (radiusMiles <= 0 || radiusMiles > 500)
            {
                return BadRequest(new ProblemDetails
                {
                    Title = "Invalid request",
                    Detail = "Radius must be between 0 and 500 miles.",
                    Status = StatusCodes.Status400BadRequest,
                });
            }

            var partners = await partnerLocationManager.GetNearbyPartnersAsync(
                latitude, longitude, radiusMiles, cancellationToken);

            return Ok(partners.Select(p => p.ToV2Dto()));
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
