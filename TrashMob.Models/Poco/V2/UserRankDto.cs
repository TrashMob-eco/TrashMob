#nullable enable

namespace TrashMob.Models.Poco.V2
{
    /// <summary>
    /// V2 API representation of the current user's rank on a leaderboard.
    /// </summary>
    public class UserRankDto
    {
        /// <summary>
        /// Gets or sets the leaderboard type.
        /// </summary>
        public string LeaderboardType { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets the time range.
        /// </summary>
        public string TimeRange { get; set; } = string.Empty;

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
        public string FormattedScore { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets the total number of ranked users.
        /// </summary>
        public int TotalRanked { get; set; }

        /// <summary>
        /// Gets or sets whether the user is eligible to appear on leaderboards.
        /// </summary>
        public bool IsEligible { get; set; }

        /// <summary>
        /// Gets or sets the reason if not eligible.
        /// </summary>
        public string IneligibleReason { get; set; } = string.Empty;
    }
}
