namespace TrashMob.Controllers.V2
{
    using System;
    using System.Threading;
    using System.Threading.Tasks;
    using Asp.Versioning;
    using Microsoft.AspNetCore.Cors;
    using Microsoft.AspNetCore.Http;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.Extensions.Logging;
    using TrashMob.Models;
    using TrashMob.Models.Extensions.V2;
    using TrashMob.Models.Poco.V2;
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
        ILogger<PartnersV2Controller> logger) : ControllerBase
    {
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
    }
}
