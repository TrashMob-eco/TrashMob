namespace TrashMob.Controllers
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading;
    using System.Threading.Tasks;
    using Microsoft.AspNetCore.Authorization;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.Extensions.Logging;
    using Microsoft.Identity.Web.Resource;
    using TrashMob.Models;
    using TrashMob.Models.Extensions;
    using TrashMob.Models.Poco;
    using TrashMob.Security;
    using TrashMob.Shared;
    using TrashMob.Shared.Managers.Interfaces;
    using TrashMob.Shared.Poco;

    /// <summary>
    /// Controller for managing litter reports, including CRUD operations and queries by status or user.
    /// </summary>
    [Route("api/litterreport")]
    public class LitterReportController(
        ILitterReportManager litterReportManager,
        ILitterImageManager litterImageManager,
        IUserManager userManager,
        IImageManager imageManager,
        ILogger<LitterReportController> logger)
        : SecureController
    {
        /// <summary>
        /// Gets a litter report by its unique identifier.
        /// </summary>
        /// <param name="litterReportId">The litter report ID.</param>
        /// <param name="cancellationToken">Cancellation token.</param>
        /// <remarks>The litter report.</remarks>
        [HttpGet("{litterReportId}")]
        public async Task<IActionResult> GetLitterReport(Guid litterReportId, CancellationToken cancellationToken)
        {
            var result = await litterReportManager.GetAsync(litterReportId, cancellationToken);
            return Ok(result);
        }

        /// <summary>
        /// Gets all litter reports.
        /// </summary>
        /// <param name="cancellationToken">Cancellation token.</param>
        /// <remarks>A list of full litter reports.</remarks>
        [HttpGet]
        public async Task<IActionResult> GetLitterReports(CancellationToken cancellationToken)
        {
            var result = await litterReportManager.GetAsync(cancellationToken);
            var fullLitterReports = await ToFullLitterReport(result, cancellationToken);

            return Ok(fullLitterReports);
        }

        /// <summary>
        /// Gets all new litter reports.
        /// </summary>
        /// <param name="cancellationToken">Cancellation token.</param>
        /// <remarks>A list of new full litter reports.</remarks>
        [HttpGet]
        [Route("new")]
        public async Task<IActionResult> GetNewLitterReports(CancellationToken cancellationToken)
        {
            var result = await litterReportManager.GetNewLitterReportsAsync(cancellationToken);
            var fullLitterReports = await ToFullLitterReport(result, cancellationToken);

            return Ok(fullLitterReports);
        }

        /// <summary>
        /// Gets all cleaned litter reports.
        /// </summary>
        /// <param name="cancellationToken">Cancellation token.</param>
        /// <remarks>A list of cleaned full litter reports.</remarks>
        [HttpGet]
        [Route("cleaned")]
        public async Task<IActionResult> GetCleanedLitterReports(CancellationToken cancellationToken)
        {
            var result = await litterReportManager.GetCleanedLitterReportsAsync(cancellationToken);
            var fullLitterReports = await ToFullLitterReport(result, cancellationToken);

            return Ok(fullLitterReports);
        }

        /// <summary>
        /// Gets all not cancelled litter reports.
        /// </summary>
        /// <param name="cancellationToken">Cancellation token.</param>
        /// <remarks>A list of not cancelled full litter reports.</remarks>
        [HttpGet]
        [Route("notcancelled")]
        public async Task<IActionResult> GetNotCancelledLitterReports(CancellationToken cancellationToken)
        {
            var result = await litterReportManager.GetNotCancelledLitterReportsAsync(cancellationToken);
            var fullLitterReports = await ToFullLitterReport(result, cancellationToken);

            return Ok(fullLitterReports);
        }

        /// <summary>
        /// Gets all assigned litter reports.
        /// </summary>
        /// <param name="cancellationToken">Cancellation token.</param>
        /// <remarks>A list of assigned full litter reports.</remarks>
        [HttpGet]
        [Route("assigned")]
        public async Task<IActionResult> GetAssignedLitterReports(CancellationToken cancellationToken)
        {
            var result = await litterReportManager.GetAssignedLitterReportsAsync(cancellationToken);
            var fullLitterReports = await ToFullLitterReport(result, cancellationToken);

            return Ok(fullLitterReports);
        }

        /// <summary>
        /// Gets all cancelled litter reports. Admin only.
        /// </summary>
        /// <param name="cancellationToken">Cancellation token.</param>
        /// <remarks>A list of cancelled full litter reports.</remarks>
        [HttpGet]
        [Authorize(Policy = AuthorizationPolicyConstants.UserIsAdmin)]
        [Route("cancelled")]
        public async Task<IActionResult> GetCancelledLitterReports(CancellationToken cancellationToken)
        {
            var result = await litterReportManager.GetCancelledLitterReportsAsync(cancellationToken);
            var fullLitterReports = await ToFullLitterReport(result, cancellationToken);

            return Ok(fullLitterReports);
        }

        /// <summary>
        /// Gets all litter reports for a specific user. Requires a valid user.
        /// </summary>
        /// <param name="userId">The user ID.</param>
        /// <param name="cancellationToken">Cancellation token.</param>
        /// <remarks>A list of user's full litter reports.</remarks>
        [HttpGet]
        [Authorize(Policy = AuthorizationPolicyConstants.ValidUser)]
        [RequiredScope(Constants.TrashMobReadScope)]
        [Route("userlitterreports/{userId}")]
        public async Task<IActionResult> GetUserLitterReports(Guid userId, CancellationToken cancellationToken)
        {
            var result = await litterReportManager.GetUserLitterReportsAsync(userId, cancellationToken);
            var fullLitterReports = await ToFullLitterReport(result, cancellationToken);

            return Ok(fullLitterReports);
        }

        /// <summary>
        /// Gets filtered litter reports based on the provided filter.
        /// </summary>
        /// <param name="filter">The filter criteria.</param>
        /// <param name="cancellationToken">Cancellation token.</param>
        /// <remarks>A list of filtered full litter reports.</remarks>
        [HttpPost]
        [Route("filteredlitterreports")]
        public async Task<IActionResult> GetFilteredLitterReports([FromBody] LitterReportFilter filter,
            CancellationToken cancellationToken)
        {
            var result = await litterReportManager.GetFilteredLitterReportsAsync(filter, cancellationToken);

            if (filter.PageSize != null)
            {
                var pagedResults = result.OrderByDescending(e => e.CreatedDate).Skip(filter.PageIndex.GetValueOrDefault(0) * filter.PageSize.GetValueOrDefault(10))
                    .Take(filter.PageSize.GetValueOrDefault(10)).ToList();

                var pagedFullLitterReports = await ToFullLitterReport(pagedResults, cancellationToken);
                return Ok(pagedFullLitterReports);
            }

            var fullResult = await ToFullLitterReport(result, cancellationToken);
            return Ok(fullResult);
        }

        /// <summary>
        /// Gets paged filtered litter reports based on the provided filter.
        /// </summary>
        /// <param name="filter">The filter criteria.</param>
        /// <param name="cancellationToken">Cancellation token.</param>
        /// <remarks>A paged list of filtered litter reports.</remarks>
        [HttpPost]
        [Route("pagedfilteredlitterreports")]
        public async Task<IActionResult> GetPagedFilteredLitterReports([FromBody] LitterReportFilter filter,
            CancellationToken cancellationToken)
        {
            var result = await litterReportManager.GetFilteredLitterReportsAsync(filter, cancellationToken);

            if (filter.PageSize != null)
            {
                var pagedResults = PaginatedList<LitterReport>.Create(result.OrderByDescending(e => e.CreatedDate).AsQueryable(),
                    filter.PageIndex.GetValueOrDefault(0), filter.PageSize.GetValueOrDefault(10));
                return Ok(pagedResults);
            }

            return Ok(result);
        }

        /// <summary>
        /// Gets litter locations by a specified time range.
        /// </summary>
        /// <param name="startTime">The start time.</param>
        /// <param name="endTime">The end time.</param>
        /// <param name="cancellationToken">Cancellation token.</param>
        /// <remarks>A list of litter locations within the time range.</remarks>
        [HttpGet]
        [Route("locationsbytimerange")]
        public async Task<IActionResult> GetLitterLocationsByTimeRange([FromQuery] DateTimeOffset? startTime,
            [FromQuery] DateTimeOffset? endTime, CancellationToken cancellationToken)
        {
            var result = await litterReportManager
                .GeLitterLocationsByTimeRangeAsync(startTime, endTime, cancellationToken);

            return Ok(result);
        }

        /// <summary>
        /// Adds a new litter report. Requires a valid user.
        /// </summary>
        /// <param name="litterReport">The litter report to add.</param>
        /// <param name="cancellationToken">Cancellation token.</param>
        /// <remarks>The newly created litter report.</remarks>
        [HttpPost]
        [Authorize(Policy = AuthorizationPolicyConstants.ValidUser)]
        [RequiredScope(Constants.TrashMobWriteScope)]
        public async Task<IActionResult> AddLitterReport(LitterReport litterReport, CancellationToken cancellationToken)
        {
            if (litterReport == null)
            {
                return BadRequest("Litter report cannot be null.");
            }

            logger.LogInformation("AddLitterReport - Name: {Name}, Description: {Description}, Status: {Status}",
                litterReport.Name, litterReport.Description, litterReport.LitterReportStatusId);

            var result = await litterReportManager.AddWithResultAsync(litterReport, UserId, cancellationToken);

            if (result.IsSuccess)
            {
                TrackEvent(nameof(AddLitterReport));
                return Ok(result.Data);
            }

            return BadRequest(result.ErrorMessage);
        }

        /// <summary>
        /// Uploads an image for a litter report. Requires a valid user.
        /// </summary>
        /// <param name="imageUpload">The image upload data.</param>
        /// <param name="litterImageId">The litter image ID.</param>
        /// <param name="cancellationToken">Cancellation token.</param>
        /// <remarks>Result of the upload operation.</remarks>
        [HttpPost("image/{litterImageId}")]
        [Authorize(Policy = AuthorizationPolicyConstants.ValidUser)]
        public async Task<IActionResult> UploadImage([FromForm] ImageUpload imageUpload, Guid litterImageId,
            CancellationToken cancellationToken)
        {
            var litterImage = await litterImageManager.GetAsync(litterImageId, cancellationToken);

            if (!await IsAuthorizedAsync(litterImage, AuthorizationPolicyConstants.UserOwnsEntity))
            {
                return Forbid();
            }

            await imageManager.UploadImage(imageUpload);

            return Ok();
        }

        /// <summary>
        /// Updates an existing litter report. Requires a valid user.
        /// </summary>
        /// <param name="litterReport">The litter report to update.</param>
        /// <param name="cancellationToken">Cancellation token.</param>
        /// <remarks>The updated litter report.</remarks>
        [HttpPut]
        [Authorize(Policy = AuthorizationPolicyConstants.ValidUser)]
        [RequiredScope(Constants.TrashMobWriteScope)]
        public async Task<IActionResult> UpdateLitterReport(LitterReport litterReport,
            CancellationToken cancellationToken)
        {
            if (!await IsAuthorizedAsync(litterReport, AuthorizationPolicyConstants.UserOwnsEntityOrIsAdmin))
            {
                return Forbid();
            }

            var updatedLitterReport = await litterReportManager.UpdateAsync(litterReport, UserId, cancellationToken);

            if (updatedLitterReport != null)
            {
                TrackEvent(nameof(UpdateLitterReport));
                return Ok(updatedLitterReport);
            }

            return BadRequest("Failed to update litter report");
        }

        /// <summary>
        /// Deletes a litter report by its unique identifier. Requires a valid user.
        /// </summary>
        /// <param name="id">The litter report ID.</param>
        /// <param name="cancellationToken">Cancellation token.</param>
        /// <remarks>The ID of the deleted litter report if successful.</remarks>
        [HttpDelete("{id}")]
        [Authorize(Policy = AuthorizationPolicyConstants.ValidUser)]
        [RequiredScope(Constants.TrashMobWriteScope)]
        public async Task<IActionResult> DeleteLitterReport(Guid id, CancellationToken cancellationToken)
        {
            var litterReport = await litterReportManager.GetAsync(id, cancellationToken);

            if (!await IsAuthorizedAsync(litterReport, AuthorizationPolicyConstants.UserOwnsEntityOrIsAdmin))
            {
                return Forbid();
            }

            var result = await litterReportManager.DeleteAsync(id, UserId, cancellationToken);

            if (result != -1)
            {
                TrackEvent(nameof(DeleteLitterReport));
                return Ok(id);
            }

            return BadRequest("Could not find the litter report, delete failed");
        }

        private async Task<IEnumerable<FullLitterReport>> ToFullLitterReport(IEnumerable<LitterReport> litterReports,
            CancellationToken cancellationToken)
        {
            var fullLitterReports = new List<FullLitterReport>();

            foreach (var litterReport in litterReports)
            {
                var user = await userManager.GetAsync(litterReport.CreatedByUserId, cancellationToken);
                fullLitterReports.Add(litterReport.ToFullLitterReport(user.UserName));
            }

            return fullLitterReports;
        }

        /// <summary>
        /// Gets the image URL for a litter image by its ID and size.
        /// </summary>
        /// <param name="litterImageId">The litter image ID.</param>
        /// <param name="imageSize">The image size.</param>
        /// <param name="cancellationToken">Cancellation token.</param>
        /// <remarks>The image URL or no content if not found.</remarks>
        [HttpGet("image/{litterImageId}/{imageSize}")]
        public async Task<IActionResult> GetImage(Guid litterImageId, ImageSizeEnum imageSize,
            CancellationToken cancellationToken)
        {
            try
            {
                var url = await imageManager.GetImageUrlAsync(litterImageId, ImageTypeEnum.LitterImage, imageSize, cancellationToken);

                if (string.IsNullOrWhiteSpace(url))
                {
                    return NoContent();
                }

                return Ok(url);
            }
            catch (Exception)
            {
                return NoContent();
            }
        }
    }
}