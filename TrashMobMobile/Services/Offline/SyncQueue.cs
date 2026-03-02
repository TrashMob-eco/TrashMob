namespace TrashMobMobile.Services.Offline;

/// <summary>
/// Manages the offline queue: enqueues pending uploads, retrieves items for sync,
/// and tracks status transitions.
/// </summary>
public class SyncQueue(OfflineDatabase offlineDatabase)
{
    private const int MaxRetryCount = 20;

    private static readonly TimeSpan[] RetryDelays =
    [
        TimeSpan.FromMinutes(1),
        TimeSpan.FromMinutes(5),
        TimeSpan.FromMinutes(15),
        TimeSpan.FromMinutes(30),
        TimeSpan.FromHours(1),
    ];

    /// <summary>
    /// Creates a new route session record in SQLite.
    /// </summary>
    public async Task<PendingRouteSession> CreateRouteSessionAsync(
        Guid eventId, Guid userId, DateTimeOffset startTime, bool skipDefaultTrim)
    {
        var db = await offlineDatabase.GetConnectionAsync();

        var session = new PendingRouteSession
        {
            SessionId = Guid.NewGuid().ToString(),
            EventId = eventId.ToString(),
            UserId = userId.ToString(),
            StartTime = startTime.ToString("O"),
            Status = RouteSessionStatus.Recording,
            SkipDefaultTrim = skipDefaultTrim,
            CreatedAt = DateTimeOffset.UtcNow.ToString("O"),
        };

        await db.InsertAsync(session);
        return session;
    }

    /// <summary>
    /// Marks a recording session as ready for upload.
    /// </summary>
    public async Task MarkSessionPendingUploadAsync(string sessionId, DateTimeOffset endTime)
    {
        var db = await offlineDatabase.GetConnectionAsync();
        var session = await db.FindAsync<PendingRouteSession>(sessionId);
        if (session == null)
        {
            return;
        }

        session.EndTime = endTime.ToString("O");
        session.Status = RouteSessionStatus.PendingUpload;
        await db.UpdateAsync(session);
    }

    /// <summary>
    /// Marks a session as successfully uploaded and deletes its route points.
    /// </summary>
    public async Task MarkSessionUploadedAsync(string sessionId)
    {
        var db = await offlineDatabase.GetConnectionAsync();
        var session = await db.FindAsync<PendingRouteSession>(sessionId);
        if (session == null)
        {
            return;
        }

        session.Status = RouteSessionStatus.Uploaded;
        await db.UpdateAsync(session);

        // Delete route points — they're on the server now
        await db.ExecuteAsync("DELETE FROM PendingRoutePoints WHERE SessionId = ?", sessionId);
    }

    /// <summary>
    /// Marks a session as failed and schedules the next retry.
    /// </summary>
    public async Task MarkSessionFailedAsync(string sessionId, string error)
    {
        var db = await offlineDatabase.GetConnectionAsync();
        var session = await db.FindAsync<PendingRouteSession>(sessionId);
        if (session == null)
        {
            return;
        }

        session.RetryCount++;
        session.LastError = error;

        if (session.RetryCount >= MaxRetryCount)
        {
            session.Status = RouteSessionStatus.PermanentlyFailed;
        }
        else
        {
            session.Status = RouteSessionStatus.Failed;
            var delayIndex = Math.Min(session.RetryCount - 1, RetryDelays.Length - 1);
            var nextRetry = DateTimeOffset.UtcNow.Add(RetryDelays[delayIndex]);
            session.NextRetryAt = nextRetry.ToString("O");
        }

        await db.UpdateAsync(session);
    }

    /// <summary>
    /// Gets all sessions that are ready to be uploaded (pending or failed with retry time elapsed).
    /// </summary>
    public async Task<List<PendingRouteSession>> GetSessionsReadyForUploadAsync()
    {
        var db = await offlineDatabase.GetConnectionAsync();
        var now = DateTimeOffset.UtcNow.ToString("O");

        var pendingSessions = await db.Table<PendingRouteSession>()
            .Where(s => s.Status == RouteSessionStatus.PendingUpload)
            .ToListAsync();

        var failedSessions = await db.Table<PendingRouteSession>()
            .Where(s => s.Status == RouteSessionStatus.Failed)
            .ToListAsync();

        // Filter failed sessions where retry time has elapsed
        var readyFailedSessions = failedSessions
            .Where(s => s.NextRetryAt == null || string.Compare(s.NextRetryAt, now, StringComparison.Ordinal) <= 0)
            .ToList();

        pendingSessions.AddRange(readyFailedSessions);
        return pendingSessions;
    }

