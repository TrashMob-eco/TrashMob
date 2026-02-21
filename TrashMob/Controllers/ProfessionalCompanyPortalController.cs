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
    public class ProfessionalCompanyPortalController(IProfessionalCompanyUserManager companyUserManager)
        : SecureController
    {

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
