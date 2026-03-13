#nullable enable

namespace TrashMob.Models.Poco.V2
{
    using System;
    using System.Text.Json.Serialization;

    /// <summary>
    /// V2 API representation of a litter image. Excludes moderation internals.
    /// </summary>
    public class LitterImageDto
    {
        /// <summary>
        /// Gets or sets the unique identifier for the image.
        /// </summary>
        [JsonPropertyName("id")]
        public Guid Id { get; set; }

        /// <summary>
        /// Gets or sets the URL of the image.
        /// </summary>
        [JsonPropertyName("imageUrl")]
        public string ImageUrl { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets the street address where the litter was photographed.
        /// </summary>
        [JsonPropertyName("streetAddress")]
        public string StreetAddress { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets the city.
        /// </summary>
        [JsonPropertyName("city")]
        public string City { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets the region (state/province).
        /// </summary>
        [JsonPropertyName("region")]
        public string Region { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets the country.
        /// </summary>
        [JsonPropertyName("country")]
        public string Country { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets the postal code.
        /// </summary>
        [JsonPropertyName("postalCode")]
        public string PostalCode { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets the latitude.
        /// </summary>
        [JsonPropertyName("latitude")]
        public double? Latitude { get; set; }

        /// <summary>
        /// Gets or sets the longitude.
        /// </summary>
        [JsonPropertyName("longitude")]
        public double? Longitude { get; set; }
    }
}
