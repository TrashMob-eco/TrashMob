namespace TrashMobMobile.Tests.ViewModels;

using Moq;
using TrashMob.Models;
using TrashMob.Models.Poco;
using TrashMobMobile.Services;
using TrashMobMobile.Tests.Helpers;
using TrashMobMobile.ViewModels;
using Xunit;

public class SearchEventsViewModelTests
{
    private readonly Mock<IMobEventManager> mockEventManager;
    private readonly Mock<INotificationService> mockNotificationService;
    private readonly Mock<IUserManager> mockUserManager;
    private readonly SearchEventsViewModel sut;

    public SearchEventsViewModelTests()
    {
        mockEventManager = new Mock<IMobEventManager>();
        mockNotificationService = new Mock<INotificationService>();
        mockUserManager = new Mock<IUserManager>();

        var testUser = TestHelpers.CreateTestUser();
        mockUserManager.Setup(m => m.CurrentUser).Returns(testUser);

        sut = new SearchEventsViewModel(
            mockEventManager.Object,
            mockNotificationService.Object,
            mockUserManager.Object);
    }

    [Fact]
    public async Task Init_SetsUpDateRanges()
    {
        // Arrange
        SetupDefaultEventManagerMocks();

        // Act
        await sut.Init();

        // Assert
        Assert.True(sut.UpcomingDateRanges.Count > 0);
        Assert.True(sut.CompletedDateRanges.Count > 0);
        Assert.True(sut.IsMapSelected);
        Assert.False(sut.IsListSelected);
        Assert.True(sut.IsUpcomingSelected);
        Assert.False(sut.IsCompletedSelected);
    }

