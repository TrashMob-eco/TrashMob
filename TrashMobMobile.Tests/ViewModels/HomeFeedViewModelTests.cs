namespace TrashMobMobile.Tests.ViewModels;

using Moq;
using TrashMob.Models;
using TrashMob.Models.Poco;
using TrashMobMobile.Services;
using TrashMobMobile.Tests.Helpers;
using TrashMobMobile.ViewModels;
using Xunit;

public class HomeFeedViewModelTests
{
    private readonly Mock<IMobEventManager> mockMobEventManager;
    private readonly Mock<ILitterReportManager> mockLitterReportManager;
    private readonly Mock<INotificationService> mockNotificationService;
    private readonly Mock<IUserManager> mockUserManager;
    private readonly HomeFeedViewModel sut;

    public HomeFeedViewModelTests()
    {
        mockMobEventManager = new Mock<IMobEventManager>();
        mockLitterReportManager = new Mock<ILitterReportManager>();
        mockNotificationService = new Mock<INotificationService>();
        mockUserManager = new Mock<IUserManager>();

        var testUser = TestHelpers.CreateTestUser();
        mockUserManager.Setup(m => m.CurrentUser).Returns(testUser);

        sut = new HomeFeedViewModel(
            mockMobEventManager.Object,
            mockLitterReportManager.Object,
            mockNotificationService.Object,
            mockUserManager.Object);
    }

    [Fact]
    public async Task Init_SetsWelcomeMessage()
    {
        // Arrange
        SetupDefaultMocks();

        // Act
        await sut.Init();

        // Assert
        Assert.Equal("Welcome, TestUser!", sut.WelcomeMessage);
    }

