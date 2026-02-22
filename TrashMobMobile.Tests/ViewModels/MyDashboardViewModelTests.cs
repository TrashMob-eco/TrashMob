namespace TrashMobMobile.Tests.ViewModels;

using Moq;
using TrashMob.Models;
using TrashMob.Models.Poco;
using TrashMobMobile.Services;
using TrashMobMobile.Tests.Helpers;
using TrashMobMobile.ViewModels;
using Xunit;

public class MyDashboardViewModelTests
{
    private readonly Mock<IMobEventManager> mockMobEventManager;
    private readonly Mock<IStatsRestService> mockStatsRestService;
    private readonly Mock<ILitterReportManager> mockLitterReportManager;
    private readonly Mock<INotificationService> mockNotificationService;
    private readonly Mock<IUserManager> mockUserManager;
    private readonly Mock<IWaiverManager> mockWaiverManager;
    private readonly MyDashboardViewModel sut;

    public MyDashboardViewModelTests()
    {
        mockMobEventManager = new Mock<IMobEventManager>();
        mockStatsRestService = new Mock<IStatsRestService>();
        mockLitterReportManager = new Mock<ILitterReportManager>();
        mockNotificationService = new Mock<INotificationService>();
        mockUserManager = new Mock<IUserManager>();
        mockWaiverManager = new Mock<IWaiverManager>();

        var testUser = TestHelpers.CreateTestUser();
        mockUserManager.Setup(m => m.CurrentUser).Returns(testUser);

        sut = new MyDashboardViewModel(
            mockMobEventManager.Object,
            mockStatsRestService.Object,
            mockLitterReportManager.Object,
            mockNotificationService.Object,
            mockUserManager.Object,
            mockWaiverManager.Object);
    }

    [Fact]
    public async Task Init_PopulatesUpcomingDateRanges()
    {
        // Arrange
        SetupDefaultMocks();

        // Act
        await sut.Init();

        // Assert
        Assert.True(sut.UpcomingDateRanges.Count > 0);
        Assert.Contains(DateRanges.ThisMonth, sut.UpcomingDateRanges);
    }

    [Fact]
    public async Task Init_PopulatesCompletedDateRanges()
    {
        // Arrange
        SetupDefaultMocks();

        // Act
        await sut.Init();

        // Assert
        Assert.True(sut.CompletedDateRanges.Count > 0);
        Assert.Contains(DateRanges.LastMonth, sut.CompletedDateRanges);
    }

    [Fact]
    public async Task Init_PopulatesCreatedDateRanges()
    {
        // Arrange
        SetupDefaultMocks();

        // Act
        await sut.Init();

        // Assert
        Assert.True(sut.CreatedDateRanges.Count > 0);
        Assert.Contains(DateRanges.LastMonth, sut.CreatedDateRanges);
    }

    [Fact]
    public async Task Init_RefreshesStatistics()
    {
        // Arrange
        var stats = TestHelpers.CreateTestStats();
        mockStatsRestService
            .Setup(m => m.GetUserStatsAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(stats);
        SetupDefaultEventMock();
        SetupDefaultLitterReportMock();

        // Act
        await sut.Init();

        // Assert
        Assert.Equal(stats.TotalBags, sut.StatisticsViewModel.TotalBags);
        Assert.Equal(stats.TotalEvents, sut.StatisticsViewModel.TotalEvents);
        Assert.Equal(stats.TotalHours, sut.StatisticsViewModel.TotalHours);
        mockStatsRestService.Verify(
            m => m.GetUserStatsAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>()),
            Times.Once);
    }

