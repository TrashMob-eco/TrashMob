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
    /// V2 controller for professional company portal operations.
    /// </summary>
    [ApiController]
    [ApiVersion("2.0")]
    [EnableCors("_myAllowSpecificOrigins")]
    [Route("api/v{version:apiVersion}/professional-companies")]
    public class ProfessionalCompanyPortalV2Controller(
        IProfessionalCompanyUserManager companyUserManager,
        ILogger<ProfessionalCompanyPortalV2Controller> logger) : ControllerBase
    {
        private Guid UserId => Guid.TryParse(HttpContext.Items["UserId"]?.ToString(), out var parsedUserId) ? parsedUserId : Guid.Empty;

        /// <summary>
        /// Gets all professional companies the current user is assigned to.
        /// </summary>
        /// <param name="cancellationToken">Cancellation token.</param>
        /// <response code="200">Returns the professional companies.</response>
        [HttpGet("mine")]
        [Authorize(Policy = AuthorizationPolicyConstants.ValidUser)]
        [RequiredScope(Constants.TrashMobReadScope)]
        [ProducesResponseType(typeof(IEnumerable<ProfessionalCompanyDto>), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetMyCompanies(CancellationToken cancellationToken)
        {
            logger.LogInformation("V2 GetMyCompanies for User={UserId}", UserId);

            var companies = await companyUserManager.GetCompaniesByUserIdAsync(UserId, cancellationToken);
            return Ok(companies.Select(c => c.ToV2Dto()));
        }
    }
}
