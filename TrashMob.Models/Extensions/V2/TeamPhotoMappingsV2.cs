namespace TrashMob.Models.Extensions.V2
{
    using TrashMob.Models.Poco.V2;

    /// <summary>
    /// Extension methods for mapping TeamPhoto entities to V2 DTOs.
    /// </summary>
    public static class TeamPhotoMappingsV2
    {
        /// <summary>
        /// Maps a TeamPhoto entity to a V2 TeamPhotoDto.
        /// </summary>
        /// <param name="entity">The TeamPhoto entity to map.</param>
        /// <returns>A TeamPhotoDto with all public-safe properties.</returns>
        public static TeamPhotoDto ToV2Dto(this TeamPhoto entity)
        {
            return new TeamPhotoDto
            {
                Id = entity.Id,
                TeamId = entity.TeamId,
                ImageUrl = entity.ImageUrl ?? string.Empty,
                Caption = entity.Caption ?? string.Empty,
                UploadedByUserId = entity.UploadedByUserId,
                UploadedDate = entity.UploadedDate,
                CreatedByUserId = entity.CreatedByUserId,
                CreatedDate = entity.CreatedDate.GetValueOrDefault(),
                LastUpdatedDate = entity.LastUpdatedDate.GetValueOrDefault(),
            };
        }
    }
}
