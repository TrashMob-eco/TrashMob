namespace TrashMob.Models.Poco
{
    /// <summary>
    /// Represents a geographic location with country, region, and city information.
    /// </summary>
    public class Location
    {
        /// <summary>
        /// Gets or sets the country name.
        /// </summary>
        public string? Country { get; set; }

        /// <summary>
        /// Gets or sets the region or state name.
        /// </summary>
        public string? Region { get; set; }

        /// <summary>
        /// Gets or sets the city name.
        /// </summary>
        public string? City { get; set; }
    }
}