namespace TrashMob.Controllers
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading;
    using System.Threading.Tasks;
    using Microsoft.AspNetCore.Authorization;
    using Microsoft.AspNetCore.Http;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.Identity.Web.Resource;
    using TrashMob.Models;
    using TrashMob.Security;
    using TrashMob.Shared;
    using TrashMob.Shared.Managers.Interfaces;

    /// <summary>
    /// Portal endpoints for sponsor users (read-only views of adoptions and cleanup logs).
    /// </summary>
    [Route("api/sponsors")]
    [ApiController]
    public class SponsorPortalController : SecureController
    {
        private readonly ISponsorManager sponsorManager;
        private readonly IPartnerAdminManager partnerAdminManager;
        private readonly IProfessionalCleanupLogManager logManager;
        private readonly IKeyedManager<Partner> partnerManager;

        /// <summary>
        /// Initializes a new instance of the <see cref="SponsorPortalController"/> class.
        /// </summary>
        /// <param name="sponsorManager">The sponsor manager.</param>
        /// <param name="partnerAdminManager">The partner admin manager.</param>
        /// <param name="logManager">The cleanup log manager.</param>
        /// <param name="partnerManager">The partner manager.</param>
        public SponsorPortalController(
            ISponsorManager sponsorManager,
            IPartnerAdminManager partnerAdminManager,
            IProfessionalCleanupLogManager logManager,
            IKeyedManager<Partner> partnerManager)
        {
            this.sponsorManager = sponsorManager;
            this.partnerAdminManager = partnerAdminManager;
            this.logManager = logManager;
            this.partnerManager = partnerManager;
        }

        /// <summary>
        /// Gets all sponsors the authenticated user has access to via partner admin memberships.
        /// </summary>
        /// <param name="cancellationToken">Cancellation token.</param>
        [HttpGet("mine")]
        [Authorize(Policy = AuthorizationPolicyConstants.ValidUser)]
        [RequiredScope(Constants.TrashMobReadScope)]
        [ProducesResponseType(typeof(IEnumerable<Sponsor>), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetMySponsors(CancellationToken cancellationToken)
        {
            var partners = await partnerAdminManager.GetPartnersByUserIdAsync(UserId, cancellationToken);
            var allSponsors = new List<Sponsor>();

            foreach (var partner in partners)
            {
                var sponsors = await sponsorManager.GetByCommunityAsync(partner.Id, cancellationToken);
                allSponsors.AddRange(sponsors);
            }

            return Ok(allSponsors);
        }

        /// <summary>
        /// Gets all cleanup logs across all adoptions for a specific sponsor.
        /// </summary>
        /// <param name="sponsorId">The sponsor ID.</param>
        /// <param name="cancellationToken">Cancellation token.</param>
        [HttpGet("{sponsorId:guid}/cleanup-logs")]
        [Authorize(Policy = AuthorizationPolicyConstants.ValidUser)]
        [RequiredScope(Constants.TrashMobReadScope)]
        [ProducesResponseType(typeof(IEnumerable<ProfessionalCleanupLog>), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetSponsorCleanupLogs(Guid sponsorId, CancellationToken cancellationToken)
        {
            var sponsor = await sponsorManager.GetAsync(sponsorId, cancellationToken);
            if (sponsor == null)
            {
                return NotFound();
            }

            var partner = await partnerManager.GetAsync(sponsor.PartnerId, cancellationToken);
            if (partner == null)
            {
                return NotFound();
            }

            if (!await IsAuthorizedAsync(partner, AuthorizationPolicyConstants.UserIsPartnerUserOrIsAdmin))
            {
                return Forbid();
            }

            var logs = await logManager.GetBySponsorIdAsync(sponsorId, cancellationToken);
            return Ok(logs);
        }

        /// <summary>
        /// Exports all cleanup logs for a sponsor as a CSV file.
        /// </summary>
        /// <param name="sponsorId">The sponsor ID.</param>
        /// <param name="cancellationToken">Cancellation token.</param>
        [HttpGet("{sponsorId:guid}/cleanup-logs/export")]
        [Authorize(Policy = AuthorizationPolicyConstants.ValidUser)]
        [RequiredScope(Constants.TrashMobReadScope)]
        [ProducesResponseType(typeof(FileContentResult), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> ExportCleanupLogs(Guid sponsorId, CancellationToken cancellationToken)
        {
            var sponsor = await sponsorManager.GetAsync(sponsorId, cancellationToken);
            if (sponsor == null)
            {
                return NotFound();
            }

            var partner = await partnerManager.GetAsync(sponsor.PartnerId, cancellationToken);
            if (partner == null)
            {
                return NotFound();
            }

            if (!await IsAuthorizedAsync(partner, AuthorizationPolicyConstants.UserIsPartnerUserOrIsAdmin))
            {
                return Forbid();
            }

            var logs = await logManager.GetBySponsorIdAsync(sponsorId, cancellationToken);

            var csv = new StringBuilder();
            csv.AppendLine("Date,Area,Duration (min),Bags,Weight (lbs),Notes");

            foreach (var log in logs)
            {
                var date = log.CleanupDate;
                var area = EscapeCsvField(log.SponsoredAdoption?.AdoptableArea?.Name ?? "");
                var duration = log.DurationMinutes.ToString();
                var bags = log.BagsCollected.ToString();
                var weight = log.WeightInPounds?.ToString("F1") ?? "";
                var notes = EscapeCsvField(log.Notes ?? "");

                csv.AppendLine($"{date},{area},{duration},{bags},{weight},{notes}");
            }

            var bytes = Encoding.UTF8.GetBytes(csv.ToString());
            var fileName = $"{sponsor.Name.Replace(" ", "_")}_CleanupLogs_{DateTime.UtcNow:yyyyMMdd}.csv";

            return File(bytes, "text/csv", fileName);
        }

        private static string EscapeCsvField(string field)
        {
            if (string.IsNullOrWhiteSpace(field))
            {
                return "";
            }

            if (field.Contains(',') || field.Contains('"') || field.Contains('\n') || field.Contains('\r'))
            {
                return $"\"{field.Replace("\"", "\"\"")}\"";
            }

            return field;
        }
    }
}
