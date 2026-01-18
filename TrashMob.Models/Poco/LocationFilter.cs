namespace TrashMob.Models.Poco
{
    /// <summary>
    /// Represents filter criteria for querying by location.
    /// </summary>
    public class LocationFilter
    {
        /// <summary>
        /// Gets or sets the optional city to filter by.
        /// </summary>
        public string? City { get; set; }

        /// <summary>
        /// Gets or sets the optional region or state to filter by.
        /// </summary>
        public string? Region { get; set; }

        /// <summary>
        /// Gets or sets the optional country to filter by.
        /// </summary>
        public string? Country { get; set; }

        /// <summary>
        /// Gets or sets the optional postal code to filter by.
        /// </summary>
        public string? PostalCode { get; set; }
    }
}