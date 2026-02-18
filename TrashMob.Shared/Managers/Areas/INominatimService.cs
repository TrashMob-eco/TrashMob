#nullable enable

namespace TrashMob.Shared.Managers.Areas
{
    using System.Collections.Generic;
    using System.Threading;
    using System.Threading.Tasks;

    public interface INominatimService
    {
        /// <summary>
        /// Searches OpenStreetMap Nominatim for a named feature and returns its polygon geometry.
        /// </summary>
        /// <param name="query">The search query (e.g. "Lincoln Park, Chicago").</param>
        /// <param name="viewBox">Optional viewport bounding box to bias results (North, South, East, West).</param>
        /// <param name="cancellationToken">Cancellation token.</param>
        /// <returns>A result with GeoJSON polygon, or null if no polygon geometry was found.</returns>
        Task<NominatimResult?> SearchWithPolygonAsync(
            string query,
            (double North, double South, double East, double West)? viewBox = null,
            CancellationToken cancellationToken = default);

        /// <summary>
        /// Searches Nominatim for all features of a category within a bounding box.
        /// Uses pagination via exclude_place_ids to retrieve all results.
        /// </summary>
        /// <param name="category">The category to search for (e.g. "school", "park").</param>
        /// <param name="bounds">The bounding box (North, South, East, West).</param>
        /// <param name="cancellationToken">Cancellation token.</param>
        /// <returns>A list of results with polygon geometry and metadata.</returns>
        Task<IEnumerable<NominatimResult>> SearchByCategoryAsync(
            string category,
            (double North, double South, double East, double West) bounds,
            CancellationToken cancellationToken = default);

        /// <summary>
        /// Looks up geographic bounding box for a location query using Nominatim search API.
        /// </summary>
        /// <param name="query">Location query string (e.g., "Issaquah, Washington, United States").</param>
        /// <param name="cancellationToken">Cancellation token.</param>
        /// <returns>Bounding box tuple (North, South, East, West) or null if not found.</returns>
        Task<(double North, double South, double East, double West)?> LookupBoundsAsync(
            string query,
            CancellationToken cancellationToken = default);

        /// <summary>
        /// Looks up geographic bounding box AND polygon geometry for a location query (single Nominatim call).
        /// </summary>
        /// <param name="query">Location query string (e.g., "Issaquah, Washington, United States").</param>
        /// <param name="cancellationToken">Cancellation token.</param>
        /// <returns>Bounds with optional GeoJSON polygon, or null if not found.</returns>
        Task<BoundsWithGeometry?> LookupBoundsWithGeometryAsync(
            string query,
            CancellationToken cancellationToken = default);
    }

    /// <summary>
    /// Bounding box with optional polygon geometry from Nominatim.
    /// </summary>
    public class BoundsWithGeometry
    {
        public double North { get; set; }
        public double South { get; set; }
        public double East { get; set; }
        public double West { get; set; }

        /// <summary>
        /// Serialized GeoJSON geometry (Polygon or LineString), or null if no polygon was returned.
        /// </summary>
        public string? GeoJson { get; set; }
    }

    public class NominatimResult
    {
        public string GeoJson { get; set; } = string.Empty;

        public string DisplayName { get; set; } = string.Empty;

        public string Category { get; set; } = string.Empty;

        public string Type { get; set; } = string.Empty;

        public string OsmId { get; set; } = string.Empty;

        public string Name { get; set; } = string.Empty;

        public double Latitude { get; set; }

        public double Longitude { get; set; }

        public string? BoundingBox { get; set; }
    }
}
