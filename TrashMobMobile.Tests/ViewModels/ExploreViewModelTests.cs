namespace TrashMobMobile.Tests.ViewModels;

using Moq;
using TrashMob.Models;
using TrashMob.Models.Poco;
using TrashMobMobile.Services;
using TrashMobMobile.Tests.Helpers;
using TrashMobMobile.ViewModels;
using Xunit;

public class ExploreViewModelTests
{
    private readonly Mock<IMobEventManager> mockMobEventManager;
    private readonly Mock<ILitterReportManager> mockLitterReportManager;
    private readonly Mock<INotificationService> mockNotificationService;
    private readonly Mock<IUserManager> mockUserManager;
    private readonly ExploreViewModel sut;

    public ExploreViewModelTests()
    {
        mockMobEventManager = new Mock<IMobEventManager>();
        mockLitterReportManager = new Mock<ILitterReportManager>();
        mockNotificationService = new Mock<INotificationService>();
        mockUserManager = new Mock<IUserManager>();

        var testUser = TestHelpers.CreateTestUser();
        mockUserManager.Setup(m => m.CurrentUser).Returns(testUser);

        sut = new ExploreViewModel(
            mockMobEventManager.Object,
            mockLitterReportManager.Object,
            mockNotificationService.Object,
            mockUserManager.Object);
    }

    [Fact]
    public async Task Init_SetsMapSelected()
    {
        // Arrange
        SetupDefaultMocks();

        // Act
        await sut.Init();

        // Assert
        Assert.True(sut.IsMapSelected);
        Assert.False(sut.IsListSelected);
    }

    [Fact]
    public async Task Init_LoadsEvents()
    {
        // Arrange
        var events = TestHelpers.CreateTestEvents(3);
        var locations = TestHelpers.CreateTestLocations();

        mockMobEventManager
            .Setup(m => m.GetFilteredEventsAsync(It.IsAny<EventFilter>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(events.ToPaginatedList());
        mockMobEventManager
            .Setup(m => m.GetLocationsByTimeRangeAsync(It.IsAny<DateTimeOffset>(), It.IsAny<DateTimeOffset>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(locations);
        SetupDefaultLitterReportMock();

        // Act
        await sut.Init();

        // Assert
        Assert.Equal(3, sut.Events.Count);
        mockMobEventManager.Verify(
            m => m.GetFilteredEventsAsync(It.IsAny<EventFilter>(), It.IsAny<CancellationToken>()),
            Times.Once);
    }

    [Fact]
    public async Task Init_LoadsLitterReports()
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
        Assert.Equal(2, sut.LitterReports.Count);
    }

    [Fact]
    public async Task Init_WithData_SetsAreItemsFound()
    {
        // Arrange
        var events = TestHelpers.CreateTestEvents(2);
        var locations = TestHelpers.CreateTestLocations();

        mockMobEventManager
            .Setup(m => m.GetFilteredEventsAsync(It.IsAny<EventFilter>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(events.ToPaginatedList());
        mockMobEventManager
            .Setup(m => m.GetLocationsByTimeRangeAsync(It.IsAny<DateTimeOffset>(), It.IsAny<DateTimeOffset>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(locations);
        SetupDefaultLitterReportMock();

        // Act
        await sut.Init();

        // Assert
        Assert.True(sut.AreItemsFound);
        Assert.False(sut.AreNoItemsFound);
    }

    [Fact]
    public async Task Init_WithNoData_SetsAreNoItemsFound()
    {
        // Arrange
        SetupDefaultMocks();

        // Act
        await sut.Init();

        // Assert
        Assert.False(sut.AreItemsFound);
        Assert.True(sut.AreNoItemsFound);
    }

    [Fact]
    public async Task MapSelectedCommand_SetsCorrectState()
    {
        // Act
        await sut.MapSelectedCommand.ExecuteAsync(null);

        // Assert
        Assert.True(sut.IsMapSelected);
        Assert.False(sut.IsListSelected);
    }

    [Fact]
    public async Task ListSelectedCommand_SetsCorrectState()
    {
        // Act
        await sut.ListSelectedCommand.ExecuteAsync(null);

        // Assert
        Assert.False(sut.IsMapSelected);
        Assert.True(sut.IsListSelected);
    }

    [Fact]
    public async Task CountryFilter_FiltersEvents()
    {
        // Arrange
        var usEvents = TestHelpers.CreateTestEvents(3, country: "United States");
        var canadaEvents = TestHelpers.CreateTestEvents(2, country: "Canada", region: "BC", city: "Vancouver");
        var allEvents = usEvents.Concat(canadaEvents).ToList();
        var locations = TestHelpers.CreateTestLocations();

        mockMobEventManager
            .Setup(m => m.GetFilteredEventsAsync(It.IsAny<EventFilter>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(allEvents.ToPaginatedList());
        mockMobEventManager
            .Setup(m => m.GetLocationsByTimeRangeAsync(It.IsAny<DateTimeOffset>(), It.IsAny<DateTimeOffset>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(locations);
        SetupDefaultLitterReportMock();

        await sut.Init();

        // Act
        sut.SelectedCountry = "United States";

        // Assert
        Assert.Equal(3, sut.Events.Count);
        Assert.All(sut.Events, e => Assert.Equal("United States", e.Address.Country));
    }

    [Fact]
    public async Task Init_PopulatesCountryCollection()
    {
        // Arrange
        var events = TestHelpers.CreateTestEvents(1);
        var locations = TestHelpers.CreateTestLocations();

        mockMobEventManager
            .Setup(m => m.GetFilteredEventsAsync(It.IsAny<EventFilter>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(events.ToPaginatedList());
        mockMobEventManager
            .Setup(m => m.GetLocationsByTimeRangeAsync(It.IsAny<DateTimeOffset>(), It.IsAny<DateTimeOffset>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(locations);
        SetupDefaultLitterReportMock();

        // Act
        await sut.Init();

        // Assert
        Assert.True(sut.CountryCollection.Count > 0);
        Assert.Contains("United States", sut.CountryCollection);
        Assert.Contains("Canada", sut.CountryCollection);
    }

    [Fact]
    public async Task ClearSelectionsCommand_ResetsFilters()
    {
        // Arrange
        var events = TestHelpers.CreateTestEvents(3);
        var locations = TestHelpers.CreateTestLocations();

        mockMobEventManager
            .Setup(m => m.GetFilteredEventsAsync(It.IsAny<EventFilter>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(events.ToPaginatedList());
        mockMobEventManager
            .Setup(m => m.GetLocationsByTimeRangeAsync(It.IsAny<DateTimeOffset>(), It.IsAny<DateTimeOffset>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(locations);
        SetupDefaultLitterReportMock();

        await sut.Init();
        sut.SelectedCountry = "United States";

        // Act
        await sut.ClearSelectionsCommand.ExecuteAsync(null);

        // Assert
        Assert.Null(sut.SelectedCountry);
        Assert.Null(sut.SelectedRegion);
        Assert.Null(sut.SelectedCity);
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
        mockMobEventManager
            .Setup(m => m.GetLocationsByTimeRangeAsync(It.IsAny<DateTimeOffset>(), It.IsAny<DateTimeOffset>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new List<Location>());
    }

    private void SetupDefaultLitterReportMock()
    {
        mockLitterReportManager
            .Setup(m => m.GetLitterReportsAsync(It.IsAny<LitterReportFilter>(), It.IsAny<ImageSizeEnum>(), It.IsAny<bool>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new PaginatedList<LitterReport>());
    }
}
