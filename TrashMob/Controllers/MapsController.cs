namespace TrashMob.Controllers
{
    using Microsoft.ApplicationInsights;
    using Microsoft.AspNetCore.Authorization;
    using Microsoft.AspNetCore.Mvc;
    using System.Threading.Tasks;
    using TrashMob.Shared.Managers.Interfaces;

    [Route("api/maps")]
    public class MapsController : SecureController
    {
        private readonly IMapManager mapRepository;

        public MapsController(IMapManager mapRepository) : base()
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
        [Authorize(Policy = "ValidUser")]
        public async Task<IActionResult> GetAddressForPoint([FromQuery] double latitude, [FromQuery] double longitude)
        {
            var address = await mapRepository.GetAddressAsync(latitude, longitude);
            TelemetryClient.TrackEvent(nameof(GetAddressForPoint));
            return Ok(address);
        }
    }
}
