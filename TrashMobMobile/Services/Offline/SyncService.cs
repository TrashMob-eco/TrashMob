namespace TrashMobMobile.Services.Offline;

using System.Diagnostics;
using TrashMob.Models;
using TrashMob.Models.Poco;

/// <summary>
/// Background sync service that retries failed uploads for routes, metrics, and photos
/// when connectivity returns. Monitors network state, processes the offline queue,
/// and manages storage cleanup.
/// </summary>
public class SyncService(
    SyncQueue syncQueue,
    OfflineDatabase offlineDatabase,
    IEventAttendeeRouteRestService eventAttendeeRouteRestService,
    IEventAttendeeMetricsRestService eventAttendeeMetricsRestService,
    IEventPhotoRestService eventPhotoRestService,
    INotificationService notificationService)
{
    private static readonly TimeSpan SyncInterval = TimeSpan.FromMinutes(5);
    private static readonly TimeSpan CleanupMaxAge = TimeSpan.FromDays(7);

    /// <summary>
    /// Maximum pending photo storage in bytes (500 MB).
    /// </summary>
    private const long MaxPendingPhotoStorageBytes = 500L * 1024 * 1024;

    /// <summary>
    /// Maximum route points stored offline (~50 hours at 1 point/2 seconds).
    /// </summary>
    private const int MaxRoutePoints = 100_000;

    private CancellationTokenSource? syncCts;
    private bool isRunning;

    /// <summary>
    /// Starts the sync service. Should be called once on app launch.
    /// Runs startup cleanup and begins periodic sync.
    /// </summary>
    public void Start()
    {
        if (isRunning)
        {
            return;
        }

        isRunning = true;
        syncCts = new CancellationTokenSource();

        // Monitor connectivity changes
        Connectivity.Current.ConnectivityChanged += OnConnectivityChanged;

        // Run startup tasks (cleanup + initial queue size telemetry)
        _ = StartupAsync();

        // Start periodic sync
        _ = PeriodicSyncAsync(syncCts.Token);
    }

    /// <summary>
    /// Stops the sync service.
    /// </summary>
    public void Stop()
    {
        isRunning = false;
        syncCts?.Cancel();
        syncCts = null;
        Connectivity.Current.ConnectivityChanged -= OnConnectivityChanged;
    }

    /// <summary>
    /// Triggers an immediate sync attempt. Returns count of successfully synced items.
    /// </summary>
    public async Task<int> SyncNowAsync()
    {
        if (Connectivity.Current.NetworkAccess != NetworkAccess.Internet)
        {
            return 0;
        }

        // Process in order: routes first, then metrics, then photos
        var routesSynced = await ProcessRouteQueueAsync();
        var metricsSynced = await ProcessMetricsQueueAsync();
        var photosSynced = await ProcessPhotoQueueAsync();

        return routesSynced + metricsSynced + photosSynced;
    }

    /// <summary>
    /// Gets the total count of items pending sync for badge display.
    /// </summary>
    public async Task<int> GetPendingCountAsync()
    {
        return await syncQueue.GetTotalPendingCountAsync();
    }

    /// <summary>
    /// Checks if pending photo storage exceeds the cap.
    /// Returns true if the cap is exceeded, meaning no more photos should be queued.
    /// </summary>
    public bool IsPhotoStorageFull()
    {
        var pendingDir = Path.Combine(FileSystem.AppDataDirectory, "pending_photos");
        if (!Directory.Exists(pendingDir))
        {
            return false;
        }

        var totalSize = new DirectoryInfo(pendingDir)
            .EnumerateFiles()
            .Sum(f => f.Length);

        return totalSize >= MaxPendingPhotoStorageBytes;
    }

    /// <summary>
    /// Checks if route point storage exceeds the cap.
    /// Returns true if the cap is exceeded.
    /// </summary>
    public async Task<bool> IsRouteStorageFullAsync()
    {
        var db = await offlineDatabase.GetConnectionAsync();
        var count = await db.Table<PendingRoutePoint>().CountAsync();
        return count >= MaxRoutePoints;
    }

    private async Task StartupAsync()
    {
        try
        {
            // Clean up uploaded records older than 7 days
            await offlineDatabase.CleanupAsync(CleanupMaxAge);

            // Log queue size at app launch for sync health monitoring
            var pendingCount = await syncQueue.GetTotalPendingCountAsync();
            if (pendingCount > 0)
            {
                SentrySdk.AddBreadcrumb(
                    $"Offline queue has {pendingCount} pending items at app launch",
                    "sync",
                    level: BreadcrumbLevel.Info);
            }
        }
        catch (Exception ex)
        {
            Debug.WriteLine($"SyncService: Startup cleanup failed: {ex.Message}");
            SentrySdk.CaptureException(ex);
        }
    }

    private void OnConnectivityChanged(object? sender, ConnectivityChangedEventArgs e)
    {
        if (e.NetworkAccess == NetworkAccess.Internet)
        {
            Debug.WriteLine("SyncService: Connectivity restored, triggering sync");
            _ = SyncNowAsync();
        }
    }

    private async Task PeriodicSyncAsync(CancellationToken cancellationToken)
    {
        try
        {
            while (!cancellationToken.IsCancellationRequested)
            {
                await Task.Delay(SyncInterval, cancellationToken);

                if (Connectivity.Current.NetworkAccess == NetworkAccess.Internet)
                {
                    var synced = await SyncNowAsync();
                    if (synced > 0)
                    {
                        await MainThread.InvokeOnMainThreadAsync(async () =>
                            await notificationService.Notify(
                                $"{synced} item{(synced > 1 ? "s" : "")} synced successfully."));
                    }
                }
            }
        }
        catch (OperationCanceledException)
        {
            // Expected on stop
        }
    }

    private async Task<int> ProcessRouteQueueAsync()
    {
        var sessions = await syncQueue.GetSessionsReadyForUploadAsync();
        var syncedCount = 0;

        foreach (var session in sessions)
        {
            try
            {
                var points = await syncQueue.GetRoutePointsAsync(session.SessionId);

                if (points.Count == 0)
                {
                    // No points means nothing to upload, discard
                    await syncQueue.DiscardSessionAsync(session.SessionId);
                    continue;
                }

                // Build the upload DTO from persisted data
                var locations = points.Select(p => new SortableLocation
                {
                    Latitude = p.Latitude,
                    Longitude = p.Longitude,
                    SortOrder = p.PointOrder,
                }).ToList();

                // Ensure at least 2 points for a line
                if (locations.Count == 1)
                {
                    locations.Add(locations[0]);
                }

                var route = new DisplayEventAttendeeRoute
                {
                    EventId = Guid.Parse(session.EventId),
                    UserId = Guid.Parse(session.UserId),
                    Locations = locations,
                    StartTime = DateTimeOffset.Parse(session.StartTime),
                    EndTime = DateTimeOffset.Parse(session.EndTime ?? session.StartTime),
                    SkipDefaultTrim = session.SkipDefaultTrim,
                };

                await eventAttendeeRouteRestService.AddEventAttendeeRouteAsync(route);
                await syncQueue.MarkSessionUploadedAsync(session.SessionId);
                syncedCount++;

                SentrySdk.AddBreadcrumb(
                    $"Route synced: session={session.SessionId}, points={points.Count}, retries={session.RetryCount}",
                    "sync",
                    level: BreadcrumbLevel.Info);
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"SyncService: Failed to upload route session {session.SessionId}: {ex.Message}");
                await syncQueue.MarkSessionFailedAsync(session.SessionId, ex.Message);
            }
        }

        return syncedCount;
    }

    private async Task<int> ProcessMetricsQueueAsync()
    {
        var items = await syncQueue.GetMetricsReadyForUploadAsync();
        var syncedCount = 0;

        foreach (var item in items)
        {
            try
            {
                var metrics = new EventAttendeeMetrics
                {
                    EventId = Guid.Parse(item.EventId),
                    UserId = Guid.Parse(item.UserId),
                    BagsCollected = item.BagsCollected,
                    PickedWeight = item.PickedWeight != null ? decimal.Parse(item.PickedWeight) : null,
                    PickedWeightUnitId = item.PickedWeightUnitId,
                    DurationMinutes = item.DurationMinutes,
                    Notes = item.Notes,
                };

                await eventAttendeeMetricsRestService.SubmitMyMetricsAsync(
                    Guid.Parse(item.EventId), metrics);
                await syncQueue.MarkMetricsUploadedAsync(item.Id);
                syncedCount++;

                SentrySdk.AddBreadcrumb(
                    $"Metrics synced: id={item.Id}, event={item.EventId}, retries={item.RetryCount}",
                    "sync",
                    level: BreadcrumbLevel.Info);
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"SyncService: Failed to upload metrics {item.Id}: {ex.Message}");
                await syncQueue.MarkMetricsFailedAsync(item.Id, ex.Message);
            }
        }

        return syncedCount;
    }

    private async Task<int> ProcessPhotoQueueAsync()
    {
        var items = await syncQueue.GetPhotosReadyForUploadAsync();
        var syncedCount = 0;

        foreach (var item in items)
        {
            try
            {
                // Verify local file still exists
                if (!File.Exists(item.LocalFilePath))
                {
                    Debug.WriteLine($"SyncService: Photo file missing for {item.Id}, discarding");
                    await syncQueue.MarkPhotoUploadedAsync(item.Id);
                    continue;
                }

                var photoType = (EventPhotoType)item.PhotoType;
                await eventPhotoRestService.UploadPhotoAsync(
                    Guid.Parse(item.EventId), item.LocalFilePath, photoType, string.Empty);
                await syncQueue.MarkPhotoUploadedAsync(item.Id);
                syncedCount++;

                SentrySdk.AddBreadcrumb(
                    $"Photo synced: id={item.Id}, event={item.EventId}, retries={item.RetryCount}",
                    "sync",
                    level: BreadcrumbLevel.Info);
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"SyncService: Failed to upload photo {item.Id}: {ex.Message}");
                await syncQueue.MarkPhotoFailedAsync(item.Id, ex.Message);
            }
        }

        return syncedCount;
    }
}
