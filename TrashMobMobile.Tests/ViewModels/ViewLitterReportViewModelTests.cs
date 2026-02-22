namespace TrashMobMobile.Tests.ViewModels;

using Moq;
using TrashMob.Models;
using TrashMob.Models.Poco;
using TrashMobMobile.Services;
using TrashMobMobile.Tests.Helpers;
using TrashMobMobile.ViewModels;
using Xunit;

public class ViewLitterReportViewModelTests
{
    private readonly Mock<ILitterReportManager> mockLitterReportManager;
    private readonly Mock<IEventLitterReportManager> mockEventLitterReportManager;
    private readonly Mock<INotificationService> mockNotificationService;
    private readonly Mock<IUserManager> mockUserManager;
    private readonly Mock<IWaiverManager> mockWaiverManager;
    private readonly ViewLitterReportViewModel sut;

    public ViewLitterReportViewModelTests()
    {
        mockLitterReportManager = new Mock<ILitterReportManager>();
        mockEventLitterReportManager = new Mock<IEventLitterReportManager>();
        mockNotificationService = new Mock<INotificationService>();
        mockUserManager = new Mock<IUserManager>();
        mockWaiverManager = new Mock<IWaiverManager>();

        var testUser = TestHelpers.CreateTestUser();
        App.CurrentUser = testUser;
        mockUserManager.Setup(m => m.CurrentUser).Returns(testUser);

        sut = new ViewLitterReportViewModel(
            mockLitterReportManager.Object,
            mockEventLitterReportManager.Object,
            mockNotificationService.Object,
            mockUserManager.Object,
            mockWaiverManager.Object);
    }

    [Fact]
    public async Task Init_LoadsLitterReport()
    {
        // Arrange
        var litterReportId = Guid.NewGuid();
        var testLitterReport = CreateLitterReportForCurrentUser();
        SetupGetLitterReport(testLitterReport);

        // Act
        await sut.Init(litterReportId);

        // Assert
        mockLitterReportManager.Verify(
            m => m.GetLitterReportAsync(litterReportId, ImageSizeEnum.Reduced, It.IsAny<CancellationToken>()),
            Times.Once);
    }

    [Fact]
    public async Task Init_SetsLitterReportViewModel()
    {
        // Arrange
        var testLitterReport = CreateLitterReportForCurrentUser();
        SetupGetLitterReport(testLitterReport);

        // Act
        await sut.Init(testLitterReport.Id);

        // Assert
        Assert.NotNull(sut.LitterReportViewModel);
        Assert.Equal(testLitterReport.Id, sut.LitterReportViewModel!.Id);
        Assert.Equal(testLitterReport.Name, sut.LitterReportViewModel.Name);
    }

    [Fact]
    public async Task Init_WhenUserIsCreator_AndStatusNew_CanDeleteIsTrue()
    {
        // Arrange
        var testLitterReport = CreateLitterReportForCurrentUser(statusId: (int)LitterReportStatusEnum.New);
        SetupGetLitterReport(testLitterReport);

        // Act
        await sut.Init(testLitterReport.Id);

        // Assert
        Assert.True(sut.CanDeleteLitterReport);
    }

    [Fact]
    public async Task Init_WhenUserIsNotCreator_CanDeleteIsFalse()
    {
        // Arrange
        var testLitterReport = TestHelpers.CreateTestLitterReport();
        testLitterReport.CreatedByUserId = Guid.NewGuid(); // Different user
        testLitterReport.LitterReportStatusId = (int)LitterReportStatusEnum.New;
        SetupGetLitterReport(testLitterReport);

        // Act
        await sut.Init(testLitterReport.Id);

        // Assert
        Assert.False(sut.CanDeleteLitterReport);
    }

    [Fact]
    public async Task Init_WhenUserIsCreator_AndStatusNew_CanEditIsTrue()
    {
        // Arrange
        var testLitterReport = CreateLitterReportForCurrentUser(statusId: (int)LitterReportStatusEnum.New);
        SetupGetLitterReport(testLitterReport);

        // Act
        await sut.Init(testLitterReport.Id);

        // Assert
        Assert.True(sut.CanEditLitterReport);
    }

    [Fact]
    public async Task Init_WhenUserIsCreator_AndStatusAssigned_CanEditIsTrue()
    {
        // Arrange
        var testLitterReport = CreateLitterReportForCurrentUser(statusId: (int)LitterReportStatusEnum.Assigned);
        SetupEventLitterReportMock(testLitterReport.Id);
        SetupGetLitterReport(testLitterReport);

        // Act
        await sut.Init(testLitterReport.Id);

        // Assert
        Assert.True(sut.CanEditLitterReport);
    }

