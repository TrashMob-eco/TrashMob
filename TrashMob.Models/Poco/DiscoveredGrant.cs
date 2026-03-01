namespace TrashMob.Models.Poco
{
    /// <summary>
    /// A single grant opportunity suggested by AI discovery.
    /// </summary>
    public class DiscoveredGrant
    {
        /// <summary>Gets or sets the foundation, agency, or corporate funder name.</summary>
        public string? FunderName { get; set; }

        /// <summary>Gets or sets the specific grant program name.</summary>
        public string? ProgramName { get; set; }

        /// <summary>Gets or sets a description of the grant purpose and requirements.</summary>
        public string? Description { get; set; }

        /// <summary>Gets or sets the minimum award amount in USD.</summary>
        public decimal? AmountMin { get; set; }

        /// <summary>Gets or sets the maximum award amount in USD.</summary>
        public decimal? AmountMax { get; set; }

        /// <summary>Gets or sets the submission deadline (ISO date string).</summary>
        public string? Deadline { get; set; }

        /// <summary>Gets or sets a link to the grant information page.</summary>
        public string? Url { get; set; }

        /// <summary>Gets or sets eligibility notes specific to TrashMob.</summary>
        public string? EligibilityNotes { get; set; }

        /// <summary>Gets or sets the rationale for why this grant is a good fit.</summary>
        public string? Rationale { get; set; }
    }
}
