namespace TrashMob.Controllers
{
    using System.Threading.Tasks;
    using Microsoft.AspNetCore.Authorization;
    using Microsoft.AspNetCore.Mvc;
    using TrashMob.Security;
    using TrashMob.Shared.Managers.Interfaces;

    /// <summary>
    /// Controller for map-related operations, including map key retrieval and address lookup.
    /// </summary>
    [Route("api/maps")]
    public class MapsController : SecureController
    {
        private readonly IMapManager mapRepository;

        /// <summary>
        /// Initializes a new instance of the <see cref="MapsController"/> class.
        /// </summary>
        /// <param name="mapRepository">The map manager.</param>
        public MapsController(IMapManager mapRepository)
        {
            this.mapRepository = mapRepository;
        }

        /// <summary>
        /// Gets the map key.
        /// </summary>
        /// <remarks>The map key.</remarks>
        [HttpGet]
        public async Task<IActionResult> GetMapKey()
        {
            var mapKey = await Task.FromResult(mapRepository.GetMapKey());
            return Ok(mapKey);
        }

        /// <summary>
        /// Gets the Google map key.
        /// </summary>
        /// <remarks>The Google map key.</remarks>
        [HttpGet("googlemapkey")]
        public async Task<IActionResult> GetGoogleMapKey()
        {
            var mapKey = await Task.FromResult(mapRepository.GetGoogleMapKey());
            return Ok(mapKey);
        }

        /// <summary>
        /// Gets the address for a given latitude and longitude. Requires a valid user.
        /// </summary>
        /// <param name="latitude">The latitude.</param>
        /// <param name="longitude">The longitude.</param>
        /// <remarks>The address for the specified coordinates.</remarks>
        [HttpGet("GetAddress")]
        [Authorize(Policy = AuthorizationPolicyConstants.ValidUser)]
        public async Task<IActionResult> GetAddressForPoint([FromQuery] double latitude, [FromQuery] double longitude)
        {
            var address = await mapRepository.GetAddressAsync(latitude, longitude);
            TrackEvent(nameof(GetAddressForPoint));
            return Ok(address);
        }
    }
}