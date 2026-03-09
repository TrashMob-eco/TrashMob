namespace TrashMob.Shared.Tests.Controllers.V2
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading;
    using System.Threading.Tasks;
    using Microsoft.AspNetCore.Http;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.Extensions.Logging;
    using Moq;
    using TrashMob.Controllers.V2;
    using TrashMob.Models;
    using TrashMob.Models.Poco.V2;
    using TrashMob.Shared.Managers.Interfaces;
    using TrashMob.Shared.Poco;
    using Xunit;

    public class AchievementsV2ControllerTests
    {
        private readonly Mock<IAchievementManager> achievementManager = new();
        private readonly Mock<ILogger<AchievementsV2Controller>> logger = new();
        private readonly AchievementsV2Controller controller;

        public AchievementsV2ControllerTests()
        {
            controller = new AchievementsV2Controller(achievementManager.Object, logger.Object);
            controller.ControllerContext = new ControllerContext
            {
                HttpContext = new DefaultHttpContext(),
            };
        }

        [Fact]
        public async Task GetAchievementTypes_ReturnsOkWithTypes()
        {
            var types = new List<AchievementType>
            {
                new()
                {
                    Id = 1,
                    Name = "first_cleanup",
                    DisplayName = "First Cleanup",
                    Description = "Attend your first event",
                    Category = "Participation",
                    IconUrl = "https://example.com/first.png",
                    Points = 10,
                    DisplayOrder = 1,
                },
                new()
                {
                    Id = 2,
                    Name = "bag_collector",
                    DisplayName = "Bag Collector",
                    Description = "Collect 100 bags",
                    Category = "Impact",
                    IconUrl = "https://example.com/bags.png",
                    Points = 50,
                    DisplayOrder = 2,
                },
            };

            achievementManager
                .Setup(m => m.GetAchievementTypesAsync(It.IsAny<CancellationToken>()))
                .ReturnsAsync(types);

            var result = await controller.GetAchievementTypes(CancellationToken.None);

            var okResult = Assert.IsType<OkObjectResult>(result);
            var dtos = Assert.IsAssignableFrom<IReadOnlyList<AchievementTypeDto>>(okResult.Value);
            Assert.Equal(2, dtos.Count);
            Assert.Equal("First Cleanup", dtos[0].DisplayName);
            Assert.Equal("Bag Collector", dtos[1].DisplayName);
        }

        [Fact]
        public async Task GetUserAchievements_ReturnsOkWithResponse()
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
                },
            };

            achievementManager
                .Setup(m => m.GetUserAchievementsAsync(userId, It.IsAny<CancellationToken>()))
                .ReturnsAsync(response);

            var result = await controller.GetUserAchievements(userId, CancellationToken.None);

            var okResult = Assert.IsType<OkObjectResult>(result);
            var dto = Assert.IsType<UserAchievementsResponseDto>(okResult.Value);
            Assert.Equal(userId, dto.UserId);
            Assert.Equal(60, dto.TotalPoints);
            Assert.Equal(2, dto.EarnedCount);
            Assert.Single(dto.Achievements);
        }

        [Fact]
        public async Task GetMyAchievements_ReturnsOkWithResponse()
        {
            var userId = Guid.NewGuid();
            controller.HttpContext.Items["UserId"] = userId.ToString();

            var response = new UserAchievementsResponse
            {
                UserId = userId,
                TotalPoints = 100,
                EarnedCount = 5,
                TotalCount = 10,
                Achievements = new List<AchievementDto>(),
            };

            achievementManager
                .Setup(m => m.GetUserAchievementsAsync(userId, It.IsAny<CancellationToken>()))
                .ReturnsAsync(response);

            var result = await controller.GetMyAchievements(CancellationToken.None);

            var okResult = Assert.IsType<OkObjectResult>(result);
            var dto = Assert.IsType<UserAchievementsResponseDto>(okResult.Value);
            Assert.Equal(userId, dto.UserId);
            Assert.Equal(100, dto.TotalPoints);
            Assert.Equal(5, dto.EarnedCount);
        }

        [Fact]
        public async Task GetUnreadAchievements_ReturnsOkWithNotifications()
        {
            var userId = Guid.NewGuid();
            controller.HttpContext.Items["UserId"] = userId.ToString();

            var earnedDate = new DateTimeOffset(2026, 3, 5, 14, 0, 0, TimeSpan.Zero);
            var notifications = new List<NewAchievementNotification>
            {
                new()
                {
                    Achievement = new AchievementDto
                    {
                        Id = 3,
                        Name = "team_player",
                        DisplayName = "Team Player",
                        Points = 25,
                        IsEarned = true,
                        EarnedDate = earnedDate,
                    },
                    EarnedDate = earnedDate,
                },
            };

            achievementManager
                .Setup(m => m.GetUnreadAchievementsAsync(userId, It.IsAny<CancellationToken>()))
                .ReturnsAsync(notifications);

            var result = await controller.GetUnreadAchievements(CancellationToken.None);

            var okResult = Assert.IsType<OkObjectResult>(result);
            var dtos = Assert.IsAssignableFrom<List<AchievementNotificationDto>>(okResult.Value);
            Assert.Single(dtos);
            Assert.Equal("Team Player", dtos[0].Achievement.DisplayName);
            Assert.Equal(earnedDate, dtos[0].EarnedDate);
        }

        [Fact]
        public async Task MarkAchievementsAsRead_ReturnsNoContent()
        {
            var userId = Guid.NewGuid();
            controller.HttpContext.Items["UserId"] = userId.ToString();

            var achievementTypeIds = new List<int> { 1, 3, 5 };

            achievementManager
                .Setup(m => m.MarkAchievementsAsReadAsync(userId, It.IsAny<IEnumerable<int>>(), It.IsAny<CancellationToken>()))
                .Returns(Task.CompletedTask);

            var result = await controller.MarkAchievementsAsRead(achievementTypeIds, CancellationToken.None);

            Assert.IsType<NoContentResult>(result);
            achievementManager.Verify(
                m => m.MarkAchievementsAsReadAsync(userId, It.IsAny<IEnumerable<int>>(), It.IsAny<CancellationToken>()),
                Times.Once);
        }
    }
}
