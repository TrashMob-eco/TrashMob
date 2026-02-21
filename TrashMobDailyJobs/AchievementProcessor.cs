namespace TrashMobDailyJobs
{
    using System;
    using System.Collections.Generic;
    using System.Text.Json;
    using System.Threading.Tasks;
    using Microsoft.Data.SqlClient;
    using Microsoft.Extensions.Configuration;
    using Microsoft.Extensions.Logging;

    /// <summary>
    /// Daily job that processes achievements for all users.
    /// Checks user stats against achievement criteria and awards achievements.
    /// </summary>
    public class AchievementProcessor(ILogger<AchievementProcessor> logger, IConfiguration configuration)
    {
        public async Task RunAsync()
        {
            logger.LogInformation("AchievementProcessor job started at: {Time}", DateTime.UtcNow);
            var connectionString = configuration["TMDBServerConnectionString"];

            if (string.IsNullOrEmpty(connectionString))
            {
                logger.LogError("Database connection string is not configured.");
                return;
            }

            using var conn = new SqlConnection(connectionString);
            await conn.OpenAsync();

            // Get all active achievement types
            var achievementTypes = await GetAchievementTypesAsync(conn);
            logger.LogInformation("Found {Count} achievement types to process.", achievementTypes.Count);

            var totalAwarded = 0;

            foreach (var achievementType in achievementTypes)
            {
                var awarded = await ProcessAchievementTypeAsync(conn, achievementType);
                totalAwarded += awarded;
            }

            logger.LogInformation("AchievementProcessor job completed at: {Time}. Total achievements awarded: {Count}", DateTime.UtcNow, totalAwarded);
        }

        private async Task<List<AchievementTypeInfo>> GetAchievementTypesAsync(SqlConnection conn)
        {
            var achievements = new List<AchievementTypeInfo>();
            var sql = "SELECT Id, Name, Criteria FROM AchievementTypes WHERE IsActive = 1";

            using var cmd = new SqlCommand(sql, conn);
            using var reader = await cmd.ExecuteReaderAsync();

            while (await reader.ReadAsync())
            {
                achievements.Add(new AchievementTypeInfo
                {
                    Id = reader.GetInt32(0),
                    Name = reader.GetString(1),
                    Criteria = reader.GetString(2)
                });
            }

            return achievements;
        }

        private async Task<int> ProcessAchievementTypeAsync(SqlConnection conn, AchievementTypeInfo achievementType)
        {
            try
            {
                var criteria = JsonDocument.Parse(achievementType.Criteria).RootElement;

                if (criteria.TryGetProperty("eventsAttended", out var eventsAttended))
                {
                    return await ProcessEventsAttendedAsync(conn, achievementType.Id, eventsAttended.GetInt32());
                }
                else if (criteria.TryGetProperty("bagsCollected", out var bagsCollected))
                {
                    return await ProcessBagsCollectedAsync(conn, achievementType.Id, bagsCollected.GetInt32());
                }
                else if (criteria.TryGetProperty("eventsCreated", out var eventsCreated))
                {
                    return await ProcessEventsCreatedAsync(conn, achievementType.Id, eventsCreated.GetInt32());
                }
                else if (criteria.TryGetProperty("joinedTeam", out var joinedTeam) && joinedTeam.GetBoolean())
                {
                    return await ProcessJoinedTeamAsync(conn, achievementType.Id);
                }
                else
                {
                    logger.LogWarning("Unknown criteria format for achievement {Name}: {Criteria}", achievementType.Name, achievementType.Criteria);
                    return 0;
                }
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Error processing achievement {Name}", achievementType.Name);
                return 0;
            }
        }

        private async Task<int> ProcessEventsAttendedAsync(SqlConnection conn, int achievementTypeId, int requiredEvents)
        {
            logger.LogInformation("Processing eventsAttended >= {Required} for achievement {Id}", requiredEvents, achievementTypeId);

            // Find users who have attended the required number of events but don't have this achievement
            var sql = @"
                INSERT INTO UserAchievements (Id, UserId, AchievementTypeId, EarnedDate, NotificationSent, CreatedByUserId, CreatedDate, LastUpdatedByUserId, LastUpdatedDate)
                SELECT NEWID(), u.Id, @AchievementTypeId, SYSDATETIMEOFFSET(), 0, u.Id, SYSDATETIMEOFFSET(), u.Id, SYSDATETIMEOFFSET()
                FROM Users u
                WHERE (
                    SELECT COUNT(DISTINCT ea.EventId)
                    FROM EventAttendees ea
                    INNER JOIN Events e ON ea.EventId = e.Id
                    WHERE ea.UserId = u.Id AND e.EventStatusId != 3
                ) >= @RequiredEvents
                AND NOT EXISTS (
                    SELECT 1 FROM UserAchievements ua WHERE ua.UserId = u.Id AND ua.AchievementTypeId = @AchievementTypeId
                )";

            using var cmd = new SqlCommand(sql, conn);
            cmd.Parameters.AddWithValue("@AchievementTypeId", achievementTypeId);
            cmd.Parameters.AddWithValue("@RequiredEvents", requiredEvents);

            var rowsAffected = await cmd.ExecuteNonQueryAsync();
            if (rowsAffected > 0)
            {
                logger.LogInformation("Awarded {Count} users the eventsAttended achievement {Id}", rowsAffected, achievementTypeId);
            }
            return rowsAffected;
        }

        private async Task<int> ProcessBagsCollectedAsync(SqlConnection conn, int achievementTypeId, int requiredBags)
        {
            logger.LogInformation("Processing bagsCollected >= {Required} for achievement {Id}", requiredBags, achievementTypeId);

            // Find users who have collected the required number of bags but don't have this achievement
            var sql = @"
                INSERT INTO UserAchievements (Id, UserId, AchievementTypeId, EarnedDate, NotificationSent, CreatedByUserId, CreatedDate, LastUpdatedByUserId, LastUpdatedDate)
                SELECT NEWID(), u.Id, @AchievementTypeId, SYSDATETIMEOFFSET(), 0, u.Id, SYSDATETIMEOFFSET(), u.Id, SYSDATETIMEOFFSET()
                FROM Users u
                WHERE (
                    SELECT ISNULL(SUM(eam.BagsCollected), 0)
                    FROM EventAttendeeMetrics eam
                    WHERE eam.UserId = u.Id AND eam.Status IN ('Approved', 'Adjusted')
                ) >= @RequiredBags
                AND NOT EXISTS (
                    SELECT 1 FROM UserAchievements ua WHERE ua.UserId = u.Id AND ua.AchievementTypeId = @AchievementTypeId
                )";

            using var cmd = new SqlCommand(sql, conn);
            cmd.Parameters.AddWithValue("@AchievementTypeId", achievementTypeId);
            cmd.Parameters.AddWithValue("@RequiredBags", requiredBags);

            var rowsAffected = await cmd.ExecuteNonQueryAsync();
            if (rowsAffected > 0)
            {
                logger.LogInformation("Awarded {Count} users the bagsCollected achievement {Id}", rowsAffected, achievementTypeId);
            }
            return rowsAffected;
        }

        private async Task<int> ProcessEventsCreatedAsync(SqlConnection conn, int achievementTypeId, int requiredEvents)
        {
            logger.LogInformation("Processing eventsCreated >= {Required} for achievement {Id}", requiredEvents, achievementTypeId);

            // Find users who have created the required number of events but don't have this achievement
            var sql = @"
                INSERT INTO UserAchievements (Id, UserId, AchievementTypeId, EarnedDate, NotificationSent, CreatedByUserId, CreatedDate, LastUpdatedByUserId, LastUpdatedDate)
                SELECT NEWID(), u.Id, @AchievementTypeId, SYSDATETIMEOFFSET(), 0, u.Id, SYSDATETIMEOFFSET(), u.Id, SYSDATETIMEOFFSET()
                FROM Users u
                WHERE (
                    SELECT COUNT(*)
                    FROM Events e
                    WHERE e.CreatedByUserId = u.Id AND e.EventStatusId != 3
                ) >= @RequiredEvents
                AND NOT EXISTS (
                    SELECT 1 FROM UserAchievements ua WHERE ua.UserId = u.Id AND ua.AchievementTypeId = @AchievementTypeId
                )";

            using var cmd = new SqlCommand(sql, conn);
            cmd.Parameters.AddWithValue("@AchievementTypeId", achievementTypeId);
            cmd.Parameters.AddWithValue("@RequiredEvents", requiredEvents);

            var rowsAffected = await cmd.ExecuteNonQueryAsync();
            if (rowsAffected > 0)
            {
                logger.LogInformation("Awarded {Count} users the eventsCreated achievement {Id}", rowsAffected, achievementTypeId);
            }
            return rowsAffected;
        }

        private async Task<int> ProcessJoinedTeamAsync(SqlConnection conn, int achievementTypeId)
        {
            logger.LogInformation("Processing joinedTeam for achievement {Id}", achievementTypeId);

            // Find users who are members of a team but don't have this achievement
            var sql = @"
                INSERT INTO UserAchievements (Id, UserId, AchievementTypeId, EarnedDate, NotificationSent, CreatedByUserId, CreatedDate, LastUpdatedByUserId, LastUpdatedDate)
                SELECT NEWID(), u.Id, @AchievementTypeId, SYSDATETIMEOFFSET(), 0, u.Id, SYSDATETIMEOFFSET(), u.Id, SYSDATETIMEOFFSET()
                FROM Users u
                WHERE EXISTS (
                    SELECT 1 FROM TeamMembers tm
                    INNER JOIN Teams t ON tm.TeamId = t.Id
                    WHERE tm.UserId = u.Id AND t.IsActive = 1
                )
                AND NOT EXISTS (
                    SELECT 1 FROM UserAchievements ua WHERE ua.UserId = u.Id AND ua.AchievementTypeId = @AchievementTypeId
                )";

            using var cmd = new SqlCommand(sql, conn);
            cmd.Parameters.AddWithValue("@AchievementTypeId", achievementTypeId);

            var rowsAffected = await cmd.ExecuteNonQueryAsync();
            if (rowsAffected > 0)
            {
                logger.LogInformation("Awarded {Count} users the joinedTeam achievement {Id}", rowsAffected, achievementTypeId);
            }
            return rowsAffected;
        }

        private class AchievementTypeInfo
        {
            public int Id { get; set; }
            public string Name { get; set; } = string.Empty;
            public string Criteria { get; set; } = string.Empty;
        }
    }
}
