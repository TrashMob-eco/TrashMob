namespace TrashMobMobile.Tests.ViewModels;

using Moq;
using TrashMob.Models;
using TrashMob.Models.Poco;
using TrashMobMobile.Services;
using TrashMobMobile.Tests.Helpers;
using TrashMobMobile.ViewModels;
using Xunit;

public class ViewEventViewModelTests
{
    private readonly Mock<IMobEventManager> mockMobEventManager;
    private readonly Mock<IEventTypeRestService> mockEventTypeRestService;
    private readonly Mock<IWaiverManager> mockWaiverManager;
    private readonly Mock<IEventAttendeeRestService> mockEventAttendeeRestService;
    private readonly Mock<IEventAttendeeRouteRestService> mockEventAttendeeRouteRestService;
    private readonly Mock<INotificationService> mockNotificationService;
    private readonly Mock<IEventLitterReportManager> mockEventLitterReportManager;
    private readonly Mock<IUserManager> mockUserManager;
    private readonly Mock<IEventPartnerLocationServiceRestService> mockEventPartnerLocationServiceRestService;
    private readonly Mock<ILitterReportManager> mockLitterReportManager;
    private readonly Mock<IEventPhotoManager> mockEventPhotoManager;
    private readonly User testUser;
    private readonly ViewEventViewModel sut;

    public ViewEventViewModelTests()
    {
        mockMobEventManager = new Mock<IMobEventManager>();
        mockEventTypeRestService = new Mock<IEventTypeRestService>();
        mockWaiverManager = new Mock<IWaiverManager>();
        mockEventAttendeeRestService = new Mock<IEventAttendeeRestService>();
        mockEventAttendeeRouteRestService = new Mock<IEventAttendeeRouteRestService>();
        mockNotificationService = new Mock<INotificationService>();
        mockEventLitterReportManager = new Mock<IEventLitterReportManager>();
        mockUserManager = new Mock<IUserManager>();
        mockEventPartnerLocationServiceRestService = new Mock<IEventPartnerLocationServiceRestService>();
        mockLitterReportManager = new Mock<ILitterReportManager>();
        mockEventPhotoManager = new Mock<IEventPhotoManager>();

        testUser = TestHelpers.CreateTestUser();
        mockUserManager.Setup(m => m.CurrentUser).Returns(testUser);

        sut = new ViewEventViewModel(
            mockMobEventManager.Object,
            mockEventTypeRestService.Object,
            mockWaiverManager.Object,
            mockEventAttendeeRestService.Object,
            mockEventAttendeeRouteRestService.Object,
            mockNotificationService.Object,
            mockEventLitterReportManager.Object,
            mockUserManager.Object,
            mockEventPartnerLocationServiceRestService.Object,
            mockLitterReportManager.Object,
            mockEventPhotoManager.Object);
    }

    private Event CreateFutureEvent(Guid? createdByUserId = null, int maxParticipants = 50)
    {
        return TestHelpers.CreateTestEvent(
            createdByUserId: createdByUserId ?? testUser.Id,
            eventDate: DateTimeOffset.UtcNow.AddDays(7));
    }

    private Event CreatePastEvent(Guid? createdByUserId = null)
    {
        return TestHelpers.CreateTestEvent(
            createdByUserId: createdByUserId ?? testUser.Id,
            eventDate: DateTimeOffset.UtcNow.AddDays(-7));
    }

    private void SetupMocks(Event testEvent, List<DisplayUser>? attendees = null, List<DisplayUser>? leads = null,
        bool isUserAttending = false)
    {
        attendees ??= new List<DisplayUser>();
        leads ??= new List<DisplayUser>();

        mockMobEventManager.Setup(m => m.GetEventAsync(It.IsAny<Guid>())).ReturnsAsync(testEvent);
        mockEventTypeRestService.Setup(m => m.GetEventTypesAsync())
            .ReturnsAsync(new List<EventType> { new() { Id = 1, Name = "Cleanup" } });
        mockEventAttendeeRestService.Setup(m => m.GetEventAttendeesAsync(It.IsAny<Guid>()))
            .ReturnsAsync(attendees);
        mockEventAttendeeRestService.Setup(m => m.GetEventLeadsAsync(It.IsAny<Guid>()))
            .ReturnsAsync(leads);
        mockMobEventManager.Setup(m => m.IsUserAttendingAsync(It.IsAny<Guid>(), It.IsAny<Guid>()))
            .ReturnsAsync(isUserAttending);
        mockEventPartnerLocationServiceRestService
            .Setup(m => m.GetEventPartnerLocationsAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new List<DisplayEventPartnerLocation>());
        mockEventLitterReportManager
            .Setup(m => m.GetEventLitterReportsAsync(It.IsAny<Guid>(), It.IsAny<ImageSizeEnum>()))
            .ReturnsAsync(new List<FullEventLitterReport>());
        mockEventPhotoManager.Setup(m => m.GetEventPhotosAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new List<EventPhoto>());
        mockEventAttendeeRouteRestService
            .Setup(m => m.GetEventAttendeeRoutesForEventAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new List<DisplayEventAttendeeRoute>());
    }

