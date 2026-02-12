namespace TrashMobMobile.Tests.ViewModels;

using Moq;
using TrashMob.Models.Poco;
using TrashMobMobile.Services;
using TrashMobMobile.Tests.Helpers;
using TrashMobMobile.ViewModels;
using Xunit;

public class ImpactViewModelTests
{
    private readonly Mock<IStatsRestService> mockStatsRestService;
    private readonly Mock<INotificationService> mockNotificationService;
    private readonly Mock<IUserManager> mockUserManager;
    private readonly ImpactViewModel sut;

    public ImpactViewModelTests()
    {
        mockStatsRestService = new Mock<IStatsRestService>();
        mockNotificationService = new Mock<INotificationService>();
        mockUserManager = new Mock<IUserManager>();

        var testUser = TestHelpers.CreateTestUser();
        mockUserManager.Setup(m => m.CurrentUser).Returns(testUser);

        sut = new ImpactViewModel(
            mockStatsRestService.Object,
            mockNotificationService.Object,
            mockUserManager.Object);
    }

    [Fact]
    public async Task Init_LoadsPersonalStats()
    {
        // Arrange
        var personalStats = TestHelpers.CreateTestStats();
        SetupMocks(personalStats: personalStats);

        // Act
        await sut.Init();

        // Assert
        mockStatsRestService.Verify(
            m => m.GetUserStatsAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>()),
            Times.Once);
    }

    [Fact]
    public async Task Init_LoadsCommunityStats()
    {
        // Arrange
        var communityStats = TestHelpers.CreateTestStats();
        SetupMocks(communityStats: communityStats);

        // Act
        await sut.Init();

        // Assert
        mockStatsRestService.Verify(
            m => m.GetStatsAsync(It.IsAny<CancellationToken>()),
            Times.Once);
    }

    [Fact]
    public async Task Init_PersonalStats_MapsTotalEvents()
    {
        // Arrange
        var personalStats = new Stats
        {
            TotalEvents = 42,
            TotalBags = 100,
            TotalHours = 80,
            TotalParticipants = 10,
            TotalLitterReportsSubmitted = 25,
        };
        SetupMocks(personalStats: personalStats);

        // Act
        await sut.Init();

        // Assert
        Assert.Equal(42, sut.PersonalStats.TotalEvents);
    }

    [Fact]
    public async Task Init_CommunityStats_MapsTotalAttendees()
    {
        // Arrange
        var communityStats = new Stats
        {
            TotalParticipants = 5000,
            TotalEvents = 300,
            TotalBags = 10000,
            TotalHours = 6000,
            TotalLitterReportsSubmitted = 800,
        };
        SetupMocks(communityStats: communityStats);

        // Act
        await sut.Init();

        // Assert
        Assert.Equal(5000, sut.CommunityStats.TotalAttendees);
    }

    [Fact]
    public async Task Init_PersonalStats_MapsTotalBags()
    {
        // Arrange
        var personalStats = new Stats
        {
            TotalEvents = 10,
            TotalBags = 250,
            TotalHours = 40,
            TotalParticipants = 5,
            TotalLitterReportsSubmitted = 15,
        };
        SetupMocks(personalStats: personalStats);

        // Act
        await sut.Init();

        // Assert
        Assert.Equal(250, sut.PersonalStats.TotalBags);
    }

    [Fact]
    public async Task Init_CommunityStats_MapsTotalEvents()
    {
        // Arrange
        var communityStats = new Stats
        {
            TotalParticipants = 2000,
            TotalEvents = 150,
            TotalBags = 8000,
            TotalHours = 4000,
            TotalLitterReportsSubmitted = 600,
        };
        SetupMocks(communityStats: communityStats);

        // Act
        await sut.Init();

        // Assert
        Assert.Equal(150, sut.CommunityStats.TotalEvents);
    }

    [Fact]
    public async Task Init_PersonalStats_MapsTotalHours()
    {
        // Arrange
        var personalStats = new Stats
        {
            TotalEvents = 20,
            TotalBags = 100,
            TotalHours = 55,
            TotalParticipants = 8,
            TotalLitterReportsSubmitted = 30,
        };
        SetupMocks(personalStats: personalStats);

        // Act
        await sut.Init();

        // Assert
        Assert.Equal(55, sut.PersonalStats.TotalHours);
    }

    [Fact]
    public async Task Init_PersonalStats_MapsLitterReportsSubmitted()
    {
        // Arrange
        var personalStats = new Stats
        {
            TotalEvents = 10,
            TotalBags = 50,
            TotalHours = 20,
            TotalParticipants = 3,
            TotalLitterReportsSubmitted = 17,
        };
        SetupMocks(personalStats: personalStats);

        // Act
        await sut.Init();

        // Assert
        Assert.Equal(17, sut.PersonalStats.TotalLitterReportsSubmitted);
    }

    private void SetupMocks(Stats? personalStats = null, Stats? communityStats = null)
    {
        mockStatsRestService
            .Setup(m => m.GetUserStatsAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(personalStats ?? TestHelpers.CreateTestStats());

        mockStatsRestService
            .Setup(m => m.GetStatsAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(communityStats ?? TestHelpers.CreateTestStats());
    }
}
