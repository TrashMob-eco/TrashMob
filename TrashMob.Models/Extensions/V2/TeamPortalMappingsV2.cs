namespace TrashMob.Models.Extensions.V2
{
    using TrashMob.Models.Poco.V2;

    /// <summary>
    /// Extension methods for mapping team and portal entities to V2 DTOs.
    /// </summary>
    public static class TeamPortalMappingsV2
    {
        #region ProfessionalCleanupLog

        /// <summary>
        /// Maps a ProfessionalCleanupLog entity to a V2 ProfessionalCleanupLogDto.
        /// </summary>
        public static ProfessionalCleanupLogDto ToV2Dto(this ProfessionalCleanupLog entity)
        {
            return new ProfessionalCleanupLogDto
            {
                Id = entity.Id,
                SponsoredAdoptionId = entity.SponsoredAdoptionId,
                ProfessionalCompanyId = entity.ProfessionalCompanyId,
                CleanupDate = entity.CleanupDate,
                DurationMinutes = entity.DurationMinutes,
                BagsCollected = entity.BagsCollected,
                WeightInPounds = entity.WeightInPounds,
                WeightInKilograms = entity.WeightInKilograms,
                Notes = entity.Notes,
                CreatedDate = entity.CreatedDate,
            };
        }

        /// <summary>
        /// Maps a V2 ProfessionalCleanupLogDto to a ProfessionalCleanupLog entity.
        /// </summary>
        public static ProfessionalCleanupLog ToEntity(this ProfessionalCleanupLogDto dto)
        {
            return new ProfessionalCleanupLog
            {
                Id = dto.Id,
                SponsoredAdoptionId = dto.SponsoredAdoptionId,
                ProfessionalCompanyId = dto.ProfessionalCompanyId,
                CleanupDate = dto.CleanupDate,
                DurationMinutes = dto.DurationMinutes,
                BagsCollected = dto.BagsCollected,
                WeightInPounds = dto.WeightInPounds,
                WeightInKilograms = dto.WeightInKilograms,
                Notes = dto.Notes ?? string.Empty,
            };
        }

        #endregion
    }
}
