namespace TrashMobMobile.Services.Offline;

using SQLite;

/// <summary>
/// Manages the local SQLite database for offline persistence of routes, metrics, and photos.
/// Uses WAL journal mode for crash-safe writes during GPS recording.
/// </summary>
public class OfflineDatabase
{
    private const string DatabaseFileName = "trashmob_offline.db";
    private const int SchemaVersion = 1;

    private SQLiteAsyncConnection? connection;
    private readonly SemaphoreSlim initLock = new(1, 1);
    private bool isInitialized;

    public async Task<SQLiteAsyncConnection> GetConnectionAsync()
    {
        if (isInitialized && connection != null)
        {
            return connection;
        }

        await initLock.WaitAsync();
        try
        {
            if (isInitialized && connection != null)
            {
                return connection;
            }

            var databasePath = Path.Combine(FileSystem.AppDataDirectory, DatabaseFileName);

            connection = new SQLiteAsyncConnection(databasePath, SQLiteOpenFlags.ReadWrite | SQLiteOpenFlags.Create | SQLiteOpenFlags.SharedCache);

            // Enable WAL mode for crash safety and concurrent read/write
            await connection.ExecuteAsync("PRAGMA journal_mode=WAL");

            // Create tables
            await connection.CreateTableAsync<PendingRouteSession>();
            await connection.CreateTableAsync<PendingRoutePoint>();
            await connection.CreateTableAsync<PendingMetrics>();
            await connection.CreateTableAsync<PendingPhoto>();

            isInitialized = true;
            return connection;
        }
        finally
        {
            initLock.Release();
        }
    }

    /// <summary>
    /// Removes uploaded records older than the specified age and runs VACUUM to reclaim space.
    /// </summary>
    public async Task CleanupAsync(TimeSpan maxAge)
    {
        var db = await GetConnectionAsync();
        var cutoff = DateTimeOffset.UtcNow.Subtract(maxAge).ToString("O");

        var deletedCount = 0;

        // Delete uploaded route sessions and their points
        var uploadedSessions = await db.Table<PendingRouteSession>()
            .Where(s => s.Status == RouteSessionStatus.Uploaded && s.CreatedAt.CompareTo(cutoff) < 0)
            .ToListAsync();

        foreach (var session in uploadedSessions)
        {
            await db.ExecuteAsync("DELETE FROM PendingRoutePoints WHERE SessionId = ?", session.SessionId);
            await db.DeleteAsync(session);
            deletedCount++;
        }

        // Delete uploaded metrics
        var uploadedMetrics = await db.Table<PendingMetrics>()
            .Where(m => m.Status == PendingUploadStatus.Uploaded && m.CreatedAt.CompareTo(cutoff) < 0)
            .ToListAsync();

        foreach (var metrics in uploadedMetrics)
        {
            await db.DeleteAsync(metrics);
            deletedCount++;
        }

        // Delete uploaded photos and their local files
        var uploadedPhotos = await db.Table<PendingPhoto>()
            .Where(p => p.Status == PendingUploadStatus.Uploaded && p.CreatedAt.CompareTo(cutoff) < 0)
            .ToListAsync();

        foreach (var photo in uploadedPhotos)
        {
            if (File.Exists(photo.LocalFilePath))
            {
                File.Delete(photo.LocalFilePath);
            }

            await db.DeleteAsync(photo);
            deletedCount++;
        }

        // VACUUM to reclaim space (only if we deleted something)
        if (deletedCount > 0)
        {
            await db.ExecuteAsync("VACUUM");
        }
    }
}
