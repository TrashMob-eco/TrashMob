namespace TrashMobDailyJobs
{
    using System;
    using System.Threading.Tasks;
    using Microsoft.Data.SqlClient;
    using Microsoft.Extensions.Configuration;
    using Microsoft.Extensions.Logging;

    /// <summary>
    /// Daily job that computes and caches leaderboard rankings.
    /// </summary>
    public class LeaderboardGenerator(ILogger<LeaderboardGenerator> logger, IConfiguration configuration)
    {
        private static readonly string[] TimeRanges = { "Week", "Month", "Year", "AllTime" };
        private static readonly string[] LeaderboardTypes = { "Events", "Bags", "Weight", "Hours" };
        private const int MinimumEventsToQualify = 3;

        public async Task RunAsync()
        {
            logger.LogInformation("LeaderboardGenerator job started at: {Time}", DateTime.UtcNow);
            var connectionString = configuration["TMDBServerConnectionString"];

            if (string.IsNullOrEmpty(connectionString))
            {
                logger.LogError("Database connection string is not configured.");
                return;
            }

            using var conn = new SqlConnection(connectionString);
            await conn.OpenAsync();

            // Clear existing cache
            await ClearLeaderboardCache(conn);

            // Compute user leaderboards for each time range
            foreach (var timeRange in TimeRanges)
            {
                logger.LogInformation("Computing user leaderboards for time range: {TimeRange}", timeRange);

                foreach (var leaderboardType in LeaderboardTypes)
                {
                    await ComputeUserLeaderboard(conn, leaderboardType, timeRange);
                }
            }

            // Compute team leaderboards for each time range
            foreach (var timeRange in TimeRanges)
            {
                logger.LogInformation("Computing team leaderboards for time range: {TimeRange}", timeRange);

                foreach (var leaderboardType in LeaderboardTypes)
                {
                    await ComputeTeamLeaderboard(conn, leaderboardType, timeRange);
                }
            }

            logger.LogInformation("LeaderboardGenerator job completed at: {Time}", DateTime.UtcNow);
        }

        private async Task ClearLeaderboardCache(SqlConnection conn)
        {
            logger.LogInformation("Clearing existing leaderboard cache...");
            var sql = "DELETE FROM dbo.LeaderboardCaches";

            using var cmd = new SqlCommand(sql, conn);
            var deleted = await cmd.ExecuteNonQueryAsync();
            logger.LogInformation("Cleared {Count} leaderboard cache entries.", deleted);
        }

        private async Task ComputeUserLeaderboard(SqlConnection conn, string leaderboardType, string timeRange)
        {
            var dateFilter = GetDateFilter(timeRange);
            var sql = GetLeaderboardQuery(leaderboardType, dateFilter);

            logger.LogInformation("Computing {Type} leaderboard for {TimeRange}...", leaderboardType, timeRange);

            using var cmd = new SqlCommand(sql, conn);
            using var reader = await cmd.ExecuteReaderAsync();

            var entries = 0;
            var computedDate = DateTimeOffset.UtcNow;

            while (await reader.ReadAsync())
            {
                var userId = reader.GetGuid(0);
                var userName = reader.GetString(1);
                var region = reader.IsDBNull(2) ? null : reader.GetString(2);
                var city = reader.IsDBNull(3) ? null : reader.GetString(3);
                var score = reader.GetDecimal(4);
                var rank = (int)reader.GetInt64(5);

                // Insert Global entry
                await InsertLeaderboardEntry(conn, "User", userId, userName, leaderboardType, timeRange, "Global", null, score, rank, computedDate);

                // Insert Region entry if available
                if (!string.IsNullOrEmpty(region))
                {
                    // Note: Regional/City ranks would need separate computation for accurate ranking within that scope
                    // For MVP, we use global rank - can enhance later
                    await InsertLeaderboardEntry(conn, "User", userId, userName, leaderboardType, timeRange, "Region", region, score, rank, computedDate);
                }

                // Insert City entry if available
                if (!string.IsNullOrEmpty(city))
                {
                    await InsertLeaderboardEntry(conn, "User", userId, userName, leaderboardType, timeRange, "City", city, score, rank, computedDate);
                }

                entries++;
            }

            logger.LogInformation("Computed {Count} entries for {Type} {TimeRange} leaderboard.", entries, leaderboardType, timeRange);
        }

        private static string GetLeaderboardQuery(string leaderboardType, string dateFilter)
        {
            var scoreExpression = leaderboardType switch
            {
                "Events" => "COUNT(DISTINCT ea.EventId)",
                "Bags" => "ISNULL(SUM(eam.BagsCollected), 0)",
                "Weight" => @"ISNULL(SUM(
                    CASE
                        WHEN eam.AdjustedPickedWeight IS NOT NULL THEN eam.AdjustedPickedWeight
                        WHEN eam.PickedWeightUnitId = 2 THEN eam.PickedWeight * 2.20462
                        ELSE eam.PickedWeight
                    END
                ), 0)",
                "Hours" => "ISNULL(SUM(eam.DurationMinutes), 0) / 60.0",
                _ => "COUNT(DISTINCT ea.EventId)"
            };

            var joinClause = leaderboardType switch
            {
                "Events" => "",
                _ => @"LEFT JOIN dbo.EventAttendeeMetrics eam ON ea.EventId = eam.EventId AND ea.UserId = eam.UserId
                       AND eam.Status IN ('Approved', 'Adjusted')"
            };

            return $@"
WITH UserStats AS (
    SELECT
        u.Id AS UserId,
        u.UserName,
        u.Region,
        u.City,
        {scoreExpression} AS Score,
        COUNT(DISTINCT ea.EventId) AS EventCount
    FROM dbo.Users u
    INNER JOIN dbo.EventAttendees ea ON u.Id = ea.UserId
    INNER JOIN dbo.Events e ON ea.EventId = e.Id
    {joinClause}
    WHERE u.ShowOnLeaderboards = 1
      AND e.EventStatusId != 3
      {dateFilter}
    GROUP BY u.Id, u.UserName, u.Region, u.City
    HAVING COUNT(DISTINCT ea.EventId) >= {MinimumEventsToQualify}
)
SELECT
    UserId,
    UserName,
    Region,
    City,
    CAST(Score AS DECIMAL(18,2)) AS Score,
    ROW_NUMBER() OVER (ORDER BY Score DESC, EventCount DESC, UserId) AS Rank
FROM UserStats
WHERE Score > 0
ORDER BY Rank";
        }

        private static string GetDateFilter(string timeRange)
        {
            return timeRange switch
            {
                "Week" => "AND e.EventDate >= DATEADD(day, -7, GETUTCDATE())",
                "Month" => "AND e.EventDate >= DATEADD(month, -1, GETUTCDATE())",
                "Year" => "AND e.EventDate >= DATEADD(year, -1, GETUTCDATE())",
                "AllTime" => "",
                _ => ""
            };
        }

        private async Task ComputeTeamLeaderboard(SqlConnection conn, string leaderboardType, string timeRange)
        {
            var dateFilter = GetDateFilter(timeRange);
            var sql = GetTeamLeaderboardQuery(leaderboardType, dateFilter);

            logger.LogInformation("Computing team {Type} leaderboard for {TimeRange}...", leaderboardType, timeRange);

            using var cmd = new SqlCommand(sql, conn);
            using var reader = await cmd.ExecuteReaderAsync();

            var entries = 0;
            var computedDate = DateTimeOffset.UtcNow;

            while (await reader.ReadAsync())
            {
                var teamId = reader.GetGuid(0);
                var teamName = reader.GetString(1);
                var region = reader.IsDBNull(2) ? null : reader.GetString(2);
                var city = reader.IsDBNull(3) ? null : reader.GetString(3);
                var score = reader.GetDecimal(4);
                var rank = (int)reader.GetInt64(5);

                // Insert Global entry
                await InsertLeaderboardEntry(conn, "Team", teamId, teamName, leaderboardType, timeRange, "Global", null, score, rank, computedDate);

                // Insert Region entry if available
                if (!string.IsNullOrEmpty(region))
                {
                    await InsertLeaderboardEntry(conn, "Team", teamId, teamName, leaderboardType, timeRange, "Region", region, score, rank, computedDate);
                }

                // Insert City entry if available
                if (!string.IsNullOrEmpty(city))
                {
                    await InsertLeaderboardEntry(conn, "Team", teamId, teamName, leaderboardType, timeRange, "City", city, score, rank, computedDate);
                }

                entries++;
            }

            logger.LogInformation("Computed {Count} entries for team {Type} {TimeRange} leaderboard.", entries, leaderboardType, timeRange);
        }

        private static string GetTeamLeaderboardQuery(string leaderboardType, string dateFilter)
        {
            var scoreExpression = leaderboardType switch
            {
                "Events" => "COUNT(DISTINCT ea.EventId)",
                "Bags" => "ISNULL(SUM(eam.BagsCollected), 0)",
                "Weight" => @"ISNULL(SUM(
                    CASE
                        WHEN eam.AdjustedPickedWeight IS NOT NULL THEN eam.AdjustedPickedWeight
                        WHEN eam.PickedWeightUnitId = 2 THEN eam.PickedWeight * 2.20462
                        ELSE eam.PickedWeight
                    END
                ), 0)",
                "Hours" => "ISNULL(SUM(eam.DurationMinutes), 0) / 60.0",
                _ => "COUNT(DISTINCT ea.EventId)"
            };

            var joinClause = leaderboardType switch
            {
                "Events" => "",
                _ => @"LEFT JOIN dbo.EventAttendeeMetrics eam ON ea.EventId = eam.EventId AND ea.UserId = eam.UserId
                       AND eam.Status IN ('Approved', 'Adjusted')"
            };

            // Team leaderboards aggregate metrics from all team members' event participation
            return $@"
