namespace TrashMob.Shared.Poco
{
    /// <summary>
    /// Represents a single entry on a leaderboard.
    /// </summary>
    public class LeaderboardEntry
    {
        /// <summary>
        /// Gets or sets the entity ID (user, team, or community).
        /// </summary>
        public System.Guid EntityId { get; set; }

        /// <summary>
        /// Gets or sets the display name for the entity.
        /// </summary>
        public string EntityName { get; set; }

        /// <summary>
        /// Gets or sets the entity type (User, Team, Community).
        /// </summary>
        public string EntityType { get; set; }

        /// <summary>
        /// Gets or sets the rank position on the leaderboard.
        /// </summary>
        public int Rank { get; set; }

        /// <summary>
        /// Gets or sets the score value.
        /// </summary>
        public decimal Score { get; set; }

        /// <summary>
        /// Gets or sets the formatted score for display (e.g., "42 events", "150 lbs").
        /// </summary>
        public string FormattedScore { get; set; }

        /// <summary>
        /// Gets or sets the region for location-based filtering.
        /// </summary>
        public string Region { get; set; }

        /// <summary>
        /// Gets or sets the city for location-based filtering.
        /// </summary>
        public string City { get; set; }
    }
}
