namespace TrashMob.Models.Poco
{
    using System.Collections.Generic;

    /// <summary>
    /// Represents a single parsed feature from an imported area file.
    /// </summary>
    public class AreaImportFeature
    {
        /// <summary>
        /// Gets or sets the geometry as a GeoJSON string.
        /// </summary>
        public string GeoJson { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets the geometry type (Polygon or LineString).
        /// </summary>
        public string GeometryType { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets the properties extracted from the source feature.
        /// </summary>
        public Dictionary<string, string> Properties { get; set; } = new();

        /// <summary>
        /// Gets or sets whether this feature has valid geometry.
        /// </summary>
        public bool IsValid { get; set; } = true;

        /// <summary>
        /// Gets or sets any validation errors for this feature.
        /// </summary>
        public List<string> ValidationErrors { get; set; } = new();
    }
}
