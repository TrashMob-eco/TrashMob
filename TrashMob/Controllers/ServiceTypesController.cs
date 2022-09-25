
namespace TrashMob.Controllers
{
    using System.Threading;
    using System.Threading.Tasks;
    using Microsoft.ApplicationInsights;
    using Microsoft.AspNetCore.Mvc;
    using TrashMob.Shared.Managers.Interfaces;
    using TrashMob.Shared.Models;
    using TrashMob.Shared.Persistence.Interfaces;

    [Route("api/servicetypes")]
    public class ServiceTypesController : BaseController
    {
        private readonly ILookupManager<ServiceType> manager;

        public ServiceTypesController(TelemetryClient telemetryClient,
                                      IUserRepository userRepository,
                                      ILookupManager<ServiceType> manager)
            : base(telemetryClient, userRepository)
        {
            this.manager = manager;
        }

        [HttpGet]
        public async Task<IActionResult> GetServiceTypes(CancellationToken cancellationToken)
        {
            var types = await manager.Get();
            TelemetryClient.TrackEvent(nameof(GetServiceTypes));

            return Ok(types);
        }
    }
}
