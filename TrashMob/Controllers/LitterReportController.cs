namespace TrashMob.Controllers
{
    using Microsoft.AspNetCore.Mvc;
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using System.Threading;
    using TrashMob.Models;
    using TrashMob.Shared.Managers.Interfaces;
    using TrashMob.Shared.Poco;
    using Microsoft.AspNetCore.Authorization;
    using Microsoft.Identity.Web.Resource;
    using TrashMob.Security;
    using TrashMob.Shared;
    using System;

    [Route("api/litterreport")]
    public class LitterReportController : SecureController
    {
        private readonly ILitterReportManager litterReportManager;
        private readonly ILitterImageManager litterImageManager;
        private readonly IUserManager userManager;

        public LitterReportController(ILitterReportManager litterReportManager, 
                                        ILitterImageManager litterImageManager, 
                                        IUserManager userManager, 
                                        IImageManager imageManager)
        {
            this.litterReportManager = litterReportManager;
            this.litterImageManager = litterImageManager;
            this.userManager = userManager;
        }

        [HttpGet]
        [Authorize(Policy = AuthorizationPolicyConstants.ValidUser)]
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
            var result = await litterReportManager.GetCleanedLitterReportsAsync(cancellationToken).ConfigureAwait(false);
            var fullLitterReports = await ToFullLitterReport(result, cancellationToken);

            return Ok(fullLitterReports);
        }

        [HttpGet]
        [Route("notcancelled")]
        public async Task<IActionResult> GetNotCancelledLitterReports(CancellationToken cancellationToken)
        {
            var result = await litterReportManager.GetNotCancelledLitterReportsAsync(cancellationToken).ConfigureAwait(false);
            var fullLitterReports =  await ToFullLitterReport(result, cancellationToken);

            return Ok(fullLitterReports);
        }
        
        [HttpGet]
        [Authorize(Policy = AuthorizationPolicyConstants.UserIsAdmin)]
        [Route("cancelled")]
        public async Task<IActionResult> GetCancelledLitterReports(CancellationToken cancellationToken)
        {
            var result = await litterReportManager.GetCancelledLitterReportsAsync(cancellationToken).ConfigureAwait(false);
            var fullLitterReports =  await ToFullLitterReport(result, cancellationToken);

            return Ok(fullLitterReports);
        }

        [HttpGet]
        [Authorize(Policy = AuthorizationPolicyConstants.ValidUser)]
        [RequiredScope(Constants.TrashMobReadScope)]
        [Route("userlitterreports/{userId}")]
        public async Task<IActionResult> GetUserLitterReports(Guid userId, CancellationToken cancellationToken)
        {
            var result = await litterReportManager.GetUserLitterReportsAsync(userId, cancellationToken).ConfigureAwait(false);
            var fullLitterReports = await ToFullLitterReport(result, cancellationToken);

            return Ok(fullLitterReports);
        }

        [HttpPost]
        [Authorize(Policy = AuthorizationPolicyConstants.ValidUser)]
        [RequiredScope(Constants.TrashMobWriteScope)]        
        public async Task<IActionResult> AddLitterReport([FromForm]FullLitterReport fullLitterReport, CancellationToken cancellationToken)
        {
            if (fullLitterReport == null)
            {
                return null;
            }

            var newLitterReport = await litterReportManager.AddAsync(fullLitterReport, UserId, cancellationToken);

            if (newLitterReport != null)
            {
                TelemetryClient.TrackEvent(nameof(AddLitterReport));
                return Ok(newLitterReport);
            }

            return BadRequest("Failed to create litter report");
        }

        [HttpPut]
        [RequiredScope(Constants.TrashMobWriteScope)]
        public async Task<IActionResult> UpdateLitterReport([FromForm]FullLitterReport fullLitterReport, CancellationToken cancellationToken)
        {
            var litterReport = fullLitterReport.ToLitterReport();

            var authResult = await AuthorizationService.AuthorizeAsync(User, litterReport, AuthorizationPolicyConstants.UserOwnsEntityOrIsAdmin);

            if (!User.Identity.IsAuthenticated || !authResult.Succeeded)
            {
                return Forbid();
            }

            var updatedLitterReport = await litterReportManager.UpdateAsync(fullLitterReport, UserId, cancellationToken);
            
            if(updatedLitterReport != null)
            {
                TelemetryClient.TrackEvent(nameof(UpdateLitterReport));
                return Ok(updatedLitterReport);
            }

            return BadRequest("Failed to update litter report");
        }

        [HttpDelete]
        [RequiredScope(Constants.TrashMobWriteScope)]
        public async Task<IActionResult> DeleteLitterReport(Guid id, CancellationToken cancellationToken)
        {
            var litterReport = await litterReportManager.GetAsync(id, cancellationToken).ConfigureAwait(false);

            var authResult = await AuthorizationService.AuthorizeAsync(User, litterReport, AuthorizationPolicyConstants.UserOwnsEntityOrIsAdmin);

            if (!User.Identity.IsAuthenticated || !authResult.Succeeded)
            {
                return Forbid();
            }

            var result = await litterReportManager.DeleteAsync(id, UserId, cancellationToken).ConfigureAwait(false);

            if(result != -1)
            {
                TelemetryClient.TrackEvent(nameof(DeleteLitterReport));
                return Ok(id);
            }

            return BadRequest("Could not find the litter report, delete failed");

        }

        private async Task<IActionResult> ToFullLitterReport(IEnumerable<LitterReport> litterReports, CancellationToken cancellationToken)
        {
            var fullLitterReports = new List<FullLitterReport>();

            foreach (var litterReport in litterReports)
            {
                var user = await userManager.GetAsync(litterReport.CreatedByUserId, cancellationToken);
                fullLitterReports.Add(litterReport.ToFullLitterReport(user.UserName));
            }

            return Ok(fullLitterReports);
        }
    }
}