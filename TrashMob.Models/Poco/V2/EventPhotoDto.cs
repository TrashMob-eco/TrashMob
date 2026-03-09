#nullable enable

namespace TrashMob.Models.Poco.V2;

/// <summary>
/// Represents an event photo for public display.
/// </summary>
public class EventPhotoDto
{
    /// <summary>Gets or sets the photo identifier.</summary>
    public Guid Id { get; set; }

    /// <summary>Gets or sets the event identifier.</summary>
    public Guid EventId { get; set; }

    /// <summary>Gets or sets the identifier of the user who uploaded the photo.</summary>
    public Guid UploadedByUserId { get; set; }

    /// <summary>Gets or sets the full-size image URL.</summary>
    public string ImageUrl { get; set; } = string.Empty;

    /// <summary>Gets or sets the thumbnail image URL.</summary>
    public string ThumbnailUrl { get; set; } = string.Empty;

    /// <summary>Gets or sets the photo type (Before=0, During=1, After=2).</summary>
    public EventPhotoType PhotoType { get; set; }

    /// <summary>Gets or sets the caption.</summary>
    public string Caption { get; set; } = string.Empty;

    /// <summary>Gets or sets when the photo was taken.</summary>
    public DateTimeOffset? TakenAt { get; set; }

    /// <summary>Gets or sets the upload date.</summary>
    public DateTimeOffset UploadedDate { get; set; }
}
