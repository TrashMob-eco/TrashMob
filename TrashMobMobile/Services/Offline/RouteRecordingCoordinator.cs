namespace TrashMobMobile.Services.Offline;

using System.Collections.ObjectModel;
using Microsoft.Maui.Devices.Sensors;
using Sentry;
using TrashMob.Models.Poco;
using TrashMobMobile.Services;

/// <inheritdoc cref="IRouteRecordingCoordinator" />
public class RouteRecordingCoordinator(
    IRouteTrackingSessionManager sessionManager,
    RoutePointWriter routePointWriter,
    SyncQueue syncQueue,
    IEventAttendeeRouteRestService routeRestService)
    : IRouteRecordingCoordinator
{
    private ObservableCollection<Microsoft.Maui.Devices.Sensors.Location>? displayCollection;
    private CancellationTokenSource? listenerCts;
    private Guid activeEventId;
    private Guid activeUserId;
    private string? activeSessionId;
    private DateTimeOffset activeStartTime;
    private bool activeSkipDefaultTrim;

    public bool IsRecording => activeSessionId != null;

    public Guid? ActiveEventId => IsRecording ? activeEventId : null;

    public string? ActiveEventName => sessionManager.ActiveEventName;

    public async Task<RouteRecordingStartResult> StartAsync(
        Guid eventId,
        string eventName,
        Guid userId,
        DateTimeOffset startTime,
        bool skipDefaultTrim,
        ObservableCollection<Microsoft.Maui.Devices.Sensors.Location> displayCollection)
    {
        if (sessionManager.IsTracking && sessionManager.ActiveEventId != eventId)
        {
            return new RouteRecordingStartResult(
                RouteRecordingStartOutcome.ConflictOtherEvent,
                SessionId: null,
                ConflictingEventName: sessionManager.ActiveEventName);
        }

        var session = await syncQueue.CreateRouteSessionAsync(
            eventId, userId, startTime, skipDefaultTrim);

        sessionManager.TryStartSession(eventId, eventName, session.SessionId);
        routePointWriter.StartSession(session.SessionId);

        this.displayCollection = displayCollection;
        activeEventId = eventId;
        activeUserId = userId;
        activeSessionId = session.SessionId;
        activeStartTime = startTime;
        activeSkipDefaultTrim = skipDefaultTrim;

        listenerCts = new CancellationTokenSource();
        var progress = new Progress<Microsoft.Maui.Devices.Sensors.Location>(OnPointReceived);

        // Fire-and-forget the listener — it awaits until cancellation. We don't await
        // it here (that would block StartAsync forever). Any exception the listener
        // throws surfaces to Sentry via the continuation.
        _ = Geolocator.Default.StartListening(progress, listenerCts.Token)
            .ContinueWith(t =>
            {
                if (t.Exception != null)
                {
                    SentrySdk.CaptureException(t.Exception);
                }
            }, TaskScheduler.Default);

        return new RouteRecordingStartResult(
            RouteRecordingStartOutcome.Success,
            SessionId: session.SessionId,
            ConflictingEventName: null);
    }

    public async Task<RouteRecordingStopResult> StopAsync(DateTimeOffset endTime)
    {
        if (!IsRecording || activeSessionId is not { } sessionId)
        {
            return new RouteRecordingStopResult(RouteRecordingStopOutcome.NotStarted, 0);
        }

        listenerCts?.Cancel();
        listenerCts?.Dispose();
        listenerCts = null;

        await routePointWriter.StopAndFlushAsync();

        sessionManager.EndSession();
        await syncQueue.MarkSessionPendingUploadAsync(sessionId, endTime);

        var points = SnapshotPoints();
        var outcome = await UploadAsync(sessionId, endTime, points);

        activeSessionId = null;
        displayCollection = null;

        return new RouteRecordingStopResult(outcome, points.Count);
    }

    private void OnPointReceived(Microsoft.Maui.Devices.Sensors.Location location)
    {
        location.Timestamp = DateTimeOffset.Now;
        displayCollection?.Add(location);
        routePointWriter.AddPoint(location.Latitude, location.Longitude, location.Altitude, location.Timestamp);
    }

    private List<Microsoft.Maui.Devices.Sensors.Location> SnapshotPoints()
    {
        return displayCollection?.ToList() ?? [];
    }

    private async Task<RouteRecordingStopOutcome> UploadAsync(
        string sessionId,
        DateTimeOffset endTime,
        List<Microsoft.Maui.Devices.Sensors.Location> points)
    {
        if (points.Count == 0)
        {
            await syncQueue.DiscardSessionAsync(sessionId);
            return RouteRecordingStopOutcome.EmptyDiscarded;
        }

        // A single-point route is degenerate — Locations.Count == 1 in the wizard flow
        // gets padded to 2 so the polyline has something to render. Preserve that.
        var normalized = points.Count == 1 ? [points[0], points[0]] : points;

        try
        {
            await routeRestService.AddEventAttendeeRouteAsync(new DisplayEventAttendeeRoute
            {
                EventId = activeEventId,
                UserId = activeUserId,
                Locations = ToSortable(normalized),
                StartTime = activeStartTime,
                EndTime = endTime,
                SkipDefaultTrim = activeSkipDefaultTrim,
                SessionId = Guid.Parse(sessionId),
            });

            await syncQueue.MarkSessionUploadedAsync(sessionId);
            return RouteRecordingStopOutcome.Uploaded;
        }
        catch (Exception ex)
        {
            await syncQueue.MarkSessionFailedAsync(sessionId, ex.Message);
            SentrySdk.AddBreadcrumb(
                $"Route queued offline: event={activeEventId}, session={sessionId}",
                "sync",
                level: BreadcrumbLevel.Info);
            return RouteRecordingStopOutcome.QueuedForRetry;
        }
    }

    private static List<SortableLocation> ToSortable(IReadOnlyList<Microsoft.Maui.Devices.Sensors.Location> locations)
    {
        var sortable = new List<SortableLocation>(locations.Count);
        var order = 0;
        foreach (var location in locations.OrderBy(l => l.Timestamp))
        {
            sortable.Add(new SortableLocation
            {
                Latitude = location.Latitude,
                Longitude = location.Longitude,
                SortOrder = order++,
            });
        }

        return sortable;
    }
}
