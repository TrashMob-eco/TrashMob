namespace TrashMob.Shared.Managers.Interfaces
{
    using System;
    using System.Threading;
    using System.Threading.Tasks;
    using TrashMob.Shared.Poco;

    /// <summary>
    /// Defines operations for retrieving leaderboard data.
    /// Leaderboards are pre-computed by a daily job and this manager provides read-only access.
    /// </summary>
    public interface ILeaderboardManager
    {
        /// <summary>
        /// Gets the leaderboard for the specified parameters.
        /// </summary>
        /// <param name="leaderboardType">The type of leaderboard (Events, Bags, Weight, Hours).</param>
        /// <param name="timeRange">The time range (Week, Month, Year, AllTime). Default: Month.</param>
        /// <param name="locationScope">The location scope (Global, Region, City). Default: Global.</param>
        /// <param name="locationValue">The location value (region or city name). Required if scope is not Global.</param>
        /// <param name="limit">Maximum number of entries to return. Default: 50.</param>
        /// <param name="cancellationToken">A token to cancel the asynchronous operation.</param>
        /// <returns>The leaderboard response containing entries and metadata.</returns>
        Task<LeaderboardResponse> GetLeaderboardAsync(
            string leaderboardType,
            string timeRange = "Month",
            string locationScope = "Global",
            string locationValue = null,
            int limit = 50,
            CancellationToken cancellationToken = default);

        /// <summary>
        /// Gets the current user's rank on a leaderboard.
        /// </summary>
        /// <param name="userId">The user ID.</param>
        /// <param name="leaderboardType">The type of leaderboard. Default: Events.</param>
        /// <param name="timeRange">The time range. Default: AllTime.</param>
        /// <param name="cancellationToken">A token to cancel the asynchronous operation.</param>
        /// <returns>The user's rank information.</returns>
        Task<UserRankResponse> GetUserRankAsync(
            Guid userId,
            string leaderboardType = "Events",
            string timeRange = "AllTime",
            CancellationToken cancellationToken = default);

        /// <summary>
        /// Gets the available time ranges for leaderboards.
        /// </summary>
        /// <returns>Array of available time ranges.</returns>
        string[] GetAvailableTimeRanges();

        /// <summary>
        /// Gets the available leaderboard types.
        /// </summary>
        /// <returns>Array of available leaderboard types.</returns>
        string[] GetAvailableLeaderboardTypes();

        /// <summary>
        /// Gets the available location scopes.
        /// </summary>
        /// <returns>Array of available location scopes.</returns>
        string[] GetAvailableLocationScopes();
    }
}
