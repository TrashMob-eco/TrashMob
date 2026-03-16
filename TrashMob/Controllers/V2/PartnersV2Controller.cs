namespace TrashMob.Controllers.V2
{
    using System;
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
    using TrashMob.Shared.Extensions;
    using TrashMob.Shared.Managers.Interfaces;

    /// <summary>
    /// V2 controller for partners with server-side pagination and filtering.
    /// </summary>
    [ApiController]
    [ApiVersion("2.0")]
    [EnableCors("_myAllowSpecificOrigins")]
    [Route("api/v{version:apiVersion}/partners")]
    public class PartnersV2Controller(
        IPartnerManager partnerManager,
        IAuthorizationService authorizationService,
        ILogger<PartnersV2Controller> logger) : ControllerBase
    {
        private Guid UserId => Guid.TryParse(HttpContext.Items["UserId"]?.ToString(), out var parsedUserId) ? parsedUserId : Guid.Empty;

        /// <summary>
        /// Gets a paginated list of active partners with optional filtering.
        /// </summary>
        /// <param name="filter">Query parameters for pagination and filtering.</param>
        /// <param name="cancellationToken">Cancellation token.</param>
        /// <returns>A paginated list of partners.</returns>
        /// <response code="200">Returns the paginated partner list.</response>
        [HttpGet]
        [ProducesResponseType(typeof(PagedResponse<PartnerDto>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> GetPartners(
            [FromQuery] PartnerQueryParameters filter,
            CancellationToken cancellationToken)
        {
            logger.LogInformation("V2 GetPartners requested with Page={Page}, PageSize={PageSize}",
                filter.Page, filter.PageSize);

            var query = partnerManager.GetFilteredPartnersQueryable(filter);
            var result = await query.ToPagedAsync(filter, p => p.ToV2Dto(), cancellationToken);

            return Ok(result);
        }

        /// <summary>
        /// Gets a single partner by its identifier.
        /// </summary>
        /// <param name="id">The partner identifier.</param>
        /// <param name="cancellationToken">Cancellation token.</param>
        /// <returns>The partner details.</returns>
        /// <response code="200">Returns the partner.</response>
        /// <response code="404">Partner not found or inactive.</response>
        [HttpGet("{id}")]
        [ProducesResponseType(typeof(PartnerDto), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> GetPartner(Guid id, CancellationToken cancellationToken)
        {
            logger.LogInformation("V2 GetPartner requested for {PartnerId}", id);

            var partner = await partnerManager.GetAsync(id, cancellationToken);

            if (partner is null || partner.PartnerStatusId == (int)PartnerStatusEnum.Inactive)
            {
                return NotFound();
            }

            return Ok(partner.ToV2Dto());
        }

        /// <summary>
        /// Updates a partner. Requires the caller to be a partner admin or site admin.
        /// </summary>
        /// <param name="partnerDto">The updated partner data.</param>
        /// <param name="cancellationToken">Cancellation token.</param>
        /// <returns>The updated partner.</returns>
        /// <response code="200">Returns the updated partner.</response>
        /// <response code="403">Not authorized to update this partner.</response>
        [HttpPut]
        [Authorize(Policy = AuthorizationPolicyConstants.ValidUser)]
        [RequiredScope(Constants.TrashMobWriteScope)]
        [ProducesResponseType(typeof(PartnerDto), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> UpdatePartner(
            [FromBody] PartnerDto partnerDto,
            CancellationToken cancellationToken)
        {
            logger.LogInformation("V2 UpdatePartner for Partner={PartnerId}", partnerDto.Id);

            var entity = partnerDto.ToEntity();

            if (!await IsAuthorizedAsync(entity, AuthorizationPolicyConstants.UserIsPartnerUserOrIsAdmin))
            {
                return Forbid();
            }

            var result = await partnerManager.UpdateAsync(entity, UserId, cancellationToken);
            return Ok(result.ToV2Dto());
        }

        /// <summary>
        /// Deletes a partner by its identifier. Requires site admin authorization.
        /// </summary>
        /// <param name="id">The partner identifier.</param>
        /// <param name="cancellationToken">Cancellation token.</param>
        /// <response code="204">Partner deleted successfully.</response>
        /// <response code="403">Not authorized to delete this partner.</response>
        /// <response code="404">Partner not found.</response>
        [HttpDelete("{id}")]
        [Authorize(Policy = AuthorizationPolicyConstants.ValidUser)]
        [RequiredScope(Constants.TrashMobWriteScope)]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
        public async Task<IActionResult> DeletePartner(Guid id, CancellationToken cancellationToken)
        {
            logger.LogInformation("V2 DeletePartner for Partner={PartnerId}", id);

            var partner = await partnerManager.GetAsync(id, cancellationToken);

            if (partner is null)
            {
                return NotFound();
            }

            if (!await IsAuthorizedAsync(partner, AuthorizationPolicyConstants.UserIsPartnerUserOrIsAdmin))
            {
                return Forbid();
            }

            await partnerManager.DeleteAsync(id, cancellationToken);
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
