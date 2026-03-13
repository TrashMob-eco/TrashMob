namespace TrashMob.Models.Extensions.V2
{
    using System.Collections.Generic;
    using System.Linq;
    using TrashMob.Models.Poco.V2;

    /// <summary>
    /// Extension methods for mapping LitterReport and LitterImage entities to V2 DTOs.
    /// </summary>
    public static class LitterReportMappingsV2
    {
        /// <summary>
        /// Maps a LitterReport entity to a V2 LitterReportDto, including non-cancelled images.
        /// </summary>
        /// <param name="entity">The LitterReport entity to map.</param>
        /// <returns>A LitterReportDto with associated image DTOs.</returns>
        public static LitterReportDto ToV2Dto(this LitterReport entity)
        {
            return new LitterReportDto
            {
                Id = entity.Id,
                Name = entity.Name,
                Description = entity.Description,
                LitterReportStatusId = entity.LitterReportStatusId,
                CreatedByUserId = entity.CreatedByUserId,
                CreatedDate = entity.CreatedDate.GetValueOrDefault(),
                LastUpdatedDate = entity.LastUpdatedDate.GetValueOrDefault(),
                Images = entity.LitterImages?
                    .Where(li => !li.IsCancelled)
                    .Select(li => li.ToV2Dto())
                    .ToList()
                    ?? [],
            };
        }

        /// <summary>
        /// Maps a LitterImage entity to a V2 LitterImageDto, excluding moderation fields.
        /// </summary>
        /// <param name="entity">The LitterImage entity to map.</param>
        /// <returns>A LitterImageDto with image URL and location data.</returns>
        public static LitterImageDto ToV2Dto(this LitterImage entity)
        {
            return new LitterImageDto
            {
                Id = entity.Id,
                ImageUrl = entity.AzureBlobURL,
                StreetAddress = entity.StreetAddress,
                City = entity.City,
                Region = entity.Region,
                Country = entity.Country,
                PostalCode = entity.PostalCode,
                Latitude = entity.Latitude,
                Longitude = entity.Longitude,
            };
        }

        /// <summary>
        /// Maps a V2 LitterReportDto to a LitterReport entity.
        /// Note: LitterImages are not reverse-mapped; they are handled separately via image upload.
        /// </summary>
        public static LitterReport ToEntity(this LitterReportDto dto)
        {
            return new LitterReport
            {
                Id = dto.Id,
                Name = dto.Name,
                Description = dto.Description,
                LitterReportStatusId = dto.LitterReportStatusId,
                CreatedByUserId = dto.CreatedByUserId,
                LitterImages = dto.Images
                    .Select(img => img.ToEntity(dto.Id))
                    .ToList(),
            };
        }

        /// <summary>
        /// Maps a V2 LitterImageDto back to a LitterImage entity.
        /// </summary>
        public static LitterImage ToEntity(this LitterImageDto dto, System.Guid litterReportId)
        {
            return new LitterImage
            {
                Id = dto.Id,
                LitterReportId = litterReportId,
                AzureBlobURL = dto.ImageUrl,
                StreetAddress = dto.StreetAddress,
                City = dto.City,
                Region = dto.Region,
                Country = dto.Country,
                PostalCode = dto.PostalCode,
                Latitude = dto.Latitude,
                Longitude = dto.Longitude,
            };
        }
    }
}
