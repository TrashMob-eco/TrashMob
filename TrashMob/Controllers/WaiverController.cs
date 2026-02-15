namespace TrashMob.Controllers
{
    using System.Threading;
    using System.Threading.Tasks;
    using Microsoft.AspNetCore.Authorization;
    using Microsoft.AspNetCore.Mvc;
    using TrashMob.Security;
    using TrashMob.Shared.Managers.Interfaces;

    /// <summary>
    /// Controller for retrieving waiver documents.
    /// </summary>
    [Route("api/waivers/by-name")]
    public class WaiverController(IWaiverManager waiverManager) : BaseController
    {
        /// <summary>
        /// Gets a waiver by name. Requires a valid user.
        /// </summary>
        /// <param name="waiverName">The name of the waiver.</param>
        /// <param name="cancellationToken">Cancellation token.</param>
        /// <remarks>The waiver document.</remarks>
        [Authorize(Policy = AuthorizationPolicyConstants.ValidUser)]
        [HttpGet("{waiverName}")]
        public async Task<IActionResult> GetWaiver(string waiverName, CancellationToken cancellationToken)
        {
            var waiver = await waiverManager.GetByNameAsync(waiverName, cancellationToken);
            return Ok(waiver);
        }
    }
}