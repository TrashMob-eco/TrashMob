namespace TrashMob.Controllers
{
    using System.Threading;
    using System.Threading.Tasks;
    using Microsoft.AspNetCore.Authorization;
    using Microsoft.AspNetCore.Mvc;
    using TrashMob.Security;
    using TrashMob.Shared.Managers.Interfaces;

    [Route("api/waivers")]
    public class WaiverController : BaseController
    {
        private readonly IWaiverManager waiverManager;

        public WaiverController(IWaiverManager waiverManager)
        {
            this.waiverManager = waiverManager;
        }

        [Authorize(Policy = AuthorizationPolicyConstants.ValidUser)]
        [HttpGet("{waiverName}")]
        public async Task<IActionResult> GetWaiver(string waiverName, CancellationToken cancellationToken)
        {
            var waiver = await waiverManager.GetByNameAsync(waiverName, cancellationToken);
            return Ok(waiver);
        }
    }
}