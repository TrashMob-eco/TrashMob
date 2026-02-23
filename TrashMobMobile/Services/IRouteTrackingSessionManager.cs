namespace TrashMobMobile.Services;

public interface IRouteTrackingSessionManager
{
    bool IsTracking { get; }

    Guid? ActiveEventId { get; }

    string? ActiveEventName { get; }

    /// <summary>
    /// Try to start a tracking session for the given event.
    /// Returns false if a session is already active for a different event.
    /// </summary>
    bool TryStartSession(Guid eventId, string eventName);

    /// <summary>
    /// End the current tracking session.
    /// </summary>
    void EndSession();
}
