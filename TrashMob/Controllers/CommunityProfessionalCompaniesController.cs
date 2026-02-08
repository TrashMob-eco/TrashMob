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
    /// Controller for managing professional cleanup companies within a community.
    /// </summary>
    [Route("api/communities/{partnerId}/professional-companies")]
    [ApiController]
    public class CommunityProfessionalCompaniesController : SecureController
    {
        private readonly IProfessionalCompanyManager companyManager;
        private readonly IProfessionalCompanyUserManager companyUserManager;
        private readonly IKeyedManager<Partner> partnerManager;

        /// <summary>
        /// Initializes a new instance of the <see cref="CommunityProfessionalCompaniesController"/> class.
        /// </summary>
        /// <param name="companyManager">The professional company manager.</param>
        /// <param name="companyUserManager">The professional company user manager.</param>
        /// <param name="partnerManager">The partner manager.</param>
        public CommunityProfessionalCompaniesController(
            IProfessionalCompanyManager companyManager,
            IProfessionalCompanyUserManager companyUserManager,
            IKeyedManager<Partner> partnerManager)
        {
            this.companyManager = companyManager;
            this.companyUserManager = companyUserManager;
            this.partnerManager = partnerManager;
        }

        /// <summary>
        /// Gets all active professional companies for a community.
        /// </summary>
        /// <param name="partnerId">The community (partner) ID.</param>
        /// <param name="cancellationToken">Cancellation token.</param>
        [HttpGet]
        [Authorize(Policy = AuthorizationPolicyConstants.ValidUser)]
        [RequiredScope(Constants.TrashMobReadScope)]
        [ProducesResponseType(typeof(IEnumerable<ProfessionalCompany>), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetCompanies(Guid partnerId, CancellationToken cancellationToken)
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

            var companies = await companyManager.GetByCommunityAsync(partnerId, cancellationToken);
            return Ok(companies);
        }

        /// <summary>
        /// Gets a single professional company by ID.
        /// </summary>
        /// <param name="partnerId">The community (partner) ID.</param>
        /// <param name="companyId">The professional company ID.</param>
        /// <param name="cancellationToken">Cancellation token.</param>
        [HttpGet("{companyId}")]
        [Authorize(Policy = AuthorizationPolicyConstants.ValidUser)]
        [RequiredScope(Constants.TrashMobReadScope)]
        [ProducesResponseType(typeof(ProfessionalCompany), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetCompany(Guid partnerId, Guid companyId, CancellationToken cancellationToken)
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

            var company = await companyManager.GetAsync(companyId, cancellationToken);
            if (company == null || company.PartnerId != partnerId)
            {
                return NotFound();
            }

            return Ok(company);
        }

        /// <summary>
        /// Creates a new professional company for a community.
        /// </summary>
        /// <param name="partnerId">The community (partner) ID.</param>
        /// <param name="company">The professional company to create.</param>
        /// <param name="cancellationToken">Cancellation token.</param>
        [HttpPost]
        [Authorize(Policy = AuthorizationPolicyConstants.ValidUser)]
        [RequiredScope(Constants.TrashMobWriteScope)]
        [ProducesResponseType(typeof(ProfessionalCompany), StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> CreateCompany(
            Guid partnerId,
            [FromBody] ProfessionalCompany company,
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

            company.PartnerId = partnerId;
            company.CreatedByUserId = UserId;
            company.LastUpdatedByUserId = UserId;

            var created = await companyManager.AddAsync(company, UserId, cancellationToken);
            return CreatedAtAction(nameof(GetCompany), new { partnerId, companyId = created.Id }, created);
        }

        /// <summary>
        /// Updates an existing professional company.
        /// </summary>
        /// <param name="partnerId">The community (partner) ID.</param>
        /// <param name="companyId">The professional company ID.</param>
        /// <param name="company">The updated company data.</param>
        /// <param name="cancellationToken">Cancellation token.</param>
        [HttpPut("{companyId}")]
        [Authorize(Policy = AuthorizationPolicyConstants.ValidUser)]
        [RequiredScope(Constants.TrashMobWriteScope)]
        [ProducesResponseType(typeof(ProfessionalCompany), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> UpdateCompany(
            Guid partnerId,
            Guid companyId,
            [FromBody] ProfessionalCompany company,
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

            var existing = await companyManager.GetAsync(companyId, cancellationToken);
            if (existing == null || existing.PartnerId != partnerId)
            {
                return NotFound();
            }

            company.Id = companyId;
            company.PartnerId = partnerId;
            company.LastUpdatedByUserId = UserId;

            var updated = await companyManager.UpdateAsync(company, UserId, cancellationToken);
            return Ok(updated);
        }

        /// <summary>
        /// Assigns a user to a professional company.
        /// </summary>
        /// <param name="partnerId">The community (partner) ID.</param>
        /// <param name="companyId">The professional company ID.</param>
        /// <param name="companyUser">The company user assignment.</param>
        /// <param name="cancellationToken">Cancellation token.</param>
        [HttpPost("{companyId}/users")]
        [Authorize(Policy = AuthorizationPolicyConstants.ValidUser)]
        [RequiredScope(Constants.TrashMobWriteScope)]
        [ProducesResponseType(typeof(ProfessionalCompanyUser), StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> AssignUser(
            Guid partnerId,
            Guid companyId,
            [FromBody] ProfessionalCompanyUser companyUser,
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

            var company = await companyManager.GetAsync(companyId, cancellationToken);
            if (company == null || company.PartnerId != partnerId)
            {
                return NotFound();
            }

            companyUser.ProfessionalCompanyId = companyId;
            companyUser.CreatedByUserId = UserId;
            companyUser.LastUpdatedByUserId = UserId;

            var created = await companyUserManager.AddAsync(companyUser, UserId, cancellationToken);
            return CreatedAtAction(null, created);
        }

        /// <summary>
        /// Removes a user from a professional company.
        /// </summary>
        /// <param name="partnerId">The community (partner) ID.</param>
        /// <param name="companyId">The professional company ID.</param>
        /// <param name="userId">The user ID to remove.</param>
        /// <param name="cancellationToken">Cancellation token.</param>
        [HttpDelete("{companyId}/users/{userId}")]
        [Authorize(Policy = AuthorizationPolicyConstants.ValidUser)]
        [RequiredScope(Constants.TrashMobWriteScope)]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> RemoveUser(
            Guid partnerId,
            Guid companyId,
            Guid userId,
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

            var company = await companyManager.GetAsync(companyId, cancellationToken);
            if (company == null || company.PartnerId != partnerId)
            {
                return NotFound();
            }

            await companyUserManager.Delete(companyId, userId, cancellationToken);
            return NoContent();
        }

        /// <summary>
        /// Gets all users assigned to a professional company.
        /// </summary>
        /// <param name="partnerId">The community (partner) ID.</param>
        /// <param name="companyId">The professional company ID.</param>
        /// <param name="cancellationToken">Cancellation token.</param>
        [HttpGet("{companyId}/users")]
        [Authorize(Policy = AuthorizationPolicyConstants.ValidUser)]
        [RequiredScope(Constants.TrashMobReadScope)]
        [ProducesResponseType(typeof(IEnumerable<User>), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetUsers(Guid partnerId, Guid companyId, CancellationToken cancellationToken)
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

            var company = await companyManager.GetAsync(companyId, cancellationToken);
            if (company == null || company.PartnerId != partnerId)
            {
                return NotFound();
            }

            var users = await companyUserManager.GetUsersForCompanyAsync(companyId, cancellationToken);
            return Ok(users);
        }
    }
}
