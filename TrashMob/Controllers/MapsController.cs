namespace TrashMob.Controllers
{
    using System;
    using System.Threading.Tasks;
    using Microsoft.AspNetCore.Authorization;
    using Microsoft.AspNetCore.Http;
    using Microsoft.AspNetCore.Mvc;
    using TrashMob.Security;
    using TrashMob.Shared.Managers.Interfaces;

    /// <summary>
    /// Controller for map-related operations, including map key retrieval and address lookup.
    /// </summary>
    [Route("api/maps")]
    public class MapsController(IMapManager mapRepository)
        : SecureController
    {

        /// <summary>
        /// Gets the map key.
        /// </summary>
        /// <remarks>The map key. DEPRECATED: Use /search and /reversegeocode endpoints instead for security.</remarks>
        [HttpGet]
        [Obsolete("Use /search and /reversegeocode endpoints instead to avoid exposing the API key")]
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

        /// <summary>
        /// Searches for addresses matching the given query (typeahead/autocomplete).
        /// Proxies the request to Azure Maps without exposing the API key.
        /// </summary>
        /// <param name="query">The search query string.</param>
        /// <param name="entityType">Optional entity type filter (e.g., Municipality, PostalCodeArea).</param>
        /// <returns>The Azure Maps search results.</returns>
        [HttpGet("search")]
        [ProducesResponseType(typeof(string), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> SearchAddress([FromQuery] string query, [FromQuery] string entityType = null)
        {
            if (string.IsNullOrWhiteSpace(query))
            {
                return BadRequest("Query parameter is required");
            }

            try
            {
                var result = await mapRepository.SearchAddressAsync(query, entityType);
                TrackEvent(nameof(SearchAddress));
                return Content(result, "application/json");
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
            }
        }

        /// <summary>
        /// Reverse geocodes a coordinate to an address.
        /// Proxies the request to Azure Maps without exposing the API key.
        /// </summary>
        /// <param name="latitude">The latitude.</param>
        /// <param name="longitude">The longitude.</param>
        /// <returns>The Azure Maps reverse geocode results.</returns>
        [HttpGet("reversegeocode")]
        [ProducesResponseType(typeof(string), StatusCodes.Status200OK)]
        public async Task<IActionResult> ReverseGeocode([FromQuery] double latitude, [FromQuery] double longitude)
        {
            try
            {
                var result = await mapRepository.ReverseGeocodeAsync(latitude, longitude);
                TrackEvent(nameof(ReverseGeocode));
                return Content(result, "application/json");
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
            }
        }
    }
}