namespace TrashMob.Controllers
{
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
    /// Portal endpoints for professional company users.
    /// </summary>
    [Route("api/professional-companies")]
    [ApiController]
    public class ProfessionalCompanyPortalController : SecureController
    {
        private readonly IProfessionalCompanyUserManager companyUserManager;

        /// <summary>
        /// Initializes a new instance of the <see cref="ProfessionalCompanyPortalController"/> class.
        /// </summary>
        /// <param name="companyUserManager">The professional company user manager.</param>
        public ProfessionalCompanyPortalController(IProfessionalCompanyUserManager companyUserManager)
        {
            this.companyUserManager = companyUserManager;
        }

        /// <summary>
        /// Gets all professional companies the authenticated user is assigned to.
        /// </summary>
        /// <param name="cancellationToken">Cancellation token.</param>
        [HttpGet("mine")]
        [Authorize(Policy = AuthorizationPolicyConstants.ValidUser)]
        [RequiredScope(Constants.TrashMobReadScope)]
        [ProducesResponseType(typeof(IEnumerable<ProfessionalCompany>), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetMyCompanies(CancellationToken cancellationToken)
        {
            var companies = await companyUserManager.GetCompaniesByUserIdAsync(UserId, cancellationToken);
            return Ok(companies);
        }
    }
}
