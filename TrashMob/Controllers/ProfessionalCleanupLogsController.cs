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
    /// Controller for professional cleanup log operations.
    /// </summary>
    [Route("api/professional-companies/{companyId}/cleanup-logs")]
    public class ProfessionalCleanupLogsController : SecureController
    {
        private readonly IProfessionalCleanupLogManager logManager;
        private readonly IProfessionalCompanyManager companyManager;
        private readonly ISponsoredAdoptionManager adoptionManager;

        /// <summary>
        /// Initializes a new instance of the <see cref="ProfessionalCleanupLogsController"/> class.
        /// </summary>
        /// <param name="logManager">The cleanup log manager.</param>
        /// <param name="companyManager">The professional company manager.</param>
        /// <param name="adoptionManager">The sponsored adoption manager.</param>
        public ProfessionalCleanupLogsController(
            IProfessionalCleanupLogManager logManager,
            IProfessionalCompanyManager companyManager,
            ISponsoredAdoptionManager adoptionManager)
        {
            this.logManager = logManager;
            this.companyManager = companyManager;
            this.adoptionManager = adoptionManager;
        }

        /// <summary>
        /// Gets all cleanup logs for a professional company.
        /// </summary>
        /// <param name="companyId">The professional company ID.</param>
        /// <param name="cancellationToken">Cancellation token.</param>
        [HttpGet]
        [Authorize(Policy = AuthorizationPolicyConstants.ValidUser)]
        [RequiredScope(Constants.TrashMobReadScope)]
        [ProducesResponseType(typeof(IEnumerable<ProfessionalCleanupLog>), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetLogs(Guid companyId, CancellationToken cancellationToken)
        {
            var company = await companyManager.GetAsync(companyId, cancellationToken);
            if (company == null)
            {
                return NotFound();
            }

            if (!await IsAuthorizedAsync(company, AuthorizationPolicyConstants.UserIsProfessionalCompanyUserOrIsAdmin))
            {
                return Forbid();
            }

            var logs = await logManager.GetByCompanyIdAsync(companyId, cancellationToken);
            return Ok(logs);
        }

        /// <summary>
        /// Logs a professional cleanup. Validates the company assignment and adoption status.
        /// </summary>
        /// <param name="companyId">The professional company ID.</param>
        /// <param name="log">The cleanup log to create.</param>
        /// <param name="cancellationToken">Cancellation token.</param>
        [HttpPost]
        [Authorize(Policy = AuthorizationPolicyConstants.ValidUser)]
        [RequiredScope(Constants.TrashMobWriteScope)]
        [ProducesResponseType(typeof(ProfessionalCleanupLog), StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> LogCleanup(
            Guid companyId,
            [FromBody] ProfessionalCleanupLog log,
            CancellationToken cancellationToken)
        {
            var company = await companyManager.GetAsync(companyId, cancellationToken);
            if (company == null)
            {
                return NotFound();
            }

            if (!await IsAuthorizedAsync(company, AuthorizationPolicyConstants.UserIsProfessionalCompanyUserOrIsAdmin))
            {
                return Forbid();
            }

            log.ProfessionalCompanyId = companyId;
            log.CreatedByUserId = UserId;
            log.LastUpdatedByUserId = UserId;

            var result = await logManager.LogCleanupAsync(log, UserId, cancellationToken);

            if (!result.IsSuccess)
            {
                return BadRequest(result.ErrorMessage);
            }

            return CreatedAtAction(null, result.Data);
        }

        /// <summary>
        /// Gets active sponsored adoptions assigned to this professional company.
        /// </summary>
        /// <param name="companyId">The professional company ID.</param>
        /// <param name="cancellationToken">Cancellation token.</param>
        [HttpGet("assignments")]
        [Authorize(Policy = AuthorizationPolicyConstants.ValidUser)]
        [RequiredScope(Constants.TrashMobReadScope)]
        [ProducesResponseType(typeof(IEnumerable<SponsoredAdoption>), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetAssignments(Guid companyId, CancellationToken cancellationToken)
        {
            var company = await companyManager.GetAsync(companyId, cancellationToken);
            if (company == null)
            {
                return NotFound();
            }

            if (!await IsAuthorizedAsync(company, AuthorizationPolicyConstants.UserIsProfessionalCompanyUserOrIsAdmin))
            {
                return Forbid();
            }

            var adoptions = await adoptionManager.GetByCompanyIdAsync(companyId, cancellationToken);
            return Ok(adoptions);
        }
    }
}
