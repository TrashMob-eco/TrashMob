namespace TrashMob.Models.Poco
{
    /// <summary>
    /// Request body for AI prospect discovery.
    /// </summary>
    public class DiscoveryRequest
    {
        /// <summary>Gets or sets the freeform research prompt (overrides location fields when set).</summary>
        public string? Prompt { get; set; }

        /// <summary>Gets or sets the city to search in.</summary>
        public string? City { get; set; }

        /// <summary>Gets or sets the region/state to search in.</summary>
        public string? Region { get; set; }

        /// <summary>Gets or sets the country to search in.</summary>
        public string? Country { get; set; }

        /// <summary>Gets or sets the maximum number of results to return.</summary>
        public int MaxResults { get; set; } = 10;
    }
}