    [Fact]
    public async Task Init_WithUpcomingEvents_SetsAreUpcomingEventsFound()
    {
        // Arrange
        var events = TestHelpers.CreateTestEvents(3);
        mockMobEventManager
            .Setup(m => m.GetUserEventsAsync(It.IsAny<EventFilter>(), It.IsAny<Guid>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(events.ToPaginatedList());
        SetupDefaultStatsMock();
        SetupDefaultLitterReportMock();

        // Act
        await sut.Init();

        // Assert
        Assert.True(sut.AreUpcomingEventsFound);
        Assert.False(sut.AreNoUpcomingEventsFound);
    }

    [Fact]
    public async Task Init_WithNoUpcomingEvents_SetsAreNoUpcomingEventsFound()
    {
        // Arrange
        mockMobEventManager
            .Setup(m => m.GetUserEventsAsync(It.IsAny<EventFilter>(), It.IsAny<Guid>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new PaginatedList<Event>());
        SetupDefaultStatsMock();
        SetupDefaultLitterReportMock();

        // Act
        await sut.Init();

        // Assert
        Assert.Empty(sut.UpcomingEvents);
        Assert.False(sut.AreUpcomingEventsFound);
        Assert.True(sut.AreNoUpcomingEventsFound);
    }

    [Fact]
    public async Task Init_WithCompletedEvents_SetsAreCompletedEventsFound()
    {
        // Arrange
        var events = TestHelpers.CreateTestEvents(2);
        mockMobEventManager
            .Setup(m => m.GetUserEventsAsync(It.IsAny<EventFilter>(), It.IsAny<Guid>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(events.ToPaginatedList());
        SetupDefaultStatsMock();
        SetupDefaultLitterReportMock();

        // Act
        await sut.Init();

        // Assert
        Assert.True(sut.AreCompletedEventsFound);
        Assert.False(sut.AreNoCompletedEventsFound);
    }

    [Fact]
    public async Task Init_RefreshesLitterReports()
    {
        // Arrange
        SetupDefaultEventMock();
        SetupDefaultStatsMock();
        var reports = new PaginatedList<LitterReport>();
        reports.Add(TestHelpers.CreateTestLitterReport());
        reports.Add(TestHelpers.CreateTestLitterReport());

        mockLitterReportManager
            .Setup(m => m.GetLitterReportsAsync(It.IsAny<LitterReportFilter>(), It.IsAny<ImageSizeEnum>(), It.IsAny<bool>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(reports);

        // Act
        await sut.Init();

        // Assert
        Assert.Equal(2, sut.LitterReports.Count);
        mockLitterReportManager.Verify(
            m => m.GetLitterReportsAsync(It.IsAny<LitterReportFilter>(), It.IsAny<ImageSizeEnum>(), It.IsAny<bool>(), It.IsAny<CancellationToken>()),
            Times.AtLeastOnce);
    }

    [Fact]
    public async Task Init_WithLitterReports_SetsAreLitterReportsFound()
    {
        // Arrange
        SetupDefaultEventMock();
        SetupDefaultStatsMock();
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
    public async Task Init_WithNoLitterReports_SetsAreNoLitterReportsFound()
    {
        // Arrange
        SetupDefaultEventMock();
        SetupDefaultStatsMock();
        mockLitterReportManager
            .Setup(m => m.GetLitterReportsAsync(It.IsAny<LitterReportFilter>(), It.IsAny<ImageSizeEnum>(), It.IsAny<bool>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new PaginatedList<LitterReport>());

        // Act
        await sut.Init();

        // Assert
        Assert.Empty(sut.LitterReports);
        Assert.False(sut.AreLitterReportsFound);
        Assert.True(sut.AreNoLitterReportsFound);
    }

    [Fact]
    public async Task Init_SetsDefaultSelectedDateRanges()
    {
        // Arrange
        SetupDefaultMocks();

        // Act
        await sut.Init();

        // Assert
        Assert.Equal(DateRanges.ThisMonth, sut.SelectedUpcomingDateRange);
        Assert.Equal(DateRanges.LastMonth, sut.SelectedCompletedDateRange);
        Assert.Equal(DateRanges.LastMonth, sut.SelectedCreatedDateRange);
    }

    [Fact]
    public async Task Init_StatisticsIncludeLitterReportMetrics()
    {
        // Arrange
        var stats = TestHelpers.CreateTestStats();
        mockStatsRestService
            .Setup(m => m.GetUserStatsAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(stats);
        SetupDefaultEventMock();
        SetupDefaultLitterReportMock();

        // Act
        await sut.Init();

        // Assert
        Assert.Equal(stats.TotalLitterReportsSubmitted, sut.StatisticsViewModel.TotalLitterReportsSubmitted);
    }

    private void SetupDefaultMocks()
    {
        SetupDefaultEventMock();
        SetupDefaultStatsMock();
        SetupDefaultLitterReportMock();
    }

    private void SetupDefaultEventMock()
    {
        mockMobEventManager
            .Setup(m => m.GetUserEventsAsync(It.IsAny<EventFilter>(), It.IsAny<Guid>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new PaginatedList<Event>());
    }

    private void SetupDefaultStatsMock()
    {
        mockStatsRestService
            .Setup(m => m.GetUserStatsAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(TestHelpers.CreateTestStats());
    }

    private void SetupDefaultLitterReportMock()
    {
        mockLitterReportManager
            .Setup(m => m.GetLitterReportsAsync(It.IsAny<LitterReportFilter>(), It.IsAny<ImageSizeEnum>(), It.IsAny<bool>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new PaginatedList<LitterReport>());
    }
}
