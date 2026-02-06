namespace TrashMob.Shared.Managers.Interfaces
{
    using System;
    using System.Threading.Tasks;
    using TrashMob.Models;

    /// <summary>
    /// Defines operations for map-related functionality including geocoding and distance calculations.
    /// </summary>
    public interface IMapManager
    {
        /// <summary>
        /// Gets the Azure Maps API key.
        /// </summary>
        /// <returns>The Azure Maps API key.</returns>
        string GetMapKey();

        /// <summary>
        /// Gets the Google Maps API key.
        /// </summary>
        /// <returns>The Google Maps API key.</returns>
        string GetGoogleMapKey();

        /// <summary>
        /// Calculates the distance between two geographic points.
        /// </summary>
        /// <param name="pointA">The first point as a tuple of latitude and longitude.</param>
        /// <param name="pointB">The second point as a tuple of latitude and longitude.</param>
        /// <param name="IsMetric">Whether to return the distance in metric units. Defaults to true.</param>
        /// <returns>The distance between the two points.</returns>
        Task<double> GetDistanceBetweenTwoPointsAsync(Tuple<double, double> pointA, Tuple<double, double> pointB,
            bool IsMetric = true);

        /// <summary>
        /// Gets the timezone information for a geographic point at a specific time.
        /// </summary>
        /// <param name="pointA">The geographic point as a tuple of latitude and longitude.</param>
        /// <param name="dateTimeOffset">The date and time for the timezone lookup.</param>
        /// <returns>The timezone identifier for the specified point and time.</returns>
        Task<string> GetTimeForPointAsync(Tuple<double, double> pointA, DateTimeOffset dateTimeOffset);

        /// <summary>
        /// Gets the address for a geographic coordinate.
        /// </summary>
        /// <param name="latitude">The latitude of the location.</param>
        /// <param name="longitude">The longitude of the location.</param>
        /// <returns>The address corresponding to the specified coordinates.</returns>
        Task<Address> GetAddressAsync(double latitude, double longitude);

        /// <summary>
        /// Searches for addresses matching the given query (typeahead/autocomplete).
        /// </summary>
        /// <param name="query">The search query string.</param>
        /// <param name="entityType">Optional entity type filter (e.g., Municipality, PostalCodeArea).</param>
        /// <returns>The raw JSON response from Azure Maps Search API.</returns>
        Task<string> SearchAddressAsync(string query, string entityType = null);

        /// <summary>
        /// Gets the full address details for a geographic coordinate (reverse geocoding).
        /// Returns the raw Azure Maps response for client compatibility.
        /// </summary>
        /// <param name="latitude">The latitude of the location.</param>
        /// <param name="longitude">The longitude of the location.</param>
        /// <returns>The raw JSON response from Azure Maps Reverse Geocoding API.</returns>
        Task<string> ReverseGeocodeAsync(double latitude, double longitude);
    }
}
