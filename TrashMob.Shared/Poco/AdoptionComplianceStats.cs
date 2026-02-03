namespace TrashMob.Shared.Poco
{
    /// <summary>
    /// Represents compliance statistics for a community's adoption program.
    /// </summary>
    public class AdoptionComplianceStats
    {
        /// <summary>
        /// Gets or sets the total number of approved adoptions.
        /// </summary>
        public int TotalAdoptions { get; set; }

        /// <summary>
        /// Gets or sets the number of compliant adoptions.
        /// </summary>
        public int CompliantAdoptions { get; set; }

        /// <summary>
        /// Gets or sets the number of at-risk adoptions (approaching delinquency).
        /// </summary>
        public int AtRiskAdoptions { get; set; }

        /// <summary>
        /// Gets or sets the number of delinquent adoptions.
        /// </summary>
        public int DelinquentAdoptions { get; set; }

        /// <summary>
        /// Gets or sets the total number of available areas.
        /// </summary>
        public int TotalAvailableAreas { get; set; }

        /// <summary>
        /// Gets or sets the total number of adopted areas.
        /// </summary>
        public int AdoptedAreas { get; set; }

        /// <summary>
        /// Gets the compliance rate as a percentage.
        /// </summary>
        public double ComplianceRate => TotalAdoptions > 0
            ? (double)CompliantAdoptions / TotalAdoptions * 100
            : 0;

        /// <summary>
        /// Gets the adoption rate as a percentage.
        /// </summary>
        public double AdoptionRate => TotalAvailableAreas > 0
            ? (double)AdoptedAreas / TotalAvailableAreas * 100
            : 0;

        /// <summary>
        /// Gets or sets the total number of events linked to adoptions.
        /// </summary>
        public int TotalLinkedEvents { get; set; }
    }
}
