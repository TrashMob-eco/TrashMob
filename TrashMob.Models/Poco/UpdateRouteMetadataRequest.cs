namespace TrashMob.Models.Poco
{
    /// <summary>
    /// Request body for updating route metadata (privacy, trim, notes).
    /// </summary>
    public class UpdateRouteMetadataRequest
    {
        /// <summary>
        /// Gets or sets the privacy level (Private, EventOnly, Public).
        /// </summary>
        public string PrivacyLevel { get; set; } = "EventOnly";

        /// <summary>
        /// Gets or sets meters to trim from the start.
        /// </summary>
        public int TrimStartMeters { get; set; }

        /// <summary>
        /// Gets or sets meters to trim from the end.
        /// </summary>
        public int TrimEndMeters { get; set; }

        /// <summary>
        /// Gets or sets notes about this route.
        /// </summary>
        public string? Notes { get; set; }

        /// <summary>
        /// Gets or sets bags collected along this route.
        /// </summary>
        public int? BagsCollected { get; set; }

        /// <summary>
        /// Gets or sets weight collected in pounds.
        /// </summary>
        public decimal? WeightCollected { get; set; }
    }
}