WITH TeamStats AS (
    SELECT
        t.Id AS TeamId,
        t.Name AS TeamName,
        t.Region,
        t.City,
        {scoreExpression} AS Score,
        COUNT(DISTINCT ea.EventId) AS EventCount
    FROM dbo.Teams t
    INNER JOIN dbo.TeamMembers tm ON t.Id = tm.TeamId
    INNER JOIN dbo.EventAttendees ea ON tm.UserId = ea.UserId
    INNER JOIN dbo.Events e ON ea.EventId = e.Id
    {joinClause}
    WHERE t.IsActive = 1
      AND e.EventStatusId != 3
      {dateFilter}
    GROUP BY t.Id, t.Name, t.Region, t.City
    HAVING COUNT(DISTINCT ea.EventId) >= {MinimumEventsToQualify}
)
SELECT
    TeamId,
    TeamName,
    Region,
    City,
    CAST(Score AS DECIMAL(18,2)) AS Score,
    ROW_NUMBER() OVER (ORDER BY Score DESC, EventCount DESC, TeamId) AS Rank
FROM TeamStats
WHERE Score > 0
ORDER BY Rank";
        }

        private async Task InsertLeaderboardEntry(
            SqlConnection conn,
            string entityType,
            Guid entityId,
            string entityName,
            string leaderboardType,
            string timeRange,
            string locationScope,
            string? locationValue,
            decimal score,
            int rank,
            DateTimeOffset computedDate)
        {
            var sql = @"
INSERT INTO dbo.LeaderboardCaches
    (Id, EntityType, EntityId, EntityName, LeaderboardType, TimeRange, LocationScope, LocationValue, Score, Rank, ComputedDate, CreatedDate)
VALUES
    (@Id, @EntityType, @EntityId, @EntityName, @LeaderboardType, @TimeRange, @LocationScope, @LocationValue, @Score, @Rank, @ComputedDate, @CreatedDate)";

            using var cmd = new SqlCommand(sql, conn);
            cmd.Parameters.AddWithValue("@Id", Guid.NewGuid());
            cmd.Parameters.AddWithValue("@EntityType", entityType);
            cmd.Parameters.AddWithValue("@EntityId", entityId);
            cmd.Parameters.AddWithValue("@EntityName", entityName);
            cmd.Parameters.AddWithValue("@LeaderboardType", leaderboardType);
            cmd.Parameters.AddWithValue("@TimeRange", timeRange);
            cmd.Parameters.AddWithValue("@LocationScope", locationScope);
            cmd.Parameters.AddWithValue("@LocationValue", locationValue ?? (object)DBNull.Value);
            cmd.Parameters.AddWithValue("@Score", score);
            cmd.Parameters.AddWithValue("@Rank", rank);
            cmd.Parameters.AddWithValue("@ComputedDate", computedDate);
            cmd.Parameters.AddWithValue("@CreatedDate", DateTimeOffset.UtcNow);

            await cmd.ExecuteNonQueryAsync();
        }
    }
}
