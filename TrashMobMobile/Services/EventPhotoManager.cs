namespace TrashMobMobile.Services;

using TrashMob.Models;

public class EventPhotoManager(IEventPhotoRestService eventPhotoRestService) : IEventPhotoManager
{
    public Task<IEnumerable<EventPhoto>> GetEventPhotosAsync(Guid eventId,
        CancellationToken cancellationToken = default)
    {
        return eventPhotoRestService.GetEventPhotosAsync(eventId, cancellationToken);
    }

    public Task<EventPhoto> UploadPhotoAsync(Guid eventId, string localFilePath, EventPhotoType photoType,
        string caption, CancellationToken cancellationToken = default)
    {
        return eventPhotoRestService.UploadPhotoAsync(eventId, localFilePath, photoType, caption, cancellationToken);
    }

    public Task DeletePhotoAsync(Guid eventId, Guid photoId, CancellationToken cancellationToken = default)
    {
        return eventPhotoRestService.DeletePhotoAsync(eventId, photoId, cancellationToken);
    }
}
