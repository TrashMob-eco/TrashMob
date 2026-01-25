namespace TrashMob.Models.Poco
{
    /// <summary>
    /// Represents a geographic location with a sort order for sequencing.
    /// </summary>
    public class SortableLocation
    {
        /// <summary>
        /// Gets or sets the sort order indicating the position of this location in a sequence.
        /// </summary>
        public int SortOrder { get; set; }

        /// <summary>
        /// Gets or sets the latitude coordinate.
        /// </summary>
        public double Latitude { get; set; }

        /// <summary>
        /// Gets or sets the longitude coordinate.
        /// </summary>
        public double Longitude { get; set; }
    }
}