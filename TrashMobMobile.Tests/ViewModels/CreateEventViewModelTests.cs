namespace TrashMobMobile.Tests.ViewModels;

using Moq;
using TrashMob.Models;
using TrashMob.Models.Poco;
using TrashMobMobile.Services;
using TrashMobMobile.Tests.Helpers;
using TrashMobMobile.ViewModels;
using Xunit;

public class CreateEventViewModelTests
{
    private readonly Mock<IMobEventManager> mockEventManager;
    private readonly Mock<IEventTypeRestService> mockEventTypeRestService;
    private readonly Mock<IMapRestService> mockMapRestService;
    private readonly Mock<INotificationService> mockNotificationService;
    private readonly Mock<IEventPartnerLocationServiceRestService> mockEventPartnerLocationServiceRestService;
    private readonly Mock<ILitterReportManager> mockLitterReportManager;
    private readonly Mock<IEventLitterReportManager> mockEventLitterReportManager;
    private readonly Mock<IUserManager> mockUserManager;
    private readonly Mock<ITeamManager> mockTeamManager;
    private readonly CreateEventViewModel sut;

    private readonly List<EventType> defaultEventTypes =
    [
        new EventType { Id = 1, Name = "Cleanup", DisplayOrder = 1 },
        new EventType { Id = 2, Name = "Beautification", DisplayOrder = 2 },
    ];

    public CreateEventViewModelTests()
    {
        mockEventManager = new Mock<IMobEventManager>();
        mockEventTypeRestService = new Mock<IEventTypeRestService>();
        mockMapRestService = new Mock<IMapRestService>();
        mockNotificationService = new Mock<INotificationService>();
        mockEventPartnerLocationServiceRestService = new Mock<IEventPartnerLocationServiceRestService>();
        mockLitterReportManager = new Mock<ILitterReportManager>();
        mockEventLitterReportManager = new Mock<IEventLitterReportManager>();
        mockUserManager = new Mock<IUserManager>();
        mockTeamManager = new Mock<ITeamManager>();

        var testUser = TestHelpers.CreateTestUser();
        mockUserManager.Setup(m => m.CurrentUser).Returns(testUser);

        sut = new CreateEventViewModel(
            mockEventManager.Object,
            mockEventTypeRestService.Object,
            mockMapRestService.Object,
            mockNotificationService.Object,
            mockEventPartnerLocationServiceRestService.Object,
            mockLitterReportManager.Object,
            mockEventLitterReportManager.Object,
            mockUserManager.Object,
            mockTeamManager.Object);

        // Set up Steps array with mock IContentView objects (5 steps in the wizard)
        var mockSteps = new IContentView[5];
        for (var i = 0; i < 5; i++)
        {
            mockSteps[i] = new Mock<IContentView>().Object;
        }

        sut.Steps = mockSteps;
    }

