namespace TrashMob.Shared.Extensions.V2
{
    using System.Linq;
    using TrashMob.Models.Poco.V2;
    using TrashMob.Shared.Poco;

    /// <summary>
    /// V2 mapping extensions for leaderboard response types.
    /// </summary>
    public static class LeaderboardMappingsV2
    {
        /// <summary>
        /// Maps a <see cref="LeaderboardResponse"/> to a <see cref="LeaderboardResponseDto"/>.
        /// </summary>
        /// <param name="response">The leaderboard response.</param>
        /// <returns>A V2 leaderboard response DTO.</returns>
        public static LeaderboardResponseDto ToV2Dto(this LeaderboardResponse response)
        {
            return new LeaderboardResponseDto
            {
                LeaderboardType = response.LeaderboardType ?? string.Empty,
                TimeRange = response.TimeRange ?? string.Empty,
                LocationScope = response.LocationScope ?? string.Empty,
                LocationValue = response.LocationValue ?? string.Empty,
                ComputedDate = response.ComputedDate,
                TotalEntries = response.TotalEntries,
                Entries = response.Entries?.Select(e => e.ToV2Dto()).ToList() ?? [],
            };
        }

        /// <summary>
        /// Maps a <see cref="LeaderboardEntry"/> to a <see cref="LeaderboardEntryDto"/>.
        /// </summary>
        /// <param name="entry">The leaderboard entry.</param>
        /// <returns>A V2 leaderboard entry DTO.</returns>
        public static LeaderboardEntryDto ToV2Dto(this LeaderboardEntry entry)
        {
            return new LeaderboardEntryDto
            {
                EntityId = entry.EntityId,
                EntityName = entry.EntityName ?? string.Empty,
                EntityType = entry.EntityType ?? string.Empty,
                Rank = entry.Rank,
                Score = entry.Score,
                FormattedScore = entry.FormattedScore ?? string.Empty,
                Region = entry.Region ?? string.Empty,
                City = entry.City ?? string.Empty,
                ProfilePhotoUrl = entry.ProfilePhotoUrl ?? string.Empty,
            };
        }

        /// <summary>
        /// Maps a <see cref="UserRankResponse"/> to a <see cref="UserRankDto"/>.
        /// </summary>
        /// <param name="response">The user rank response.</param>
        /// <returns>A V2 user rank DTO.</returns>
        public static UserRankDto ToV2Dto(this UserRankResponse response)
        {
            return new UserRankDto
            {
                LeaderboardType = response.LeaderboardType ?? string.Empty,
                TimeRange = response.TimeRange ?? string.Empty,
                Rank = response.Rank,
                Score = response.Score,
                FormattedScore = response.FormattedScore ?? string.Empty,
                TotalRanked = response.TotalRanked,
                IsEligible = response.IsEligible,
                IneligibleReason = response.IneligibleReason ?? string.Empty,
            };
        }

        /// <summary>
        /// Maps a <see cref="TeamRankResponse"/> to a <see cref="TeamRankDto"/>.
        /// </summary>
        /// <param name="response">The team rank response.</param>
        /// <returns>A V2 team rank DTO.</returns>
        public static TeamRankDto ToV2Dto(this TeamRankResponse response)
        {
            return new TeamRankDto
            {
                TeamId = response.TeamId,
                TeamName = response.TeamName ?? string.Empty,
                LeaderboardType = response.LeaderboardType ?? string.Empty,
                TimeRange = response.TimeRange ?? string.Empty,
                Rank = response.Rank,
                Score = response.Score,
                FormattedScore = response.FormattedScore ?? string.Empty,
                TotalRanked = response.TotalRanked,
                IsEligible = response.IsEligible,
                IneligibleReason = response.IneligibleReason ?? string.Empty,
            };
        }
    }
}
