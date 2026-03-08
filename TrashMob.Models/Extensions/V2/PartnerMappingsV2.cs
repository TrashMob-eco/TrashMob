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
    }
}
