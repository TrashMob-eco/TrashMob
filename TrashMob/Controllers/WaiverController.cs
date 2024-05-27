namespace TrashMob.Controllers
{
    using System.Threading;
    using System.Threading.Tasks;
    using Microsoft.AspNetCore.Mvc;
    using TrashMob.Shared.Managers.Interfaces;

    [Route("api/waivers")]
    public class WaiverController : BaseController
    {
        private readonly IWaiverManager waiverManager;

        public WaiverController(IWaiverManager waiverManager)
        {
            this.waiverManager = waiverManager;
        }

        [HttpGet("{waiverName}")]
        public async Task<IActionResult> GetWaiver(string waiverName, CancellationToken cancellationToken)
        {
            var waiver = await waiverManager.GetByNameAsync(waiverName, cancellationToken);
            return Ok(waiver);
        }
    }
}