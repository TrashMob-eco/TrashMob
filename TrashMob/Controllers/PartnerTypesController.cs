
namespace TrashMob.Controllers
{
    using System.Data.Entity;
    using System.Threading.Tasks;
    using Microsoft.ApplicationInsights;
    using Microsoft.AspNetCore.Mvc;
    using TrashMob.Shared.Managers;
    using TrashMob.Shared.Models;

    [Route("api/partnertypes")]
    public class PartnerTypesController : BaseController
    {
        private readonly ILookupManager<PartnerType> manager;

        public PartnerTypesController(ILookupManager<PartnerType> manager,
                                      TelemetryClient telemetryClient)
            : base(telemetryClient)
        {
            this.manager = manager;
        }

        [HttpGet]
        public async Task<IActionResult> GetPartnerTypes()
        {
            var types = await manager.Get().ToListAsync();
            TelemetryClient.TrackEvent(nameof(GetPartnerTypes));

            return Ok(types);
        }
    }
}
