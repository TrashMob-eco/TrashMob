namespace TrashMobMobile.Tests.ViewModels;

using Moq;
using TrashMob.Models;
using TrashMob.Models.Poco;
using TrashMobMobile.Services;
using TrashMobMobile.Tests.Helpers;
using TrashMobMobile.ViewModels;
using Xunit;

public class EditEventViewModelTests
{
    private readonly Mock<IMobEventManager> mockMobEventManager;
    private readonly Mock<IEventTypeRestService> mockEventTypeRestService;
    private readonly Mock<IMapRestService> mockMapRestService;
    private readonly Mock<INotificationService> mockNotificationService;
    private readonly Mock<IUserManager> mockUserManager;
    private readonly Mock<IEventPartnerLocationServiceRestService> mockEventPartnerLocationServiceRestService;
    private readonly Mock<ILitterReportManager> mockLitterReportManager;
    private readonly Mock<IEventLitterReportManager> mockEventLitterReportManager;
    private readonly Mock<ITeamManager> mockTeamManager;
    private readonly EditEventViewModel sut;

    private readonly User testUser;
    private readonly Event testEvent;

    public EditEventViewModelTests()
    {
        mockMobEventManager = new Mock<IMobEventManager>();
        mockEventTypeRestService = new Mock<IEventTypeRestService>();
        mockMapRestService = new Mock<IMapRestService>();
        mockNotificationService = new Mock<INotificationService>();
        mockUserManager = new Mock<IUserManager>();
        mockEventPartnerLocationServiceRestService = new Mock<IEventPartnerLocationServiceRestService>();
        mockLitterReportManager = new Mock<ILitterReportManager>();
        mockEventLitterReportManager = new Mock<IEventLitterReportManager>();
        mockTeamManager = new Mock<ITeamManager>();

        testUser = TestHelpers.CreateTestUser();
        testEvent = TestHelpers.CreateTestEvent(createdByUserId: testUser.Id);

        mockUserManager.Setup(m => m.CurrentUser).Returns(testUser);

        sut = new EditEventViewModel(
            mockMobEventManager.Object,
            mockEventTypeRestService.Object,
            mockMapRestService.Object,
            mockNotificationService.Object,
            mockUserManager.Object,
            mockEventPartnerLocationServiceRestService.Object,
            mockLitterReportManager.Object,
            mockEventLitterReportManager.Object,
            mockTeamManager.Object);
    }

