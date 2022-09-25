
namespace TrashMob.Controllers
{
    using System.Data.Entity;
    using System.Threading.Tasks;
    using Microsoft.ApplicationInsights;
    using Microsoft.AspNetCore.Mvc;
    using TrashMob.Models;
    using TrashMob.Shared.Managers.Interfaces;
    using TrashMob.Shared.Persistence.Interfaces;

    [Route("api/partnertypes")]
    public class PartnerTypesController : BaseController
    {
        private readonly ILookupManager<PartnerType> manager;

        public PartnerTypesController(TelemetryClient telemetryClient,
                                      IUserRepository userRepository,
                                      ILookupManager<PartnerType> manager)
            : base(telemetryClient, userRepository)
        {
            this.manager = manager;
        }

        [HttpGet]
        public async Task<IActionResult> GetPartnerTypes()
        {
            var types = await manager.Get();
            TelemetryClient.TrackEvent(nameof(GetPartnerTypes));

            return Ok(types);
        }
    }
}
