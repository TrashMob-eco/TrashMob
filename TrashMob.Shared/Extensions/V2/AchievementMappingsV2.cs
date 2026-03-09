namespace TrashMob.Shared.Extensions.V2
{
    using System.Linq;
    using TrashMob.Models;
    using TrashMob.Models.Poco.V2;
    using TrashMob.Shared.Poco;

    /// <summary>
    /// V2 mapping extensions for achievement types.
    /// </summary>
    public static class AchievementMappingsV2
    {
        /// <summary>
        /// Maps an <see cref="AchievementType"/> entity to an <see cref="AchievementTypeDto"/>.
        /// </summary>
        /// <param name="entity">The achievement type entity.</param>
        /// <returns>A V2 achievement type DTO.</returns>
        public static AchievementTypeDto ToV2Dto(this AchievementType entity)
        {
            return new AchievementTypeDto
            {
                Id = entity.Id,
                Name = entity.Name ?? string.Empty,
                DisplayName = entity.DisplayName ?? string.Empty,
                Description = entity.Description ?? string.Empty,
                Category = entity.Category ?? string.Empty,
                IconUrl = entity.IconUrl ?? string.Empty,
                Points = entity.Points,
                DisplayOrder = entity.DisplayOrder.GetValueOrDefault(),
            };
        }

        /// <summary>
        /// Maps an <see cref="AchievementDto"/> to a <see cref="UserAchievementDto"/>.
        /// </summary>
        /// <param name="dto">The achievement DTO.</param>
        /// <returns>A V2 user achievement DTO.</returns>
        public static UserAchievementDto ToV2Dto(this AchievementDto dto)
        {
            return new UserAchievementDto
            {
                Id = dto.Id,
                Name = dto.Name ?? string.Empty,
                DisplayName = dto.DisplayName ?? string.Empty,
                Description = dto.Description ?? string.Empty,
                Category = dto.Category ?? string.Empty,
                IconUrl = dto.IconUrl ?? string.Empty,
                Points = dto.Points,
                IsEarned = dto.IsEarned,
                EarnedDate = dto.EarnedDate,
            };
        }

        /// <summary>
        /// Maps a <see cref="UserAchievementsResponse"/> to a <see cref="UserAchievementsResponseDto"/>.
        /// </summary>
        /// <param name="response">The user achievements response.</param>
        /// <returns>A V2 user achievements response DTO.</returns>
        public static UserAchievementsResponseDto ToV2Dto(this UserAchievementsResponse response)
        {
            return new UserAchievementsResponseDto
            {
                UserId = response.UserId,
                TotalPoints = response.TotalPoints,
                EarnedCount = response.EarnedCount,
                TotalCount = response.TotalCount,
                Achievements = response.Achievements?.Select(a => a.ToV2Dto()).ToList() ?? [],
            };
        }

        /// <summary>
        /// Maps a <see cref="NewAchievementNotification"/> to an <see cref="AchievementNotificationDto"/>.
        /// </summary>
        /// <param name="notification">The achievement notification.</param>
        /// <returns>A V2 achievement notification DTO.</returns>
        public static AchievementNotificationDto ToV2Dto(this NewAchievementNotification notification)
        {
            return new AchievementNotificationDto
            {
                Achievement = notification.Achievement?.ToV2Dto() ?? new UserAchievementDto(),
                EarnedDate = notification.EarnedDate,
            };
        }
    }
}
