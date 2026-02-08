namespace TrashMobMobile.Services;

using TrashMob.Models;

public interface IEventPhotoRestService
{
    Task<IEnumerable<EventPhoto>> GetEventPhotosAsync(Guid eventId, CancellationToken cancellationToken = default);

    Task<EventPhoto> UploadPhotoAsync(Guid eventId, string localFilePath, EventPhotoType photoType, string caption,
        CancellationToken cancellationToken = default);

    Task DeletePhotoAsync(Guid eventId, Guid photoId, CancellationToken cancellationToken = default);
}
