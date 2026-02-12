namespace TrashMobMobile.Tests.ViewModels;

using Moq;
using TrashMob.Models;
using TrashMob.Models.Poco;
using TrashMobMobile.Services;
using TrashMobMobile.Tests.Helpers;
using TrashMobMobile.ViewModels;
using Xunit;

public class ViewEventSummaryViewModelTests
{
    private readonly Mock<IMobEventManager> mockMobEventManager;
    private readonly Mock<IPickupLocationManager> mockPickupLocationManager;
    private readonly Mock<IEventAttendeeRouteRestService> mockEventAttendeeRouteRestService;
    private readonly Mock<INotificationService> mockNotificationService;
    private readonly Mock<IUserManager> mockUserManager;
    private readonly ViewEventSummaryViewModel sut;
    private readonly Guid testUserId;

    public ViewEventSummaryViewModelTests()
    {
        mockMobEventManager = new Mock<IMobEventManager>();
        mockPickupLocationManager = new Mock<IPickupLocationManager>();
        mockEventAttendeeRouteRestService = new Mock<IEventAttendeeRouteRestService>();
        mockNotificationService = new Mock<INotificationService>();
        mockUserManager = new Mock<IUserManager>();

        var testUser = TestHelpers.CreateTestUser();
        testUserId = testUser.Id;
        mockUserManager.Setup(m => m.CurrentUser).Returns(testUser);

        sut = new ViewEventSummaryViewModel(
            mockMobEventManager.Object,
            mockPickupLocationManager.Object,
            mockEventAttendeeRouteRestService.Object,
            mockNotificationService.Object,
            mockUserManager.Object);
    }

    [Fact]
    public async Task Init_LoadsEventViewModel()
    {
        // Arrange
        var testEvent = TestHelpers.CreateTestEvent(createdByUserId: testUserId);
        SetupDefaultMocks(testEvent);

        // Act
        await sut.Init(testEvent.Id, () => { });

        // Assert
        Assert.Equal(testEvent.Name, sut.EventViewModel.Name);
    }

    [Fact]
    public async Task Init_WhenUserIsLead_EnablesEditEventSummary()
    {
        // Arrange
        var testEvent = TestHelpers.CreateTestEvent(createdByUserId: testUserId);
        SetupDefaultMocks(testEvent);

        // Act
        await sut.Init(testEvent.Id, () => { });

        // Assert
        Assert.True(sut.EnableEditEventSummary);
    }

    [Fact]
    public async Task Init_WhenUserIsNotLead_DisablesEditEventSummary()
    {
        // Arrange
        var otherUserId = Guid.NewGuid();
        var testEvent = TestHelpers.CreateTestEvent(createdByUserId: otherUserId);
        SetupDefaultMocks(testEvent);

        // Act
        await sut.Init(testEvent.Id, () => { });

        // Assert
        Assert.False(sut.EnableEditEventSummary);
    }

    [Fact]
    public async Task Init_WithNoPickupLocations_CollectionIsEmpty()
    {
        // Arrange
        var testEvent = TestHelpers.CreateTestEvent(createdByUserId: testUserId);
        SetupDefaultMocks(testEvent);

        // Act
        await sut.Init(testEvent.Id, () => { });

        // Assert
        Assert.Empty(sut.PickupLocations);
    }

    [Fact]
    public async Task Init_SetsMapSelected()
    {
        // Arrange
        var testEvent = TestHelpers.CreateTestEvent(createdByUserId: testUserId);
        SetupDefaultMocks(testEvent);

        // Act
        await sut.Init(testEvent.Id, () => { });

        // Assert
        Assert.True(sut.IsMapSelected);
        Assert.False(sut.IsListSelected);
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

    private void SetupDefaultMocks(Event testEvent)
    {
        mockMobEventManager.Setup(m => m.GetEventAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(testEvent);

        mockMobEventManager.Setup(m => m.GetEventSummaryAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((EventSummary)null!);

        mockPickupLocationManager.Setup(m => m.GetPickupLocationsAsync(It.IsAny<Guid>(), It.IsAny<ImageSizeEnum>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new List<PickupLocationImage>());

        mockEventAttendeeRouteRestService.Setup(m => m.GetEventAttendeeRoutesForEventAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new List<DisplayEventAttendeeRoute>());
    }
}
