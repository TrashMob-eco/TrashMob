namespace TrashMobMobile.Tests.ViewModels;

using Moq;
using TrashMob.Models;
using TrashMob.Models.Poco;
using TrashMobMobile.Authentication;
using TrashMobMobile.Services;
using TrashMobMobile.Tests.Helpers;
using TrashMobMobile.ViewModels;
using Xunit;

public class MainViewModelTests
{
    private readonly Mock<IAuthService> mockAuthService;
    private readonly Mock<IUserRestService> mockUserRestService;
    private readonly Mock<IStatsRestService> mockStatsRestService;
    private readonly Mock<IMobEventManager> mockEventManager;
    private readonly Mock<ILitterReportManager> mockLitterReportManager;
    private readonly Mock<INotificationService> mockNotificationService;
    private readonly Mock<IUserManager> mockUserManager;
    private readonly MainViewModel sut;
    private readonly User testUser;

    public MainViewModelTests()
    {
        mockAuthService = new Mock<IAuthService>();
        mockUserRestService = new Mock<IUserRestService>();
        mockStatsRestService = new Mock<IStatsRestService>();
        mockEventManager = new Mock<IMobEventManager>();
        mockLitterReportManager = new Mock<ILitterReportManager>();
        mockNotificationService = new Mock<INotificationService>();
        mockUserManager = new Mock<IUserManager>();

        testUser = TestHelpers.CreateTestUser();
        mockUserManager.Setup(m => m.CurrentUser).Returns(testUser);

        sut = new MainViewModel(
            mockAuthService.Object,
            mockUserRestService.Object,
            mockStatsRestService.Object,
            mockEventManager.Object,
            mockLitterReportManager.Object,
            mockNotificationService.Object,
            mockUserManager.Object);
    }

    [Fact]
    public async Task Init_WhenSignInSucceeds_LoadsUserAndData()
    {
        // Arrange
        SetupSuccessfulSignIn();

        // Act
        await sut.Init();

        // Assert
        Assert.Equal($"Welcome, {testUser.UserName}!", sut.WelcomeMessage);
        Assert.True(sut.IsMapSelected);
        Assert.False(sut.IsListSelected);
    }

    [Fact]
    public async Task Init_WhenSignInSucceeds_LoadsStatistics()
    {
        // Arrange
        SetupSuccessfulSignIn();

        // Act
        await sut.Init();

        // Assert
        Assert.Equal(1000, sut.StatisticsViewModel.TotalAttendees);
        Assert.Equal(5000, sut.StatisticsViewModel.TotalBags);
        Assert.Equal(200, sut.StatisticsViewModel.TotalEvents);
        Assert.Equal(3000, sut.StatisticsViewModel.TotalHours);
        Assert.Equal(500, sut.StatisticsViewModel.TotalLitterReportsSubmitted);
    }

