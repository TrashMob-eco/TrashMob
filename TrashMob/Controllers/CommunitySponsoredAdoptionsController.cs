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
    using TrashMob.Shared.Poco;

    /// <summary>
    /// Controller for managing sponsored adoptions within a community.
    /// </summary>
    [Route("api/communities/{partnerId}/sponsored-adoptions")]
    [ApiController]
    public class CommunitySponsoredAdoptionsController : SecureController
    {
        private readonly ISponsoredAdoptionManager adoptionManager;
        private readonly IKeyedManager<Partner> partnerManager;

        /// <summary>
        /// Initializes a new instance of the <see cref="CommunitySponsoredAdoptionsController"/> class.
        /// </summary>
        /// <param name="adoptionManager">The sponsored adoption manager.</param>
        /// <param name="partnerManager">The partner manager.</param>
        public CommunitySponsoredAdoptionsController(
            ISponsoredAdoptionManager adoptionManager,
            IKeyedManager<Partner> partnerManager)
        {
            this.adoptionManager = adoptionManager;
            this.partnerManager = partnerManager;
        }

        /// <summary>
        /// Gets all sponsored adoptions for a community.
        /// </summary>
        /// <param name="partnerId">The community (partner) ID.</param>
        /// <param name="cancellationToken">Cancellation token.</param>
        [HttpGet]
        [Authorize(Policy = AuthorizationPolicyConstants.ValidUser)]
        [RequiredScope(Constants.TrashMobReadScope)]
        [ProducesResponseType(typeof(IEnumerable<SponsoredAdoption>), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetSponsoredAdoptions(Guid partnerId, CancellationToken cancellationToken)
        {
            var partner = await partnerManager.GetAsync(partnerId, cancellationToken);
            if (partner == null)
            {
                return NotFound();
            }

            var authResult = await AuthorizationService.AuthorizeAsync(
                User, partner, AuthorizationPolicyConstants.UserIsPartnerUserOrIsAdmin);
            if (!authResult.Succeeded)
            {
                return Forbid();
            }

            var adoptions = await adoptionManager.GetByCommunityAsync(partnerId, cancellationToken);
            return Ok(adoptions);
        }

        /// <summary>
        /// Gets a single sponsored adoption by ID.
        /// </summary>
        /// <param name="partnerId">The community (partner) ID.</param>
        /// <param name="id">The sponsored adoption ID.</param>
        /// <param name="cancellationToken">Cancellation token.</param>
        [HttpGet("{id}")]
        [Authorize(Policy = AuthorizationPolicyConstants.ValidUser)]
        [RequiredScope(Constants.TrashMobReadScope)]
        [ProducesResponseType(typeof(SponsoredAdoption), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetSponsoredAdoption(Guid partnerId, Guid id, CancellationToken cancellationToken)
        {
            var partner = await partnerManager.GetAsync(partnerId, cancellationToken);
            if (partner == null)
            {
                return NotFound();
            }

            var authResult = await AuthorizationService.AuthorizeAsync(
                User, partner, AuthorizationPolicyConstants.UserIsPartnerUserOrIsAdmin);
            if (!authResult.Succeeded)
            {
                return Forbid();
            }

            var adoption = await adoptionManager.GetAsync(id, cancellationToken);
            if (adoption == null)
            {
                return NotFound();
            }

            return Ok(adoption);
        }

        /// <summary>
        /// Creates a new sponsored adoption.
        /// </summary>
        /// <param name="partnerId">The community (partner) ID.</param>
        /// <param name="adoption">The sponsored adoption to create.</param>
        /// <param name="cancellationToken">Cancellation token.</param>
        [HttpPost]
        [Authorize(Policy = AuthorizationPolicyConstants.ValidUser)]
        [RequiredScope(Constants.TrashMobWriteScope)]
        [ProducesResponseType(typeof(SponsoredAdoption), StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> CreateSponsoredAdoption(
            Guid partnerId,
            [FromBody] SponsoredAdoption adoption,
            CancellationToken cancellationToken)
        {
            var partner = await partnerManager.GetAsync(partnerId, cancellationToken);
            if (partner == null)
            {
                return NotFound();
            }

            var authResult = await AuthorizationService.AuthorizeAsync(
                User, partner, AuthorizationPolicyConstants.UserIsPartnerUserOrIsAdmin);
            if (!authResult.Succeeded)
            {
                return Forbid();
            }

            adoption.CreatedByUserId = UserId;
            adoption.LastUpdatedByUserId = UserId;

            var created = await adoptionManager.AddAsync(adoption, UserId, cancellationToken);
            return CreatedAtAction(nameof(GetSponsoredAdoption), new { partnerId, id = created.Id }, created);
        }

        /// <summary>
        /// Updates an existing sponsored adoption (e.g. terminate, expire, change frequency).
        /// </summary>
        /// <param name="partnerId">The community (partner) ID.</param>
        /// <param name="id">The sponsored adoption ID.</param>
        /// <param name="adoption">The updated sponsored adoption data.</param>
        /// <param name="cancellationToken">Cancellation token.</param>
        [HttpPut("{id}")]
        [Authorize(Policy = AuthorizationPolicyConstants.ValidUser)]
        [RequiredScope(Constants.TrashMobWriteScope)]
        [ProducesResponseType(typeof(SponsoredAdoption), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> UpdateSponsoredAdoption(
            Guid partnerId,
            Guid id,
            [FromBody] SponsoredAdoption adoption,
            CancellationToken cancellationToken)
        {
            var partner = await partnerManager.GetAsync(partnerId, cancellationToken);
            if (partner == null)
            {
                return NotFound();
            }

            var authResult = await AuthorizationService.AuthorizeAsync(
                User, partner, AuthorizationPolicyConstants.UserIsPartnerUserOrIsAdmin);
            if (!authResult.Succeeded)
            {
                return Forbid();
            }

            var existing = await adoptionManager.GetAsync(id, cancellationToken);
            if (existing == null)
            {
                return NotFound();
            }

            adoption.Id = id;
            adoption.LastUpdatedByUserId = UserId;

            var updated = await adoptionManager.UpdateAsync(adoption, UserId, cancellationToken);
            return Ok(updated);
        }

        /// <summary>
        /// Gets compliance statistics for a community's sponsored adoption program.
        /// </summary>
        /// <param name="partnerId">The community (partner) ID.</param>
        /// <param name="cancellationToken">Cancellation token.</param>
        [HttpGet("compliance")]
        [Authorize(Policy = AuthorizationPolicyConstants.ValidUser)]
        [RequiredScope(Constants.TrashMobReadScope)]
        [ProducesResponseType(typeof(SponsoredAdoptionComplianceStats), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetComplianceStats(Guid partnerId, CancellationToken cancellationToken)
        {
            var partner = await partnerManager.GetAsync(partnerId, cancellationToken);
            if (partner == null)
            {
                return NotFound();
            }

            var authResult = await AuthorizationService.AuthorizeAsync(
                User, partner, AuthorizationPolicyConstants.UserIsPartnerUserOrIsAdmin);
            if (!authResult.Succeeded)
            {
                return Forbid();
            }

            var stats = await adoptionManager.GetComplianceByCommunityAsync(partnerId, cancellationToken);
            return Ok(stats);
        }
    }
}
