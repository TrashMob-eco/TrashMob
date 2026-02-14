namespace TrashMob.Controllers
{
    using System;
    using System.Collections.Generic;
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
    /// Controller for sponsor adoption reports and cleanup log summaries.
    /// </summary>
    [Route("api/sponsors/{sponsorId}/adoptions")]
    public class SponsorReportsController(
        ISponsoredAdoptionManager adoptionManager,
        IProfessionalCleanupLogManager logManager,
        ISponsorManager sponsorManager,
        IKeyedManager<Partner> partnerManager)
        : SecureController
    {

        /// <summary>
        /// Gets all sponsored adoptions for a specific sponsor.
        /// </summary>
        /// <param name="sponsorId">The sponsor ID.</param>
        /// <param name="cancellationToken">Cancellation token.</param>
        [HttpGet]
        [Authorize(Policy = AuthorizationPolicyConstants.ValidUser)]
        [RequiredScope(Constants.TrashMobReadScope)]
        [ProducesResponseType(typeof(IEnumerable<SponsoredAdoption>), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetAdoptions(Guid sponsorId, CancellationToken cancellationToken)
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

            var adoptions = await adoptionManager.GetBySponsorIdAsync(sponsorId, cancellationToken);
            return Ok(adoptions);
        }

        /// <summary>
        /// Gets cleanup logs for a specific sponsored adoption under a sponsor.
        /// </summary>
        /// <param name="sponsorId">The sponsor ID.</param>
        /// <param name="adoptionId">The sponsored adoption ID.</param>
        /// <param name="cancellationToken">Cancellation token.</param>
        [HttpGet("{adoptionId}/reports")]
        [Authorize(Policy = AuthorizationPolicyConstants.ValidUser)]
        [RequiredScope(Constants.TrashMobReadScope)]
        [ProducesResponseType(typeof(IEnumerable<ProfessionalCleanupLog>), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetAdoptionReports(
            Guid sponsorId,
            Guid adoptionId,
            CancellationToken cancellationToken)
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

            var adoption = await adoptionManager.GetAsync(adoptionId, cancellationToken);
            if (adoption == null || adoption.SponsorId != sponsorId)
            {
                return NotFound();
            }

            var logs = await logManager.GetBySponsoredAdoptionIdAsync(adoptionId, cancellationToken);
            return Ok(logs);
        }
    }
}
