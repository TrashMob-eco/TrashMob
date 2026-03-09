#nullable enable

namespace TrashMob.Models.Poco.V2
{
    /// <summary>
    /// V2 API representation of available leaderboard query options.
    /// </summary>
    public class LeaderboardOptionsDto
    {
        /// <summary>
        /// Gets or sets the available leaderboard types (e.g., Events, Bags, Weight, Hours).
        /// </summary>
        public string[] Types { get; set; } = [];

        /// <summary>
        /// Gets or sets the available time ranges (e.g., Week, Month, Year, AllTime).
        /// </summary>
        public string[] TimeRanges { get; set; } = [];

        /// <summary>
        /// Gets or sets the available location scopes (e.g., Global, Region, City).
        /// </summary>
        public string[] LocationScopes { get; set; } = [];
    }
}
