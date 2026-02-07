namespace TrashMob.Models.Poco
{
    /// <summary>
    /// Breakdown of the individual factors contributing to a prospect's FitScore.
    /// </summary>
    public class FitScoreBreakdown
    {
        /// <summary>Gets or sets the total FitScore (0-100).</summary>
        public int TotalScore { get; set; }

        /// <summary>Gets or sets the event density score component (0-30).</summary>
        public int EventDensityScore { get; set; }

        /// <summary>Gets or sets the population score component (0-25).</summary>
        public int PopulationScore { get; set; }

        /// <summary>Gets or sets the geographic gap score component (0-30).</summary>
        public int GeographicGapScore { get; set; }

        /// <summary>Gets or sets the community type fit score component (0-15).</summary>
        public int CommunityTypeFitScore { get; set; }

        /// <summary>Gets or sets the number of nearby events used in scoring.</summary>
        public int NearbyEventCount { get; set; }

        /// <summary>Gets or sets the distance to the nearest active community partner in miles.</summary>
        public double? NearestPartnerDistanceMiles { get; set; }
    }
}
