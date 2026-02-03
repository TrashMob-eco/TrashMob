#nullable disable

namespace TrashMob.Models
{
    /// <summary>
    /// Represents a cached leaderboard entry computed by the daily job.
    /// Entries are pre-computed for fast API reads and refreshed daily.
    /// </summary>
    public class LeaderboardCache
    {
        /// <summary>
        /// Gets or sets the unique identifier for this cache entry.
        /// </summary>
        public Guid Id { get; set; }

        /// <summary>
        /// Gets or sets the entity type (User, Team, Community).
        /// </summary>
        public string EntityType { get; set; }

        /// <summary>
        /// Gets or sets the entity identifier (UserId, TeamId, or CommunityId).
        /// </summary>
        public Guid EntityId { get; set; }

        /// <summary>
        /// Gets or sets the display name for the entity.
        /// </summary>
        public string EntityName { get; set; }

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
        /// Gets or sets the location value (region or city name). Null for Global scope.
        /// </summary>
        public string LocationValue { get; set; }

        /// <summary>
        /// Gets or sets the score value for this entry.
        /// </summary>
        public decimal Score { get; set; }

        /// <summary>
        /// Gets or sets the rank position on the leaderboard.
        /// </summary>
        public int Rank { get; set; }

        /// <summary>
        /// Gets or sets when this leaderboard data was computed.
        /// </summary>
        public DateTimeOffset ComputedDate { get; set; }

        /// <summary>
        /// Gets or sets when this cache entry was created.
        /// </summary>
        public DateTimeOffset CreatedDate { get; set; }
    }
}
