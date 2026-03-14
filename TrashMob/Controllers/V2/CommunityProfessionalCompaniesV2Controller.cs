namespace TrashMob.Controllers.V2
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
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

    /// <summary>
    /// V2 controller for managing professional cleanup companies within a community.
    /// </summary>
    [ApiController]
    [ApiVersion("2.0")]
    [EnableCors("_myAllowSpecificOrigins")]
    [Route("api/v{version:apiVersion}/communities/{partnerId}/professional-companies")]
    public class CommunityProfessionalCompaniesV2Controller(
        IProfessionalCompanyManager companyManager,
        IProfessionalCompanyUserManager companyUserManager,
        IKeyedManager<Partner> partnerManager,
        IAuthorizationService authorizationService,
        ILogger<CommunityProfessionalCompaniesV2Controller> logger) : ControllerBase
    {
        private Guid UserId => new(HttpContext.Items["UserId"]?.ToString() ?? string.Empty);

        /// <summary>
        /// Gets all active professional companies for a community.
        /// </summary>
        /// <param name="partnerId">The community (partner) ID.</param>
        /// <param name="cancellationToken">Cancellation token.</param>
        /// <response code="200">Returns the professional companies.</response>
        /// <response code="403">Not authorized.</response>
        /// <response code="404">Partner not found.</response>
        [HttpGet]
        [Authorize(Policy = AuthorizationPolicyConstants.ValidUser)]
        [RequiredScope(Constants.TrashMobReadScope)]
        [ProducesResponseType(typeof(IEnumerable<ProfessionalCompanyDto>), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetCompanies(Guid partnerId, CancellationToken cancellationToken)
        {
            logger.LogInformation("V2 GetCompanies for Partner={PartnerId}", partnerId);

            var partner = await partnerManager.GetAsync(partnerId, cancellationToken);
            if (partner is null)
            {
                return NotFound();
            }

            if (!await IsAuthorizedAsync(partner, AuthorizationPolicyConstants.UserIsPartnerUserOrIsAdmin))
            {
                return Forbid();
            }

            var companies = await companyManager.GetByCommunityAsync(partnerId, cancellationToken);
            return Ok(companies.Select(c => c.ToV2Dto()));
        }

        /// <summary>
        /// Gets a single professional company by ID.
        /// </summary>
        /// <param name="partnerId">The community (partner) ID.</param>
        /// <param name="companyId">The professional company ID.</param>
        /// <param name="cancellationToken">Cancellation token.</param>
        /// <response code="200">Returns the professional company.</response>
        /// <response code="403">Not authorized.</response>
        /// <response code="404">Partner or company not found.</response>
        [HttpGet("{companyId}")]
        [Authorize(Policy = AuthorizationPolicyConstants.ValidUser)]
        [RequiredScope(Constants.TrashMobReadScope)]
        [ProducesResponseType(typeof(ProfessionalCompanyDto), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetCompany(Guid partnerId, Guid companyId, CancellationToken cancellationToken)
        {
            logger.LogInformation("V2 GetCompany for Partner={PartnerId}, Company={CompanyId}", partnerId, companyId);

            var partner = await partnerManager.GetAsync(partnerId, cancellationToken);
            if (partner is null)
            {
                return NotFound();
            }

            if (!await IsAuthorizedAsync(partner, AuthorizationPolicyConstants.UserIsPartnerUserOrIsAdmin))
            {
                return Forbid();
            }

            var company = await companyManager.GetAsync(companyId, cancellationToken);
            if (company is null || company.PartnerId != partnerId)
            {
                return NotFound();
            }

            return Ok(company.ToV2Dto());
        }

        /// <summary>
        /// Creates a new professional company for a community.
        /// </summary>
        /// <param name="partnerId">The community (partner) ID.</param>
        /// <param name="companyDto">The professional company to create.</param>
        /// <param name="cancellationToken">Cancellation token.</param>
        /// <response code="201">Professional company created.</response>
        /// <response code="400">Invalid data.</response>
        /// <response code="403">Not authorized.</response>
        /// <response code="404">Partner not found.</response>
        [HttpPost]
        [Authorize(Policy = AuthorizationPolicyConstants.ValidUser)]
        [RequiredScope(Constants.TrashMobWriteScope)]
        [ProducesResponseType(typeof(ProfessionalCompanyDto), StatusCodes.Status201Created)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
        public async Task<IActionResult> CreateCompany(
            Guid partnerId,
            [FromBody] ProfessionalCompanyDto companyDto,
            CancellationToken cancellationToken)
        {
            logger.LogInformation("V2 CreateCompany for Partner={PartnerId}", partnerId);

            var partner = await partnerManager.GetAsync(partnerId, cancellationToken);
            if (partner is null)
            {
                return NotFound();
            }

            if (!await IsAuthorizedAsync(partner, AuthorizationPolicyConstants.UserIsPartnerUserOrIsAdmin))
            {
                return Forbid();
            }

            var entity = companyDto.ToEntity();
            entity.PartnerId = partnerId;
            entity.CreatedByUserId = UserId;
            entity.LastUpdatedByUserId = UserId;

            var created = await companyManager.AddAsync(entity, UserId, cancellationToken);
            return CreatedAtAction(nameof(GetCompany), new { partnerId, companyId = created.Id }, created.ToV2Dto());
        }

        /// <summary>
        /// Updates an existing professional company.
        /// </summary>
        /// <param name="partnerId">The community (partner) ID.</param>
        /// <param name="companyId">The professional company ID.</param>
        /// <param name="companyDto">The updated company data.</param>
        /// <param name="cancellationToken">Cancellation token.</param>
        /// <response code="200">Returns the updated professional company.</response>
        /// <response code="400">Invalid data.</response>
        /// <response code="403">Not authorized.</response>
        /// <response code="404">Partner or company not found.</response>
        [HttpPut("{companyId}")]
        [Authorize(Policy = AuthorizationPolicyConstants.ValidUser)]
        [RequiredScope(Constants.TrashMobWriteScope)]
        [ProducesResponseType(typeof(ProfessionalCompanyDto), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
        public async Task<IActionResult> UpdateCompany(
            Guid partnerId,
            Guid companyId,
            [FromBody] ProfessionalCompanyDto companyDto,
            CancellationToken cancellationToken)
        {
            logger.LogInformation("V2 UpdateCompany for Partner={PartnerId}, Company={CompanyId}", partnerId, companyId);

            var partner = await partnerManager.GetAsync(partnerId, cancellationToken);
            if (partner is null)
            {
                return NotFound();
            }

            if (!await IsAuthorizedAsync(partner, AuthorizationPolicyConstants.UserIsPartnerUserOrIsAdmin))
            {
                return Forbid();
            }

            var existing = await companyManager.GetAsync(companyId, cancellationToken);
            if (existing is null || existing.PartnerId != partnerId)
            {
                return NotFound();
            }

            var entity = companyDto.ToEntity();
            entity.Id = companyId;
            entity.PartnerId = partnerId;
            entity.LastUpdatedByUserId = UserId;

            var updated = await companyManager.UpdateAsync(entity, UserId, cancellationToken);
            return Ok(updated.ToV2Dto());
        }

        /// <summary>
        /// Assigns a user to a professional company.
        /// </summary>
        /// <param name="partnerId">The community (partner) ID.</param>
        /// <param name="companyId">The professional company ID.</param>
        /// <param name="companyUserDto">The company user assignment.</param>
        /// <param name="cancellationToken">Cancellation token.</param>
        /// <response code="201">User assigned to company.</response>
        /// <response code="400">Invalid data.</response>
        /// <response code="403">Not authorized.</response>
        /// <response code="404">Partner or company not found.</response>
        [HttpPost("{companyId}/users")]
        [Authorize(Policy = AuthorizationPolicyConstants.ValidUser)]
        [RequiredScope(Constants.TrashMobWriteScope)]
        [ProducesResponseType(typeof(ProfessionalCompanyUserDto), StatusCodes.Status201Created)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
        public async Task<IActionResult> AssignUser(
            Guid partnerId,
            Guid companyId,
            [FromBody] ProfessionalCompanyUserDto companyUserDto,
            CancellationToken cancellationToken)
        {
            logger.LogInformation("V2 AssignUser for Partner={PartnerId}, Company={CompanyId}", partnerId, companyId);

            var partner = await partnerManager.GetAsync(partnerId, cancellationToken);
            if (partner is null)
            {
                return NotFound();
            }

            if (!await IsAuthorizedAsync(partner, AuthorizationPolicyConstants.UserIsPartnerUserOrIsAdmin))
            {
                return Forbid();
            }

            var company = await companyManager.GetAsync(companyId, cancellationToken);
            if (company is null || company.PartnerId != partnerId)
            {
                return NotFound();
            }

            var entity = companyUserDto.ToEntity();
            entity.ProfessionalCompanyId = companyId;
            entity.CreatedByUserId = UserId;
            entity.LastUpdatedByUserId = UserId;

            var created = await companyUserManager.AddAsync(entity, UserId, cancellationToken);
            return StatusCode(StatusCodes.Status201Created, created.ToV2Dto());
        }

        /// <summary>
        /// Removes a user from a professional company.
        /// </summary>
        /// <param name="partnerId">The community (partner) ID.</param>
        /// <param name="companyId">The professional company ID.</param>
        /// <param name="userId">The user ID to remove.</param>
        /// <param name="cancellationToken">Cancellation token.</param>
        /// <response code="204">User removed from company.</response>
        /// <response code="403">Not authorized.</response>
        /// <response code="404">Partner or company not found.</response>
        [HttpDelete("{companyId}/users/{userId}")]
        [Authorize(Policy = AuthorizationPolicyConstants.ValidUser)]
        [RequiredScope(Constants.TrashMobWriteScope)]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
        public async Task<IActionResult> RemoveUser(
            Guid partnerId,
            Guid companyId,
            Guid userId,
            CancellationToken cancellationToken)
        {
            logger.LogInformation("V2 RemoveUser for Partner={PartnerId}, Company={CompanyId}, User={TargetUserId}",
                partnerId, companyId, userId);

            var partner = await partnerManager.GetAsync(partnerId, cancellationToken);
            if (partner is null)
            {
                return NotFound();
            }

            if (!await IsAuthorizedAsync(partner, AuthorizationPolicyConstants.UserIsPartnerUserOrIsAdmin))
            {
                return Forbid();
            }

            var company = await companyManager.GetAsync(companyId, cancellationToken);
            if (company is null || company.PartnerId != partnerId)
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
        /// <response code="200">Returns the company users.</response>
        /// <response code="403">Not authorized.</response>
        /// <response code="404">Partner or company not found.</response>
        [HttpGet("{companyId}/users")]
        [Authorize(Policy = AuthorizationPolicyConstants.ValidUser)]
        [RequiredScope(Constants.TrashMobReadScope)]
        [ProducesResponseType(typeof(IEnumerable<UserDto>), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetUsers(Guid partnerId, Guid companyId, CancellationToken cancellationToken)
        {
            logger.LogInformation("V2 GetCompanyUsers for Partner={PartnerId}, Company={CompanyId}", partnerId, companyId);

            var partner = await partnerManager.GetAsync(partnerId, cancellationToken);
            if (partner is null)
            {
                return NotFound();
            }

            if (!await IsAuthorizedAsync(partner, AuthorizationPolicyConstants.UserIsPartnerUserOrIsAdmin))
            {
                return Forbid();
            }

            var company = await companyManager.GetAsync(companyId, cancellationToken);
            if (company is null || company.PartnerId != partnerId)
            {
                return NotFound();
            }

            var users = await companyUserManager.GetUsersForCompanyAsync(companyId, cancellationToken);
            return Ok(users.Select(u => u.ToV2Dto()));
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
