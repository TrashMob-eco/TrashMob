namespace TrashMobMobile.Services;

public class RouteTrackingSessionManager : IRouteTrackingSessionManager
{
    private readonly object syncLock = new();

    public bool IsTracking { get; private set; }

    public Guid? ActiveEventId { get; private set; }

    public string? ActiveEventName { get; private set; }

    public bool TryStartSession(Guid eventId, string eventName)
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
        }
    }
}
