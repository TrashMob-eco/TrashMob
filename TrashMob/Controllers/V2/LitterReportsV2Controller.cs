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
    using TrashMob.Models.Poco;
    using TrashMob.Models.Poco.V2;
    using TrashMob.Security;
    using TrashMob.Shared;
    using TrashMob.Shared.Extensions;
    using TrashMob.Shared.Managers.Interfaces;
    using TrashMob.Shared.Poco;

    /// <summary>
    /// V2 controller for litter reports with server-side pagination and filtering.
    /// </summary>
    [ApiController]
    [ApiVersion("2.0")]
    [EnableCors("_myAllowSpecificOrigins")]
    [Route("api/v{version:apiVersion}/litterreports")]
    public class LitterReportsV2Controller(
        ILitterReportManager litterReportManager,
        ILitterImageManager litterImageManager,
        IImageManager imageManager,
        IAuthorizationService authorizationService,
        ILogger<LitterReportsV2Controller> logger) : ControllerBase
    {
        private Guid UserId => new(HttpContext.Items["UserId"]?.ToString() ?? string.Empty);

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

        /// <summary>
        /// Adds a new litter report.
        /// </summary>
        /// <param name="litterReport">The litter report to add.</param>
        /// <param name="cancellationToken">Cancellation token.</param>
        /// <response code="200">Returns the created litter report.</response>
        /// <response code="400">Invalid litter report data.</response>
        [HttpPost]
        [Authorize(Policy = AuthorizationPolicyConstants.ValidUser)]
        [RequiredScope(Constants.TrashMobWriteScope)]
        [ProducesResponseType(typeof(LitterReport), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> AddLitterReport(LitterReport litterReport, CancellationToken cancellationToken)
        {
            logger.LogInformation("V2 AddLitterReport Name={Name}", litterReport.Name);

            var result = await litterReportManager.AddWithResultAsync(litterReport, UserId, cancellationToken);

            if (result.IsSuccess)
            {
                return Ok(result.Data);
            }

            return BadRequest(result.ErrorMessage);
        }

        /// <summary>
        /// Updates an existing litter report. Only the owner or admin can update.
        /// </summary>
        /// <param name="litterReport">The litter report to update.</param>
        /// <param name="cancellationToken">Cancellation token.</param>
        /// <response code="200">Returns the updated litter report.</response>
        /// <response code="403">User is not authorized.</response>
        [HttpPut]
        [Authorize(Policy = AuthorizationPolicyConstants.ValidUser)]
        [RequiredScope(Constants.TrashMobWriteScope)]
        [ProducesResponseType(typeof(LitterReport), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> UpdateLitterReport(LitterReport litterReport,
            CancellationToken cancellationToken)
        {
            logger.LogInformation("V2 UpdateLitterReport Id={Id}", litterReport.Id);

            if (!await IsAuthorizedAsync(litterReport, AuthorizationPolicyConstants.UserOwnsEntityOrIsAdmin))
            {
                return Forbid();
            }

            var updatedLitterReport = await litterReportManager.UpdateAsync(litterReport, UserId, cancellationToken);

            if (updatedLitterReport is not null)
            {
                return Ok(updatedLitterReport);
            }

            return BadRequest("Failed to update litter report");
        }

        /// <summary>
        /// Deletes a litter report. Only the owner or admin can delete.
        /// </summary>
        /// <param name="id">The litter report ID.</param>
        /// <param name="cancellationToken">Cancellation token.</param>
        /// <response code="204">Litter report deleted.</response>
        /// <response code="403">User is not authorized.</response>
        [HttpDelete("{id}")]
        [Authorize(Policy = AuthorizationPolicyConstants.ValidUser)]
        [RequiredScope(Constants.TrashMobWriteScope)]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> DeleteLitterReport(Guid id, CancellationToken cancellationToken)
        {
            logger.LogInformation("V2 DeleteLitterReport Id={Id}", id);

            var litterReport = await litterReportManager.GetAsync(id, cancellationToken);

            if (!await IsAuthorizedAsync(litterReport, AuthorizationPolicyConstants.UserOwnsEntityOrIsAdmin))
            {
                return Forbid();
            }

            var result = await litterReportManager.DeleteAsync(id, UserId, cancellationToken);

            if (result != -1)
            {
                return NoContent();
            }

            return BadRequest("Could not find the litter report, delete failed");
        }

        /// <summary>
        /// Gets all litter reports for a specific user.
        /// </summary>
        /// <param name="userId">The user ID.</param>
        /// <param name="cancellationToken">Cancellation token.</param>
        /// <response code="200">Returns the user's litter reports.</response>
        [HttpGet("userlitterreports/{userId}")]
        [Authorize(Policy = AuthorizationPolicyConstants.ValidUser)]
        [RequiredScope(Constants.TrashMobReadScope)]
        [ProducesResponseType(typeof(IEnumerable<LitterReport>), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetUserLitterReports(Guid userId, CancellationToken cancellationToken)
        {
            logger.LogInformation("V2 GetUserLitterReports User={UserId}", userId);

            var result = await litterReportManager.GetUserLitterReportsAsync(userId, cancellationToken);
            return Ok(result);
        }

        /// <summary>
        /// Gets a paginated list of filtered litter reports.
        /// </summary>
        /// <param name="filter">The litter report filter.</param>
        /// <param name="cancellationToken">Cancellation token.</param>
        /// <response code="200">Returns a paginated list of filtered litter reports.</response>
        [HttpPost("pagedfilteredlitterreports")]
        [ProducesResponseType(typeof(PaginatedList<LitterReport>), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetPagedFilteredLitterReports(
            [FromBody] LitterReportFilter filter,
            CancellationToken cancellationToken)
        {
            logger.LogInformation("V2 GetPagedFilteredLitterReports");

            var result = await litterReportManager.GetFilteredLitterReportsAsync(filter, cancellationToken);

            if (filter.PageSize is not null)
            {
                var pagedResults = PaginatedList<LitterReport>.Create(
                    result.OrderByDescending(e => e.CreatedDate).AsQueryable(),
                    filter.PageIndex.GetValueOrDefault(0),
                    filter.PageSize.GetValueOrDefault(10));
                return Ok(pagedResults);
            }

            return Ok(result);
        }

        /// <summary>
        /// Gets litter locations within a specific time range.
        /// </summary>
        /// <param name="startTime">The start time.</param>
        /// <param name="endTime">The end time.</param>
        /// <param name="cancellationToken">Cancellation token.</param>
        /// <response code="200">Returns litter locations in the time range.</response>
        [HttpGet("locationsbytimerange")]
        [ProducesResponseType(typeof(IEnumerable<Location>), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetLitterLocationsByTimeRange(
            [FromQuery] DateTimeOffset? startTime,
            [FromQuery] DateTimeOffset? endTime,
            CancellationToken cancellationToken)
        {
            logger.LogInformation("V2 GetLitterLocationsByTimeRange Start={Start}, End={End}", startTime, endTime);

            var result = await litterReportManager
                .GeLitterLocationsByTimeRangeAsync(startTime, endTime, cancellationToken);
            return Ok(result);
        }

        /// <summary>
        /// Uploads an image for a litter report.
        /// </summary>
        /// <param name="imageUpload">The image upload data.</param>
        /// <param name="litterImageId">The litter image ID.</param>
        /// <param name="cancellationToken">Cancellation token.</param>
        /// <response code="200">Image uploaded.</response>
        /// <response code="403">User is not authorized.</response>
        [HttpPost("image/{litterImageId}")]
        [Authorize(Policy = AuthorizationPolicyConstants.ValidUser)]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        public async Task<IActionResult> UploadImage(
            [FromForm] ImageUpload imageUpload,
            Guid litterImageId,
            CancellationToken cancellationToken)
        {
            logger.LogInformation("V2 UploadLitterImage LitterImageId={LitterImageId}", litterImageId);

            var litterImage = await litterImageManager.GetAsync(litterImageId, cancellationToken);

            if (!await IsAuthorizedAsync(litterImage, AuthorizationPolicyConstants.UserOwnsEntity))
            {
                return Forbid();
            }

            await imageManager.UploadImageAsync(imageUpload);

            var imageUrl = await imageManager.GetImageUrlAsync(litterImageId, ImageTypeEnum.LitterImage,
                ImageSizeEnum.Reduced, cancellationToken);
            litterImage.AzureBlobURL = imageUrl;
            await litterImageManager.UpdateAsync(litterImage, UserId, cancellationToken);

            return Ok();
        }

        /// <summary>
        /// Gets the image URL for a litter image.
        /// </summary>
        /// <param name="litterImageId">The litter image ID.</param>
        /// <param name="imageSize">The image size.</param>
        /// <param name="cancellationToken">Cancellation token.</param>
        /// <response code="200">Returns the image URL.</response>
        /// <response code="204">Image not found.</response>
        [HttpGet("image/{litterImageId}/{imageSize}")]
        [ProducesResponseType(typeof(string), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        public async Task<IActionResult> GetImage(Guid litterImageId, ImageSizeEnum imageSize,
            CancellationToken cancellationToken)
        {
            logger.LogInformation("V2 GetLitterImage LitterImageId={LitterImageId}", litterImageId);

            try
            {
                var url = await imageManager.GetImageUrlAsync(litterImageId, ImageTypeEnum.LitterImage,
                    imageSize, cancellationToken);

                if (string.IsNullOrWhiteSpace(url))
                {
                    return NoContent();
                }

                return Ok(url);
            }
            catch (Exception ex)
            {
                logger.LogWarning(ex, "Failed to get image URL for litter image {LitterImageId}", litterImageId);
                return NoContent();
            }
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
