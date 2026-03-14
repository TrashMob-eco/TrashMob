namespace TrashMob.Models.Extensions.V2
{
    using TrashMob.Models.Poco.V2;

    /// <summary>
    /// V2 mapping extensions for lookup models.
    /// </summary>
    public static class LookupMappingsV2
    {
        /// <summary>
        /// Maps a <see cref="LookupModel"/> to a <see cref="LookupItemDto"/>.
        /// </summary>
        public static LookupItemDto ToV2Dto(this LookupModel entity)
        {
            return new LookupItemDto
            {
                Id = entity.Id,
                Name = entity.Name ?? string.Empty,
                Description = entity.Description ?? string.Empty,
                DisplayOrder = entity.DisplayOrder ?? 0,
                IsActive = entity.IsActive ?? true,
            };
        }

        /// <summary>
        /// Maps a V2 <see cref="LookupItemDto"/> to an <see cref="EventType"/> entity.
        /// Sets IsActive to true by default.
        /// </summary>
        public static EventType ToEventType(this LookupItemDto dto)
        {
            return new EventType
            {
                Id = dto.Id,
                Name = dto.Name,
                Description = dto.Description,
                DisplayOrder = dto.DisplayOrder,
                IsActive = dto.IsActive,
            };
        }

        /// <summary>
        /// Maps a V2 <see cref="LookupItemDto"/> to a <see cref="ServiceType"/> entity.
        /// Sets IsActive to true by default.
        /// </summary>
        public static ServiceType ToServiceType(this LookupItemDto dto)
        {
            return new ServiceType
            {
                Id = dto.Id,
                Name = dto.Name,
                Description = dto.Description,
                DisplayOrder = dto.DisplayOrder,
                IsActive = dto.IsActive,
            };
        }

        /// <summary>
        /// Maps a V2 <see cref="LookupItemDto"/> to an <see cref="EventPartnerLocationServiceStatus"/> entity.
        /// Sets IsActive to true by default.
        /// </summary>
        public static EventPartnerLocationServiceStatus ToEventPartnerLocationServiceStatus(this LookupItemDto dto)
        {
            return new EventPartnerLocationServiceStatus
            {
                Id = dto.Id,
                Name = dto.Name,
                Description = dto.Description,
                DisplayOrder = dto.DisplayOrder,
                IsActive = dto.IsActive,
            };
        }
    }
}
