namespace TrashMobMobile.Services.Offline;

using System.Diagnostics;
using TrashMob.Models;
using TrashMob.Models.Poco;

/// <summary>
/// Background sync service that retries failed uploads for routes, metrics, and photos
/// when connectivity returns. Monitors network state and processes the offline queue.
/// </summary>
public class SyncService(
    SyncQueue syncQueue,
    IEventAttendeeRouteRestService eventAttendeeRouteRestService,
    IEventAttendeeMetricsRestService eventAttendeeMetricsRestService,
    IEventPhotoRestService eventPhotoRestService,
    INotificationService notificationService)
{
    private static readonly TimeSpan SyncInterval = TimeSpan.FromMinutes(5);

    private CancellationTokenSource? syncCts;
    private bool isRunning;

    /// <summary>
    /// Starts the sync service. Should be called once on app launch.
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

                Debug.WriteLine($"SyncService: Route session {session.SessionId} uploaded successfully");
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

                Debug.WriteLine($"SyncService: Metrics {item.Id} for event {item.EventId} uploaded successfully");
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

                Debug.WriteLine($"SyncService: Photo {item.Id} for event {item.EventId} uploaded successfully");
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
