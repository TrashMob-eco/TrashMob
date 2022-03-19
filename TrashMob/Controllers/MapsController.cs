namespace TrashMob.Controllers
{
    using Microsoft.ApplicationInsights;
    using Microsoft.AspNetCore.Authorization;
    using Microsoft.AspNetCore.Mvc;
    using System.Threading.Tasks;
    using TrashMob.Shared.Persistence;

    [Route("api/maps")]
    public class MapsController : BaseController
    {
        private readonly IMapRepository mapRepository;

        public MapsController(IMapRepository mapRepository,
                              TelemetryClient telemetryClient)
            : base(telemetryClient)
        {
            this.mapRepository = mapRepository;
        }

        [HttpGet]
        public async Task<IActionResult> GetMapKey()
        {
            var mapKey = await Task.FromResult(mapRepository.GetMapKey());
            return Ok(mapKey);
        }

        [HttpGet("GetAddress")]
        [Authorize]
        public async Task<IActionResult> GetAddressForPoint([FromQuery] double latitude, [FromQuery] double longitude)
        {
            var address = await mapRepository.GetAddress(latitude, longitude);
            TelemetryClient.TrackEvent(nameof(GetAddressForPoint));
            return Ok(address);
        }
    }
}