    // 1. Init_LoadsEventDetails
    [Fact]
    public async Task Init_LoadsEventDetails()
    {
        // Arrange
        var testEvent = CreateFutureEvent();
        SetupMocks(testEvent);

        // Act
        await sut.Init(testEvent.Id, () => { });

        // Assert
        Assert.Equal(testEvent.Id, sut.EventViewModel.Id);
        Assert.Equal(testEvent.Name, sut.EventViewModel.Name);
        Assert.Equal(testEvent.Description, sut.EventViewModel.Description);
        Assert.Single(sut.Events);
        Assert.Single(sut.Addresses);
    }

    // 2. Init_SetsSelectedEventType
    [Fact]
    public async Task Init_SetsSelectedEventType()
    {
        // Arrange
        var testEvent = CreateFutureEvent();
        SetupMocks(testEvent);

        // Act
        await sut.Init(testEvent.Id, () => { });

        // Assert
        Assert.Equal("Cleanup", sut.SelectedEventType);
    }

    // 3. Init_WhenUserIsLead_EnablesEditEvent
    [Fact]
    public async Task Init_WhenUserIsLead_EnablesEditEvent()
    {
        // Arrange
        var testEvent = CreateFutureEvent(createdByUserId: testUser.Id);
        SetupMocks(testEvent);

        // Act
        await sut.Init(testEvent.Id, () => { });

        // Assert
        Assert.True(sut.EnableEditEvent);
    }

    // 4. Init_WhenUserIsNotLead_DisablesEditEvent
    [Fact]
    public async Task Init_WhenUserIsNotLead_DisablesEditEvent()
    {
        // Arrange
        var otherUserId = Guid.NewGuid();
        var testEvent = CreateFutureEvent(createdByUserId: otherUserId);
        SetupMocks(testEvent);

        // Act
        await sut.Init(testEvent.Id, () => { });

        // Assert
        Assert.False(sut.EnableEditEvent);
    }

    // 5. Init_WhenEventCompleted_EnablesViewEventSummary
    [Fact]
    public async Task Init_WhenEventCompleted_EnablesViewEventSummary()
    {
        // Arrange - past event is completed (EventDate in the past)
        var testEvent = CreatePastEvent(createdByUserId: testUser.Id);
        SetupMocks(testEvent);

        // Act
        await sut.Init(testEvent.Id, () => { });

        // Assert
        Assert.True(sut.EnableViewEventSummary);
    }

    // 6. Init_WhenEventNotCompleted_DisablesViewEventSummary
    [Fact]
    public async Task Init_WhenEventNotCompleted_DisablesViewEventSummary()
    {
        // Arrange - future event is not completed
        var testEvent = CreateFutureEvent(createdByUserId: testUser.Id);
        SetupMocks(testEvent);

        // Act
        await sut.Init(testEvent.Id, () => { });

        // Assert
        Assert.False(sut.EnableViewEventSummary);
    }

    // 7. Init_SetsAttendeeCount_SinglePerson
    [Fact]
    public async Task Init_SetsAttendeeCount_SinglePerson()
    {
        // Arrange
        var testEvent = CreateFutureEvent();
        var attendees = new List<DisplayUser>
        {
            new() { Id = Guid.NewGuid(), UserName = "User1", MemberSince = DateTimeOffset.UtcNow.AddYears(-1) },
        };
        SetupMocks(testEvent, attendees: attendees);

        // Act
        await sut.Init(testEvent.Id, () => { });

        // Assert
        Assert.Equal("1 person is going!", sut.AttendeeCount);
    }

