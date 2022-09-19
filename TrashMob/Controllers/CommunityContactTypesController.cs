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
    [Route("api/communitycontacttypes")]
    public class CommunityContactTypesController : BaseController
    {
        private readonly ILookupManager<CommunityContactType> manager;

        public CommunityContactTypesController(ILookupManager<CommunityContactType> manager, 
                                             TelemetryClient telemetryClient)
            : base(telemetryClient)
        {
            this.manager = manager;
        }

        [HttpGet]
        public async Task<IActionResult> GetCommunityContactTypes()
        {
            var types = await manager.Get().ToListAsync();
            TelemetryClient.TrackEvent(nameof(GetCommunityContactTypes));

            return Ok(types);
        }
    }
}
