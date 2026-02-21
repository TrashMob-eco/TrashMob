namespace TrashMob.Models.Poco
{
    /// <summary>
    /// A geographic area that has TrashMob events but no active community partner.
    /// </summary>
    public class GeographicGap
    {
        /// <summary>Gets or sets the city name.</summary>
        public string? City { get; set; }

        /// <summary>Gets or sets the region/state.</summary>
        public string? Region { get; set; }

        /// <summary>Gets or sets the country.</summary>
        public string? Country { get; set; }

        /// <summary>Gets or sets the number of events in this area.</summary>
        public int EventCount { get; set; }

        /// <summary>Gets or sets the nearest active community partner distance in miles.</summary>
        public double? NearestPartnerDistanceMiles { get; set; }

        /// <summary>Gets or sets the average latitude of events in this area.</summary>
        public double? AverageLatitude { get; set; }

        /// <summary>Gets or sets the average longitude of events in this area.</summary>
        public double? AverageLongitude { get; set; }
    }
}