    /// <summary>
    /// Gets route points for a session, ordered by point order.
    /// </summary>
    public async Task<List<PendingRoutePoint>> GetRoutePointsAsync(string sessionId)
    {
        var db = await offlineDatabase.GetConnectionAsync();
        return await db.Table<PendingRoutePoint>()
            .Where(p => p.SessionId == sessionId)
            .OrderBy(p => p.PointOrder)
            .ToListAsync();
    }

    /// <summary>
    /// Finds any sessions that were left in the Recording state (crashed sessions).
    /// </summary>
    public async Task<List<PendingRouteSession>> GetInterruptedSessionsAsync()
    {
        var db = await offlineDatabase.GetConnectionAsync();
        return await db.Table<PendingRouteSession>()
            .Where(s => s.Status == RouteSessionStatus.Recording)
            .ToListAsync();
    }

    /// <summary>
    /// Gets the total count of items pending upload (for sync badge).
    /// </summary>
    public async Task<int> GetPendingCountAsync()
    {
        var db = await offlineDatabase.GetConnectionAsync();
        return await db.Table<PendingRouteSession>()
            .Where(s => s.Status == RouteSessionStatus.PendingUpload
                || s.Status == RouteSessionStatus.Failed
                || s.Status == RouteSessionStatus.Uploading)
            .CountAsync();
    }

    /// <summary>
    /// Discards a session and its route points entirely.
    /// </summary>
    public async Task DiscardSessionAsync(string sessionId)
    {
        var db = await offlineDatabase.GetConnectionAsync();
        await db.ExecuteAsync("DELETE FROM PendingRoutePoints WHERE SessionId = ?", sessionId);
        await db.ExecuteAsync("DELETE FROM PendingRouteSessions WHERE SessionId = ?", sessionId);
    }

    /// <summary>
    /// Gets the highest point order for a session (for resuming after crash).
    /// </summary>
    public async Task<int> GetMaxPointOrderAsync(string sessionId)
    {
        var db = await offlineDatabase.GetConnectionAsync();
        var result = await db.ExecuteScalarAsync<int?>(
            "SELECT MAX(PointOrder) FROM PendingRoutePoints WHERE SessionId = ?", sessionId);
        return result ?? 0;
    }

    // ── Metrics Queue ──────────────────────────────────────────────────

    /// <summary>
    /// Saves metrics to SQLite for offline persistence.
    /// </summary>
    public async Task<PendingMetrics> EnqueueMetricsAsync(
        Guid eventId, Guid userId, int? bagsCollected, decimal? pickedWeight,
        int? pickedWeightUnitId, int? durationMinutes, string? notes)
    {
        var db = await offlineDatabase.GetConnectionAsync();

        var pending = new PendingMetrics
        {
            EventId = eventId.ToString(),
            UserId = userId.ToString(),
            BagsCollected = bagsCollected,
            PickedWeight = pickedWeight?.ToString("G"),
            PickedWeightUnitId = pickedWeightUnitId,
            DurationMinutes = durationMinutes,
            Notes = notes,
            Status = PendingUploadStatus.PendingUpload,
            CreatedAt = DateTimeOffset.UtcNow.ToString("O"),
        };

        await db.InsertAsync(pending);
        return pending;
    }

    /// <summary>
    /// Gets all metrics pending upload (pending or failed with retry time elapsed).
    /// </summary>
    public async Task<List<PendingMetrics>> GetMetricsReadyForUploadAsync()
    {
        var db = await offlineDatabase.GetConnectionAsync();
        var now = DateTimeOffset.UtcNow.ToString("O");

        var pending = await db.Table<PendingMetrics>()
            .Where(m => m.Status == PendingUploadStatus.PendingUpload)
            .ToListAsync();

        var failed = await db.Table<PendingMetrics>()
            .Where(m => m.Status == PendingUploadStatus.Failed)
            .ToListAsync();

        var readyFailed = failed
            .Where(m => m.NextRetryAt == null || string.Compare(m.NextRetryAt, now, StringComparison.Ordinal) <= 0)
            .ToList();

        pending.AddRange(readyFailed);
        return pending;
    }

    /// <summary>
    /// Marks metrics as successfully uploaded.
    /// </summary>
    public async Task MarkMetricsUploadedAsync(int id)
    {
        var db = await offlineDatabase.GetConnectionAsync();
        var item = await db.FindAsync<PendingMetrics>(id);
        if (item == null)
        {
            return;
        }

        item.Status = PendingUploadStatus.Uploaded;
        await db.UpdateAsync(item);
    }

    /// <summary>
    /// Marks metrics as failed and schedules the next retry.
    /// </summary>
    public async Task MarkMetricsFailedAsync(int id, string error)
    {
        var db = await offlineDatabase.GetConnectionAsync();
        var item = await db.FindAsync<PendingMetrics>(id);
        if (item == null)
        {
            return;
        }

        item.RetryCount++;
        item.LastError = error;

        if (item.RetryCount >= MaxRetryCount)
        {
            item.Status = PendingUploadStatus.PermanentlyFailed;
        }
        else
        {
            item.Status = PendingUploadStatus.Failed;
            var delayIndex = Math.Min(item.RetryCount - 1, RetryDelays.Length - 1);
            var nextRetry = DateTimeOffset.UtcNow.Add(RetryDelays[delayIndex]);
            item.NextRetryAt = nextRetry.ToString("O");
        }

        await db.UpdateAsync(item);
    }

