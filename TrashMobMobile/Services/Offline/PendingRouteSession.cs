namespace TrashMobMobile.Services.Offline;

using SQLite;

/// <summary>
/// Represents a route recording session that may be in-progress, pending upload, or completed.
/// </summary>
[Table("PendingRouteSessions")]
public class PendingRouteSession
{
    [PrimaryKey]
    public string SessionId { get; set; } = Guid.NewGuid().ToString();

    [Indexed]
    public string EventId { get; set; } = string.Empty;

    public string UserId { get; set; } = string.Empty;

    public string StartTime { get; set; } = string.Empty;

    public string? EndTime { get; set; }

    /// <summary>
    /// Recording, PendingUpload, Uploading, Uploaded, Failed, PermanentlyFailed
    /// </summary>
    [Indexed]
    public string Status { get; set; } = RouteSessionStatus.Recording;

    public bool SkipDefaultTrim { get; set; }

    public string? LastError { get; set; }

    public int RetryCount { get; set; }

    public string? NextRetryAt { get; set; }

    public string CreatedAt { get; set; } = DateTimeOffset.UtcNow.ToString("O");
}

public static class RouteSessionStatus
{
    public const string Recording = "Recording";
    public const string PendingUpload = "PendingUpload";
    public const string Uploading = "Uploading";
    public const string Uploaded = "Uploaded";
    public const string Failed = "Failed";
    public const string PermanentlyFailed = "PermanentlyFailed";
}
