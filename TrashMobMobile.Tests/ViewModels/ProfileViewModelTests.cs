namespace TrashMobMobile.Tests.ViewModels;

using Moq;
using TrashMob.Models;
using TrashMob.Models.Poco;
using TrashMobMobile.Services;
using TrashMobMobile.Tests.Helpers;
using TrashMobMobile.ViewModels;
using Xunit;

public class ProfileViewModelTests
{
    private readonly Mock<IMobEventManager> mockEventManager;
    private readonly Mock<ILitterReportManager> mockLitterReportManager;
    private readonly Mock<ITeamManager> mockTeamManager;
    private readonly Mock<INotificationService> mockNotificationService;
    private readonly Mock<IUserManager> mockUserManager;
    private readonly ProfileViewModel sut;

    public ProfileViewModelTests()
    {
        mockEventManager = new Mock<IMobEventManager>();
        mockLitterReportManager = new Mock<ILitterReportManager>();
        mockTeamManager = new Mock<ITeamManager>();
        mockNotificationService = new Mock<INotificationService>();
        mockUserManager = new Mock<IUserManager>();

        var testUser = TestHelpers.CreateTestUser();
        testUser.MemberSince = DateTimeOffset.UtcNow.AddYears(-1);
        mockUserManager.Setup(m => m.CurrentUser).Returns(testUser);

        SetupDefaultMocks();

        sut = new ProfileViewModel(
            mockEventManager.Object,
            mockLitterReportManager.Object,
            mockTeamManager.Object,
            mockNotificationService.Object,
            mockUserManager.Object);
    }

    [Fact]
    public async Task Init_SetsUserInfo()
    {
        // Act
        await sut.Init();

        // Assert
        Assert.Equal("TestUser", sut.UserName);
        Assert.Equal("test@example.com", sut.UserEmail);
        Assert.Contains("Member since", sut.MemberSince);
    }

    [Fact]
    public async Task Init_SetsLocationSummary()
    {
        // Act
        await sut.Init();

        // Assert
        Assert.Contains("Seattle", sut.LocationSummary);
        Assert.Contains("WA", sut.LocationSummary);
    }

    [Fact]
    public async Task Init_LoadsMyTeams()
    {
        // Arrange
        var teams = new List<Team>
        {
            new() { Id = Guid.NewGuid(), Name = "Team 1", IsActive = true, Members = [] },
            new() { Id = Guid.NewGuid(), Name = "Team 2", IsActive = true, Members = [] },
        };
        mockTeamManager.Setup(m => m.GetMyTeamsAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(teams);

        // Act
        await sut.Init();

        // Assert
        Assert.Equal(2, sut.MyTeams.Count);
        Assert.True(sut.AreTeamsFound);
        Assert.False(sut.AreNoTeamsFound);
    }

    [Fact]
    public async Task Init_WithNoTeams_ShowsNoTeamsFound()
    {
        // Arrange
        mockTeamManager.Setup(m => m.GetMyTeamsAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(new List<Team>());

        // Act
        await sut.Init();

        // Assert
        Assert.Empty(sut.MyTeams);
        Assert.False(sut.AreTeamsFound);
        Assert.True(sut.AreNoTeamsFound);
    }

    [Fact]
    public async Task Init_FiltersInactiveTeams()
    {
        // Arrange
        var teams = new List<Team>
        {
            new() { Id = Guid.NewGuid(), Name = "Active Team", IsActive = true, Members = [] },
            new() { Id = Guid.NewGuid(), Name = "Inactive Team", IsActive = false, Members = [] },
        };
        mockTeamManager.Setup(m => m.GetMyTeamsAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(teams);

        // Act
        await sut.Init();

        // Assert
        Assert.Single(sut.MyTeams);
        Assert.Equal("Active Team", sut.MyTeams[0].Name);
    }

    [Fact]
    public async Task ShowUpcomingEventsCommand_SetsUpcomingState()
    {
        // Act
        sut.ShowUpcomingEventsCommand.Execute(null);

        // Assert
        Assert.True(sut.ShowUpcoming);
        Assert.False(sut.ShowCompleted);
    }

    [Fact]
    public async Task ShowCompletedEventsCommand_SetsCompletedState()
    {
        // Act
        sut.ShowCompletedEventsCommand.Execute(null);

        // Assert
        Assert.False(sut.ShowUpcoming);
        Assert.True(sut.ShowCompleted);
    }

    private void SetupDefaultMocks()
    {
        mockEventManager.Setup(m => m.GetUserEventsAsync(It.IsAny<EventFilter>(), It.IsAny<Guid>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new PaginatedList<Event>());
        mockLitterReportManager.Setup(m => m.GetLitterReportsAsync(It.IsAny<LitterReportFilter>(), It.IsAny<ImageSizeEnum>(), It.IsAny<bool>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new PaginatedList<LitterReport>());
        mockTeamManager.Setup(m => m.GetMyTeamsAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(new List<Team>());
    }
}
