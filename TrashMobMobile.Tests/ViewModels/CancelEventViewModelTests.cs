namespace TrashMobMobile.Tests.ViewModels;

using Moq;
using TrashMob.Models;
using TrashMobMobile.Models;
using TrashMobMobile.Services;
using TrashMobMobile.Tests.Helpers;
using TrashMobMobile.ViewModels;
using Xunit;

public class CancelEventViewModelTests
{
    private readonly Mock<IMobEventManager> mockMobEventManager;
    private readonly Mock<INotificationService> mockNotificationService;
    private readonly Mock<IUserManager> mockUserManager;
    private readonly CancelEventViewModel sut;

    private readonly User testUser;
    private readonly Event testEvent;

    public CancelEventViewModelTests()
    {
        mockMobEventManager = new Mock<IMobEventManager>();
        mockNotificationService = new Mock<INotificationService>();
        mockUserManager = new Mock<IUserManager>();

        testUser = TestHelpers.CreateTestUser();
        testEvent = TestHelpers.CreateTestEvent(createdByUserId: testUser.Id);

        mockUserManager.Setup(m => m.CurrentUser).Returns(testUser);

        sut = new CancelEventViewModel(
            mockMobEventManager.Object,
            mockNotificationService.Object,
            mockUserManager.Object);
    }

    private void SetupDefaultMocks(Event? mobEvent = null)
    {
        var evt = mobEvent ?? testEvent;
        mockMobEventManager.Setup(m => m.GetEventAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(evt);
    }

    [Fact]
    public async Task Init_LoadsEventViewModel()
    {
        // Arrange
        SetupDefaultMocks();

        // Act
        await sut.Init(testEvent.Id);

        // Assert
        Assert.NotNull(sut.EventViewModel);
        mockMobEventManager.Verify(m => m.GetEventAsync(testEvent.Id, It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task Init_SetsEventViewModelProperties()
    {
        // Arrange
        SetupDefaultMocks();

        // Act
        await sut.Init(testEvent.Id);

        // Assert
        Assert.Equal(testEvent.Id, sut.EventViewModel.Id);
        Assert.Equal(testEvent.Name, sut.EventViewModel.Name);
        Assert.Equal(testEvent.Description, sut.EventViewModel.Description);
        Assert.Equal(testEvent.EventDate, sut.EventViewModel.EventDate);
        Assert.Equal(testEvent.EventVisibilityId, sut.EventViewModel.EventVisibilityId);
    }

    [Fact]
    public async Task Init_SetsUserRoleForEvent()
    {
        // Arrange — testEvent.CreatedByUserId == testUser.Id, so user is the event lead
        SetupDefaultMocks();

        // Act
        await sut.Init(testEvent.Id);

        // Assert
        Assert.Equal("Lead", sut.EventViewModel.UserRoleForEvent);
    }

    [Fact]
    public async Task CancelEventCommand_CallsDeleteEventAsync()
    {
        // Arrange
        SetupDefaultMocks();
        await sut.Init(testEvent.Id);

        sut.EventViewModel.CancellationReason = "Weather conditions";

        var mockNavigation = new Mock<INavigation>();
        sut.Navigation = mockNavigation.Object;

        mockMobEventManager.Setup(m => m.DeleteEventAsync(It.IsAny<EventCancellationRequest>(), It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);

        // Act
        await sut.CancelEventCommand.ExecuteAsync(null);

        // Assert
        mockMobEventManager.Verify(
            m => m.DeleteEventAsync(
                It.Is<EventCancellationRequest>(r =>
                    r.EventId == testEvent.Id &&
                    r.CancellationReason == "Weather conditions"),
                It.IsAny<CancellationToken>()),
            Times.Once);
    }

    [Fact]
    public async Task CancelEventCommand_NotifiesUser()
    {
        // Arrange
        SetupDefaultMocks();
        await sut.Init(testEvent.Id);

        var mockNavigation = new Mock<INavigation>();
        sut.Navigation = mockNavigation.Object;

        mockMobEventManager.Setup(m => m.DeleteEventAsync(It.IsAny<EventCancellationRequest>(), It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);

        // Act
        await sut.CancelEventCommand.ExecuteAsync(null);

        // Assert
        mockNotificationService.Verify(
            m => m.Notify(It.Is<string>(s => s.Contains("cancelled")), It.IsAny<double>(), It.IsAny<CommunityToolkit.Maui.Core.ToastDuration>()),
            Times.Once);
    }

    [Fact]
    public async Task CancelEventCommand_NavigatesBack()
    {
        // Arrange
        SetupDefaultMocks();
        await sut.Init(testEvent.Id);

        var mockNavigation = new Mock<INavigation>();
        sut.Navigation = mockNavigation.Object;

        mockMobEventManager.Setup(m => m.DeleteEventAsync(It.IsAny<EventCancellationRequest>(), It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);

        // Act
        await sut.CancelEventCommand.ExecuteAsync(null);

        // Assert
        mockNavigation.Verify(m => m.PopAsync(), Times.Once);
    }

    [Fact]
    public async Task Init_SetsEventAddress()
    {
        // Arrange
        SetupDefaultMocks();

        // Act
        await sut.Init(testEvent.Id);

        // Assert
        Assert.Equal(testEvent.City, sut.EventViewModel.Address.City);
        Assert.Equal(testEvent.Country, sut.EventViewModel.Address.Country);
        Assert.Equal(testEvent.Region, sut.EventViewModel.Address.Region);
        Assert.Equal(testEvent.StreetAddress, sut.EventViewModel.Address.StreetAddress);
        Assert.Equal(testEvent.PostalCode, sut.EventViewModel.Address.PostalCode);
    }
}
