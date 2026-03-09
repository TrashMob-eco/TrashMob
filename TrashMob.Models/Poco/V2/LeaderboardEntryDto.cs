#nullable enable

namespace TrashMob.Models.Poco.V2
{
    using System;

    /// <summary>
    /// V2 API representation of a single leaderboard entry.
    /// </summary>
    public class LeaderboardEntryDto
    {
        /// <summary>
        /// Gets or sets the entity identifier (user, team, or community).
        /// </summary>
        public Guid EntityId { get; set; }

        /// <summary>
        /// Gets or sets the display name for the entity.
        /// </summary>
        public string EntityName { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets the entity type (User, Team, Community).
        /// </summary>
        public string EntityType { get; set; } = string.Empty;

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
        public string FormattedScore { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets the region for location-based filtering.
        /// </summary>
        public string Region { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets the city for location-based filtering.
        /// </summary>
        public string City { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets the profile photo URL for the entity.
        /// </summary>
        public string ProfilePhotoUrl { get; set; } = string.Empty;
    }
}
