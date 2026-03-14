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
    using TrashMob.Shared.Poco;

    /// <summary>
    /// V2 controller for professional company cleanup log operations.
    /// </summary>
    [ApiController]
    [ApiVersion("2.0")]
    [EnableCors("_myAllowSpecificOrigins")]
    [Route("api/v{version:apiVersion}/professional-companies/{companyId}/cleanup-logs")]
    public class ProfessionalCleanupLogsV2Controller(
        IProfessionalCleanupLogManager logManager,
        IProfessionalCompanyManager companyManager,
        ISponsoredAdoptionManager adoptionManager,
        IAuthorizationService authorizationService,
        ILogger<ProfessionalCleanupLogsV2Controller> logger) : ControllerBase
    {
        private Guid UserId => new(HttpContext.Items["UserId"]?.ToString() ?? string.Empty);

        /// <summary>
        /// Gets all cleanup logs for a professional company.
        /// </summary>
        /// <param name="companyId">The professional company ID.</param>
        /// <param name="cancellationToken">Cancellation token.</param>
        /// <response code="200">Returns the cleanup logs.</response>
        /// <response code="403">Not authorized.</response>
        /// <response code="404">Company not found.</response>
        [HttpGet]
        [Authorize(Policy = AuthorizationPolicyConstants.ValidUser)]
        [RequiredScope(Constants.TrashMobReadScope)]
        [ProducesResponseType(typeof(IEnumerable<ProfessionalCleanupLogDto>), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetLogs(Guid companyId, CancellationToken cancellationToken)
        {
            logger.LogInformation("V2 GetLogs for Company={CompanyId}", companyId);

            var company = await companyManager.GetAsync(companyId, cancellationToken);
            if (company is null)
            {
                return NotFound();
            }

            if (!await IsAuthorizedAsync(company, AuthorizationPolicyConstants.UserIsProfessionalCompanyUserOrIsAdmin))
            {
                return Forbid();
            }

            var logs = await logManager.GetByCompanyIdAsync(companyId, cancellationToken);
            return Ok(logs.Select(l => l.ToV2Dto()));
        }

        /// <summary>
        /// Logs a professional cleanup for a company.
        /// </summary>
        /// <param name="companyId">The professional company ID.</param>
        /// <param name="logDto">The cleanup log data.</param>
        /// <param name="cancellationToken">Cancellation token.</param>
        /// <response code="201">Cleanup log created.</response>
        /// <response code="400">Invalid data or validation failure.</response>
        /// <response code="403">Not authorized.</response>
        /// <response code="404">Company not found.</response>
        [HttpPost]
        [Authorize(Policy = AuthorizationPolicyConstants.ValidUser)]
        [RequiredScope(Constants.TrashMobWriteScope)]
        [ProducesResponseType(typeof(ProfessionalCleanupLogDto), StatusCodes.Status201Created)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
        public async Task<IActionResult> LogCleanup(
            Guid companyId,
            [FromBody] ProfessionalCleanupLogDto logDto,
            CancellationToken cancellationToken)
        {
            logger.LogInformation("V2 LogCleanup for Company={CompanyId}", companyId);

            var company = await companyManager.GetAsync(companyId, cancellationToken);
            if (company is null)
            {
                return NotFound();
            }

            if (!await IsAuthorizedAsync(company, AuthorizationPolicyConstants.UserIsProfessionalCompanyUserOrIsAdmin))
            {
                return Forbid();
            }

            var entity = logDto.ToEntity();
            entity.ProfessionalCompanyId = companyId;
            entity.CreatedByUserId = UserId;
            entity.LastUpdatedByUserId = UserId;

            var result = await logManager.LogCleanupAsync(entity, UserId, cancellationToken);

            if (result.IsSuccess)
            {
                return CreatedAtAction(nameof(GetLogs), new { companyId }, result.Data.ToV2Dto());
            }

            return BadRequest(result.ErrorMessage);
        }

        /// <summary>
        /// Gets all sponsored adoption assignments for a professional company.
        /// </summary>
        /// <param name="companyId">The professional company ID.</param>
        /// <param name="cancellationToken">Cancellation token.</param>
        /// <response code="200">Returns the sponsored adoption assignments.</response>
        /// <response code="403">Not authorized.</response>
        /// <response code="404">Company not found.</response>
        [HttpGet("assignments")]
        [Authorize(Policy = AuthorizationPolicyConstants.ValidUser)]
        [RequiredScope(Constants.TrashMobReadScope)]
        [ProducesResponseType(typeof(IEnumerable<SponsoredAdoptionDto>), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetAssignments(Guid companyId, CancellationToken cancellationToken)
        {
            logger.LogInformation("V2 GetAssignments for Company={CompanyId}", companyId);

            var company = await companyManager.GetAsync(companyId, cancellationToken);
            if (company is null)
            {
                return NotFound();
            }

            if (!await IsAuthorizedAsync(company, AuthorizationPolicyConstants.UserIsProfessionalCompanyUserOrIsAdmin))
            {
                return Forbid();
            }

            var adoptions = await adoptionManager.GetByCompanyIdAsync(companyId, cancellationToken);
            return Ok(adoptions.Select(a => a.ToV2Dto()));
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