    [Fact]
    public async Task Init_LoadsUpcomingEvents()
    {
        // Arrange
        var events = TestHelpers.CreateTestEvents(3);
        mockMobEventManager
            .Setup(m => m.GetFilteredEventsAsync(It.IsAny<EventFilter>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(events.ToPaginatedList());
        SetupDefaultLitterReportMock();

        // Act
        await sut.Init();

        // Assert
        Assert.Equal(3, sut.UpcomingEvents.Count);
        mockMobEventManager.Verify(
            m => m.GetFilteredEventsAsync(It.IsAny<EventFilter>(), It.IsAny<CancellationToken>()),
            Times.Once);
    }

    [Fact]
    public async Task Init_WithNoEvents_SetsAreNoEventsFound()
    {
        // Arrange
        mockMobEventManager
            .Setup(m => m.GetFilteredEventsAsync(It.IsAny<EventFilter>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new PaginatedList<Event>());
        SetupDefaultLitterReportMock();

        // Act
        await sut.Init();

        // Assert
        Assert.Empty(sut.UpcomingEvents);
        Assert.False(sut.AreEventsFound);
        Assert.True(sut.AreNoEventsFound);
    }

    [Fact]
    public async Task Init_WithEvents_SetsAreEventsFound()
    {
        // Arrange
        var events = TestHelpers.CreateTestEvents(3);
        mockMobEventManager
            .Setup(m => m.GetFilteredEventsAsync(It.IsAny<EventFilter>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(events.ToPaginatedList());
        SetupDefaultLitterReportMock();

        // Act
        await sut.Init();

        // Assert
        Assert.True(sut.AreEventsFound);
        Assert.False(sut.AreNoEventsFound);
    }

    [Fact]
    public async Task Init_LoadsNearbyLitterReports()
    {
        // Arrange
        SetupDefaultEventMock();
        var reports = new PaginatedList<LitterReport>();
        reports.Add(TestHelpers.CreateTestLitterReport());
        reports.Add(TestHelpers.CreateTestLitterReport());

        mockLitterReportManager
            .Setup(m => m.GetLitterReportsAsync(It.IsAny<LitterReportFilter>(), It.IsAny<ImageSizeEnum>(), It.IsAny<bool>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(reports);

        // Act
        await sut.Init();

        // Assert
        Assert.Equal(2, sut.NearbyLitterReports.Count);
        mockLitterReportManager.Verify(
            m => m.GetLitterReportsAsync(It.IsAny<LitterReportFilter>(), It.IsAny<ImageSizeEnum>(), It.IsAny<bool>(), It.IsAny<CancellationToken>()),
            Times.Once);
    }

    [Fact]
    public async Task Init_WithNoLitterReports_SetsAreNoLitterReportsFound()
    {
        // Arrange
        SetupDefaultEventMock();
        mockLitterReportManager
            .Setup(m => m.GetLitterReportsAsync(It.IsAny<LitterReportFilter>(), It.IsAny<ImageSizeEnum>(), It.IsAny<bool>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new PaginatedList<LitterReport>());

        // Act
        await sut.Init();

        // Assert
        Assert.Empty(sut.NearbyLitterReports);
        Assert.False(sut.AreLitterReportsFound);
        Assert.True(sut.AreNoLitterReportsFound);
    }

    [Fact]
    public async Task Init_WithLitterReports_SetsAreLitterReportsFound()
    {
        // Arrange
        SetupDefaultEventMock();
        var reports = new PaginatedList<LitterReport>();
        reports.Add(TestHelpers.CreateTestLitterReport());

        mockLitterReportManager
            .Setup(m => m.GetLitterReportsAsync(It.IsAny<LitterReportFilter>(), It.IsAny<ImageSizeEnum>(), It.IsAny<bool>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(reports);

        // Act
        await sut.Init();

        // Assert
        Assert.True(sut.AreLitterReportsFound);
        Assert.False(sut.AreNoLitterReportsFound);
    }

    [Fact]
    public async Task Init_LimitsEventsTo10()
    {
        // Arrange
        var events = TestHelpers.CreateTestEvents(15);
        mockMobEventManager
            .Setup(m => m.GetFilteredEventsAsync(It.IsAny<EventFilter>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(events.ToPaginatedList());
        SetupDefaultLitterReportMock();

        // Act
        await sut.Init();

        // Assert
        Assert.Equal(10, sut.UpcomingEvents.Count);
    }

    [Fact]
    public async Task Init_LimitsLitterReportsTo3()
    {
        // Arrange
        SetupDefaultEventMock();
        var reports = new PaginatedList<LitterReport>();
        for (var i = 0; i < 10; i++)
        {
            reports.Add(TestHelpers.CreateTestLitterReport());
        }

        mockLitterReportManager
            .Setup(m => m.GetLitterReportsAsync(It.IsAny<LitterReportFilter>(), It.IsAny<ImageSizeEnum>(), It.IsAny<bool>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(reports);

        // Act
        await sut.Init();

        // Assert
        Assert.Equal(3, sut.NearbyLitterReports.Count);
    }

    [Fact]
    public async Task Init_ExcludesCompletedEvents()
    {
        // Arrange
        var futureEvents = TestHelpers.CreateTestEvents(2);
        var pastEvent = TestHelpers.CreateTestEvent(
            name: "Past Event",
            eventDate: DateTimeOffset.UtcNow.AddDays(-7));
        var allEvents = futureEvents.Concat(new[] { pastEvent }).ToList();

        mockMobEventManager
            .Setup(m => m.GetFilteredEventsAsync(It.IsAny<EventFilter>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(allEvents.ToPaginatedList());
        SetupDefaultLitterReportMock();

        // Act
        await sut.Init();

        // Assert
        Assert.Equal(2, sut.UpcomingEvents.Count);
    }

    private void SetupDefaultMocks()
    {
        SetupDefaultEventMock();
        SetupDefaultLitterReportMock();
    }

    private void SetupDefaultEventMock()
    {
        mockMobEventManager
            .Setup(m => m.GetFilteredEventsAsync(It.IsAny<EventFilter>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new PaginatedList<Event>());
    }

    private void SetupDefaultLitterReportMock()
    {
        mockLitterReportManager
            .Setup(m => m.GetLitterReportsAsync(It.IsAny<LitterReportFilter>(), It.IsAny<ImageSizeEnum>(), It.IsAny<bool>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new PaginatedList<LitterReport>());
    }
}
