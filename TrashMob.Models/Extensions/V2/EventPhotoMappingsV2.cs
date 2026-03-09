namespace TrashMob.Models.Extensions.V2
{
    using TrashMob.Models.Poco.V2;

    /// <summary>
    /// V2 mapping extensions for event photos.
    /// </summary>
    public static class EventPhotoMappingsV2
    {
        /// <summary>
        /// Maps an <see cref="EventPhoto"/> to an <see cref="EventPhotoDto"/>.
        /// </summary>
        public static EventPhotoDto ToV2Dto(this EventPhoto entity)
        {
            return new EventPhotoDto
            {
                Id = entity.Id,
                EventId = entity.EventId,
                UploadedByUserId = entity.UploadedByUserId,
                ImageUrl = entity.ImageUrl ?? string.Empty,
                ThumbnailUrl = entity.ThumbnailUrl ?? string.Empty,
                PhotoType = entity.PhotoType,
                Caption = entity.Caption ?? string.Empty,
                TakenAt = entity.TakenAt,
                UploadedDate = entity.UploadedDate,
            };
        }
    }
}
