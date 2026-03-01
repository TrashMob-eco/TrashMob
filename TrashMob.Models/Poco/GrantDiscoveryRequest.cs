namespace TrashMob.Models.Poco
{
    /// <summary>
    /// Request body for AI grant discovery.
    /// </summary>
    public class GrantDiscoveryRequest
    {
        /// <summary>Gets or sets the freeform research prompt (overrides focus areas when set).</summary>
        public string? Prompt { get; set; }

        /// <summary>Gets or sets comma-separated focus areas (e.g., "Environmental Cleanup, Conservation").</summary>
        public string? FocusAreas { get; set; }

        /// <summary>Gets or sets the maximum number of results to return.</summary>
        public int MaxResults { get; set; } = 10;
    }
}
