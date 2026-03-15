namespace TrashMob.Controllers.V2
{
    using System;
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
        /// Gets the Google Maps API key.
        /// </summary>
        /// <returns>The Google Maps API key string.</returns>
        /// <response code="200">Returns the API key.</response>
        [HttpGet("googlemapkey")]
        [ProducesResponseType(typeof(string), StatusCodes.Status200OK)]
        public IActionResult GetGoogleMapKey()
        {
            logger.LogInformation("V2 GetGoogleMapKey requested");
            var mapKey = mapManager.GetGoogleMapKey();
            return Ok(mapKey);
        }

        /// <summary>
        /// Searches for addresses matching the given query (typeahead/autocomplete).
        /// Proxies the request to Azure Maps without exposing the API key.
        /// </summary>
        /// <param name="query">The search query string.</param>
        /// <param name="entityType">Optional entity type filter (e.g., Municipality, PostalCodeArea).</param>
        /// <param name="cancellationToken">Cancellation token.</param>
        /// <returns>The Azure Maps search results.</returns>
        /// <response code="200">Returns the search results.</response>
        /// <response code="400">Query parameter is required.</response>
        [HttpGet("search")]
        [ProducesResponseType(typeof(string), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> SearchAddress(
            [FromQuery] string query,
            [FromQuery] string entityType = null,
            CancellationToken cancellationToken = default)
        {
            if (string.IsNullOrWhiteSpace(query))
            {
                return BadRequest("Query parameter is required");
            }

            logger.LogInformation("V2 SearchAddress: query={Query}, entityType={EntityType}", query, entityType);

            var result = await mapManager.SearchAddressAsync(query, entityType);
            return Content(result, "application/json");
        }

        /// <summary>
        /// Reverse geocodes a coordinate to an address.
        /// Proxies the request to Azure Maps without exposing the API key.
        /// </summary>
        /// <param name="latitude">The latitude.</param>
        /// <param name="longitude">The longitude.</param>
        /// <param name="cancellationToken">Cancellation token.</param>
        /// <returns>The Azure Maps reverse geocode results.</returns>
        /// <response code="200">Returns the reverse geocode results.</response>
        [HttpGet("reversegeocode")]
        [ProducesResponseType(typeof(string), StatusCodes.Status200OK)]
        public async Task<IActionResult> ReverseGeocode(
            [FromQuery] double latitude,
            [FromQuery] double longitude,
            CancellationToken cancellationToken = default)
        {
            logger.LogInformation("V2 ReverseGeocode: Lat={Latitude}, Lon={Longitude}", latitude, longitude);

            var result = await mapManager.ReverseGeocodeAsync(latitude, longitude);
            return Content(result, "application/json");
        }

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
