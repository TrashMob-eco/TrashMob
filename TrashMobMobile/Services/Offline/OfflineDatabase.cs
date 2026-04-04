namespace TrashMobMobile.Services.Offline;

using System.Diagnostics;
using SQLite;

/// <summary>
/// Manages the local SQLite database for offline persistence of routes, metrics, and photos.
/// Uses WAL journal mode for crash-safe writes during GPS recording.
/// Schema versioning via PRAGMA user_version enables safe table migrations on app updates.
/// </summary>
public class OfflineDatabase
{
    private const string DatabaseFileName = "trashmob_offline.db";

    /// <summary>
    /// Current schema version. Increment when adding new tables or columns.
    /// v1: PendingRouteSession, PendingRoutePoint, PendingMetrics, PendingPhoto
    /// </summary>
    private const int CurrentSchemaVersion = 1;

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
            // Use ExecuteScalarAsync because PRAGMA journal_mode returns a result row;
            // ExecuteAsync maps to ExecuteNonQuery which throws on result-returning statements
            await connection.ExecuteScalarAsync<string>("PRAGMA journal_mode=WAL");

            // Check and apply schema migrations
            await MigrateSchemaAsync(connection);

            isInitialized = true;
            return connection;
        }
        finally
        {
            initLock.Release();
        }
    }

    /// <summary>
    /// Checks the stored schema version and applies any needed migrations.
    /// Uses PRAGMA user_version which is an integer stored in the database file header.
    /// </summary>
    private static async Task MigrateSchemaAsync(SQLiteAsyncConnection db)
    {
        var storedVersion = await db.ExecuteScalarAsync<int>("PRAGMA user_version");

        if (storedVersion < 1)
        {
            // v1: Initial tables
            await db.CreateTableAsync<PendingRouteSession>();
            await db.CreateTableAsync<PendingRoutePoint>();
            await db.CreateTableAsync<PendingMetrics>();
            await db.CreateTableAsync<PendingPhoto>();
        }

        // Future migrations:
        // if (storedVersion < 2) { /* add new columns/tables for v2 */ }

        if (storedVersion != CurrentSchemaVersion)
        {
            await db.ExecuteAsync($"PRAGMA user_version = {CurrentSchemaVersion}");
            Debug.WriteLine($"OfflineDatabase: Migrated schema from v{storedVersion} to v{CurrentSchemaVersion}");
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
        // Note: filter by date in memory because SQLite cannot translate .CompareTo()
        var uploadedSessions = (await db.Table<PendingRouteSession>()
            .Where(s => s.Status == RouteSessionStatus.Uploaded)
            .ToListAsync())
            .Where(s => string.Compare(s.CreatedAt, cutoff, StringComparison.Ordinal) < 0)
            .ToList();

        foreach (var session in uploadedSessions)
        {
            await db.ExecuteAsync("DELETE FROM PendingRoutePoints WHERE SessionId = ?", session.SessionId);
            await db.DeleteAsync(session);
            deletedCount++;
        }

        // Delete uploaded metrics
        var uploadedMetrics = (await db.Table<PendingMetrics>()
            .Where(m => m.Status == PendingUploadStatus.Uploaded)
            .ToListAsync())
            .Where(m => string.Compare(m.CreatedAt, cutoff, StringComparison.Ordinal) < 0)
            .ToList();

        foreach (var metrics in uploadedMetrics)
        {
            await db.DeleteAsync(metrics);
            deletedCount++;
        }

        // Delete uploaded photos and their local files
        var uploadedPhotos = (await db.Table<PendingPhoto>()
            .Where(p => p.Status == PendingUploadStatus.Uploaded)
            .ToListAsync())
            .Where(p => string.Compare(p.CreatedAt, cutoff, StringComparison.Ordinal) < 0)
            .ToList();

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
