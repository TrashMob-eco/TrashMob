#nullable enable

namespace TrashMob.Models.Poco.V2
{
    using System;

    /// <summary>
    /// V2 API representation of a team's rank on a leaderboard.
    /// </summary>
    public class TeamRankDto
    {
        /// <summary>
        /// Gets or sets the team identifier.
        /// </summary>
        public Guid TeamId { get; set; }

        /// <summary>
        /// Gets or sets the team name.
        /// </summary>
        public string TeamName { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets the leaderboard type.
        /// </summary>
        public string LeaderboardType { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets the time range.
        /// </summary>
        public string TimeRange { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets the team's current rank. Null if not ranked.
        /// </summary>
        public int? Rank { get; set; }

        /// <summary>
        /// Gets or sets the team's score. Null if not ranked.
        /// </summary>
        public decimal? Score { get; set; }

        /// <summary>
        /// Gets or sets the formatted score for display.
        /// </summary>
        public string FormattedScore { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets the total number of ranked teams.
        /// </summary>
        public int TotalRanked { get; set; }

        /// <summary>
        /// Gets or sets whether the team is eligible to appear on leaderboards.
        /// </summary>
        public bool IsEligible { get; set; }

        /// <summary>
        /// Gets or sets the reason if not eligible.
        /// </summary>
        public string IneligibleReason { get; set; } = string.Empty;
    }
}
