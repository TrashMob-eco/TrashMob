namespace TrashMob.Models.Extensions.V2
{
    using TrashMob.Models.Poco.V2;

    /// <summary>
    /// Extension methods for mapping PartnerPhoto entities to V2 DTOs.
    /// </summary>
    public static class PartnerPhotoMappingsV2
    {
        /// <summary>
        /// Maps a PartnerPhoto entity to a V2 PartnerPhotoDto.
        /// </summary>
        /// <param name="entity">The PartnerPhoto entity to map.</param>
        /// <returns>A PartnerPhotoDto with all public-safe properties.</returns>
        public static PartnerPhotoDto ToV2Dto(this PartnerPhoto entity)
        {
            return new PartnerPhotoDto
            {
                Id = entity.Id,
                PartnerId = entity.PartnerId,
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
