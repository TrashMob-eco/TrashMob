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
    /// V2 controller for managing partner location services.
    /// </summary>
    [ApiController]
    [ApiVersion("2.0")]
    [EnableCors("_myAllowSpecificOrigins")]
    [Route("api/v{version:apiVersion}/partner-location-services")]
    public class PartnerLocationServicesV2Controller(
        IBaseManager<PartnerLocationService> partnerLocationServiceManager,
        IPartnerLocationManager partnerLocationManager,
        IKeyedManager<Partner> partnerManager,
        IAuthorizationService authorizationService,
        ILogger<PartnerLocationServicesV2Controller> logger) : ControllerBase
    {
        private Guid UserId => Guid.TryParse(HttpContext.Items["UserId"]?.ToString(), out var parsedUserId) ? parsedUserId : Guid.Empty;

        /// <summary>
        /// Gets all services for a partner location.
        /// </summary>
        /// <param name="partnerLocationId">The partner location ID.</param>
        /// <param name="cancellationToken">Cancellation token.</param>
        /// <response code="200">Returns the partner location services.</response>
        [HttpGet("by-location/{partnerLocationId}")]
        [Authorize(Policy = AuthorizationPolicyConstants.ValidUser)]
        [ProducesResponseType(typeof(IEnumerable<PartnerLocationServiceDto>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> GetByLocation(Guid partnerLocationId, CancellationToken cancellationToken)
        {
            logger.LogInformation("V2 GetPartnerLocationServices for Location={PartnerLocationId}", partnerLocationId);

            var results = await partnerLocationServiceManager.GetByParentIdAsync(partnerLocationId, cancellationToken);

            return Ok(results.Select(s => s.ToV2Dto()));
        }

        /// <summary>
        /// Gets a single partner location service by its composite key.
        /// </summary>
        /// <param name="partnerLocationId">The partner location ID.</param>
        /// <param name="serviceTypeId">The service type ID.</param>
        /// <param name="cancellationToken">Cancellation token.</param>
        /// <response code="200">Returns the partner location service.</response>
        /// <response code="404">Service not found.</response>
        [HttpGet("{partnerLocationId}/{serviceTypeId}")]
        [Authorize(Policy = AuthorizationPolicyConstants.ValidUser)]
        [ProducesResponseType(typeof(PartnerLocationServiceDto), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> Get(Guid partnerLocationId, int serviceTypeId, CancellationToken cancellationToken)
        {
            logger.LogInformation("V2 GetPartnerLocationService for Location={PartnerLocationId}, ServiceType={ServiceTypeId}",
                partnerLocationId, serviceTypeId);

            var service = await partnerLocationServiceManager.GetAsync(partnerLocationId, serviceTypeId, cancellationToken);

            if (service is null)
            {
                return NotFound();
            }

            return Ok(service.ToV2Dto());
        }

        /// <summary>
        /// Creates a new partner location service.
        /// </summary>
        /// <param name="dto">The partner location service to create.</param>
        /// <param name="cancellationToken">Cancellation token.</param>
        /// <response code="201">Service created.</response>
        /// <response code="403">Not authorized.</response>
        /// <response code="404">Partner location not found.</response>
        [HttpPost]
        [Authorize(Policy = AuthorizationPolicyConstants.ValidUser)]
        [RequiredScope(Constants.TrashMobWriteScope)]
        [ProducesResponseType(typeof(PartnerLocationServiceDto), StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> Add(PartnerLocationServiceDto dto, CancellationToken cancellationToken)
        {
            logger.LogInformation("V2 AddPartnerLocationService for Location={PartnerLocationId}, ServiceType={ServiceTypeId}",
                dto.PartnerLocationId, dto.ServiceTypeId);

            var partnerLocation = await partnerLocationManager.GetAsync(dto.PartnerLocationId, cancellationToken);

            if (partnerLocation is null)
            {
                return NotFound(new ProblemDetails
                {
                    Title = "Not found",
                    Detail = "Partner location not found.",
                    Status = StatusCodes.Status404NotFound,
                });
            }

            var partner = await partnerManager.GetAsync(partnerLocation.PartnerId, cancellationToken);

            if (!await IsAuthorizedAsync(partner, AuthorizationPolicyConstants.UserIsPartnerUserOrIsAdmin))
            {
                return Forbid();
            }

            var entity = dto.ToEntity();
            var result = await partnerLocationServiceManager.AddAsync(entity, UserId, cancellationToken);

            return CreatedAtAction(nameof(Get),
                new { partnerLocationId = result.PartnerLocationId, serviceTypeId = result.ServiceTypeId },
                result.ToV2Dto());
        }

        /// <summary>
        /// Updates an existing partner location service.
        /// </summary>
        /// <param name="dto">The partner location service to update.</param>
        /// <param name="cancellationToken">Cancellation token.</param>
        /// <response code="200">Service updated.</response>
        /// <response code="403">Not authorized.</response>
        [HttpPut]
        [Authorize(Policy = AuthorizationPolicyConstants.ValidUser)]
        [RequiredScope(Constants.TrashMobWriteScope)]
        [ProducesResponseType(typeof(PartnerLocationServiceDto), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> Update(PartnerLocationServiceDto dto, CancellationToken cancellationToken)
        {
            logger.LogInformation("V2 UpdatePartnerLocationService for Location={PartnerLocationId}, ServiceType={ServiceTypeId}",
                dto.PartnerLocationId, dto.ServiceTypeId);

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
            var result = await partnerLocationServiceManager.UpdateAsync(entity, UserId, cancellationToken);

            return Ok(result.ToV2Dto());
        }

        /// <summary>
        /// Deletes a partner location service.
        /// </summary>
        /// <param name="partnerLocationId">The partner location ID.</param>
        /// <param name="serviceTypeId">The service type ID.</param>
        /// <param name="cancellationToken">Cancellation token.</param>
        /// <response code="204">Service deleted.</response>
        /// <response code="403">Not authorized.</response>
        [HttpDelete("{partnerLocationId}/{serviceTypeId}")]
        [Authorize(Policy = AuthorizationPolicyConstants.ValidUser)]
        [RequiredScope(Constants.TrashMobWriteScope)]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> Delete(Guid partnerLocationId, int serviceTypeId, CancellationToken cancellationToken)
        {
            logger.LogInformation("V2 DeletePartnerLocationService for Location={PartnerLocationId}, ServiceType={ServiceTypeId}",
                partnerLocationId, serviceTypeId);

            var partner = await partnerLocationManager.GetPartnerForLocationAsync(partnerLocationId, cancellationToken);

            if (partner is null)
            {
                return NotFound();
            }

            if (!await IsAuthorizedAsync(partner, AuthorizationPolicyConstants.UserIsPartnerUserOrIsAdmin))
            {
                return Forbid();
            }

            await partnerLocationServiceManager.Delete(partnerLocationId, serviceTypeId, cancellationToken);

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
