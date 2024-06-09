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

    [Route("api/litterreport")]
    public class LitterReportController(
        ILitterReportManager litterReportManager,
        ILitterImageManager litterImageManager,
        IUserManager userManager,
        IImageManager imageManager,
        ILogger<LitterReportController> logger)
        : SecureController
    {
        [HttpGet("{litterReportId}")]
        public async Task<IActionResult> GetLitterReport(Guid litterReportId, CancellationToken cancellationToken)
        {
            var result = await litterReportManager.GetAsync(litterReportId, cancellationToken).ConfigureAwait(false);
            return Ok(result);
        }

        [HttpGet]
        public async Task<IActionResult> GetLitterReports(CancellationToken cancellationToken)
        {
            var result = await litterReportManager.GetAsync(cancellationToken).ConfigureAwait(false);
            var fullLitterReports = await ToFullLitterReport(result, cancellationToken);

            return Ok(fullLitterReports);
        }

        [HttpGet]
        [Route("new")]
        public async Task<IActionResult> GetNewLitterReports(CancellationToken cancellationToken)
        {
            var result = await litterReportManager.GetNewLitterReportsAsync(cancellationToken).ConfigureAwait(false);
            var fullLitterReports = await ToFullLitterReport(result, cancellationToken);

            return Ok(fullLitterReports);
        }

        [HttpGet]
        [Route("cleaned")]
        public async Task<IActionResult> GetCleanedLitterReports(CancellationToken cancellationToken)
        {
            var result = await litterReportManager.GetCleanedLitterReportsAsync(cancellationToken)
                .ConfigureAwait(false);
            var fullLitterReports = await ToFullLitterReport(result, cancellationToken);

            return Ok(fullLitterReports);
        }

        [HttpGet]
        [Route("notcancelled")]
        public async Task<IActionResult> GetNotCancelledLitterReports(CancellationToken cancellationToken)
        {
            var result = await litterReportManager.GetNotCancelledLitterReportsAsync(cancellationToken)
                .ConfigureAwait(false);
            var fullLitterReports = await ToFullLitterReport(result, cancellationToken);

            return Ok(fullLitterReports);
        }

        [HttpGet]
        [Route("assigned")]
        public async Task<IActionResult> GetAssignedLitterReports(CancellationToken cancellationToken)
        {
            var result = await litterReportManager.GetAssignedLitterReportsAsync(cancellationToken)
                .ConfigureAwait(false);
            var fullLitterReports = await ToFullLitterReport(result, cancellationToken);

            return Ok(fullLitterReports);
        }

        [HttpGet]
        [Authorize(Policy = AuthorizationPolicyConstants.UserIsAdmin)]
        [Route("cancelled")]
        public async Task<IActionResult> GetCancelledLitterReports(CancellationToken cancellationToken)
        {
            var result = await litterReportManager.GetCancelledLitterReportsAsync(cancellationToken)
                .ConfigureAwait(false);
            var fullLitterReports = await ToFullLitterReport(result, cancellationToken);

            return Ok(fullLitterReports);
        }

        [HttpGet]
        [Authorize(Policy = AuthorizationPolicyConstants.ValidUser)]
        [RequiredScope(Constants.TrashMobReadScope)]
        [Route("userlitterreports/{userId}")]
        public async Task<IActionResult> GetUserLitterReports(Guid userId, CancellationToken cancellationToken)
        {
            var result = await litterReportManager.GetUserLitterReportsAsync(userId, cancellationToken)
                .ConfigureAwait(false);
            var fullLitterReports = await ToFullLitterReport(result, cancellationToken);

            return Ok(fullLitterReports);
        }

        [HttpGet]
        [Route("filteredlitterreports")]
        public async Task<IActionResult> GetFilteredLitterReports([FromBody] LitterReportFilter filter,
            CancellationToken cancellationToken)
        {
            var result = await litterReportManager.GetFilteredLitterReportsAsync(filter, cancellationToken)
                .ConfigureAwait(false);

            if (filter.PageSize != null)
            {
                var pagedResults = result.Skip(filter.PageIndex.GetValueOrDefault(0) * filter.PageSize.GetValueOrDefault(10))
                    .Take(filter.PageSize.GetValueOrDefault(10)).ToList();
                return Ok(pagedResults);
            }

            return Ok(result);
        }

        [HttpGet]
        [Route("pagedfilteredlitterreports")]
        public async Task<IActionResult> GetPagedFilteredLitterReports([FromBody] LitterReportFilter filter,
            CancellationToken cancellationToken)
        {
            var result = await litterReportManager.GetFilteredLitterReportsAsync(filter, cancellationToken)
                .ConfigureAwait(false);

            if (filter.PageSize != null)
            {
                var pagedResults = PaginatedList<LitterReport>.Create(result.AsQueryable(),
                    filter.PageIndex.GetValueOrDefault(0), filter.PageSize.GetValueOrDefault(10));
                return Ok(pagedResults);
            }

            return Ok(result);
        }

        [HttpGet]
        [Route("locationsbytimerange")]
        public async Task<IActionResult> GetLitterLocationsByTimeRange([FromQuery] DateTimeOffset? startTime,
            [FromQuery] DateTimeOffset? endTime, CancellationToken cancellationToken)
        {
            var result = await litterReportManager
                .GeLitterLocationsByTimeRangeAsync(startTime, endTime, cancellationToken).ConfigureAwait(false);

            return Ok(result);
        }

        [HttpPost]
        [Authorize(Policy = AuthorizationPolicyConstants.ValidUser)]
        [RequiredScope(Constants.TrashMobWriteScope)]
        public async Task<IActionResult> AddLitterReport(LitterReport litterReport, CancellationToken cancellationToken)
        {
            if (litterReport == null)
            {
                return null;
            }

            logger.LogInformation("AddLitterReport - Name: {Name}, Description: {Description}, Status: {Status}",
                litterReport.Name, litterReport.Description, litterReport.LitterReportStatusId);

            var newLitterReport = await litterReportManager.AddAsync(litterReport, UserId, cancellationToken);

            if (newLitterReport != null)
            {
                TelemetryClient.TrackEvent(nameof(AddLitterReport));
                return Ok(newLitterReport);
            }

            return BadRequest("Failed to create litter report");
        }

        [HttpPost("image/{litterImageId}")]
        [Authorize(Policy = AuthorizationPolicyConstants.ValidUser)]
        public async Task<IActionResult> UploadImage([FromForm] ImageUpload imageUpload, Guid litterImageId,
            CancellationToken cancellationToken)
        {
            var litterImage = await litterImageManager.GetAsync(litterImageId, cancellationToken);
            var authResult =
                await AuthorizationService.AuthorizeAsync(User, litterImage,
                    AuthorizationPolicyConstants.UserOwnsEntity);

            if (!User.Identity.IsAuthenticated || !authResult.Succeeded)
            {
                return Forbid();
            }

            await imageManager.UploadImage(imageUpload);

            return Ok();
        }

        [HttpPut]
        [Authorize(Policy = AuthorizationPolicyConstants.ValidUser)]
        [RequiredScope(Constants.TrashMobWriteScope)]
        public async Task<IActionResult> UpdateLitterReport(LitterReport litterReport,
            CancellationToken cancellationToken)
        {
            var authResult = await AuthorizationService.AuthorizeAsync(User, litterReport,
                AuthorizationPolicyConstants.UserOwnsEntityOrIsAdmin);

            if (!User.Identity.IsAuthenticated || !authResult.Succeeded)
            {
                return Forbid();
            }

            var updatedLitterReport = await litterReportManager.UpdateAsync(litterReport, UserId, cancellationToken);

            if (updatedLitterReport != null)
            {
                TelemetryClient.TrackEvent(nameof(UpdateLitterReport));
                return Ok(updatedLitterReport);
            }

            return BadRequest("Failed to update litter report");
        }

        [HttpDelete("{id}")]
        [Authorize(Policy = AuthorizationPolicyConstants.ValidUser)]
        [RequiredScope(Constants.TrashMobWriteScope)]
        public async Task<IActionResult> DeleteLitterReport(Guid id, CancellationToken cancellationToken)
        {
            var litterReport = await litterReportManager.GetAsync(id, cancellationToken).ConfigureAwait(false);

            var authResult = await AuthorizationService.AuthorizeAsync(User, litterReport,
                AuthorizationPolicyConstants.UserOwnsEntityOrIsAdmin);

            if (!User.Identity.IsAuthenticated || !authResult.Succeeded)
            {
                return Forbid();
            }

            var result = await litterReportManager.DeleteAsync(id, UserId, cancellationToken).ConfigureAwait(false);

            if (result != -1)
            {
                TelemetryClient.TrackEvent(nameof(DeleteLitterReport));
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

        [HttpGet("image/{litterImageId}/{imageSize}")]
        public async Task<IActionResult> GetImage(Guid litterImageId, ImageSizeEnum imageSize,
            CancellationToken cancellationToken)
        {
            var url = await imageManager.GetImageUrlAsync(litterImageId, ImageTypeEnum.LitterImage, imageSize, cancellationToken);

            if (string.IsNullOrEmpty(url))
            {
                return NoContent();
            }

            return Ok(url);
        }
    }
}