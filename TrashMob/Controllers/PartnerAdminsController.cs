namespace TrashMob.Controllers
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading;
    using System.Threading.Tasks;
    using Microsoft.AspNetCore.Authorization;
    using Microsoft.AspNetCore.Mvc;
    using TrashMob.Models;
    using TrashMob.Models.Poco;
    using TrashMob.Security;
    using TrashMob.Shared.Managers.Interfaces;
    using TrashMob.Shared.Poco;

    /// <summary>
    /// Controller for managing partner admins, including retrieval and assignment.
    /// </summary>
    [Authorize]
    [Route("api/partneradmins")]
    public class PartnerAdminsController : SecureController
    {
        private readonly IPartnerAdminManager partnerAdminManager;
        private readonly IKeyedManager<Partner> partnerManager;
        private readonly IKeyedManager<User> userManager;

        /// <summary>
        /// Initializes a new instance of the <see cref="PartnerAdminsController"/> class.
        /// </summary>
        /// <param name="userManager">The user manager.</param>
        /// <param name="partnerAdminManager">The partner admin manager.</param>
        /// <param name="partnerManager">The partner manager.</param>
        public PartnerAdminsController(IKeyedManager<User> userManager,
            IPartnerAdminManager partnerAdminManager,
            IKeyedManager<Partner> partnerManager)
        {
            this.partnerManager = partnerManager;
            this.userManager = userManager;
            this.partnerAdminManager = partnerAdminManager;
        }

        /// <summary>
        /// Gets all partner admins for a given partner.
        /// </summary>
        /// <param name="partnerId">The partner ID.</param>
        /// <param name="cancellationToken">Cancellation token.</param>
        /// <remarks>Returns a list of partner admins.</remarks>
        [HttpGet("{partnerId}")]
        [ProducesResponseType(typeof(IEnumerable<PartnerAdmin>), 200)]
        public async Task<IActionResult> GetPartnerAdmins(Guid partnerId, CancellationToken cancellationToken)
        {
            var partner = await partnerManager.GetAsync(partnerId, cancellationToken);
            var authResult = await AuthorizationService.AuthorizeAsync(User, partner,
                AuthorizationPolicyConstants.UserIsPartnerUserOrIsAdmin);

            if (!User.Identity.IsAuthenticated || !authResult.Succeeded)
            {
                return Forbid();
            }

            return Ok(await partnerAdminManager.GetAdminsForPartnerAsync(partnerId, cancellationToken));
        }

        /// <summary>
        /// Gets all partners for a given user. Requires a valid user.
        /// </summary>
        /// <param name="userId">The user ID.</param>
        /// <param name="cancellationToken">Cancellation token.</param>
        /// <remarks>List of partners for the user.</remarks>
        [HttpGet("getpartnersforuser/{userId}")]
        [Authorize(Policy = AuthorizationPolicyConstants.ValidUser)]
        public async Task<IActionResult> GetPartnersForUser(Guid userId, CancellationToken cancellationToken)
        {
            var partners = await partnerAdminManager.GetPartnersByUserIdAsync(userId, cancellationToken);
            return Ok(partners);
        }

        /// <summary>
        /// Retrieves a specific user associated with a partner.
        /// </summary>
        /// <param name="partnerId">The unique identifier of the partner the user belongs to.</param>
        /// <param name="userId">The unique identifier of the user to retrieve.
        /// 
        /// </param>
        /// <param name="cancellationToken">
        /// Token to cancel the operation.
        /// </param>
        /// <remarks>
        /// The partner user if found and the caller is authorized.
        /// </remarks>
        /// <response code="200">
        /// Returns the partner user.
        /// </response>
        /// <response code="403">
        /// Returned when the caller is not authenticated or is not authorized to access this partner.
        /// </response>
        /// <response code="404">
        /// Returned when the specified user does not exist for the given partner.
        /// </response>
        [HttpGet("{partnerId}/{userId}")]
        public async Task<IActionResult> GetPartnerUser(Guid partnerId, Guid userId,
            CancellationToken cancellationToken = default)
        {
            var partner = await partnerManager.GetAsync(partnerId, cancellationToken);
            var authResult = await AuthorizationService.AuthorizeAsync(User, partner,
                AuthorizationPolicyConstants.UserIsPartnerUserOrIsAdmin);

            if (!User.Identity.IsAuthenticated || !authResult.Succeeded)
            {
                return Forbid();
            }

            var partnerUser =
                (await partnerAdminManager.GetByParentIdAsync(partnerId, cancellationToken)).FirstOrDefault(pu =>
                    pu.UserId == userId);

            if (partnerUser == null)
            {
                return NotFound();
            }

            return Ok(partnerUser);
        }

        [HttpGet("users/{partnerId}")]
        public async Task<IActionResult> GetUsers(Guid partnerId, CancellationToken cancellationToken)
        {
            var partner = await partnerManager.GetAsync(partnerId, cancellationToken);
            var authResult = await AuthorizationService.AuthorizeAsync(User, partner,
                AuthorizationPolicyConstants.UserIsPartnerUserOrIsAdmin);

            if (!User.Identity.IsAuthenticated || !authResult.Succeeded)
            {
                return Forbid();
            }

            var partnerAdmins = await partnerAdminManager.GetByParentIdAsync(partnerId, cancellationToken);

            if (partnerAdmins == null || !partnerAdmins.Any())
            {
                return NotFound();
            }

            var users = new List<DisplayUser>();

            foreach (var pu in partnerAdmins)
            {
                var user = await userManager.GetAsync(pu.UserId, cancellationToken).ConfigureAwait(false);
                users.Add(user.ToDisplayUser());
            }

            return Ok(users);
        }

        /// <summary>
        /// Adds a user as a partner admin.
        /// </summary>
        /// <param name="partnerId">The partner ID.</param>
        /// <param name="userId">The user ID.</param>
        /// <param name="cancellationToken">Cancellation token.</param>
        /// <remarks>Returns the created partner admin.</remarks>
        [HttpPost("{partnerId}/{userId}")]
        [ProducesResponseType(typeof(PartnerAdmin), 201)]
        public async Task<IActionResult> AddPartnerUser(Guid partnerId, Guid userId,
            CancellationToken cancellationToken)
        {
            // Make sure the person adding the user is either an admin or already a user for the partner
            var partner = await partnerManager.GetAsync(partnerId, cancellationToken);
            var authResult = await AuthorizationService.AuthorizeAsync(User, partner,
                AuthorizationPolicyConstants.UserIsPartnerUserOrIsAdmin);

            if (!User.Identity.IsAuthenticated || !authResult.Succeeded)
            {
                return Forbid();
            }

            if (userId == Guid.Empty)
            {
                return BadRequest("UserId is required.");
            }

            var partnerUser = new PartnerAdmin
            {
                PartnerId = partnerId,
                UserId = userId,
                CreatedByUserId = UserId,
                LastUpdatedByUserId = UserId,
            };

            var result = await partnerAdminManager.AddAsync(partnerUser, UserId, cancellationToken)
                .ConfigureAwait(false);
            TelemetryClient.TrackEvent(nameof(AddPartnerUser));

            return CreatedAtAction(nameof(GetPartnerUser), new { partnerId, userId }, result);
        }
    }
}