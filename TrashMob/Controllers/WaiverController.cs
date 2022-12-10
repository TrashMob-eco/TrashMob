namespace TrashMob.Controllers
{
    using Microsoft.AspNetCore.Mvc;
    using System.Threading;
    using System.Threading.Tasks;
    using TrashMob.Shared.Managers.Interfaces;

    [Route("api/waivers")]
    public class WaiverController : BaseController
    {
        private readonly IWaiverManager waiverManager;

        public WaiverController(IWaiverManager waiverManager) : base()
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
