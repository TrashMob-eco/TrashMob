namespace TrashMobMobile.ViewModels;

using TrashMob.Models;

public class EventPhotoViewModel
{
    public Guid Id { get; set; }

    public Guid EventId { get; set; }

    public string ImageUrl { get; set; } = string.Empty;

    public string ThumbnailUrl { get; set; } = string.Empty;

    public EventPhotoType PhotoType { get; set; }

    public string Caption { get; set; } = string.Empty;

    public string PhotoTypeDisplay => PhotoType.ToString();

    public DateTimeOffset UploadedDate { get; set; }

    public string UploadedDateDisplay => UploadedDate.LocalDateTime.ToString("MMM d, yyyy");

    public Guid UploadedByUserId { get; set; }

    public bool CanDelete { get; set; }
}
