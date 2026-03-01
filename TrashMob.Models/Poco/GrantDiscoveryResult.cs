namespace TrashMob.Models.Poco
{
    using System.Collections.Generic;

    /// <summary>
    /// Result of an AI grant discovery operation.
    /// </summary>
    public class GrantDiscoveryResult
    {
        /// <summary>Gets or sets the discovered grant opportunities.</summary>
        public List<DiscoveredGrant> Grants { get; set; } = [];

        /// <summary>Gets or sets the token count used for this request.</summary>
        public int TokensUsed { get; set; }

        /// <summary>Gets or sets any warning or informational message.</summary>
        public string? Message { get; set; }
    }
}
