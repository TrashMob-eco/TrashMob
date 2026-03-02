namespace TrashMobMobile.Services;

public interface IRouteTrackingSessionManager
{
    bool IsTracking { get; }

    Guid? ActiveEventId { get; }

    string? ActiveEventName { get; }

    /// <summary>
    /// Gets the SQLite session ID for the active route recording.
    /// </summary>
    string? ActiveSessionId { get; }

    /// <summary>
    /// Try to start a tracking session for the given event.
    /// Returns false if a session is already active for a different event.
    /// </summary>
    bool TryStartSession(Guid eventId, string eventName, string sessionId);

    /// <summary>
    /// End the current tracking session.
    /// </summary>
    void EndSession();

    /// <summary>
    /// Restores session state from persisted preferences (e.g., after app restart).
    /// Returns true if a session was restored.
    /// </summary>
    bool TryRestoreSession();
}
