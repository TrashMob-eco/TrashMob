namespace TrashMob.Models.Extensions.V2
{
    using TrashMob.Models.Poco.V2;

    /// <summary>
    /// V2 mapping extensions for job opportunities.
    /// </summary>
    public static class JobOpportunityMappingsV2
    {
        /// <summary>
        /// Maps a <see cref="JobOpportunity"/> to a <see cref="JobOpportunityDto"/>.
        /// </summary>
        public static JobOpportunityDto ToV2Dto(this JobOpportunity entity)
        {
            return new JobOpportunityDto
            {
                Id = entity.Id,
                Title = entity.Title ?? string.Empty,
                TagLine = entity.TagLine ?? string.Empty,
                FullDescription = entity.FullDescription ?? string.Empty,
                IsActive = entity.IsActive,
                CreatedDate = entity.CreatedDate ?? DateTimeOffset.MinValue,
                LastUpdatedDate = entity.LastUpdatedDate ?? DateTimeOffset.MinValue,
            };
        }

        /// <summary>
        /// Maps a <see cref="JobOpportunityDto"/> to a <see cref="JobOpportunity"/> entity.
        /// </summary>
        public static JobOpportunity ToEntity(this JobOpportunityDto dto)
        {
            return new JobOpportunity
            {
                Id = dto.Id,
                Title = dto.Title,
                TagLine = dto.TagLine,
                FullDescription = dto.FullDescription,
                IsActive = dto.IsActive,
            };
        }
    }
}
