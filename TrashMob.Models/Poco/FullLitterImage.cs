namespace TrashMob.Models.Poco
{
    /// <summary>
    /// Represents a litter image with full details including the image URL for display purposes.
    /// </summary>
    public class FullLitterImage
    {
        /// <summary>
        /// Gets or sets the unique identifier of the litter image.
        /// </summary>
        public Guid Id { get; set; }

        /// <summary>
        /// Gets or sets the identifier of the associated litter report.
        /// </summary>
        public Guid LitterReportId { get; set; }

        /// <summary>
        /// Gets or sets the URL of the image.
        /// </summary>
        public string ImageURL { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets the street address where the litter was found.
        /// </summary>
        public string StreetAddress { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets the city where the litter was found.
        /// </summary>
        public string City { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets the region or state where the litter was found.
        /// </summary>
        public string Region { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets the country where the litter was found.
        /// </summary>
        public string Country { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets the postal code where the litter was found.
        /// </summary>
        public string PostalCode { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets the latitude coordinate of the litter location.
        /// </summary>
        public double? Latitude { get; set; }

        /// <summary>
        /// Gets or sets the longitude coordinate of the litter location.
        /// </summary>
        public double? Longitude { get; set; }

        /// <summary>
        /// Gets or sets the identifier of the user who created the image record.
        /// </summary>
        public Guid CreatedByUserId { get; set; }

        /// <summary>
        /// Gets or sets the date and time when the image was created.
        /// </summary>
        public DateTimeOffset? CreatedDate { get; set; }

        /// <summary>
        /// Gets or sets the identifier of the user who last updated the image record.
        /// </summary>
        public Guid LastUpdatedByUserId { get; set; }

        /// <summary>
        /// Gets or sets the date and time when the image was last updated.
        /// </summary>
        public DateTimeOffset? LastUpdatedDate { get; set; }
    }
}