    private void SetupDefaultMocks()
    {
        mockEventTypeRestService.Setup(m => m.GetEventTypesAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(defaultEventTypes);

        mockEventPartnerLocationServiceRestService
            .Setup(m => m.GetEventPartnerLocationsAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new List<DisplayEventPartnerLocation>());

        mockTeamManager.Setup(m => m.GetMyTeamsAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(new List<Team>());
    }

    [Fact]
    public async Task Init_WithNoLitterReport_SetsDefaultEventName()
    {
        // Arrange
        SetupDefaultMocks();

        // Act
        await sut.Init(null);

        // Assert
        Assert.Equal("New Event", sut.EventViewModel.Name);
    }

    [Fact]
    public async Task Init_WithNoLitterReport_SetsDefaultEventTime()
    {
        // Arrange
        SetupDefaultMocks();

        // Act
        await sut.Init(null);

        // Assert
        Assert.Equal(TimeSpan.FromHours(9), sut.EventViewModel.EventTime);
    }

    [Fact]
    public async Task Init_LoadsEventTypes()
    {
        // Arrange
        SetupDefaultMocks();

        // Act
        await sut.Init(null);

        // Assert
        Assert.Equal(2, sut.ETypes.Count);
        Assert.Contains("Cleanup", sut.ETypes);
        Assert.Contains("Beautification", sut.ETypes);
    }

    [Fact]
    public async Task Init_WithLitterReportId_SetsAddressFromLitterReport()
    {
        // Arrange
        SetupDefaultMocks();

        var litterReportId = Guid.NewGuid();
        var litterReport = TestHelpers.CreateTestLitterReport(litterReportId);
        litterReport.LitterImages =
        [
            new LitterImage
            {
                City = "Portland",
                Country = "United States",
                Region = "OR",
                Latitude = 45.5155,
                Longitude = -122.6789,
                PostalCode = "97201",
                StreetAddress = "456 Oak Ave",
            },
        ];

        mockLitterReportManager
            .Setup(m => m.GetLitterReportAsync(litterReportId, ImageSizeEnum.Thumb, It.IsAny<CancellationToken>()))
            .ReturnsAsync(litterReport);

        // Act
        await sut.Init(litterReportId);

        // Assert
        Assert.Equal("Portland", sut.EventViewModel.Address.City);
        Assert.Equal("United States", sut.EventViewModel.Address.Country);
        Assert.Equal(45.5155, sut.EventViewModel.Address.Latitude);
        Assert.Equal(-122.6789, sut.EventViewModel.Address.Longitude);
    }

    [Fact]
    public async Task SetCurrentStep_Forward_IncrementsStep()
    {
        // Arrange
        SetupDefaultMocks();
        await sut.Init(null);

        Assert.Equal(0, sut.CurrentStep);

        // Act
        await sut.SetCurrentStep(CreateEventViewModel.StepType.Forward);

        // Assert
        Assert.Equal(1, sut.CurrentStep);
    }

    [Fact]
    public async Task SetCurrentStep_Backward_DecrementsStep()
    {
        // Arrange
        SetupDefaultMocks();
        await sut.Init(null);

        // Move to step 1 first
        await sut.SetCurrentStep(CreateEventViewModel.StepType.Forward);
        Assert.Equal(1, sut.CurrentStep);

        // Act
        await sut.SetCurrentStep(CreateEventViewModel.StepType.Backward);

        // Assert
        Assert.Equal(0, sut.CurrentStep);
    }

    [Fact]
    public async Task SetCurrentStep_Backward_AtStepZero_DoesNotDecrement()
    {
        // Arrange
        SetupDefaultMocks();
        await sut.Init(null);

        Assert.Equal(0, sut.CurrentStep);

        // Act
        await sut.SetCurrentStep(CreateEventViewModel.StepType.Backward);

        // Assert
        Assert.Equal(0, sut.CurrentStep);
    }

    [Fact]
    public async Task CanGoBack_AtStepZero_IsFalse()
    {
        // Arrange
        SetupDefaultMocks();
        await sut.Init(null);

        // Assert
        Assert.False(sut.CanGoBack);
    }

    [Fact]
    public async Task CanGoBack_AtStepOne_IsTrue()
    {
        // Arrange
        SetupDefaultMocks();
        await sut.Init(null);

        // Act
        await sut.SetCurrentStep(CreateEventViewModel.StepType.Forward);

        // Assert
        Assert.Equal(1, sut.CurrentStep);
        Assert.True(sut.CanGoBack);
    }

    [Fact]
    public async Task ValidateCurrentStep_Step0_ValidInput_SetsIsStepValidTrue()
    {
        // Arrange
        SetupDefaultMocks();
        await sut.Init(null);

        // Act - Init sets defaults that should make step 0 valid:
        // Name = "New Event", EventDate = tomorrow, SelectedEventType = "Cleanup",
        // Duration = 2 hours, but Description is empty by default
        sut.EventViewModel.Description = "A test event description";

        // Assert
        Assert.True(sut.IsStepValid);
    }

    [Fact]
    public async Task ValidateCurrentStep_Step0_MissingName_SetsIsStepValidFalse()
    {
        // Arrange
        SetupDefaultMocks();
        await sut.Init(null);

        sut.EventViewModel.Description = "A test event description";

        // Act
        sut.EventViewModel.Name = string.Empty;

        // Assert
        Assert.False(sut.IsStepValid);
    }

    [Fact]
    public async Task ValidateCurrentStep_Step0_MissingDescription_SetsIsStepValidFalse()
    {
        // Arrange
        SetupDefaultMocks();
        await sut.Init(null);

        // EventViewModel.Description defaults to empty string
        // Trigger validation by setting it explicitly
        sut.EventViewModel.Description = string.Empty;

        // Assert
        Assert.False(sut.IsStepValid);
    }

    [Fact]
    public async Task DurationHours_And_Minutes_SetCorrectly()
    {
        // Arrange
        SetupDefaultMocks();
        await sut.Init(null);

        // Act
        sut.EventViewModel.DurationHours = 4;
        sut.EventViewModel.DurationMinutes = 30;

        // Assert
        Assert.Equal(4, sut.EventViewModel.DurationHours);
        Assert.Equal(30, sut.EventViewModel.DurationMinutes);
    }

    [Fact]
    public async Task DisplayDuration_ReturnsFormattedString()
    {
        // Arrange
        SetupDefaultMocks();
        await sut.Init(null);

        // Act
        sut.EventViewModel.DurationHours = 2;
        sut.EventViewModel.DurationMinutes = 30;

        // Assert
        Assert.Equal("2h 30m", sut.EventViewModel.DisplayDuration);
    }

    [Fact]
    public async Task EventDuration_Over10Hours_SetsError()
    {
        // Arrange
        SetupDefaultMocks();
        await sut.Init(null);

        // Act - set duration to 11 hours
        sut.EventViewModel.DurationHours = 11;
        sut.EventViewModel.DurationMinutes = 0;

        // Assert
        Assert.Equal("Event maximum duration can only be 10 hours", sut.EventDurationError);
    }

    [Fact]
    public async Task EventDuration_Under1Hour_SetsError()
    {
        // Arrange
        SetupDefaultMocks();
        await sut.Init(null);

        // Act - set duration to 30 minutes
        sut.EventViewModel.DurationHours = 0;
        sut.EventViewModel.DurationMinutes = 30;

        // Assert
        Assert.Equal("Event minimum duration must be at least 1 hour", sut.EventDurationError);
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
    public async Task ChangeLocation_UpdatesEventAddress()
    {
        // Arrange
        SetupDefaultMocks();
        await sut.Init(null);

        var newLocation = new Microsoft.Maui.Devices.Sensors.Location(45.5155, -122.6789);
        var returnedAddress = new Address
        {
            City = "Portland",
            Country = "United States",
            Region = "OR",
            PostalCode = "97201",
            StreetAddress = "456 Oak Ave",
        };

        mockMapRestService.Setup(m => m.GetAddressAsync(45.5155, -122.6789, It.IsAny<CancellationToken>()))
            .ReturnsAsync(returnedAddress);

        // Act
        await sut.ChangeLocation(newLocation);

        // Assert
        Assert.Equal("Portland", sut.EventViewModel.Address.City);
        Assert.Equal("United States", sut.EventViewModel.Address.Country);
        Assert.Equal("OR", sut.EventViewModel.Address.Region);
        Assert.Equal("97201", sut.EventViewModel.Address.PostalCode);
        Assert.Equal("456 Oak Ave", sut.EventViewModel.Address.StreetAddress);
        Assert.Equal(45.5155, sut.EventViewModel.Address.Latitude);
        Assert.Equal(-122.6789, sut.EventViewModel.Address.Longitude);
    }

    [Fact]
    public async Task Init_SetsDefaultEventProperties()
    {
        // Arrange
        SetupDefaultMocks();

        // Act
        await sut.Init(null);

        // Assert
        Assert.Equal((int)EventVisibilityEnum.Public, sut.EventViewModel.EventVisibilityId);
        Assert.Equal(0, sut.EventViewModel.MaxNumberOfParticipants);
        Assert.Equal(2, sut.EventViewModel.DurationHours);
        Assert.Equal(0, sut.EventViewModel.DurationMinutes);
        Assert.Equal(1, sut.EventViewModel.EventTypeId);
    }

    [Fact]
    public async Task Init_SetsSelectedEventTypeToFirstByDisplayOrder()
    {
        // Arrange
        SetupDefaultMocks();

        // Act
        await sut.Init(null);

        // Assert
        Assert.Equal("Cleanup", sut.SelectedEventType);
    }

    [Fact]
    public async Task Init_SetsUserLocation()
    {
        // Arrange
        SetupDefaultMocks();

        // Act
        await sut.Init(null);

        // Assert
        Assert.NotNull(sut.UserLocation);
        Assert.Equal("Seattle", sut.UserLocation.City);
        Assert.Equal("WA", sut.UserLocation.Region);
        Assert.Equal("United States", sut.UserLocation.Country);
    }

    [Fact]
    public async Task Init_WithNoLitterReport_SetsAddressFromUserLocation()
    {
        // Arrange
        SetupDefaultMocks();

        // Act
        await sut.Init(null);

        // Assert
        Assert.Equal("Seattle", sut.EventViewModel.Address.City);
        Assert.Equal("WA", sut.EventViewModel.Address.Region);
        Assert.Equal("United States", sut.EventViewModel.Address.Country);
    }

    [Fact]
    public async Task Init_AddsEventToEventsCollection()
    {
        // Arrange
        SetupDefaultMocks();

        // Act
        await sut.Init(null);

        // Assert
        Assert.Single(sut.Events);
        Assert.Same(sut.EventViewModel, sut.Events[0]);
    }

    [Fact]
    public async Task SelectedVisibility_TeamOnly_ShowsTeamPicker()
    {
        // Arrange
        SetupDefaultMocks();
        await sut.Init(null);

        // Act
        sut.SelectedVisibility = "Team Only";

        // Assert
        Assert.True(sut.IsTeamPickerVisible);
    }

    [Fact]
    public async Task SelectedVisibility_Public_HidesTeamPicker()
    {
        // Arrange
        SetupDefaultMocks();
        await sut.Init(null);

        sut.SelectedVisibility = "Team Only";
        Assert.True(sut.IsTeamPickerVisible);

        // Act
        sut.SelectedVisibility = "Public";

        // Assert
        Assert.False(sut.IsTeamPickerVisible);
    }

    [Fact]
    public async Task SelectedVisibility_TeamOnly_SetsEventVisibilityId()
    {
        // Arrange
        SetupDefaultMocks();
        await sut.Init(null);

        // Act
        sut.SelectedVisibility = "Team Only";

        // Assert
        Assert.Equal((int)EventVisibilityEnum.TeamOnly, sut.EventViewModel.EventVisibilityId);
    }

    [Fact]
    public async Task Init_SetsDefaultEventTime()
    {
        // Arrange
        SetupDefaultMocks();

        // Act
        await sut.Init(null);

        // Assert
        Assert.Equal(TimeSpan.FromHours(9), sut.EventViewModel.EventTime);
    }

    [Fact]
    public async Task Init_LoadsTeamNames()
    {
        // Arrange
        var teams = new List<Team>
        {
            new Team { Id = Guid.NewGuid(), Name = "Green Warriors" },
            new Team { Id = Guid.NewGuid(), Name = "Eco Squad" },
        };

        mockEventTypeRestService.Setup(m => m.GetEventTypesAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(defaultEventTypes);

        mockEventPartnerLocationServiceRestService
            .Setup(m => m.GetEventPartnerLocationsAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new List<DisplayEventPartnerLocation>());

        mockTeamManager.Setup(m => m.GetMyTeamsAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(teams);

        // Act
        await sut.Init(null);

        // Assert
        Assert.Equal(2, sut.TeamNames.Count);
        Assert.Contains("Green Warriors", sut.TeamNames);
        Assert.Contains("Eco Squad", sut.TeamNames);
    }
}
