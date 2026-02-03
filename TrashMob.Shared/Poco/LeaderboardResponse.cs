namespace TrashMob.Shared.Poco
{
    using System.Collections.Generic;

    /// <summary>
    /// Represents the response for a leaderboard query.
    /// </summary>
    public class LeaderboardResponse
    {
        /// <summary>
        /// Gets or sets the leaderboard type (Events, Bags, Weight, Hours).
        /// </summary>
        public string LeaderboardType { get; set; }

        /// <summary>
        /// Gets or sets the time range (Week, Month, Year, AllTime).
        /// </summary>
        public string TimeRange { get; set; }

        /// <summary>
        /// Gets or sets the location scope (Global, Region, City).
        /// </summary>
        public string LocationScope { get; set; }

        /// <summary>
        /// Gets or sets the location value (region or city name).
        /// </summary>
        public string LocationValue { get; set; }

        /// <summary>
        /// Gets or sets when this leaderboard data was last computed.
        /// </summary>
        public System.DateTimeOffset ComputedDate { get; set; }

        /// <summary>
        /// Gets or sets the total number of entries available.
        /// </summary>
        public int TotalEntries { get; set; }

        /// <summary>
        /// Gets or sets the leaderboard entries.
        /// </summary>
        public List<LeaderboardEntry> Entries { get; set; } = new();
    }

    /// <summary>
    /// Represents the current user's rank on a leaderboard.
    /// </summary>
    public class UserRankResponse
    {
        /// <summary>
        /// Gets or sets the leaderboard type.
        /// </summary>
        public string LeaderboardType { get; set; }

        /// <summary>
        /// Gets or sets the time range.
        /// </summary>
        public string TimeRange { get; set; }

        /// <summary>
        /// Gets or sets the user's current rank. Null if not ranked.
        /// </summary>
        public int? Rank { get; set; }

        /// <summary>
        /// Gets or sets the user's score. Null if not ranked.
        /// </summary>
        public decimal? Score { get; set; }

        /// <summary>
        /// Gets or sets the formatted score for display.
        /// </summary>
        public string FormattedScore { get; set; }

        /// <summary>
        /// Gets or sets the total number of ranked users.
        /// </summary>
        public int TotalRanked { get; set; }

        /// <summary>
        /// Gets or sets whether the user is eligible to appear on leaderboards.
        /// False if user has fewer than 3 events or has opted out.
        /// </summary>
        public bool IsEligible { get; set; }

        /// <summary>
        /// Gets or sets the reason if not eligible.
        /// </summary>
        public string IneligibleReason { get; set; }
    }
}
