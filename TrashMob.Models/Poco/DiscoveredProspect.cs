namespace TrashMob.Models.Poco
{
    /// <summary>
    /// A single prospect suggested by AI discovery.
    /// </summary>
    public class DiscoveredProspect
    {
        /// <summary>Gets or sets the organization or city name.</summary>
        public string? Name { get; set; }

        /// <summary>Gets or sets the prospect type (Municipality, Nonprofit, HOA, CivicOrg, Other).</summary>
        public string? Type { get; set; }

        /// <summary>Gets or sets the estimated population served.</summary>
        public int? EstimatedPopulation { get; set; }

        /// <summary>Gets or sets the website URL.</summary>
        public string? Website { get; set; }

        /// <summary>Gets or sets a suggested contact role or name.</summary>
        public string? ContactSuggestion { get; set; }

        /// <summary>Gets or sets the rationale for why this is a good prospect.</summary>
        public string? Rationale { get; set; }
    }
}
