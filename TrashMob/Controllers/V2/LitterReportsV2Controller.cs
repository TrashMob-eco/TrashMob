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
    using TrashMob.Models.Extensions.V2;
    using TrashMob.Models.Poco.V2;
    using TrashMob.Shared.Extensions;
    using TrashMob.Shared.Managers.Interfaces;

    /// <summary>
    /// V2 controller for litter reports with server-side pagination and filtering.
    /// </summary>
    [ApiController]
    [ApiVersion("2.0")]
    [EnableCors("_myAllowSpecificOrigins")]
    [Route("api/v{version:apiVersion}/litterreports")]
    public class LitterReportsV2Controller(
        ILitterReportManager litterReportManager,
        ILogger<LitterReportsV2Controller> logger) : ControllerBase
    {
        /// <summary>
        /// Gets a paginated list of litter reports with optional filtering.
        /// </summary>
        /// <param name="filter">Query parameters for pagination and filtering.</param>
        /// <param name="cancellationToken">Cancellation token.</param>
        /// <returns>A paginated list of litter reports with images.</returns>
        /// <response code="200">Returns the paginated litter report list.</response>
        [HttpGet]
        [ProducesResponseType(typeof(PagedResponse<LitterReportDto>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> GetLitterReports(
            [FromQuery] LitterReportQueryParameters filter,
            CancellationToken cancellationToken)
        {
            logger.LogInformation("V2 GetLitterReports requested with Page={Page}, PageSize={PageSize}",
                filter.Page, filter.PageSize);

            var query = litterReportManager.GetFilteredLitterReportsQueryable(filter);
            var result = await query.ToPagedAsync(filter, lr => lr.ToV2Dto(), cancellationToken);

            return Ok(result);
        }

        /// <summary>
        /// Gets a single litter report by its identifier, including images.
        /// </summary>
        /// <param name="id">The litter report identifier.</param>
        /// <param name="cancellationToken">Cancellation token.</param>
        /// <returns>The litter report details with images.</returns>
        /// <response code="200">Returns the litter report.</response>
        /// <response code="404">Litter report not found.</response>
        [HttpGet("{id}")]
        [ProducesResponseType(typeof(LitterReportDto), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> GetLitterReport(Guid id, CancellationToken cancellationToken)
        {
            logger.LogInformation("V2 GetLitterReport requested for {LitterReportId}", id);

            var litterReport = await litterReportManager.GetAsync(id, cancellationToken);

            if (litterReport is null)
            {
                return NotFound();
            }

            return Ok(litterReport.ToV2Dto());
        }
    }
}
