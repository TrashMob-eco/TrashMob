#nullable enable

namespace TrashMob.Shared.Managers.Areas
{
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
    }

    public class NominatimResult
    {
        public string GeoJson { get; set; } = string.Empty;

        public string DisplayName { get; set; } = string.Empty;

        public string Category { get; set; } = string.Empty;

        public string Type { get; set; } = string.Empty;
    }
}
