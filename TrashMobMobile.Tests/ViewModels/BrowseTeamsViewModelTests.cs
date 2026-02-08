namespace TrashMobMobile.Tests.ViewModels;

using Moq;
using TrashMob.Models;
using TrashMobMobile.Services;
using TrashMobMobile.Tests.Helpers;
using TrashMobMobile.ViewModels;
using Xunit;

public class BrowseTeamsViewModelTests
{
    private readonly Mock<ITeamManager> mockTeamManager;
    private readonly Mock<INotificationService> mockNotificationService;
    private readonly Mock<IUserManager> mockUserManager;
    private readonly BrowseTeamsViewModel sut;

    public BrowseTeamsViewModelTests()
    {
        mockTeamManager = new Mock<ITeamManager>();
        mockNotificationService = new Mock<INotificationService>();
        mockUserManager = new Mock<IUserManager>();

        var testUser = TestHelpers.CreateTestUser();
        mockUserManager.Setup(m => m.CurrentUser).Returns(testUser);

        sut = new BrowseTeamsViewModel(
            mockTeamManager.Object,
            mockNotificationService.Object,
            mockUserManager.Object);
    }

    [Fact]
    public async Task Init_LoadsPublicTeams()
    {
        // Arrange
        var teams = CreateTestTeams(3);
        mockTeamManager.Setup(m => m.GetPublicTeamsAsync(
                It.IsAny<double?>(), It.IsAny<double?>(), It.IsAny<double?>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(teams);
        mockTeamManager.Setup(m => m.GetMyTeamsAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(new List<Team>());

        // Act
        await sut.Init();

        // Assert
        Assert.Equal(3, sut.Teams.Count);
        Assert.True(sut.AreTeamsFound);
        Assert.False(sut.AreNoTeamsFound);
    }

    [Fact]
    public async Task Init_WithNoTeams_ShowsNoTeamsFound()
    {
        // Arrange
        mockTeamManager.Setup(m => m.GetPublicTeamsAsync(
                It.IsAny<double?>(), It.IsAny<double?>(), It.IsAny<double?>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new List<Team>());
        mockTeamManager.Setup(m => m.GetMyTeamsAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(new List<Team>());

        // Act
        await sut.Init();

        // Assert
        Assert.Empty(sut.Teams);
        Assert.False(sut.AreTeamsFound);
        Assert.True(sut.AreNoTeamsFound);
    }

    [Fact]
    public async Task Init_MarksUserMemberTeams()
    {
        // Arrange
        var teams = CreateTestTeams(3);
        var myTeams = new List<Team> { teams[1] };

        mockTeamManager.Setup(m => m.GetPublicTeamsAsync(
                It.IsAny<double?>(), It.IsAny<double?>(), It.IsAny<double?>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(teams);
        mockTeamManager.Setup(m => m.GetMyTeamsAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(myTeams);

        // Act
        await sut.Init();

        // Assert
        Assert.Equal(3, sut.Teams.Count);
        Assert.False(sut.Teams[0].IsUserMember);
        Assert.True(sut.Teams[1].IsUserMember);
        Assert.False(sut.Teams[2].IsUserMember);
    }

    [Fact]
    public async Task Init_FiltersInactiveTeams()
    {
        // Arrange
        var teams = CreateTestTeams(3);
        teams[2].IsActive = false;

        mockTeamManager.Setup(m => m.GetPublicTeamsAsync(
                It.IsAny<double?>(), It.IsAny<double?>(), It.IsAny<double?>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(teams);
        mockTeamManager.Setup(m => m.GetMyTeamsAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(new List<Team>());

        // Act
        await sut.Init();

        // Assert
        Assert.Equal(2, sut.Teams.Count);
    }

    [Fact]
    public async Task RefreshCommand_ReloadsTeams()
    {
        // Arrange
        var teams = CreateTestTeams(2);
        mockTeamManager.Setup(m => m.GetPublicTeamsAsync(
                It.IsAny<double?>(), It.IsAny<double?>(), It.IsAny<double?>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(teams);
        mockTeamManager.Setup(m => m.GetMyTeamsAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(new List<Team>());

        // Act
        await sut.RefreshCommand.ExecuteAsync(null);

        // Assert
        Assert.Equal(2, sut.Teams.Count);
        mockTeamManager.Verify(m => m.GetPublicTeamsAsync(
            It.IsAny<double?>(), It.IsAny<double?>(), It.IsAny<double?>(), It.IsAny<CancellationToken>()), Times.Once);
    }

    private static List<Team> CreateTestTeams(int count)
    {
        var teams = new List<Team>();
        for (var i = 0; i < count; i++)
        {
            teams.Add(new Team
            {
                Id = Guid.NewGuid(),
                Name = $"Team {i + 1}",
                Description = $"Description for team {i + 1}",
                City = "Seattle",
                Region = "WA",
                Country = "United States",
                IsPublic = true,
                IsActive = true,
                Members = [],
            });
        }

        return teams;
    }
}