    // 8. Init_SetsAttendeeCount_MultiplePeople
    [Fact]
    public async Task Init_SetsAttendeeCount_MultiplePeople()
    {
        // Arrange
        var testEvent = CreateFutureEvent();
        var attendees = new List<DisplayUser>
        {
            new() { Id = Guid.NewGuid(), UserName = "User1", MemberSince = DateTimeOffset.UtcNow.AddYears(-1) },
            new() { Id = Guid.NewGuid(), UserName = "User2", MemberSince = DateTimeOffset.UtcNow.AddMonths(-6) },
            new() { Id = Guid.NewGuid(), UserName = "User3", MemberSince = DateTimeOffset.UtcNow.AddMonths(-3) },
        };
        SetupMocks(testEvent, attendees: attendees);

        // Act
        await sut.Init(testEvent.Id, () => { });

        // Assert
        Assert.Equal("3 people are going!", sut.AttendeeCount);
    }

    // 9. Init_WithMaxParticipants_ShowsSpotsLeft
    [Fact]
    public async Task Init_WithMaxParticipants_ShowsSpotsLeft()
    {
        // Arrange
        var testEvent = CreateFutureEvent();
        testEvent.MaxNumberOfParticipants = 10;
        var attendees = new List<DisplayUser>
        {
            new() { Id = Guid.NewGuid(), UserName = "User1", MemberSince = DateTimeOffset.UtcNow },
            new() { Id = Guid.NewGuid(), UserName = "User2", MemberSince = DateTimeOffset.UtcNow },
        };
        SetupMocks(testEvent, attendees: attendees);

        // Act
        await sut.Init(testEvent.Id, () => { });

        // Assert
        Assert.Equal("8 spot left!", sut.SpotsLeft);
    }

    // 10. Init_WithMaxParticipantsReached_ShowsFullMessage
    [Fact]
    public async Task Init_WithMaxParticipantsReached_ShowsFullMessage()
    {
        // Arrange
        var testEvent = CreateFutureEvent();
        testEvent.MaxNumberOfParticipants = 2;
        var attendees = new List<DisplayUser>
        {
            new() { Id = Guid.NewGuid(), UserName = "User1", MemberSince = DateTimeOffset.UtcNow },
            new() { Id = Guid.NewGuid(), UserName = "User2", MemberSince = DateTimeOffset.UtcNow },
        };
        SetupMocks(testEvent, attendees: attendees);

        // Act
        await sut.Init(testEvent.Id, () => { });

        // Assert
        Assert.Equal("We're sorry. This event is currently full.", sut.SpotsLeft);
    }

    // 11. Init_WithNoMaxParticipants_HidesSpotsLeft
    [Fact]
    public async Task Init_WithNoMaxParticipants_HidesSpotsLeft()
    {
        // Arrange
        var testEvent = CreateFutureEvent();
        testEvent.MaxNumberOfParticipants = 0;
        var attendees = new List<DisplayUser>
        {
            new() { Id = Guid.NewGuid(), UserName = "User1", MemberSince = DateTimeOffset.UtcNow },
        };
        SetupMocks(testEvent, attendees: attendees);

        // Act
        await sut.Init(testEvent.Id, () => { });

        // Assert
        Assert.Equal("", sut.SpotsLeft);
    }

