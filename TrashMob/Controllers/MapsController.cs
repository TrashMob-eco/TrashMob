namespace TrashMob.Controllers
{
    using System.Threading.Tasks;
    using Microsoft.AspNetCore.Authorization;
    using Microsoft.AspNetCore.Mvc;
    using TrashMob.Security;
    using TrashMob.Shared.Managers.Interfaces;

    [Route("api/maps")]
    public class MapsController : SecureController
    {
        private readonly IMapManager mapRepository;

        public MapsController(IMapManager mapRepository)
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
        [Authorize(Policy = AuthorizationPolicyConstants.ValidUser)]
        public async Task<IActionResult> GetAddressForPoint([FromQuery] double latitude, [FromQuery] double longitude)
        {
            var address = await mapRepository.GetAddressAsync(latitude, longitude);
            TelemetryClient.TrackEvent(nameof(GetAddressForPoint));
            return Ok(address);
        }
    }
}