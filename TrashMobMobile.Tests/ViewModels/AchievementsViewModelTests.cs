namespace TrashMobMobile.Tests.ViewModels;

using Moq;
using TrashMobMobile.Models;
using TrashMobMobile.Services;
using TrashMobMobile.ViewModels;
using Xunit;

public class AchievementsViewModelTests
{
    private readonly Mock<IAchievementManager> mockAchievementManager;
    private readonly Mock<INotificationService> mockNotificationService;
    private readonly AchievementsViewModel sut;

    public AchievementsViewModelTests()
    {
        mockAchievementManager = new Mock<IAchievementManager>();
        mockNotificationService = new Mock<INotificationService>();

        sut = new AchievementsViewModel(
            mockAchievementManager.Object,
            mockNotificationService.Object);
    }

    [Fact]
    public async Task Init_LoadsAchievements()
    {
        // Arrange
        SetupDefaultMocks(earnedCount: 3, totalCount: 10);

        // Act
        await sut.Init();

        // Assert
        Assert.Equal(10, sut.Achievements.Count);
        Assert.True(sut.AreAchievementsFound);
        Assert.False(sut.AreNoAchievementsFound);
    }

    [Fact]
    public async Task Init_ShowsProgressDisplay()
    {
        // Arrange
        SetupDefaultMocks(earnedCount: 5, totalCount: 12, totalPoints: 250);

        // Act
        await sut.Init();

        // Assert
        Assert.Equal("5 of 12 earned", sut.ProgressDisplay);
        Assert.Equal("250 points", sut.TotalPointsDisplay);
    }

    [Fact]
    public async Task Init_WithNoAchievements_ShowsEmptyState()
    {
        // Arrange
        SetupDefaultMocks(earnedCount: 0, totalCount: 0);

        // Act
        await sut.Init();

        // Assert
        Assert.Empty(sut.Achievements);
        Assert.False(sut.AreAchievementsFound);
        Assert.True(sut.AreNoAchievementsFound);
    }

    [Fact]
    public async Task Init_EarnedAchievementsAppearFirst()
    {
        // Arrange
        SetupDefaultMocks(earnedCount: 2, totalCount: 4);

        // Act
        await sut.Init();

        // Assert
        Assert.True(sut.Achievements[0].IsEarned);
        Assert.True(sut.Achievements[1].IsEarned);
        Assert.False(sut.Achievements[2].IsEarned);
        Assert.False(sut.Achievements[3].IsEarned);
    }

    [Fact]
    public async Task Init_EarnedAchievementsHaveFullOpacity()
    {
        // Arrange
        SetupDefaultMocks(earnedCount: 1, totalCount: 2);

        // Act
        await sut.Init();

        // Assert
        Assert.Equal(1.0, sut.Achievements[0].Opacity);
        Assert.Equal(0.4, sut.Achievements[1].Opacity);
    }

    [Fact]
    public async Task Init_MarkEarnedAsRead()
    {
        // Arrange
        SetupDefaultMocks(earnedCount: 3, totalCount: 5);

        // Act
        await sut.Init();

        // Assert
        mockAchievementManager.Verify(
            m => m.MarkAsReadAsync(It.Is<IEnumerable<int>>(ids => ids.Count() == 3), It.IsAny<CancellationToken>()),
            Times.Once);
    }

    [Fact]
    public async Task Init_WithNoEarned_DoesNotCallMarkAsRead()
    {
        // Arrange
        SetupDefaultMocks(earnedCount: 0, totalCount: 3);

        // Act
        await sut.Init();

        // Assert
        mockAchievementManager.Verify(
            m => m.MarkAsReadAsync(It.IsAny<IEnumerable<int>>(), It.IsAny<CancellationToken>()),
            Times.Never);
    }

    private void SetupDefaultMocks(int earnedCount = 0, int totalCount = 5, int totalPoints = 100)
    {
        var achievements = new List<AchievementDto>();

        for (var i = 0; i < totalCount; i++)
        {
            var isEarned = i < earnedCount;
            achievements.Add(new AchievementDto
            {
                Id = i + 1,
                Name = $"achievement_{i}",
                DisplayName = $"Achievement {i + 1}",
                Description = $"Description for achievement {i + 1}",
                Category = "Participation",
                IconUrl = $"https://example.com/icon_{i}.png",
                Points = 10 + i * 5,
                IsEarned = isEarned,
                EarnedDate = isEarned ? DateTimeOffset.UtcNow.AddDays(-i) : null,
            });
        }

        var response = new UserAchievementsResponse
        {
            UserId = Guid.NewGuid(),
            TotalPoints = totalPoints,
            EarnedCount = earnedCount,
            TotalCount = totalCount,
            Achievements = achievements,
        };

        mockAchievementManager
            .Setup(m => m.GetMyAchievementsAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(response);

        mockAchievementManager
            .Setup(m => m.MarkAsReadAsync(It.IsAny<IEnumerable<int>>(), It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);
    }
}
