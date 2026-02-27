namespace TrashMobDailyJobs
{
    using System;
    using System.Threading.Tasks;
    using Microsoft.Data.SqlClient;
    using Microsoft.Extensions.Configuration;
    using Microsoft.Extensions.Logging;

    /// <summary>
    /// One-time backfill processor that creates EventAttendeeMetrics records
    /// for historical events that only have aggregate EventSummary data.
    /// This allows past event contributions to appear on leaderboards and achievements.
    /// Remove this processor after it has run successfully on both DEV and PROD.
    /// </summary>
    public class HistoricalMetricsBackfill(
        ILogger<HistoricalMetricsBackfill> logger,
        IConfiguration configuration)
    {
        public async Task RunAsync()
        {
            var connectionString = configuration["TMDBServerConnectionString"];
            using var conn = new SqlConnection(connectionString);
            await conn.OpenAsync();

            // First, check how many events need backfilling
            var countSql = @"
                SELECT COUNT(DISTINCT es.EventId)
                FROM dbo.EventSummary es
                INNER JOIN dbo.Events e ON es.EventId = e.Id
                WHERE e.EventStatusId != 3
                  AND (es.NumberOfBags > 0 OR es.PickedWeight > 0 OR es.DurationInMinutes > 0)
                  AND NOT EXISTS (
                      SELECT 1 FROM dbo.EventAttendeeMetrics eam
                      WHERE eam.EventId = es.EventId
                  )
                  AND EXISTS (
                      SELECT 1 FROM dbo.EventAttendees ea
                      WHERE ea.EventId = es.EventId AND ea.CanceledDate IS NULL
                  )";

            int eventsToBackfill;
            using (var countCmd = new SqlCommand(countSql, conn))
            {
                eventsToBackfill = (int)(await countCmd.ExecuteScalarAsync() ?? 0);
            }

            if (eventsToBackfill == 0)
            {
                logger.LogInformation("No events need backfilling. All events already have EventAttendeeMetrics records.");
                return;
            }

            logger.LogInformation("Found {Count} events that need EventAttendeeMetrics backfill.", eventsToBackfill);

            // Perform the backfill in a transaction
            using var transaction = conn.BeginTransaction();

            try
            {
                var insertSql = @"
                    INSERT INTO dbo.EventAttendeeMetrics (
                        Id, EventId, UserId,
                        BagsCollected, PickedWeight, PickedWeightUnitId, DurationMinutes,
                        Notes, Status, ReviewedByUserId, ReviewedDate,
                        CreatedByUserId, CreatedDate, LastUpdatedByUserId, LastUpdatedDate
                    )
                    SELECT
                        NEWID(),
                        ea.EventId,
                        ea.UserId,
                        -- Bags: integer division, remainder distributed to first N attendees
                        es.NumberOfBags / attendees.AttendeeCount +
                            CASE WHEN ROW_NUMBER() OVER (PARTITION BY ea.EventId ORDER BY ea.UserId)
                                      <= (es.NumberOfBags % attendees.AttendeeCount)
                                 THEN 1 ELSE 0 END,
                        -- Weight: decimal division, rounded to 1 decimal place
                        ROUND(CAST(es.PickedWeight AS DECIMAL(10,1)) / attendees.AttendeeCount, 1),
                        es.PickedWeightUnitId,
                        -- Duration: same for everyone (all attended the full event)
                        es.DurationInMinutes,
                        'Backfilled from event summary',
                        'Approved',
                        e.CreatedByUserId,
                        e.LastUpdatedDate,
                        ea.UserId,
                        GETUTCDATE(),
                        ea.UserId,
                        GETUTCDATE()
                    FROM dbo.EventAttendees ea
                    INNER JOIN dbo.Events e ON ea.EventId = e.Id
                    INNER JOIN dbo.EventSummary es ON e.Id = es.EventId
                    CROSS APPLY (
                        SELECT COUNT(*) AS AttendeeCount
                        FROM dbo.EventAttendees ea2
                        WHERE ea2.EventId = ea.EventId AND ea2.CanceledDate IS NULL
                    ) attendees
                    WHERE ea.CanceledDate IS NULL
                      AND e.EventStatusId != 3
                      AND attendees.AttendeeCount > 0
                      AND (es.NumberOfBags > 0 OR es.PickedWeight > 0 OR es.DurationInMinutes > 0)
                      AND NOT EXISTS (
                          SELECT 1 FROM dbo.EventAttendeeMetrics eam
                          WHERE eam.EventId = ea.EventId
                      )";

                int rowsInserted;
                using (var insertCmd = new SqlCommand(insertSql, conn))
                {
                    insertCmd.Transaction = transaction;
                    insertCmd.CommandTimeout = 120;
                    rowsInserted = await insertCmd.ExecuteNonQueryAsync();
                }

                transaction.Commit();

                logger.LogInformation(
                    "Historical metrics backfill complete. Inserted {RowCount} EventAttendeeMetrics records for {EventCount} events.",
                    rowsInserted, eventsToBackfill);
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Historical metrics backfill failed. Rolling back transaction.");
                transaction.Rollback();
                throw;
            }
        }
    }
}