    // ── Photo Queue ────────────────────────────────────────────────────

    /// <summary>
    /// Saves a photo record to SQLite for offline persistence.
    /// The actual image file should already be at localFilePath.
    /// </summary>
    public async Task<PendingPhoto> EnqueuePhotoAsync(
        Guid eventId, Guid userId, string localFilePath, int photoType)
    {
        var db = await offlineDatabase.GetConnectionAsync();

        var pending = new PendingPhoto
        {
            PhotoId = Guid.NewGuid().ToString(),
            EventId = eventId.ToString(),
            UserId = userId.ToString(),
            LocalFilePath = localFilePath,
            PhotoType = photoType,
            Status = PendingUploadStatus.PendingUpload,
            CreatedAt = DateTimeOffset.UtcNow.ToString("O"),
        };

        await db.InsertAsync(pending);
        return pending;
    }

    /// <summary>
    /// Gets all photos pending upload (pending or failed with retry time elapsed).
    /// </summary>
    public async Task<List<PendingPhoto>> GetPhotosReadyForUploadAsync()
    {
        var db = await offlineDatabase.GetConnectionAsync();
        var now = DateTimeOffset.UtcNow.ToString("O");

        var pending = await db.Table<PendingPhoto>()
            .Where(p => p.Status == PendingUploadStatus.PendingUpload)
            .ToListAsync();

        var failed = await db.Table<PendingPhoto>()
            .Where(p => p.Status == PendingUploadStatus.Failed)
            .ToListAsync();

        var readyFailed = failed
            .Where(p => p.NextRetryAt == null || string.Compare(p.NextRetryAt, now, StringComparison.Ordinal) <= 0)
            .ToList();

        pending.AddRange(readyFailed);
        return pending;
    }

    /// <summary>
    /// Marks a photo as successfully uploaded and deletes the local file.
    /// </summary>
    public async Task MarkPhotoUploadedAsync(int id)
    {
        var db = await offlineDatabase.GetConnectionAsync();
        var item = await db.FindAsync<PendingPhoto>(id);
        if (item == null)
        {
            return;
        }

        item.Status = PendingUploadStatus.Uploaded;
        await db.UpdateAsync(item);

        // Delete local file — it's on the server now
        if (File.Exists(item.LocalFilePath))
        {
            File.Delete(item.LocalFilePath);
        }
    }

    /// <summary>
    /// Marks a photo as failed and schedules the next retry.
    /// </summary>
    public async Task MarkPhotoFailedAsync(int id, string error)
    {
        var db = await offlineDatabase.GetConnectionAsync();
        var item = await db.FindAsync<PendingPhoto>(id);
        if (item == null)
        {
            return;
        }

        item.RetryCount++;
        item.LastError = error;

        if (item.RetryCount >= MaxRetryCount)
        {
            item.Status = PendingUploadStatus.PermanentlyFailed;

            // Delete local file for permanently failed photos to reclaim space
            if (File.Exists(item.LocalFilePath))
            {
                File.Delete(item.LocalFilePath);
            }
        }
        else
        {
            item.Status = PendingUploadStatus.Failed;
            var delayIndex = Math.Min(item.RetryCount - 1, RetryDelays.Length - 1);
            var nextRetry = DateTimeOffset.UtcNow.Add(RetryDelays[delayIndex]);
            item.NextRetryAt = nextRetry.ToString("O");
        }

        await db.UpdateAsync(item);
    }

    // ── Combined Pending Count ─────────────────────────────────────────

    /// <summary>
    /// Gets the total count of all items pending upload across routes, metrics, and photos.
    /// </summary>
    public async Task<int> GetTotalPendingCountAsync()
    {
        var routeCount = await GetPendingCountAsync();

        var db = await offlineDatabase.GetConnectionAsync();

        var metricsCount = await db.Table<PendingMetrics>()
            .Where(m => m.Status == PendingUploadStatus.PendingUpload
                || m.Status == PendingUploadStatus.Failed
                || m.Status == PendingUploadStatus.Uploading)
            .CountAsync();

        var photoCount = await db.Table<PendingPhoto>()
            .Where(p => p.Status == PendingUploadStatus.PendingUpload
                || p.Status == PendingUploadStatus.Failed
                || p.Status == PendingUploadStatus.Uploading)
            .CountAsync();

        return routeCount + metricsCount + photoCount;
    }
}
