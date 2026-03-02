namespace TrashMobMobile.Services;

/// <summary>
/// Manages route tracking session state. Persists active session info to Preferences
/// so it survives app restarts and enables crash recovery.
/// </summary>
public class RouteTrackingSessionManager : IRouteTrackingSessionManager
{
    private const string PrefKeyIsTracking = "RouteTracking_IsTracking";
    private const string PrefKeyEventId = "RouteTracking_EventId";
    private const string PrefKeyEventName = "RouteTracking_EventName";
    private const string PrefKeySessionId = "RouteTracking_SessionId";

    private readonly object syncLock = new();

    public bool IsTracking { get; private set; }

    public Guid? ActiveEventId { get; private set; }

    public string? ActiveEventName { get; private set; }

    public string? ActiveSessionId { get; private set; }

    public bool TryStartSession(Guid eventId, string eventName, string sessionId)
    {
        lock (syncLock)
        {
            if (IsTracking && ActiveEventId != eventId)
            {
                return false;
            }

            IsTracking = true;
            ActiveEventId = eventId;
            ActiveEventName = eventName;
            ActiveSessionId = sessionId;

            // Persist to Preferences for crash recovery
            Preferences.Default.Set(PrefKeyIsTracking, true);
            Preferences.Default.Set(PrefKeyEventId, eventId.ToString());
            Preferences.Default.Set(PrefKeyEventName, eventName);
            Preferences.Default.Set(PrefKeySessionId, sessionId);

            return true;
        }
    }

    public void EndSession()
    {
        lock (syncLock)
        {
            IsTracking = false;
            ActiveEventId = null;
            ActiveEventName = null;
            ActiveSessionId = null;

            // Clear persisted state
            Preferences.Default.Remove(PrefKeyIsTracking);
            Preferences.Default.Remove(PrefKeyEventId);
            Preferences.Default.Remove(PrefKeyEventName);
            Preferences.Default.Remove(PrefKeySessionId);
        }
    }

    public bool TryRestoreSession()
    {
        lock (syncLock)
        {
            if (!Preferences.Default.Get(PrefKeyIsTracking, false))
            {
                return false;
            }

            var eventIdStr = Preferences.Default.Get(PrefKeyEventId, string.Empty);
            var eventName = Preferences.Default.Get(PrefKeyEventName, string.Empty);
            var sessionId = Preferences.Default.Get(PrefKeySessionId, string.Empty);

            if (string.IsNullOrEmpty(eventIdStr) || !Guid.TryParse(eventIdStr, out var eventId) || string.IsNullOrEmpty(sessionId))
            {
                // Invalid persisted state, clear it
                EndSession();
                return false;
            }

            IsTracking = true;
            ActiveEventId = eventId;
            ActiveEventName = eventName;
            ActiveSessionId = sessionId;
            return true;
        }
    }
}
