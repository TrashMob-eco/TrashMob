namespace TrashMob.Models.Extensions.V2
{
    using TrashMob.Models.Poco.V2;

    /// <summary>
    /// Extension methods for mapping Partner entities to V2 DTOs.
    /// </summary>
    public static class PartnerMappingsV2
    {
        /// <summary>
        /// Maps a Partner entity to a V2 PartnerDto, excluding admin-only fields.
        /// </summary>
        /// <param name="entity">The Partner entity to map.</param>
        /// <returns>A PartnerDto with public-safe properties.</returns>
        public static PartnerDto ToV2Dto(this Partner entity)
        {
            return new PartnerDto
            {
                Id = entity.Id,
                Name = entity.Name,
                Website = entity.Website,
                PublicNotes = entity.PublicNotes,
                LogoUrl = entity.LogoUrl,
                PartnerStatusId = entity.PartnerStatusId,
                PartnerTypeId = entity.PartnerTypeId,
                Slug = entity.Slug,
                HomePageEnabled = entity.HomePageEnabled,
                IsFeatured = entity.IsFeatured,
                BrandingPrimaryColor = entity.BrandingPrimaryColor,
                BrandingSecondaryColor = entity.BrandingSecondaryColor,
                BannerImageUrl = entity.BannerImageUrl,
                Tagline = entity.Tagline,
                Latitude = entity.Latitude,
                Longitude = entity.Longitude,
                City = entity.City,
                Region = entity.Region,
                Country = entity.Country,
                PhysicalAddress = entity.PhysicalAddress,
                BoundsNorth = entity.BoundsNorth,
                BoundsSouth = entity.BoundsSouth,
                BoundsEast = entity.BoundsEast,
                BoundsWest = entity.BoundsWest,
                BoundaryGeoJson = entity.BoundaryGeoJson,
                RegionType = entity.RegionType,
                CountyName = entity.CountyName,
                CreatedByUserId = entity.CreatedByUserId,
                CreatedDate = entity.CreatedDate.GetValueOrDefault(),
                LastUpdatedDate = entity.LastUpdatedDate.GetValueOrDefault(),
            };
        }

        /// <summary>
        /// Maps a V2 <see cref="PartnerDto"/> back to a <see cref="Partner"/> entity.
        /// </summary>
        public static Partner ToEntity(this PartnerDto dto)
        {
            return new Partner
            {
                Id = dto.Id,
                Name = dto.Name,
                Website = dto.Website,
                PublicNotes = dto.PublicNotes,
                LogoUrl = dto.LogoUrl,
                PartnerStatusId = dto.PartnerStatusId,
                PartnerTypeId = dto.PartnerTypeId,
                Slug = dto.Slug,
                HomePageEnabled = dto.HomePageEnabled,
                IsFeatured = dto.IsFeatured,
                BrandingPrimaryColor = dto.BrandingPrimaryColor,
                BrandingSecondaryColor = dto.BrandingSecondaryColor,
                BannerImageUrl = dto.BannerImageUrl,
                Tagline = dto.Tagline,
                Latitude = dto.Latitude,
                Longitude = dto.Longitude,
                City = dto.City,
                Region = dto.Region,
                Country = dto.Country,
                PhysicalAddress = dto.PhysicalAddress,
                BoundsNorth = dto.BoundsNorth,
                BoundsSouth = dto.BoundsSouth,
                BoundsEast = dto.BoundsEast,
                BoundsWest = dto.BoundsWest,
                BoundaryGeoJson = dto.BoundaryGeoJson,
                RegionType = dto.RegionType,
                CountyName = dto.CountyName,
                CreatedByUserId = dto.CreatedByUserId,
            };
        }
    }
}
