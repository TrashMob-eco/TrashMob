namespace TrashMob.Controllers
{
    using Microsoft.ApplicationInsights;
    using Microsoft.AspNetCore.Authorization;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.EntityFrameworkCore;
    using System.Threading.Tasks;
    using TrashMob.Shared.Managers;
    using TrashMob.Shared.Models;

    [Authorize]
    [Route("api/communitystatuses")]
    public class CommunityStatusesController : BaseController
    {
        private readonly ILookupManager<CommunityStatus> manager;

        public CommunityStatusesController(ILookupManager<CommunityStatus> manager, 
                                           TelemetryClient telemetryClient)
            : base(telemetryClient)
        {
            this.manager = manager;
        }

        [HttpGet]
        public async Task<IActionResult> GetCommunityStatuses()
        {
            var types = await manager.Get().ToListAsync();
            TelemetryClient.TrackEvent(nameof(GetCommunityStatuses));

            return Ok(types);
        }
    }
}
