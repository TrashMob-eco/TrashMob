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
    /// Controller for managing sponsors within a community's sponsored adoption program.
    /// </summary>
    [Route("api/communities/{partnerId}/sponsors")]
    public class CommunitySponsorsController : SecureController
    {
        private readonly ISponsorManager sponsorManager;
        private readonly IKeyedManager<Partner> partnerManager;

        /// <summary>
        /// Initializes a new instance of the <see cref="CommunitySponsorsController"/> class.
        /// </summary>
        /// <param name="sponsorManager">The sponsor manager.</param>
        /// <param name="partnerManager">The partner manager.</param>
        public CommunitySponsorsController(
            ISponsorManager sponsorManager,
            IKeyedManager<Partner> partnerManager)
        {
            this.sponsorManager = sponsorManager;
            this.partnerManager = partnerManager;
        }

        /// <summary>
        /// Gets all active sponsors for a community.
        /// </summary>
        /// <param name="partnerId">The community (partner) ID.</param>
        /// <param name="cancellationToken">Cancellation token.</param>
        [HttpGet]
        [Authorize(Policy = AuthorizationPolicyConstants.ValidUser)]
        [RequiredScope(Constants.TrashMobReadScope)]
        [ProducesResponseType(typeof(IEnumerable<Sponsor>), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetSponsors(Guid partnerId, CancellationToken cancellationToken)
        {
            var partner = await partnerManager.GetAsync(partnerId, cancellationToken);
            if (partner == null)
            {
                return NotFound();
            }

            if (!await IsAuthorizedAsync(partner, AuthorizationPolicyConstants.UserIsPartnerUserOrIsAdmin))
            {
                return Forbid();
            }

            var sponsors = await sponsorManager.GetByCommunityAsync(partnerId, cancellationToken);
            return Ok(sponsors);
        }

        /// <summary>
        /// Gets a single sponsor by ID.
        /// </summary>
        /// <param name="partnerId">The community (partner) ID.</param>
        /// <param name="sponsorId">The sponsor ID.</param>
        /// <param name="cancellationToken">Cancellation token.</param>
        [HttpGet("{sponsorId}")]
        [Authorize(Policy = AuthorizationPolicyConstants.ValidUser)]
        [RequiredScope(Constants.TrashMobReadScope)]
        [ProducesResponseType(typeof(Sponsor), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetSponsor(Guid partnerId, Guid sponsorId, CancellationToken cancellationToken)
        {
            var partner = await partnerManager.GetAsync(partnerId, cancellationToken);
            if (partner == null)
            {
                return NotFound();
            }

            if (!await IsAuthorizedAsync(partner, AuthorizationPolicyConstants.UserIsPartnerUserOrIsAdmin))
            {
                return Forbid();
            }

            var sponsor = await sponsorManager.GetAsync(sponsorId, cancellationToken);
            if (sponsor == null || sponsor.PartnerId != partnerId)
            {
                return NotFound();
            }

            return Ok(sponsor);
        }

        /// <summary>
        /// Creates a new sponsor for a community.
        /// </summary>
        /// <param name="partnerId">The community (partner) ID.</param>
        /// <param name="sponsor">The sponsor to create.</param>
        /// <param name="cancellationToken">Cancellation token.</param>
        [HttpPost]
        [Authorize(Policy = AuthorizationPolicyConstants.ValidUser)]
        [RequiredScope(Constants.TrashMobWriteScope)]
        [ProducesResponseType(typeof(Sponsor), StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> CreateSponsor(Guid partnerId, [FromBody] Sponsor sponsor, CancellationToken cancellationToken)
        {
            var partner = await partnerManager.GetAsync(partnerId, cancellationToken);
            if (partner == null)
            {
                return NotFound();
            }

            if (!await IsAuthorizedAsync(partner, AuthorizationPolicyConstants.UserIsPartnerUserOrIsAdmin))
            {
                return Forbid();
            }

            sponsor.PartnerId = partnerId;
            sponsor.CreatedByUserId = UserId;
            sponsor.LastUpdatedByUserId = UserId;

            var created = await sponsorManager.AddAsync(sponsor, UserId, cancellationToken);
            return CreatedAtAction(nameof(GetSponsor), new { partnerId, sponsorId = created.Id }, created);
        }

        /// <summary>
        /// Updates an existing sponsor.
        /// </summary>
        /// <param name="partnerId">The community (partner) ID.</param>
        /// <param name="sponsorId">The sponsor ID.</param>
        /// <param name="sponsor">The updated sponsor data.</param>
        /// <param name="cancellationToken">Cancellation token.</param>
        [HttpPut("{sponsorId}")]
        [Authorize(Policy = AuthorizationPolicyConstants.ValidUser)]
        [RequiredScope(Constants.TrashMobWriteScope)]
        [ProducesResponseType(typeof(Sponsor), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> UpdateSponsor(
            Guid partnerId,
            Guid sponsorId,
            [FromBody] Sponsor sponsor,
            CancellationToken cancellationToken)
        {
            var partner = await partnerManager.GetAsync(partnerId, cancellationToken);
            if (partner == null)
            {
                return NotFound();
            }

            if (!await IsAuthorizedAsync(partner, AuthorizationPolicyConstants.UserIsPartnerUserOrIsAdmin))
            {
                return Forbid();
            }

            var existing = await sponsorManager.GetAsync(sponsorId, cancellationToken);
            if (existing == null || existing.PartnerId != partnerId)
            {
                return NotFound();
            }

            sponsor.Id = sponsorId;
            sponsor.PartnerId = partnerId;
            sponsor.LastUpdatedByUserId = UserId;

            var updated = await sponsorManager.UpdateAsync(sponsor, UserId, cancellationToken);
            return Ok(updated);
        }

        /// <summary>
        /// Deactivates a sponsor (soft delete).
        /// </summary>
        /// <param name="partnerId">The community (partner) ID.</param>
        /// <param name="sponsorId">The sponsor ID.</param>
        /// <param name="cancellationToken">Cancellation token.</param>
        [HttpDelete("{sponsorId}")]
        [Authorize(Policy = AuthorizationPolicyConstants.ValidUser)]
        [RequiredScope(Constants.TrashMobWriteScope)]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> DeactivateSponsor(Guid partnerId, Guid sponsorId, CancellationToken cancellationToken)
        {
            var partner = await partnerManager.GetAsync(partnerId, cancellationToken);
            if (partner == null)
            {
                return NotFound();
            }

            if (!await IsAuthorizedAsync(partner, AuthorizationPolicyConstants.UserIsPartnerUserOrIsAdmin))
            {
                return Forbid();
            }

            var existing = await sponsorManager.GetAsync(sponsorId, cancellationToken);
            if (existing == null || existing.PartnerId != partnerId)
            {
                return NotFound();
            }

            existing.IsActive = false;
            existing.LastUpdatedByUserId = UserId;
            await sponsorManager.UpdateAsync(existing, UserId, cancellationToken);

            return NoContent();
        }
    }
}