    private void SetupDefaultMocks(Event? mobEvent = null)
    {
        var evt = mobEvent ?? testEvent;

        mockMobEventManager.Setup(m => m.GetEventAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(evt);
        mockEventTypeRestService.Setup(m => m.GetEventTypesAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(new List<EventType>
            {
                new() { Id = 1, Name = "Cleanup" },
                new() { Id = 2, Name = "Beautification" },
            });
        mockEventPartnerLocationServiceRestService
            .Setup(m => m.GetEventPartnerLocationsAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new List<DisplayEventPartnerLocation>());
        mockLitterReportManager
            .Setup(m => m.GetLitterReportsAsync(It.IsAny<LitterReportFilter>(), It.IsAny<ImageSizeEnum>(), It.IsAny<bool>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new PaginatedList<LitterReport>());
        mockEventLitterReportManager
            .Setup(m => m.GetEventLitterReportsAsync(It.IsAny<Guid>(), It.IsAny<ImageSizeEnum>(), It.IsAny<bool>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new List<FullEventLitterReport>());
    }

    [Fact]
    public async Task Init_LoadsEvent()
    {
        // Arrange
        SetupDefaultMocks();

        // Act
        await sut.Init(testEvent.Id);

        // Assert
        Assert.NotNull(sut.EventViewModel);
        Assert.Equal(testEvent.Id, sut.EventViewModel.Id);
        mockMobEventManager.Verify(m => m.GetEventAsync(testEvent.Id, It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task Init_SetsSelectedEventType()
    {
        // Arrange
        SetupDefaultMocks();

        // Act
        await sut.Init(testEvent.Id);

        // Assert
        Assert.Equal("Cleanup", sut.SelectedEventType);
    }

    [Fact]
    public async Task Init_PopulatesETypes()
    {
        // Arrange
        SetupDefaultMocks();

        // Act
        await sut.Init(testEvent.Id);

        // Assert
        Assert.Equal(2, sut.ETypes.Count);
        Assert.Contains("Cleanup", sut.ETypes);
        Assert.Contains("Beautification", sut.ETypes);
    }

    [Fact]
    public async Task Init_LoadsPartners_WhenAvailable()
    {
        // Arrange
        var partners = new List<DisplayEventPartnerLocation>
        {
            new()
            {
                EventId = testEvent.Id,
                PartnerId = Guid.NewGuid(),
                PartnerLocationId = Guid.NewGuid(),
                PartnerLocationName = "Test Partner Location",
                PartnerLocationNotes = "Some notes",
                PartnerServicesEngaged = "Hauling",
            },
        };

        SetupDefaultMocks();
        mockEventPartnerLocationServiceRestService
            .Setup(m => m.GetEventPartnerLocationsAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(partners);

        // Act
        await sut.Init(testEvent.Id);

        // Assert
        Assert.Single(sut.AvailablePartners);
        Assert.True(sut.ArePartnersAvailable);
        Assert.False(sut.AreNoPartnersAvailable);
    }

    [Fact]
    public async Task Init_LoadsPartners_WhenNone_SetsAreNoPartnersAvailable()
    {
        // Arrange
        SetupDefaultMocks();

        // Act
        await sut.Init(testEvent.Id);

        // Assert
        Assert.Empty(sut.AvailablePartners);
        Assert.False(sut.ArePartnersAvailable);
        Assert.True(sut.AreNoPartnersAvailable);
    }

    [Fact]
    public async Task Init_LoadsLitterReports()
    {
        // Arrange
        var litterReport = TestHelpers.CreateTestLitterReport();
        var litterReports = new PaginatedList<LitterReport>();
        litterReports.Add(litterReport);

        SetupDefaultMocks();
        mockLitterReportManager
            .Setup(m => m.GetLitterReportsAsync(It.IsAny<LitterReportFilter>(), It.IsAny<ImageSizeEnum>(), It.IsAny<bool>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(litterReports);

        // Act
        await sut.Init(testEvent.Id);

        // Assert
        mockLitterReportManager.Verify(
            m => m.GetLitterReportsAsync(It.IsAny<LitterReportFilter>(), It.IsAny<ImageSizeEnum>(), It.IsAny<bool>(), It.IsAny<CancellationToken>()),
            Times.Once);
        mockEventLitterReportManager.Verify(
            m => m.GetEventLitterReportsAsync(It.IsAny<Guid>(), It.IsAny<ImageSizeEnum>(), It.IsAny<bool>(), It.IsAny<CancellationToken>()),
            Times.Once);
    }

    [Fact]
    public async Task ChangeLocation_UpdatesEventAddress()
    {
        // Arrange
        SetupDefaultMocks();
        await sut.Init(testEvent.Id);

        var newLocation = new Microsoft.Maui.Devices.Sensors.Location(48.8566, 2.3522);
        var newAddress = new Address
        {
            City = "Paris",
            Country = "France",
            PostalCode = "75001",
            Region = "Ile-de-France",
            StreetAddress = "1 Rue de Rivoli",
        };

        mockMapRestService.Setup(m => m.GetAddressAsync(48.8566, 2.3522, It.IsAny<CancellationToken>()))
            .ReturnsAsync(newAddress);

        // Act
        await sut.ChangeLocation(newLocation);

        // Assert
        Assert.Equal("Paris", sut.EventViewModel.Address.City);
        Assert.Equal("France", sut.EventViewModel.Address.Country);
        Assert.Equal("75001", sut.EventViewModel.Address.PostalCode);
        Assert.Equal("Ile-de-France", sut.EventViewModel.Address.Region);
        Assert.Equal("1 Rue de Rivoli", sut.EventViewModel.Address.StreetAddress);
        Assert.Equal(48.8566, sut.EventViewModel.Address.Latitude);
        Assert.Equal(2.3522, sut.EventViewModel.Address.Longitude);
    }

    [Fact]
    public async Task MapSelectedCommand_SetsCorrectState()
    {
        // Act
        await sut.MapSelectedCommand.ExecuteAsync(null);

        // Assert
        Assert.True(sut.IsLitterReportMapSelected);
        Assert.False(sut.IsLitterReportListSelected);
    }

    [Fact]
    public async Task ListSelectedCommand_SetsCorrectState()
    {
        // Act
        await sut.ListSelectedCommand.ExecuteAsync(null);

        // Assert
        Assert.False(sut.IsLitterReportMapSelected);
        Assert.True(sut.IsLitterReportListSelected);
    }

    [Fact]
    public async Task Init_SetsEventsCollection()
    {
        // Arrange
        SetupDefaultMocks();

        // Act
        await sut.Init(testEvent.Id);

        // Assert
        Assert.Single(sut.Events);
        Assert.Equal(testEvent.Id, sut.Events[0].Id);
    }

    [Fact]
    public async Task Init_SetsUserLocation()
    {
        // Arrange
        SetupDefaultMocks();

        // Act
        await sut.Init(testEvent.Id);

        // Assert
        Assert.NotNull(sut.UserLocation);
        Assert.Equal(testUser.City, sut.UserLocation.City);
        Assert.Equal(testUser.Country, sut.UserLocation.Country);
        Assert.Equal(testUser.Region, sut.UserLocation.Region);
    }

    [Fact]
    public async Task Init_SetsEventViewModelProperties()
    {
        // Arrange
        SetupDefaultMocks();

        // Act
        await sut.Init(testEvent.Id);

        // Assert
        Assert.Equal(testEvent.Name, sut.EventViewModel.Name);
        Assert.Equal(testEvent.Description, sut.EventViewModel.Description);
        Assert.Equal(testEvent.EventDate, sut.EventViewModel.EventDate);
        Assert.Equal(testEvent.EventTypeId, sut.EventViewModel.EventTypeId);
        Assert.Equal(testEvent.EventVisibilityId, sut.EventViewModel.EventVisibilityId);
        Assert.Equal(testEvent.MaxNumberOfParticipants, sut.EventViewModel.MaxNumberOfParticipants);
    }

    [Fact]
    public async Task Init_SetsLitterReportDefaultViewState()
    {
        // Arrange
        SetupDefaultMocks();

        // Act
        await sut.Init(testEvent.Id);

        // Assert — LoadLitterReports sets map selected by default
        Assert.True(sut.IsLitterReportMapSelected);
        Assert.False(sut.IsLitterReportListSelected);
    }
}