    [Fact]
    public async Task Init_WhenSignInSucceeds_LoadsUpcomingEvents()
    {
        // Arrange
        SetupSuccessfulSignIn();
        var events = TestHelpers.CreateTestEvents(3);
        mockEventManager.Setup(m => m.GetFilteredEventsAsync(It.IsAny<EventFilter>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(events.ToPaginatedList());

        // Act
        await sut.Init();

        // Assert
        Assert.Equal(3, sut.UpcomingEvents.Count);
    }

    [Fact]
    public async Task Init_WhenSignInSucceeds_SetsUserLocation()
    {
        // Arrange
        SetupSuccessfulSignIn();

        // Act
        await sut.Init();

        // Assert
        Assert.NotNull(sut.UserLocation);
        Assert.Equal("Seattle", sut.UserLocation.City);
        Assert.Equal("WA", sut.UserLocation.Region);
        Assert.Contains("Seattle", sut.UserLocationDisplay);
    }

    [Fact]
    public async Task Init_WhenUserHasNoLocation_UsesDefaults()
    {
        // Arrange
        var userWithoutLocation = TestHelpers.CreateTestUser();
        userWithoutLocation.Latitude = null;
        userWithoutLocation.Longitude = null;

        mockAuthService.Setup(a => a.SignInSilentAsync(It.IsAny<bool>()))
            .ReturnsAsync(new SignInResult { Succeeded = true });
        mockAuthService.Setup(a => a.GetUserEmail()).Returns("test@example.com");
        mockUserRestService.Setup(u => u.GetUserByEmailAsync(It.IsAny<string>(), It.IsAny<UserContext>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(userWithoutLocation);
        mockStatsRestService.Setup(s => s.GetStatsAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(TestHelpers.CreateTestStats());
        SetupEmptyEventAndLitterMocks();

        // Act
        await sut.Init();

        // Assert
        Assert.NotNull(sut.UserLocation);
        Assert.Equal(Config.Settings.DefaultTravelDistance, sut.TravelDistance);
    }

    [Fact]
    public async Task Init_MarksEventsUserIsAttending()
    {
        // Arrange
        SetupSuccessfulSignIn();
        var events = TestHelpers.CreateTestEvents(3);
        var attendingEvent = events[1];

        mockEventManager.Setup(m => m.GetFilteredEventsAsync(It.IsAny<EventFilter>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(events.ToPaginatedList());
        mockEventManager.Setup(m => m.GetEventsUserIsAttending(testUser.Id, It.IsAny<CancellationToken>()))
            .ReturnsAsync(new List<Event> { attendingEvent });

        // Act
        await sut.Init();

        // Assert
        var attendingVm = sut.UpcomingEvents.FirstOrDefault(e => e.Id == attendingEvent.Id);
        Assert.NotNull(attendingVm);
        Assert.True(attendingVm.IsUserAttending);

        var notAttendingVm = sut.UpcomingEvents.First(e => e.Id != attendingEvent.Id);
        Assert.False(notAttendingVm.IsUserAttending);
    }

    [Fact]
    public async Task MapSelectedCommand_SetsMapState()
    {
        // Act
        await sut.MapSelectedCommand.ExecuteAsync(null);

        // Assert
        Assert.True(sut.IsMapSelected);
        Assert.False(sut.IsListSelected);
    }

    [Fact]
    public async Task ListSelectedCommand_SetsListState()
    {
        // Act
        await sut.ListSelectedCommand.ExecuteAsync(null);

        // Assert
        Assert.False(sut.IsMapSelected);
        Assert.True(sut.IsListSelected);
    }

    [Fact]
    public async Task Init_WhenSignInFails_DoesNotLoadData()
    {
        // Arrange
        mockAuthService.Setup(a => a.SignInSilentAsync(It.IsAny<bool>()))
            .ReturnsAsync(new SignInResult { Succeeded = false });
        mockStatsRestService.Setup(s => s.GetStatsAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(TestHelpers.CreateTestStats());

        // Act
        await sut.Init();

        // Assert
        Assert.Null(sut.WelcomeMessage);
        Assert.Empty(sut.UpcomingEvents);
    }

    [Fact]
    public async Task Init_LoadsLitterReports()
    {
        // Arrange
        SetupSuccessfulSignIn();
        var litterReports = new PaginatedList<LitterReport>
        {
            TestHelpers.CreateTestLitterReport(),
            TestHelpers.CreateTestLitterReport(),
        };
        mockLitterReportManager.Setup(m => m.GetLitterReportsAsync(It.IsAny<LitterReportFilter>(), It.IsAny<ImageSizeEnum>(), It.IsAny<bool>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(litterReports);

        // Act
        await sut.Init();

        // Assert
        Assert.Equal(2, sut.LitterReports.Count);
    }

    private void SetupSuccessfulSignIn()
    {
        mockAuthService.Setup(a => a.SignInSilentAsync(It.IsAny<bool>()))
            .ReturnsAsync(new SignInResult { Succeeded = true });
        mockAuthService.Setup(a => a.GetUserEmail()).Returns("test@example.com");
        mockUserRestService.Setup(u => u.GetUserByEmailAsync(It.IsAny<string>(), It.IsAny<UserContext>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(testUser);
        mockStatsRestService.Setup(s => s.GetStatsAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(TestHelpers.CreateTestStats());
        SetupEmptyEventAndLitterMocks();
    }

    private void SetupEmptyEventAndLitterMocks()
    {
        mockEventManager.Setup(m => m.GetFilteredEventsAsync(It.IsAny<EventFilter>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new PaginatedList<Event>());
        mockEventManager.Setup(m => m.GetEventsUserIsAttending(It.IsAny<Guid>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new List<Event>());
        mockLitterReportManager.Setup(m => m.GetLitterReportsAsync(It.IsAny<LitterReportFilter>(), It.IsAny<ImageSizeEnum>(), It.IsAny<bool>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new PaginatedList<LitterReport>());
    }
}
