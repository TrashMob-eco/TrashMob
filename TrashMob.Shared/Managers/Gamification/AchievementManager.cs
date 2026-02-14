namespace TrashMob.Shared.Managers.Gamification
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading;
    using System.Threading.Tasks;
    using Microsoft.EntityFrameworkCore;
    using TrashMob.Models;
    using TrashMob.Shared.Managers.Interfaces;
    using TrashMob.Shared.Persistence;
    using TrashMob.Shared.Poco;

    /// <summary>
    /// Manager for handling user achievements.
    /// </summary>
    public class AchievementManager(MobDbContext dbContext) : IAchievementManager
    {

        /// <inheritdoc />
        public async Task<UserAchievementsResponse> GetUserAchievementsAsync(
            Guid userId,
            CancellationToken cancellationToken = default)
        {
            // Get all active achievement types
            var achievementTypes = await dbContext.AchievementTypes
                .AsNoTracking()
                .Where(a => a.IsActive == true)
                .OrderBy(a => a.DisplayOrder)
                .ToListAsync(cancellationToken);

            // Get user's earned achievements
            var earnedAchievements = await dbContext.UserAchievements
                .AsNoTracking()
                .Where(ua => ua.UserId == userId)
                .ToDictionaryAsync(ua => ua.AchievementTypeId, ua => ua.EarnedDate, cancellationToken);

            var achievements = achievementTypes.Select(at => new AchievementDto
            {
                Id = at.Id,
                Name = at.Name,
                DisplayName = at.DisplayName,
                Description = at.Description,
                Category = at.Category,
                IconUrl = at.IconUrl,
                Points = at.Points,
                IsEarned = earnedAchievements.ContainsKey(at.Id),
                EarnedDate = earnedAchievements.TryGetValue(at.Id, out var earnedDate) ? earnedDate : null
            }).ToList();

            var earnedCount = achievements.Count(a => a.IsEarned);
            var totalPoints = achievements.Where(a => a.IsEarned).Sum(a => a.Points);

            return new UserAchievementsResponse
            {
                UserId = userId,
                TotalPoints = totalPoints,
                EarnedCount = earnedCount,
                TotalCount = achievements.Count,
                Achievements = achievements
            };
        }

        /// <inheritdoc />
        public async Task<IEnumerable<AchievementType>> GetAchievementTypesAsync(
            CancellationToken cancellationToken = default)
        {
            return await dbContext.AchievementTypes
                .AsNoTracking()
                .Where(a => a.IsActive == true)
                .OrderBy(a => a.DisplayOrder)
                .ToListAsync(cancellationToken);
        }

        /// <inheritdoc />
        public async Task<IEnumerable<NewAchievementNotification>> GetUnreadAchievementsAsync(
            Guid userId,
            CancellationToken cancellationToken = default)
        {
            var unreadAchievements = await dbContext.UserAchievements
                .AsNoTracking()
                .Include(ua => ua.AchievementType)
                .Where(ua => ua.UserId == userId && !ua.NotificationSent)
                .OrderByDescending(ua => ua.EarnedDate)
                .ToListAsync(cancellationToken);

            return unreadAchievements.Select(ua => new NewAchievementNotification
            {
                Achievement = new AchievementDto
                {
                    Id = ua.AchievementType.Id,
                    Name = ua.AchievementType.Name,
                    DisplayName = ua.AchievementType.DisplayName,
                    Description = ua.AchievementType.Description,
                    Category = ua.AchievementType.Category,
                    IconUrl = ua.AchievementType.IconUrl,
                    Points = ua.AchievementType.Points,
                    IsEarned = true,
                    EarnedDate = ua.EarnedDate
                },
                EarnedDate = ua.EarnedDate
            });
        }

        /// <inheritdoc />
        public async Task MarkAchievementsAsReadAsync(
            Guid userId,
            IEnumerable<int> achievementTypeIds,
            CancellationToken cancellationToken = default)
        {
            var achievements = await dbContext.UserAchievements
                .Where(ua => ua.UserId == userId && achievementTypeIds.Contains(ua.AchievementTypeId))
                .ToListAsync(cancellationToken);

            foreach (var achievement in achievements)
            {
                achievement.NotificationSent = true;
                achievement.LastUpdatedByUserId = userId;
                achievement.LastUpdatedDate = DateTimeOffset.UtcNow;
            }

            await dbContext.SaveChangesAsync(cancellationToken);
        }

        /// <inheritdoc />
        public async Task<bool> AwardAchievementAsync(
            Guid userId,
            int achievementTypeId,
            CancellationToken cancellationToken = default)
        {
            // Check if user already has this achievement
            var existingAchievement = await dbContext.UserAchievements
                .AsNoTracking()
                .Where(ua => ua.UserId == userId && ua.AchievementTypeId == achievementTypeId)
                .FirstOrDefaultAsync(cancellationToken);

            if (existingAchievement is not null)
            {
                return false; // Already earned
            }

            var userAchievement = new UserAchievement
            {
                Id = Guid.NewGuid(),
                UserId = userId,
                AchievementTypeId = achievementTypeId,
                EarnedDate = DateTimeOffset.UtcNow,
                NotificationSent = false,
                CreatedByUserId = userId,
                CreatedDate = DateTimeOffset.UtcNow,
                LastUpdatedByUserId = userId,
                LastUpdatedDate = DateTimeOffset.UtcNow
            };

            dbContext.UserAchievements.Add(userAchievement);
            await dbContext.SaveChangesAsync(cancellationToken);

            return true;
        }
    }
}
