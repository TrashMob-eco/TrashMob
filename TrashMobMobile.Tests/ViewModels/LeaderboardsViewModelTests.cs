namespace TrashMobMobile.Tests.ViewModels;

using Moq;
using TrashMobMobile.Models;
using TrashMobMobile.Services;
using TrashMobMobile.Tests.Helpers;
using TrashMobMobile.ViewModels;
using Xunit;

public class LeaderboardsViewModelTests
{
    private readonly Mock<ILeaderboardManager> mockLeaderboardManager;
    private readonly Mock<INotificationService> mockNotificationService;
    private readonly Mock<IUserManager> mockUserManager;
    private readonly LeaderboardsViewModel sut;
    private readonly Guid testUserId;

    public LeaderboardsViewModelTests()
    {
        mockLeaderboardManager = new Mock<ILeaderboardManager>();
        mockNotificationService = new Mock<INotificationService>();
        mockUserManager = new Mock<IUserManager>();

        var testUser = TestHelpers.CreateTestUser();
        testUserId = testUser.Id;
        mockUserManager.Setup(m => m.CurrentUser).Returns(testUser);

        sut = new LeaderboardsViewModel(
            mockLeaderboardManager.Object,
            mockNotificationService.Object,
            mockUserManager.Object);
    }

    [Fact]
    public async Task Init_LoadsUserLeaderboard()
    {
        // Arrange
        SetupDefaultMocks(entryCount: 5);

        // Act
        await sut.Init();

        // Assert
        Assert.Equal(5, sut.Entries.Count);
        Assert.True(sut.AreEntriesFound);
        Assert.False(sut.AreNoEntriesFound);
        mockLeaderboardManager.Verify(m => m.GetLeaderboardAsync(
            "Events", "Month", 50, It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task Init_WithNoEntries_ShowsEmptyState()
    {
        // Arrange
        SetupDefaultMocks(entryCount: 0);

        // Act
        await sut.Init();

        // Assert
        Assert.Empty(sut.Entries);
        Assert.False(sut.AreEntriesFound);
        Assert.True(sut.AreNoEntriesFound);
    }

    [Fact]
    public async Task Init_LoadsMyRank()
    {
        // Arrange
        SetupDefaultMocks(entryCount: 3, myRank: 5, myScore: 10);

        // Act
        await sut.Init();

        // Assert
        Assert.True(sut.IsMyRankVisible);
        Assert.Contains("#5", sut.MyRankDisplay);
    }

    [Fact]
    public async Task Init_HighlightsCurrentUser()
    {
        // Arrange
        var entries = new List<LeaderboardEntry>
        {
            new() { EntityId = Guid.NewGuid(), EntityName = "Other User", Rank = 1, FormattedScore = "20 events" },
            new() { EntityId = testUserId, EntityName = "Test User", Rank = 2, FormattedScore = "15 events" },
        };
        SetupDefaultMocks(entries: entries);

        // Act
        await sut.Init();

        // Assert
        Assert.Equal(2, sut.Entries.Count);
        Assert.False(sut.Entries[0].IsCurrentUser);
        Assert.True(sut.Entries[1].IsCurrentUser);
    }

    [Fact]
    public async Task ShowTeamLeaderboard_LoadsTeamEntries()
    {
        // Arrange
        SetupDefaultMocks(entryCount: 3);
        await sut.Init();

        // Act
        sut.ShowTeamLeaderboardCommand.Execute(null);
        await Task.Delay(100); // Allow async command to complete

        // Assert
        Assert.True(sut.ShowTeams);
        Assert.False(sut.ShowUsers);
        mockLeaderboardManager.Verify(m => m.GetTeamLeaderboardAsync(
            It.IsAny<string>(), It.IsAny<string>(), It.IsAny<int>(), It.IsAny<CancellationToken>()), Times.AtLeastOnce);
    }

    [Fact]
    public async Task Init_DefaultsToUsersAndEventsAndMonth()
    {
        // Arrange
        SetupDefaultMocks();

        // Act
        await sut.Init();

        // Assert
        Assert.True(sut.ShowUsers);
        Assert.False(sut.ShowTeams);
        Assert.Equal("Events", sut.SelectedType);
        Assert.Equal("Month", sut.SelectedTimeRange);
    }

    private void SetupDefaultMocks(
        int entryCount = 0,
        int? myRank = null,
        decimal? myScore = null,
        List<LeaderboardEntry>? entries = null)
    {
        var leaderboardEntries = entries ?? Enumerable.Range(1, entryCount)
            .Select(i => new LeaderboardEntry
            {
                EntityId = Guid.NewGuid(),
                EntityName = $"User {i}",
                Rank = i,
                Score = 100 - i,
                FormattedScore = $"{100 - i} events",
            })
            .ToList();

        var response = new LeaderboardResponse
        {
            LeaderboardType = "Events",
            TimeRange = "Month",
            TotalEntries = leaderboardEntries.Count,
            Entries = leaderboardEntries,
        };

        mockLeaderboardManager
            .Setup(m => m.GetLeaderboardAsync(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<int>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(response);

        mockLeaderboardManager
            .Setup(m => m.GetTeamLeaderboardAsync(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<int>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(response);

        mockLeaderboardManager
            .Setup(m => m.GetMyRankAsync(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new UserRankResponse
            {
                Rank = myRank,
                Score = myScore,
                FormattedScore = myScore.HasValue ? $"{myScore} events" : string.Empty,
                TotalRanked = leaderboardEntries.Count,
                IsEligible = true,
            });
    }
}
