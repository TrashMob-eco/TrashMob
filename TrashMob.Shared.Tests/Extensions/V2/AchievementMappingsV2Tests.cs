namespace TrashMob.Shared.Tests.Extensions.V2
{
    using System;
    using System.Collections.Generic;
    using TrashMob.Models;
    using TrashMob.Shared.Extensions.V2;
    using TrashMob.Shared.Poco;
    using Xunit;

    public class AchievementMappingsV2Tests
    {
        [Fact]
        public void AchievementType_ToV2Dto_MapsAllProperties()
        {
            var entity = new AchievementType
            {
                Id = 5,
                Name = "first_cleanup",
                DisplayName = "First Cleanup",
                Description = "Attend your first cleanup event",
                Category = "Participation",
                IconUrl = "https://example.com/badge.png",
                Points = 10,
                DisplayOrder = 1,
                IsActive = true,
            };

            var dto = entity.ToV2Dto();

            Assert.Equal(5, dto.Id);
            Assert.Equal("first_cleanup", dto.Name);
            Assert.Equal("First Cleanup", dto.DisplayName);
            Assert.Equal("Attend your first cleanup event", dto.Description);
            Assert.Equal("Participation", dto.Category);
            Assert.Equal("https://example.com/badge.png", dto.IconUrl);
            Assert.Equal(10, dto.Points);
            Assert.Equal(1, dto.DisplayOrder);
        }

        [Fact]
        public void AchievementType_ToV2Dto_HandlesNullProperties()
        {
            var entity = new AchievementType
            {
                Id = 1,
                Name = null,
                DisplayName = null,
                Description = null,
                Category = null,
                IconUrl = null,
                DisplayOrder = null,
            };

            var dto = entity.ToV2Dto();

            Assert.Equal(string.Empty, dto.Name);
            Assert.Equal(string.Empty, dto.DisplayName);
            Assert.Equal(string.Empty, dto.Description);
            Assert.Equal(string.Empty, dto.Category);
            Assert.Equal(string.Empty, dto.IconUrl);
            Assert.Equal(0, dto.DisplayOrder);
        }

        [Fact]
        public void AchievementDto_ToV2Dto_MapsAllProperties()
        {
            var dto = new AchievementDto
            {
                Id = 3,
                Name = "bag_collector",
                DisplayName = "Bag Collector",
                Description = "Collect 100 bags",
                Category = "Impact",
                IconUrl = "https://example.com/bags.png",
                Points = 50,
                IsEarned = true,
                EarnedDate = new DateTimeOffset(2026, 2, 15, 10, 0, 0, TimeSpan.Zero),
            };

            var v2Dto = dto.ToV2Dto();

            Assert.Equal(3, v2Dto.Id);
            Assert.Equal("bag_collector", v2Dto.Name);
            Assert.Equal("Bag Collector", v2Dto.DisplayName);
            Assert.Equal("Collect 100 bags", v2Dto.Description);
            Assert.Equal("Impact", v2Dto.Category);
            Assert.Equal("https://example.com/bags.png", v2Dto.IconUrl);
            Assert.Equal(50, v2Dto.Points);
            Assert.True(v2Dto.IsEarned);
            Assert.Equal(dto.EarnedDate, v2Dto.EarnedDate);
        }

        [Fact]
        public void AchievementDto_ToV2Dto_HandlesUnearnedAchievement()
        {
            var dto = new AchievementDto
            {
                Id = 1,
                Name = "event_leader",
                IsEarned = false,
                EarnedDate = null,
            };

            var v2Dto = dto.ToV2Dto();

            Assert.False(v2Dto.IsEarned);
            Assert.Null(v2Dto.EarnedDate);
        }

        [Fact]
        public void UserAchievementsResponse_ToV2Dto_MapsAllProperties()
        {
            var userId = Guid.NewGuid();
            var response = new UserAchievementsResponse
            {
                UserId = userId,
                TotalPoints = 60,
                EarnedCount = 2,
                TotalCount = 10,
                Achievements = new List<AchievementDto>
                {
                    new()
                    {
                        Id = 1,
                        Name = "first_cleanup",
                        DisplayName = "First Cleanup",
                        Points = 10,
                        IsEarned = true,
                        EarnedDate = DateTimeOffset.UtcNow,
                    },
                    new()
                    {
                        Id = 2,
                        Name = "team_player",
                        DisplayName = "Team Player",
                        Points = 50,
                        IsEarned = true,
                        EarnedDate = DateTimeOffset.UtcNow,
                    },
                },
            };

            var v2Dto = response.ToV2Dto();

            Assert.Equal(userId, v2Dto.UserId);
            Assert.Equal(60, v2Dto.TotalPoints);
            Assert.Equal(2, v2Dto.EarnedCount);
            Assert.Equal(10, v2Dto.TotalCount);
            Assert.Equal(2, v2Dto.Achievements.Count);
            Assert.Equal("First Cleanup", v2Dto.Achievements[0].DisplayName);
            Assert.Equal("Team Player", v2Dto.Achievements[1].DisplayName);
        }

        [Fact]
        public void NewAchievementNotification_ToV2Dto_MapsAllProperties()
        {
            var earnedDate = new DateTimeOffset(2026, 3, 5, 14, 0, 0, TimeSpan.Zero);
            var notification = new NewAchievementNotification
            {
                Achievement = new AchievementDto
                {
                    Id = 7,
                    Name = "weight_champion",
                    DisplayName = "Weight Champion",
                    Points = 100,
                    IsEarned = true,
                    EarnedDate = earnedDate,
                },
                EarnedDate = earnedDate,
            };

            var v2Dto = notification.ToV2Dto();

            Assert.Equal(7, v2Dto.Achievement.Id);
            Assert.Equal("Weight Champion", v2Dto.Achievement.DisplayName);
            Assert.Equal(100, v2Dto.Achievement.Points);
            Assert.Equal(earnedDate, v2Dto.EarnedDate);
        }

        [Fact]
        public void NewAchievementNotification_ToV2Dto_HandlesNullAchievement()
        {
            var notification = new NewAchievementNotification
            {
                Achievement = null,
                EarnedDate = DateTimeOffset.UtcNow,
            };

            var v2Dto = notification.ToV2Dto();

            Assert.NotNull(v2Dto.Achievement);
            Assert.Equal(0, v2Dto.Achievement.Id);
        }
    }
}
