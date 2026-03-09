#nullable enable

namespace TrashMob.Models.Poco.V2
{
    using System;
    using System.Collections.Generic;

    /// <summary>
    /// V2 API representation of a leaderboard query response.
    /// </summary>
    public class LeaderboardResponseDto
    {
        /// <summary>
        /// Gets or sets the leaderboard type (Events, Bags, Weight, Hours).
        /// </summary>
        public string LeaderboardType { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets the time range (Week, Month, Year, AllTime).
        /// </summary>
        public string TimeRange { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets the location scope (Global, Region, City).
        /// </summary>
        public string LocationScope { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets the location value (region or city name).
        /// </summary>
        public string LocationValue { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets when this leaderboard data was last computed.
        /// </summary>
        public DateTimeOffset ComputedDate { get; set; }

        /// <summary>
        /// Gets or sets the total number of entries available.
        /// </summary>
        public int TotalEntries { get; set; }

        /// <summary>
        /// Gets or sets the leaderboard entries.
        /// </summary>
        public IReadOnlyList<LeaderboardEntryDto> Entries { get; set; } = [];
    }
}