    [Fact]
    public async Task Init_LoadsEventsAndLocations()
    {
        // Arrange
        var events = TestHelpers.CreateTestEvents(3);
        var locations = TestHelpers.CreateTestLocations();

        mockEventManager.Setup(m => m.GetFilteredEventsAsync(It.IsAny<EventFilter>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(events.ToPaginatedList());
        mockEventManager.Setup(m => m.GetLocationsByTimeRangeAsync(It.IsAny<DateTimeOffset>(), It.IsAny<DateTimeOffset>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(locations);

        // Act
        await sut.Init();

        // Assert
        Assert.True(sut.Events.Count > 0);
        Assert.True(sut.CountryCollection.Count > 0);
        Assert.True(sut.AreEventsFound);
        Assert.False(sut.AreNoEventsFound);
    }

    [Fact]
    public async Task Init_WithNoEvents_ShowsNoEventsFound()
    {
        // Arrange
        mockEventManager.Setup(m => m.GetFilteredEventsAsync(It.IsAny<EventFilter>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new PaginatedList<Event>());
        mockEventManager.Setup(m => m.GetLocationsByTimeRangeAsync(It.IsAny<DateTimeOffset>(), It.IsAny<DateTimeOffset>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new List<Location>());

        // Act
        await sut.Init();

        // Assert
        Assert.Empty(sut.Events);
        Assert.False(sut.AreEventsFound);
        Assert.True(sut.AreNoEventsFound);
    }

    [Fact]
    public async Task ViewUpcomingCommand_SetsUpcomingState()
    {
        // Arrange
        SetupDefaultEventManagerMocks();
        await sut.Init();

        // Act
        await sut.ViewUpcomingCommand.ExecuteAsync(null);

        // Assert
        Assert.True(sut.IsUpcomingSelected);
        Assert.False(sut.IsCompletedSelected);
    }

    [Fact]
    public async Task ViewCompletedCommand_SetsCompletedState()
    {
        // Arrange
        SetupDefaultEventManagerMocks();
        await sut.Init();

        // Act
        await sut.ViewCompletedCommand.ExecuteAsync(null);

        // Assert
        Assert.False(sut.IsUpcomingSelected);
        Assert.True(sut.IsCompletedSelected);
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
    public async Task CountryFilter_FiltersEventsToMatchingCountry()
    {
        // Arrange
        var usEvents = TestHelpers.CreateTestEvents(3, country: "United States");
        var canadaEvents = TestHelpers.CreateTestEvents(2, country: "Canada", region: "BC", city: "Vancouver");
        var allEvents = usEvents.Concat(canadaEvents).ToList();
        var locations = TestHelpers.CreateTestLocations();

        mockEventManager.Setup(m => m.GetFilteredEventsAsync(It.IsAny<EventFilter>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(allEvents.ToPaginatedList());
        mockEventManager.Setup(m => m.GetLocationsByTimeRangeAsync(It.IsAny<DateTimeOffset>(), It.IsAny<DateTimeOffset>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(locations);

        await sut.Init();
        var totalBeforeFilter = sut.Events.Count;

        // Act
        sut.SelectedCountry = "United States";

        // Assert
        Assert.Equal(3, sut.Events.Count);
        Assert.All(sut.Events, e => Assert.Equal("United States", e.Address.Country));
    }

    [Fact]
    public async Task RegionFilter_FiltersEventsToMatchingRegion()
    {
        // Arrange
        var waEvents = TestHelpers.CreateTestEvents(2, country: "United States", region: "WA", city: "Seattle");
        var orEvents = TestHelpers.CreateTestEvents(1, country: "United States", region: "OR", city: "Portland");
        var allEvents = waEvents.Concat(orEvents).ToList();

        var locations = new List<Location>
        {
            new() { Country = "United States", Region = "WA", City = "Seattle" },
            new() { Country = "United States", Region = "OR", City = "Portland" },
        };

        mockEventManager.Setup(m => m.GetFilteredEventsAsync(It.IsAny<EventFilter>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(allEvents.ToPaginatedList());
        mockEventManager.Setup(m => m.GetLocationsByTimeRangeAsync(It.IsAny<DateTimeOffset>(), It.IsAny<DateTimeOffset>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(locations);

        await sut.Init();

        sut.SelectedCountry = "United States";

        // Act
        sut.SelectedRegion = "WA";

        // Assert
        Assert.Equal(2, sut.Events.Count);
        Assert.All(sut.Events, e => Assert.Equal("WA", e.Address.Region));
    }

    [Fact]
    public async Task CityFilter_FiltersEventsToMatchingCity()
    {
        // Arrange
        var seattleEvents = TestHelpers.CreateTestEvents(2, country: "United States", region: "WA", city: "Seattle");
        var tacomaEvents = TestHelpers.CreateTestEvents(1, country: "United States", region: "WA", city: "Tacoma");
        var allEvents = seattleEvents.Concat(tacomaEvents).ToList();

        var locations = new List<Location>
        {
            new() { Country = "United States", Region = "WA", City = "Seattle" },
            new() { Country = "United States", Region = "WA", City = "Tacoma" },
        };

        mockEventManager.Setup(m => m.GetFilteredEventsAsync(It.IsAny<EventFilter>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(allEvents.ToPaginatedList());
        mockEventManager.Setup(m => m.GetLocationsByTimeRangeAsync(It.IsAny<DateTimeOffset>(), It.IsAny<DateTimeOffset>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(locations);

        await sut.Init();

        sut.SelectedCountry = "United States";
        sut.SelectedRegion = "WA";

        // Act
        sut.SelectedCity = "Seattle";

        // Assert
        Assert.Equal(2, sut.Events.Count);
        Assert.All(sut.Events, e => Assert.Equal("Seattle", e.Address.City));
    }

    [Fact]
    public async Task ClearSelectionsCommand_ResetsAllFilters()
    {
        // Arrange
        var events = TestHelpers.CreateTestEvents(5);
        var locations = TestHelpers.CreateTestLocations();

        mockEventManager.Setup(m => m.GetFilteredEventsAsync(It.IsAny<EventFilter>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(events.ToPaginatedList());
        mockEventManager.Setup(m => m.GetLocationsByTimeRangeAsync(It.IsAny<DateTimeOffset>(), It.IsAny<DateTimeOffset>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(locations);

        await sut.Init();
        sut.SelectedCountry = "United States";

        // Act
        await sut.ClearSelectionsCommand.ExecuteAsync(null);

        // Assert
        Assert.Null(sut.SelectedCountry);
        Assert.Null(sut.SelectedRegion);
        Assert.Null(sut.SelectedCity);
    }

    [Fact]
    public async Task CountrySelection_PopulatesRegionCollection()
    {
        // Arrange
        var events = TestHelpers.CreateTestEvents(3);
        var locations = new List<Location>
        {
            new() { Country = "United States", Region = "WA", City = "Seattle" },
            new() { Country = "United States", Region = "OR", City = "Portland" },
            new() { Country = "Canada", Region = "BC", City = "Vancouver" },
        };

        mockEventManager.Setup(m => m.GetFilteredEventsAsync(It.IsAny<EventFilter>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(events.ToPaginatedList());
        mockEventManager.Setup(m => m.GetLocationsByTimeRangeAsync(It.IsAny<DateTimeOffset>(), It.IsAny<DateTimeOffset>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(locations);

        await sut.Init();

        // Act
        sut.SelectedCountry = "United States";

        // Assert
        Assert.Equal(2, sut.RegionCollection.Count);
        Assert.Contains("WA", sut.RegionCollection);
        Assert.Contains("OR", sut.RegionCollection);
    }

    [Fact]
    public async Task RegionSelection_PopulatesCityCollection()
    {
        // Arrange
        var events = TestHelpers.CreateTestEvents(3);
        var locations = new List<Location>
        {
            new() { Country = "United States", Region = "WA", City = "Seattle" },
            new() { Country = "United States", Region = "WA", City = "Tacoma" },
            new() { Country = "United States", Region = "OR", City = "Portland" },
        };

        mockEventManager.Setup(m => m.GetFilteredEventsAsync(It.IsAny<EventFilter>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(events.ToPaginatedList());
        mockEventManager.Setup(m => m.GetLocationsByTimeRangeAsync(It.IsAny<DateTimeOffset>(), It.IsAny<DateTimeOffset>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(locations);

        await sut.Init();
        sut.SelectedCountry = "United States";

        // Act
        sut.SelectedRegion = "WA";

        // Assert
        Assert.Equal(2, sut.CityCollection.Count);
        Assert.Contains("Seattle", sut.CityCollection);
        Assert.Contains("Tacoma", sut.CityCollection);
    }

    [Fact]
    public async Task CountryChange_ClearsRegionAndCitySelections()
    {
        // Arrange
        var events = TestHelpers.CreateTestEvents(3);
        var locations = TestHelpers.CreateTestLocations();

        mockEventManager.Setup(m => m.GetFilteredEventsAsync(It.IsAny<EventFilter>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(events.ToPaginatedList());
        mockEventManager.Setup(m => m.GetLocationsByTimeRangeAsync(It.IsAny<DateTimeOffset>(), It.IsAny<DateTimeOffset>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(locations);

        await sut.Init();
        sut.SelectedCountry = "United States";
        sut.SelectedRegion = "WA";
        sut.SelectedCity = "Seattle";

        // Act
        sut.SelectedCountry = "Canada";

        // Assert
        Assert.Null(sut.SelectedRegion);
        Assert.Null(sut.SelectedCity);
    }

    private void SetupDefaultEventManagerMocks()
    {
        var events = TestHelpers.CreateTestEvents(3);
        var locations = TestHelpers.CreateTestLocations();

        mockEventManager.Setup(m => m.GetFilteredEventsAsync(It.IsAny<EventFilter>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(events.ToPaginatedList());
        mockEventManager.Setup(m => m.GetLocationsByTimeRangeAsync(It.IsAny<DateTimeOffset>(), It.IsAny<DateTimeOffset>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(locations);
    }
}
