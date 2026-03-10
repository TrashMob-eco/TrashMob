namespace TrashMob.Models.Extensions.V2
{
    using TrashMob.Models.Poco.V2;

    /// <summary>
    /// Extension methods for mapping Team entities to V2 DTOs.
    /// </summary>
    public static class TeamMappingsV2
    {
        /// <summary>
        /// Maps a Team entity to a V2 TeamDto.
        /// </summary>
        /// <param name="entity">The Team entity to map.</param>
        /// <returns>A TeamDto with all public-safe properties.</returns>
        public static TeamDto ToV2Dto(this Team entity)
        {
            return new TeamDto
            {
                Id = entity.Id,
                Name = entity.Name,
                Description = entity.Description,
                LogoUrl = entity.LogoUrl,
                IsPublic = entity.IsPublic,
                RequiresApproval = entity.RequiresApproval,
                Latitude = entity.Latitude,
                Longitude = entity.Longitude,
                City = entity.City,
                Region = entity.Region,
                Country = entity.Country,
                PostalCode = entity.PostalCode,
                IsActive = entity.IsActive,
                CreatedByUserId = entity.CreatedByUserId,
                CreatedDate = entity.CreatedDate.GetValueOrDefault(),
                LastUpdatedDate = entity.LastUpdatedDate.GetValueOrDefault(),
            };
        }
        /// <summary>
        /// Maps a V2 TeamDto to a Team entity.
        /// </summary>
        public static Team ToEntity(this TeamDto dto)
        {
            return new Team
            {
                Id = dto.Id,
                Name = dto.Name,
                Description = dto.Description,
                LogoUrl = dto.LogoUrl,
                IsPublic = dto.IsPublic,
                RequiresApproval = dto.RequiresApproval,
                Latitude = dto.Latitude,
                Longitude = dto.Longitude,
                City = dto.City,
                Region = dto.Region,
                Country = dto.Country,
                PostalCode = dto.PostalCode,
                IsActive = dto.IsActive,
                CreatedByUserId = dto.CreatedByUserId,
            };
        }
    }
}
