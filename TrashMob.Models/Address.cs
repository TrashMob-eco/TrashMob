namespace TrashMob.Models
{
    /// <summary>
    /// Represents a physical address with street, city, region, postal code, country, and county information.
    /// </summary>
    public class Address
    {
        /// <summary>
        /// Gets or sets the street address.
        /// </summary>
        public string StreetAddress { get; set; } = "";

        /// <summary>
        /// Gets or sets the city name.
        /// </summary>
        public string City { get; set; } = "";

        /// <summary>
        /// Gets or sets the region or state.
        /// </summary>
        public string Region { get; set; } = "";

        /// <summary>
        /// Gets or sets the postal or ZIP code.
        /// </summary>
        public string PostalCode { get; set; } = "";

        /// <summary>
        /// Gets or sets the country name.
        /// </summary>
        public string Country { get; set; } = "";

        /// <summary>
        /// Gets or sets the county name.
        /// </summary>
        public string County { get; set; } = "";
    }
}