    [Fact]
    public async Task Init_WhenStatusCleaned_CanEditIsFalse()
    {
        // Arrange
        var testLitterReport = CreateLitterReportForCurrentUser(statusId: (int)LitterReportStatusEnum.Cleaned);
        SetupGetLitterReport(testLitterReport);

        // Act
        await sut.Init(testLitterReport.Id);

        // Assert
        Assert.False(sut.CanEditLitterReport);
    }

    [Fact]
    public async Task Init_WhenStatusAssigned_IsAssignedToEventIsTrue()
    {
        // Arrange
        var testLitterReport = CreateLitterReportForCurrentUser(statusId: (int)LitterReportStatusEnum.Assigned);
        var eventId = Guid.NewGuid();
        SetupEventLitterReportMock(testLitterReport.Id, eventId);
        SetupGetLitterReport(testLitterReport);

        // Act
        await sut.Init(testLitterReport.Id);

        // Assert
        Assert.True(sut.IsAssignedToEvent);
        Assert.False(sut.IsNotAssignedToEvent);
        Assert.Equal(eventId, sut.EventIdAssignedTo);
    }

    [Fact]
    public async Task Init_WhenStatusNew_IsNotAssignedToEvent()
    {
        // Arrange
        var testLitterReport = CreateLitterReportForCurrentUser(statusId: (int)LitterReportStatusEnum.New);
        SetupGetLitterReport(testLitterReport);

        // Act
        await sut.Init(testLitterReport.Id);

        // Assert
        Assert.False(sut.IsAssignedToEvent);
        Assert.True(sut.IsNotAssignedToEvent);
    }

    [Fact]
    public async Task Init_PopulatesLitterImageViewModels()
    {
        // Arrange
        var testLitterReport = CreateLitterReportForCurrentUser();
        testLitterReport.LitterImages = new List<LitterImage>
        {
            CreateTestLitterImage(testLitterReport.Id),
            CreateTestLitterImage(testLitterReport.Id),
        };
        SetupGetLitterReport(testLitterReport);

        // Act
        await sut.Init(testLitterReport.Id);

        // Assert
        Assert.Equal(2, sut.LitterImageViewModels.Count);
    }

    [Fact]
    public async Task Init_WhenStatusNew_CanMarkCleanedIsTrue()
    {
        // Arrange
        var testLitterReport = CreateLitterReportForCurrentUser(statusId: (int)LitterReportStatusEnum.New);
        SetupGetLitterReport(testLitterReport);

        // Act
        await sut.Init(testLitterReport.Id);

        // Assert
        Assert.True(sut.CanMarkLitterReportCleaned);
    }

    [Fact]
    public async Task Init_WhenStatusCleaned_CanMarkCleanedIsFalse()
    {
        // Arrange
        var testLitterReport = CreateLitterReportForCurrentUser(statusId: (int)LitterReportStatusEnum.Cleaned);
        SetupGetLitterReport(testLitterReport);

        // Act
        await sut.Init(testLitterReport.Id);

        // Assert
        Assert.False(sut.CanMarkLitterReportCleaned);
    }

    private LitterReport CreateLitterReportForCurrentUser(int statusId = (int)LitterReportStatusEnum.New)
    {
        var testLitterReport = TestHelpers.CreateTestLitterReport();
        testLitterReport.CreatedByUserId = App.CurrentUser!.Id;
        testLitterReport.LitterReportStatusId = statusId;
        return testLitterReport;
    }

    private void SetupGetLitterReport(LitterReport litterReport)
    {
        mockLitterReportManager
            .Setup(m => m.GetLitterReportAsync(It.IsAny<Guid>(), It.IsAny<ImageSizeEnum>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(litterReport);
    }

    private void SetupEventLitterReportMock(Guid litterReportId, Guid? eventId = null)
    {
        var fullEventLitterReport = new FullEventLitterReport
        {
            EventId = eventId ?? Guid.NewGuid(),
            LitterReportId = litterReportId,
            LitterReport = new FullLitterReport
            {
                Id = litterReportId,
                Name = "Test",
                Description = "Test",
                LitterReportStatusId = (int)LitterReportStatusEnum.Assigned,
                LitterImages = [],
            },
        };

        mockEventLitterReportManager
            .Setup(m => m.GetEventLitterReportByLitterReportIdAsync(litterReportId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(fullEventLitterReport);
    }

    private static LitterImage CreateTestLitterImage(Guid litterReportId)
    {
        return new LitterImage
        {
            Id = Guid.NewGuid(),
            LitterReportId = litterReportId,
            Latitude = 47.6062,
            Longitude = -122.3321,
            City = "Seattle",
            Region = "WA",
            Country = "United States",
            PostalCode = "98101",
            StreetAddress = "123 Main St",
            AzureBlobURL = "https://example.blob.core.windows.net/image.jpg",
            CreatedByUserId = Guid.NewGuid(),
            LastUpdatedByUserId = Guid.NewGuid(),
            CreatedDate = DateTimeOffset.UtcNow,
            LastUpdatedDate = DateTimeOffset.UtcNow,
        };
    }
}
