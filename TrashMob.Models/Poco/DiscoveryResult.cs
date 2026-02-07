namespace TrashMob.Models.Poco
{
    using System.Collections.Generic;

    /// <summary>
    /// Result of an AI discovery operation.
    /// </summary>
    public class DiscoveryResult
    {
        /// <summary>Gets or sets the discovered prospects.</summary>
        public List<DiscoveredProspect> Prospects { get; set; } = [];

        /// <summary>Gets or sets the token count used for this request.</summary>
        public int TokensUsed { get; set; }

        /// <summary>Gets or sets any warning or informational message.</summary>
        public string? Message { get; set; }
    }
}
