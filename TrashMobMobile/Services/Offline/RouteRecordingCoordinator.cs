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

        StartGpsListener();

        return new RouteRecordingStartResult(
            RouteRecordingStartOutcome.Success,
            SessionId: session.SessionId,
            ConflictingEventName: null);
    }

    public async Task<RouteRecordingStartResult> TryResumeAsync(
        Guid eventId,
        string eventName,
        Guid userId,
        ObservableCollection<Microsoft.Maui.Devices.Sensors.Location> displayCollection)
    {
        // Hydrate session manager's in-memory state from Preferences. If the previous
        // process died before Stop, this brings back IsTracking + ActiveEventId +
        // ActiveSessionId. No-op if the manager already has state in memory.
        if (!sessionManager.IsTracking)
        {
            sessionManager.TryRestoreSession();
        }

        if (!sessionManager.IsTracking
            || sessionManager.ActiveEventId != eventId
            || sessionManager.ActiveSessionId is not { } sessionId)
        {
            return new RouteRecordingStartResult(
                RouteRecordingStartOutcome.NothingToResume, null, null);
        }

        // Cross-check against SQLite. GetInterruptedSessionsAsync returns everything in
        // Recording status — the source of truth for "was actually mid-flight when the
        // process died." If sessionManager thinks we're tracking but SQLite disagrees,
        // clear the stale sessionManager state and bail.
        var interrupted = await syncQueue.GetInterruptedSessionsAsync();
        var session = interrupted.FirstOrDefault(s => s.SessionId == sessionId);
        if (session == null)
        {
            sessionManager.EndSession();
            return new RouteRecordingStartResult(
                RouteRecordingStartOutcome.NothingToResume, null, null);
        }

        // Load existing points into the display collection so the map shows the
        // pre-close portion of the route.
        var existingPoints = await syncQueue.GetRoutePointsAsync(sessionId);
        foreach (var p in existingPoints.OrderBy(p => p.PointOrder))
        {
            displayCollection.Add(new Microsoft.Maui.Devices.Sensors.Location(p.Latitude, p.Longitude)
            {
                Altitude = p.Altitude,
                Timestamp = DateTimeOffset.TryParse(p.Timestamp, out var ts) ? ts : DateTimeOffset.UtcNow,
            });
        }

        // Rehydrate coordinator state.
        this.displayCollection = displayCollection;
        activeEventId = eventId;
        activeUserId = userId;
        activeSessionId = sessionId;
        activeStartTime = DateTimeOffset.TryParse(session.StartTime, out var startTime)
            ? startTime
            : DateTimeOffset.UtcNow;
        activeSkipDefaultTrim = session.SkipDefaultTrim;

        // Reopen the writer. StartSession resets pointOrder to 0 (fine for fresh
        // sessions), so use SetPointOrder to continue past the existing max so new
        // points don't collide with old ones on the same SessionId.
        routePointWriter.StartSession(sessionId);
        var maxOrder = await syncQueue.GetMaxPointOrderAsync(sessionId);
        routePointWriter.SetPointOrder(maxOrder);

        StartGpsListener();

        return new RouteRecordingStartResult(
            RouteRecordingStartOutcome.Success,
            SessionId: sessionId,
            ConflictingEventName: null);
    }

    private void StartGpsListener()
    {
        listenerCts?.Dispose();
        listenerCts = new CancellationTokenSource();

        var progress = new Progress<Microsoft.Maui.Devices.Sensors.Location>(OnPointReceived);

        // Fire-and-forget the listener — it awaits until cancellation. Not awaited here
        // (that would block the caller forever). Any exception surfaces to Sentry via
        // the continuation.
        _ = Geolocator.Default.StartListening(progress, listenerCts.Token)
            .ContinueWith(t =>
            {
                if (t.Exception != null)
                {
                    SentrySdk.CaptureException(t.Exception);
                }
            }, TaskScheduler.Default);
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
