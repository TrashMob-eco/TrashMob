namespace TrashMobMobile.Services.Offline;

using System.Collections.ObjectModel;

/// <summary>
/// Encapsulates the full route-recording pipeline: SQLite session creation, the
/// session-tracking Preferences marker, the SQLite point writer, the GPS listener, and
/// the on-stop upload with offline fallback. Callers handle consent prompts + UI state
/// separately; this service just runs the pipeline.
///
/// Used by <see cref="TrashMobMobile.ViewModels.InstantEventViewModel"/> to attach
/// route recording to an Instant Event's Start/Stop lifecycle. The wizard-created event
/// flow in <c>ViewEventViewModel</c> still has its own inline implementation as of
/// Project 65 Phase 2 slice 2 — refactoring that call site to use this coordinator is a
/// follow-up PR to reduce risk to already-shipped wizard behavior.
/// </summary>
public interface IRouteRecordingCoordinator
{
    bool IsRecording { get; }

    Guid? ActiveEventId { get; }

    string? ActiveEventName { get; }

    /// <summary>
    /// Creates a SQLite session, starts the point writer, and begins listening for GPS
    /// updates on the platform Geolocator. Every received point is appended to
    /// <paramref name="displayCollection"/> (for map display) and persisted for crash
    /// safety. Returns <see cref="RouteRecordingStartOutcome.ConflictOtherEvent"/> if
    /// another event is currently being tracked (only one route can be recorded at a
    /// time).
    /// </summary>
    Task<RouteRecordingStartResult> StartAsync(
        Guid eventId,
        string eventName,
        Guid userId,
        DateTimeOffset startTime,
        bool skipDefaultTrim,
        ObservableCollection<Microsoft.Maui.Devices.Sensors.Location> displayCollection);

    /// <summary>
    /// Stops the current recording (if any), flushes the writer, and posts the route to
    /// the server. On upload failure the session is left marked pending so the
    /// background sync service will retry. Safe to call when not recording — returns
    /// <see cref="RouteRecordingStopOutcome.NotStarted"/>.
    /// </summary>
    Task<RouteRecordingStopResult> StopAsync(DateTimeOffset endTime);

    /// <summary>
    /// Attempt to resume an in-progress route recording after an app-close/reopen. If
    /// <see cref="IRouteTrackingSessionManager"/> holds a persisted session for the
    /// specified event AND the SQLite session record is still in <c>Recording</c>
    /// state, this method hydrates the coordinator's in-memory state, loads existing
    /// GPS points into <paramref name="displayCollection"/>, reopens the writer
    /// preserving point order, and restarts the GPS listener. Returns
    /// <see cref="RouteRecordingStartOutcome.NothingToResume"/> when no matching
    /// interrupted session exists.
    /// </summary>
    Task<RouteRecordingStartResult> TryResumeAsync(
        Guid eventId,
        string eventName,
        Guid userId,
        ObservableCollection<Microsoft.Maui.Devices.Sensors.Location> displayCollection);
}

public enum RouteRecordingStartOutcome
{
    Success,
    ConflictOtherEvent,
    NothingToResume,
}

public enum RouteRecordingStopOutcome
{
    Uploaded,
    QueuedForRetry,
    EmptyDiscarded,
    NotStarted,
}

public record RouteRecordingStartResult(
    RouteRecordingStartOutcome Outcome,
    string? SessionId,
    string? ConflictingEventName);

public record RouteRecordingStopResult(
    RouteRecordingStopOutcome Outcome,
    int PointCount);
