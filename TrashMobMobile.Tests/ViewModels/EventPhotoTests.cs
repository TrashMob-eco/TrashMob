namespace TrashMobMobile.Tests.ViewModels;

using Moq;
using TrashMob.Models;
using TrashMob.Models.Poco;
using TrashMobMobile.Services;
using TrashMobMobile.Tests.Helpers;
using TrashMobMobile.ViewModels;
using Xunit;

public class EventPhotoTests
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
    private readonly ViewEventViewModel sut;
    private readonly User testUser;
    private readonly Guid testEventId = Guid.NewGuid();

    public EventPhotoTests()
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

    [Fact]
    public async Task Init_LoadsPhotos()
    {
        // Arrange
        SetupDefaultMocks(photoCount: 3);

        // Act
        await sut.Init(testEventId, () => { });

        // Assert
        Assert.Equal(3, sut.EventPhotos.Count);
        Assert.True(sut.ArePhotosFound);
        Assert.False(sut.AreNoPhotosFound);
        Assert.Equal("3 photos", sut.PhotoCountDisplay);
    }

    [Fact]
    public async Task Init_WithNoPhotos_ShowsEmptyState()
    {
        // Arrange
        SetupDefaultMocks(photoCount: 0);

        // Act
        await sut.Init(testEventId, () => { });

        // Assert
        Assert.Empty(sut.EventPhotos);
        Assert.False(sut.ArePhotosFound);
        Assert.True(sut.AreNoPhotosFound);
        Assert.Equal("0 photos", sut.PhotoCountDisplay);
    }

    [Fact]
    public async Task Init_WithOnePhoto_ShowsSingularDisplay()
    {
        // Arrange
        SetupDefaultMocks(photoCount: 1);

        // Act
        await sut.Init(testEventId, () => { });

        // Assert
        Assert.Single(sut.EventPhotos);
        Assert.Equal("1 photo", sut.PhotoCountDisplay);
    }

    [Fact]
    public async Task Init_PhotosAreSortedByUploadDateDescending()
    {
        // Arrange
        var photos = new List<EventPhoto>
        {
            CreateTestPhoto(uploadedDate: DateTimeOffset.UtcNow.AddDays(-2)),
            CreateTestPhoto(uploadedDate: DateTimeOffset.UtcNow),
            CreateTestPhoto(uploadedDate: DateTimeOffset.UtcNow.AddDays(-1)),
        };
        SetupDefaultMocks(photos: photos);

        // Act
        await sut.Init(testEventId, () => { });

        // Assert
        Assert.Equal(3, sut.EventPhotos.Count);
        Assert.True(sut.EventPhotos[0].UploadedDate > sut.EventPhotos[1].UploadedDate);
        Assert.True(sut.EventPhotos[1].UploadedDate > sut.EventPhotos[2].UploadedDate);
    }

    [Fact]
    public async Task Init_UserIsLead_CanUploadPhoto()
    {
        // Arrange
        SetupDefaultMocks(isUserLead: true);

        // Act
        await sut.Init(testEventId, () => { });

        // Assert
        Assert.True(sut.CanUploadPhoto);
    }

    [Fact]
    public async Task Init_UserIsAttendee_CanUploadPhoto()
    {
        // Arrange
        SetupDefaultMocks(isUserAttending: true);

        // Act
        await sut.Init(testEventId, () => { });

        // Assert
        Assert.True(sut.CanUploadPhoto);
    }

    [Fact]
    public async Task Init_UserIsNotAttendeeOrLead_CannotUploadPhoto()
    {
        // Arrange
        SetupDefaultMocks(isUserLead: false, isUserAttending: false);

        // Act
        await sut.Init(testEventId, () => { });

        // Assert
        Assert.False(sut.CanUploadPhoto);
    }

    [Fact]
    public async Task Init_LeadCanDeleteAnyPhoto()
    {
        // Arrange
        var otherUserId = Guid.NewGuid();
        var photos = new List<EventPhoto>
        {
            CreateTestPhoto(uploadedByUserId: otherUserId),
        };
        SetupDefaultMocks(isUserLead: true, photos: photos);

        // Act
        await sut.Init(testEventId, () => { });

        // Assert
        Assert.Single(sut.EventPhotos);
        Assert.True(sut.EventPhotos[0].CanDelete);
    }

    [Fact]
    public async Task Init_UserCanDeleteOwnPhoto()
    {
        // Arrange
        var photos = new List<EventPhoto>
        {
            CreateTestPhoto(uploadedByUserId: testUser.Id),
        };
        SetupDefaultMocks(isUserLead: false, photos: photos);

        // Act
        await sut.Init(testEventId, () => { });

        // Assert
        Assert.Single(sut.EventPhotos);
        Assert.True(sut.EventPhotos[0].CanDelete);
    }

    [Fact]
    public async Task Init_UserCannotDeleteOthersPhoto()
    {
        // Arrange
        var otherUserId = Guid.NewGuid();
        var photos = new List<EventPhoto>
        {
            CreateTestPhoto(uploadedByUserId: otherUserId),
        };
        SetupDefaultMocks(isUserLead: false, photos: photos);

        // Act
        await sut.Init(testEventId, () => { });

        // Assert
        Assert.Single(sut.EventPhotos);
        Assert.False(sut.EventPhotos[0].CanDelete);
    }

    private EventPhoto CreateTestPhoto(
        Guid? id = null,
        Guid? uploadedByUserId = null,
        EventPhotoType photoType = EventPhotoType.During,
        DateTimeOffset? uploadedDate = null)
    {
        return new EventPhoto
        {
            Id = id ?? Guid.NewGuid(),
            EventId = testEventId,
            UploadedByUserId = uploadedByUserId ?? testUser.Id,
            ImageUrl = "https://example.com/photo.jpg",
            ThumbnailUrl = "https://example.com/thumb.jpg",
            PhotoType = photoType,
            Caption = "Test caption",
            UploadedDate = uploadedDate ?? DateTimeOffset.UtcNow,
        };
    }

    private void SetupDefaultMocks(
        int photoCount = 0,
        bool isUserLead = false,
        bool isUserAttending = false,
        List<EventPhoto>? photos = null)
    {
        var testEvent = TestHelpers.CreateTestEvent(
            id: testEventId,
            createdByUserId: isUserLead ? testUser.Id : Guid.NewGuid());

        mockMobEventManager
            .Setup(m => m.GetEventAsync(testEventId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(testEvent);

        var eventTypes = new List<EventType> { new() { Id = testEvent.EventTypeId, Name = "Cleanup" } };
        mockEventTypeRestService
            .Setup(m => m.GetEventTypesAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(eventTypes);

        mockMobEventManager
            .Setup(m => m.IsUserAttendingAsync(testEventId, testUser.Id, It.IsAny<CancellationToken>()))
            .ReturnsAsync(isUserAttending || isUserLead);

        var attendees = new List<DisplayUser>
        {
            new() { Id = testEvent.CreatedByUserId, UserName = "Lead" },
        };
        mockEventAttendeeRestService
            .Setup(m => m.GetEventAttendeesAsync(testEventId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(attendees);

        mockEventAttendeeRestService
            .Setup(m => m.GetEventLeadsAsync(testEventId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(new List<DisplayUser> { new() { Id = testEvent.CreatedByUserId } });

        mockEventPartnerLocationServiceRestService
            .Setup(m => m.GetEventPartnerLocationsAsync(testEventId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(new List<DisplayEventPartnerLocation>());

        mockEventLitterReportManager
            .Setup(m => m.GetEventLitterReportsAsync(testEventId, ImageSizeEnum.Thumb, It.IsAny<bool>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new List<FullEventLitterReport>());

        mockEventAttendeeRouteRestService
            .Setup(m => m.GetEventAttendeeRoutesForEventAsync(testEventId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(new List<DisplayEventAttendeeRoute>());

        var eventPhotos = photos ?? Enumerable.Range(0, photoCount)
            .Select(_ => CreateTestPhoto())
            .ToList();
        mockEventPhotoManager
            .Setup(m => m.GetEventPhotosAsync(testEventId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(eventPhotos);
    }
}
