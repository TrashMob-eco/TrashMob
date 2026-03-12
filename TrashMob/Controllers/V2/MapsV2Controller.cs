namespace TrashMob.Controllers.V2
{
    using System.Threading;
    using System.Threading.Tasks;
    using Asp.Versioning;
    using Microsoft.AspNetCore.Authorization;
    using Microsoft.AspNetCore.Cors;
    using Microsoft.AspNetCore.Http;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.Extensions.Logging;
    using Microsoft.Identity.Web.Resource;
    using TrashMob.Models.Extensions.V2;
    using TrashMob.Models.Poco.V2;
    using TrashMob.Security;
    using TrashMob.Shared;
    using TrashMob.Shared.Managers.Interfaces;

    /// <summary>
    /// V2 controller for map/geocoding operations.
    /// </summary>
    [ApiController]
    [ApiVersion("2.0")]
    [EnableCors("_myAllowSpecificOrigins")]
    [Route("api/v{version:apiVersion}/maps")]
    public class MapsV2Controller(
        IMapManager mapManager,
        ILogger<MapsV2Controller> logger) : ControllerBase
    {
        /// <summary>
        /// Gets the address for a given latitude and longitude.
        /// </summary>
        /// <param name="latitude">The latitude.</param>
        /// <param name="longitude">The longitude.</param>
        /// <param name="cancellationToken">Cancellation token.</param>
        /// <returns>The address at the specified coordinates.</returns>
        /// <response code="200">Returns the address.</response>
        /// <response code="401">Authentication required.</response>
        [HttpGet("address")]
        [Authorize(Policy = AuthorizationPolicyConstants.ValidUser)]
        [RequiredScope(Constants.TrashMobReadScope)]
        [ProducesResponseType(typeof(AddressDto), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> GetAddress(
            [FromQuery] double latitude,
            [FromQuery] double longitude,
            CancellationToken cancellationToken = default)
        {
            logger.LogInformation("V2 GetAddress requested Lat={Latitude}, Lon={Longitude}",
                latitude, longitude);

            var address = await mapManager.GetAddressAsync(latitude, longitude);

            return Ok(address.ToV2Dto());
        }
    }
}
