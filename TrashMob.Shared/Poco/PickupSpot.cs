namespace TrashMob.Shared.Poco
{
    /// <summary>
    /// Represents a pickup spot location for collected trash or materials.
    /// </summary>
    public class PickupSpot
    {
        /// <summary>
        /// Gets or sets the street address of the pickup spot.
        /// </summary>
        public string StreetAddress { get; set; }

        /// <summary>
        /// Gets or sets the Google Maps URL for the pickup spot location.
        /// </summary>
        public string GoogleMapsUrl { get; set; }

        /// <summary>
        /// Gets or sets any additional notes about the pickup spot.
        /// </summary>
        public string Notes { get; set; }

        /// <summary>
        /// Gets or sets the name or identifier of the pickup spot.
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Gets or sets the URL of an image showing the pickup spot.
        /// </summary>
        public string ImageUrl { get; set; }
    }
}