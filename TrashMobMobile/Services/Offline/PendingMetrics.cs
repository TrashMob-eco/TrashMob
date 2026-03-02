namespace TrashMobMobile.Services.Offline;

using SQLite;

/// <summary>
/// Queued attendee metrics awaiting upload. Persisted to SQLite so data survives
/// app crashes and upload failures.
/// </summary>
[Table("PendingMetrics")]
public class PendingMetrics
{
    [PrimaryKey]
    [AutoIncrement]
    public int Id { get; set; }

    [Indexed]
    public string EventId { get; set; } = string.Empty;

    public string UserId { get; set; } = string.Empty;

    public int? BagsCollected { get; set; }

    /// <summary>
    /// Stored as string for decimal precision preservation in SQLite.
    /// </summary>
    public string? PickedWeight { get; set; }

    public int? PickedWeightUnitId { get; set; }

    public int? DurationMinutes { get; set; }

    public string? Notes { get; set; }

    [Indexed]
    public string Status { get; set; } = PendingUploadStatus.PendingUpload;

    public string? LastError { get; set; }

    public int RetryCount { get; set; }

    public string? NextRetryAt { get; set; }

    public string CreatedAt { get; set; } = DateTimeOffset.UtcNow.ToString("O");
}

/// <summary>
/// Queued photo upload awaiting sync. The photo file is stored in AppDataDirectory
/// (not CacheDirectory) so the OS won't delete it before upload.
/// </summary>
[Table("PendingPhotos")]
public class PendingPhoto
{
    [PrimaryKey]
    [AutoIncrement]
    public int Id { get; set; }

    /// <summary>
    /// Client-generated Guid for server-side dedup.
    /// </summary>
    public string PhotoId { get; set; } = Guid.NewGuid().ToString();

    [Indexed]
    public string EventId { get; set; } = string.Empty;

    public string UserId { get; set; } = string.Empty;

    /// <summary>
    /// Path to the compressed image in AppDataDirectory/pending_photos/.
    /// </summary>
    public string LocalFilePath { get; set; } = string.Empty;

    /// <summary>
    /// EventPhotoType as int: 0=Before, 1=After, 2=During.
    /// </summary>
    public int PhotoType { get; set; }

    [Indexed]
    public string Status { get; set; } = PendingUploadStatus.PendingUpload;

    public string? LastError { get; set; }

    public int RetryCount { get; set; }

    public string? NextRetryAt { get; set; }

    public string CreatedAt { get; set; } = DateTimeOffset.UtcNow.ToString("O");
}

/// <summary>
/// Shared status constants for metrics and photo queues.
/// </summary>
public static class PendingUploadStatus
{
    public const string PendingUpload = "PendingUpload";
    public const string Uploading = "Uploading";
    public const string Uploaded = "Uploaded";
    public const string Failed = "Failed";
    public const string PermanentlyFailed = "PermanentlyFailed";
}
