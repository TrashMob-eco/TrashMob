namespace TrashMobMobile.Services.Offline;

using System.Collections.Concurrent;

/// <summary>
/// Buffers GPS points in memory and flushes them to SQLite in batches.
/// This minimizes I/O during active route recording while ensuring data
/// survives app crashes (points are flushed every 5 seconds or 10 points).
/// </summary>
public class RoutePointWriter(OfflineDatabase offlineDatabase)
{
    private const int MaxBufferSize = 10;
    private static readonly TimeSpan FlushInterval = TimeSpan.FromSeconds(5);

    private readonly ConcurrentQueue<PendingRoutePoint> buffer = new();
    private CancellationTokenSource? flushCts;
    private int pointOrder;
    private string? activeSessionId;

    /// <summary>
    /// Starts batched writing for a new route session.
    /// </summary>
    public void StartSession(string sessionId)
    {
        activeSessionId = sessionId;
        pointOrder = 0;
        buffer.Clear();

        flushCts?.Cancel();
        flushCts = new CancellationTokenSource();

        // Start periodic flush loop
        _ = FlushLoopAsync(flushCts.Token);
    }

    /// <summary>
    /// Adds a GPS point to the buffer. The point will be persisted to SQLite
    /// when the buffer reaches MaxBufferSize or the flush interval elapses.
    /// </summary>
    public void AddPoint(double latitude, double longitude, double? altitude, DateTimeOffset timestamp)
    {
        if (activeSessionId == null)
        {
            return;
        }

        var point = new PendingRoutePoint
        {
            SessionId = activeSessionId,
            Latitude = latitude,
            Longitude = longitude,
            Altitude = altitude,
            Timestamp = timestamp.ToString("O"),
            PointOrder = Interlocked.Increment(ref pointOrder),
        };

        buffer.Enqueue(point);

        // Flush immediately if buffer is full
        if (buffer.Count >= MaxBufferSize)
        {
            _ = FlushAsync();
        }
    }

    /// <summary>
    /// Flushes any remaining buffered points and stops the periodic flush loop.
    /// Call this when the user stops route recording.
    /// </summary>
    public async Task StopAndFlushAsync()
    {
        flushCts?.Cancel();
        flushCts = null;
        await FlushAsync();
        activeSessionId = null;
    }

    /// <summary>
    /// Sets the point order counter. Used when resuming a crashed session
    /// to continue from the last persisted point order.
    /// </summary>
    public void SetPointOrder(int startOrder)
    {
        pointOrder = startOrder;
    }

    private async Task FlushLoopAsync(CancellationToken cancellationToken)
    {
        try
        {
            while (!cancellationToken.IsCancellationRequested)
            {
                await Task.Delay(FlushInterval, cancellationToken);
                await FlushAsync();
            }
        }
        catch (OperationCanceledException)
        {
            // Expected when session stops
        }
    }

    private async Task FlushAsync()
    {
        if (buffer.IsEmpty)
        {
            return;
        }

        var pointsToWrite = new List<PendingRoutePoint>();
        while (buffer.TryDequeue(out var point))
        {
            pointsToWrite.Add(point);
        }

        if (pointsToWrite.Count == 0)
        {
            return;
        }

        try
        {
            var db = await offlineDatabase.GetConnectionAsync();
            await db.InsertAllAsync(pointsToWrite);
        }
        catch (Exception ex)
        {
            // Re-enqueue points that failed to write so they aren't lost
            foreach (var point in pointsToWrite)
            {
                buffer.Enqueue(point);
            }

            System.Diagnostics.Debug.WriteLine($"RoutePointWriter flush failed: {ex.Message}");
        }
    }
}
