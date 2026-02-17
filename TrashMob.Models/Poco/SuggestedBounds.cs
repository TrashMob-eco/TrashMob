#nullable enable

namespace TrashMob.Models.Poco
{
    /// <summary>
    /// Suggested geographic bounds auto-derived from a community's location.
    /// </summary>
    public class SuggestedBounds
    {
        /// <summary>Northern latitude bound.</summary>
        public double North { get; set; }

        /// <summary>Southern latitude bound.</summary>
        public double South { get; set; }

        /// <summary>Eastern longitude bound.</summary>
        public double East { get; set; }

        /// <summary>Western longitude bound.</summary>
        public double West { get; set; }

        /// <summary>Center latitude (midpoint of north and south).</summary>
        public double CenterLatitude { get; set; }

        /// <summary>Center longitude (midpoint of east and west).</summary>
        public double CenterLongitude { get; set; }

        /// <summary>The query string that was sent to Nominatim.</summary>
        public string Query { get; set; } = string.Empty;
    }
}
