namespace TrashMobMobile.Services.Offline;

using SQLite;

/// <summary>
/// A single GPS point recorded during a route session. Persisted to SQLite
/// so data survives app crashes and upload failures.
/// </summary>
[Table("PendingRoutePoints")]
public class PendingRoutePoint
{
    [PrimaryKey]
    [AutoIncrement]
    public int Id { get; set; }

    [Indexed]
    public string SessionId { get; set; } = string.Empty;

    public double Latitude { get; set; }

    public double Longitude { get; set; }

    public double? Altitude { get; set; }

    public string Timestamp { get; set; } = string.Empty;

    public int PointOrder { get; set; }
}
