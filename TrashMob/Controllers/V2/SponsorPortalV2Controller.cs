namespace TrashMob.Controllers.V2
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
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
    /// V2 controller for sponsor portal operations — viewing sponsors and their cleanup logs.
    /// </summary>
    [ApiController]
    [ApiVersion("2.0")]
    [EnableCors("_myAllowSpecificOrigins")]
    [Route("api/v{version:apiVersion}/sponsors")]
    public class SponsorPortalV2Controller(
        ISponsorManager sponsorManager,
        IPartnerAdminManager partnerAdminManager,
        IProfessionalCleanupLogManager logManager,
        IKeyedManager<Partner> partnerManager,
        IAuthorizationService authorizationService,
        ILogger<SponsorPortalV2Controller> logger) : ControllerBase
    {
        private Guid UserId => Guid.TryParse(HttpContext.Items["UserId"]?.ToString(), out var parsedUserId) ? parsedUserId : Guid.Empty;

        /// <summary>
        /// Gets all sponsors across all communities the current user administers.
        /// </summary>
        /// <param name="cancellationToken">Cancellation token.</param>
        /// <response code="200">Returns the sponsors.</response>
        [HttpGet("mine")]
        [Authorize(Policy = AuthorizationPolicyConstants.ValidUser)]
        [RequiredScope(Constants.TrashMobReadScope)]
        [ProducesResponseType(typeof(IEnumerable<SponsorDto>), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetMySponsors(CancellationToken cancellationToken)
        {
            logger.LogInformation("V2 GetMySponsors for User={UserId}", UserId);

            var partners = await partnerAdminManager.GetPartnersByUserIdAsync(UserId, cancellationToken);
            var allSponsors = new List<SponsorDto>();

            foreach (var partner in partners)
            {
                var sponsors = await sponsorManager.GetByCommunityAsync(partner.Id, cancellationToken);
                allSponsors.AddRange(sponsors.Select(s => s.ToV2Dto()));
            }

            return Ok(allSponsors);
        }

        /// <summary>
        /// Gets cleanup logs for a specific sponsor.
        /// </summary>
        /// <param name="sponsorId">The sponsor ID.</param>
        /// <param name="cancellationToken">Cancellation token.</param>
        /// <response code="200">Returns the cleanup logs.</response>
        /// <response code="403">Not authorized.</response>
        /// <response code="404">Sponsor or partner not found.</response>
        [HttpGet("{sponsorId:guid}/cleanup-logs")]
        [Authorize(Policy = AuthorizationPolicyConstants.ValidUser)]
        [RequiredScope(Constants.TrashMobReadScope)]
        [ProducesResponseType(typeof(IEnumerable<ProfessionalCleanupLogDto>), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetSponsorCleanupLogs(Guid sponsorId, CancellationToken cancellationToken)
        {
            logger.LogInformation("V2 GetSponsorCleanupLogs for Sponsor={SponsorId}", sponsorId);

            var sponsor = await sponsorManager.GetAsync(sponsorId, cancellationToken);
            if (sponsor is null)
            {
                return NotFound();
            }

            var partner = await partnerManager.GetAsync(sponsor.PartnerId, cancellationToken);
            if (partner is null)
            {
                return NotFound();
            }

            if (!await IsAuthorizedAsync(partner, AuthorizationPolicyConstants.UserIsPartnerUserOrIsAdmin))
            {
                return Forbid();
            }

            var logs = await logManager.GetBySponsorIdAsync(sponsorId, cancellationToken);
            return Ok(logs.Select(l => l.ToV2Dto()));
        }

        /// <summary>
        /// Exports cleanup logs for a specific sponsor as a CSV file.
        /// </summary>
        /// <param name="sponsorId">The sponsor ID.</param>
        /// <param name="cancellationToken">Cancellation token.</param>
        /// <response code="200">Returns a CSV file.</response>
        /// <response code="403">Not authorized.</response>
        /// <response code="404">Sponsor or partner not found.</response>
        [HttpGet("{sponsorId:guid}/cleanup-logs/export")]
        [Authorize(Policy = AuthorizationPolicyConstants.ValidUser)]
        [RequiredScope(Constants.TrashMobReadScope)]
        [ProducesResponseType(typeof(FileContentResult), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
        public async Task<IActionResult> ExportCleanupLogs(Guid sponsorId, CancellationToken cancellationToken)
        {
            logger.LogInformation("V2 ExportCleanupLogs for Sponsor={SponsorId}", sponsorId);

            var sponsor = await sponsorManager.GetAsync(sponsorId, cancellationToken);
            if (sponsor is null)
            {
                return NotFound();
            }

            var partner = await partnerManager.GetAsync(sponsor.PartnerId, cancellationToken);
            if (partner is null)
            {
                return NotFound();
            }

            if (!await IsAuthorizedAsync(partner, AuthorizationPolicyConstants.UserIsPartnerUserOrIsAdmin))
            {
                return Forbid();
            }

            var logs = await logManager.GetBySponsorIdAsync(sponsorId, cancellationToken);

            var sb = new StringBuilder();
            sb.AppendLine("Date,Area,Duration (min),Bags,Weight (lbs),Notes");

            foreach (var log in logs)
            {
                var date = log.CleanupDate.ToString("yyyy-MM-dd");
                var area = EscapeCsvField(log.SponsoredAdoption?.AdoptableArea?.Name ?? string.Empty);
                var duration = log.DurationMinutes.ToString();
                var bags = log.BagsCollected.ToString();
                var weight = log.WeightInPounds?.ToString("F1") ?? string.Empty;
                var notes = EscapeCsvField(log.Notes ?? string.Empty);

                sb.AppendLine($"{date},{area},{duration},{bags},{weight},{notes}");
            }

            var csvBytes = Encoding.UTF8.GetBytes(sb.ToString());
            var filename = $"{sponsor.Name}_CleanupLogs_{DateTime.UtcNow:yyyyMMdd}.csv";

            return File(csvBytes, "text/csv", filename);
        }

        private static string EscapeCsvField(string field)
        {
            if (field.Contains(',') || field.Contains('"') || field.Contains('\n') || field.Contains('\r'))
            {
                return "\"" + field.Replace("\"", "\"\"") + "\"";
            }

            return field;
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