    // 12. Init_LoadsPartners_WhenAvailable
    [Fact]
    public async Task Init_LoadsPartners_WhenAvailable()
    {
        // Arrange
        var testEvent = CreateFutureEvent();
        SetupMocks(testEvent);

        var partners = new List<DisplayEventPartnerLocation>
        {
            new()
            {
                EventId = testEvent.Id,
                PartnerId = Guid.NewGuid(),
                PartnerLocationId = Guid.NewGuid(),
                PartnerLocationName = "Partner Location 1",
                PartnerLocationNotes = "Some notes",
                PartnerServicesEngaged = "Hauling, Supplies",
            },
        };

        mockEventPartnerLocationServiceRestService
            .Setup(m => m.GetEventPartnerLocationsAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(partners);

        // Act
        await sut.Init(testEvent.Id, () => { });

        // Assert
        Assert.True(sut.ArePartnersAvailable);
        Assert.False(sut.AreNoPartnersAvailable);
        Assert.Single(sut.AvailablePartners);
        Assert.Equal("Partner Location 1", sut.AvailablePartners[0].PartnerLocationName);
    }

    // 13. Init_LoadsPhotos
    [Fact]
    public async Task Init_LoadsPhotos()
    {
        // Arrange
        var testEvent = CreateFutureEvent(createdByUserId: testUser.Id);
        SetupMocks(testEvent);

        var photos = new List<EventPhoto>
        {
            new()
            {
                Id = Guid.NewGuid(),
                EventId = testEvent.Id,
                ImageUrl = "https://example.com/photo1.jpg",
                ThumbnailUrl = "https://example.com/thumb1.jpg",
                PhotoType = EventPhotoType.Before,
                Caption = "Before cleanup",
                UploadedDate = DateTimeOffset.UtcNow,
                UploadedByUserId = testUser.Id,
            },
            new()
            {
                Id = Guid.NewGuid(),
                EventId = testEvent.Id,
                ImageUrl = "https://example.com/photo2.jpg",
                ThumbnailUrl = "https://example.com/thumb2.jpg",
                PhotoType = EventPhotoType.After,
                Caption = "After cleanup",
                UploadedDate = DateTimeOffset.UtcNow.AddMinutes(30),
                UploadedByUserId = testUser.Id,
            },
        };

        mockEventPhotoManager.Setup(m => m.GetEventPhotosAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(photos);

        // Act
        await sut.Init(testEvent.Id, () => { });

        // Assert
        Assert.True(sut.ArePhotosFound);
        Assert.False(sut.AreNoPhotosFound);
        Assert.Equal(2, sut.EventPhotos.Count);
        Assert.Equal("2 photos", sut.PhotoCountDisplay);
    }

    // 14. MapSelectedCommand_SetsCorrectState
    [Fact]
    public async Task MapSelectedCommand_SetsCorrectState()
    {
        // Act
        await sut.MapSelectedCommand.ExecuteAsync(null);

        // Assert
        Assert.True(sut.IsLitterReportMapSelected);
        Assert.False(sut.IsLitterReportListSelected);
    }

    // 15. ListSelectedCommand_SetsCorrectState
    [Fact]
    public async Task ListSelectedCommand_SetsCorrectState()
    {
        // Act
        await sut.ListSelectedCommand.ExecuteAsync(null);

        // Assert
        Assert.False(sut.IsLitterReportMapSelected);
        Assert.True(sut.IsLitterReportListSelected);
    }

    // 16. Init_SetsCanManageCoLeads_WhenUserIsLead
    [Fact]
    public async Task Init_SetsCanManageCoLeads_WhenUserIsLead()
    {
        // Arrange
        var testEvent = CreateFutureEvent(createdByUserId: testUser.Id);
        var leads = new List<DisplayUser>
        {
            new() { Id = testUser.Id, UserName = testUser.UserName, MemberSince = DateTimeOffset.UtcNow },
        };
        SetupMocks(testEvent, leads: leads);

        // Act
        await sut.Init(testEvent.Id, () => { });

        // Assert
        Assert.True(sut.CanManageCoLeads);
        Assert.Equal(1, sut.CoLeadCount);
        Assert.Equal("Co-leads: 1/5", sut.CoLeadCountDisplay);
    }

    // 17. Init_WhenUserAttending_DisablesRegister
    [Fact]
    public async Task Init_WhenUserAttending_DisablesRegister()
    {
        // Arrange - user is attending but not the lead
        var otherUserId = Guid.NewGuid();
        var testEvent = CreateFutureEvent(createdByUserId: otherUserId);
        SetupMocks(testEvent, isUserAttending: true);

        // Act
        await sut.Init(testEvent.Id, () => { });

        // Assert
        Assert.False(sut.EnableRegister);
    }

    // 18. Init_WhenUserNotAttending_EnablesRegister
    [Fact]
    public async Task Init_WhenUserNotAttending_EnablesRegister()
    {
        // Arrange - user is not attending and not the lead, future event
        var otherUserId = Guid.NewGuid();
        var testEvent = CreateFutureEvent(createdByUserId: otherUserId);
        SetupMocks(testEvent, isUserAttending: false);

        // Act
        await sut.Init(testEvent.Id, () => { });

        // Assert
        Assert.True(sut.EnableRegister);
    }
}
