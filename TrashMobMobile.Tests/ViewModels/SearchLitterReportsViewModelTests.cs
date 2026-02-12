namespace TrashMobMobile.Tests.ViewModels;

using Moq;
using TrashMob.Models;
using TrashMob.Models.Poco;
using TrashMobMobile.Services;
using TrashMobMobile.Tests.Helpers;
using TrashMobMobile.ViewModels;
using Xunit;

public class SearchLitterReportsViewModelTests
{
    private readonly Mock<ILitterReportManager> mockLitterReportManager;
    private readonly Mock<INotificationService> mockNotificationService;
    private readonly Mock<IUserManager> mockUserManager;
    private readonly SearchLitterReportsViewModel sut;

    public SearchLitterReportsViewModelTests()
    {
        mockLitterReportManager = new Mock<ILitterReportManager>();
        mockNotificationService = new Mock<INotificationService>();
        mockUserManager = new Mock<IUserManager>();

        var testUser = TestHelpers.CreateTestUser();
        mockUserManager.Setup(m => m.CurrentUser).Returns(testUser);

        sut = new SearchLitterReportsViewModel(
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
    public async Task Init_SetsNewSelected()
    {
        // Arrange
        SetupDefaultMocks();

        // Act
        await sut.Init();

        // Assert
        Assert.True(sut.IsNewSelected);
        Assert.False(sut.IsAssignedSelected);
        Assert.False(sut.IsCleanedSelected);
    }

    [Fact]
    public async Task Init_SetsUserLocation()
    {
        // Arrange
        SetupDefaultMocks();

        // Act
        await sut.Init();

        // Assert
        Assert.NotNull(sut.UserLocation);
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
    public async Task ViewNewCommand_SetsNewState()
    {
        // Arrange
        SetupDefaultMocks();

        // Act
        await sut.ViewNewCommand.ExecuteAsync(null);

        // Assert
        Assert.True(sut.IsNewSelected);
        Assert.False(sut.IsAssignedSelected);
        Assert.False(sut.IsCleanedSelected);
    }

    [Fact]
    public async Task ViewAssignedCommand_SetsAssignedState()
    {
        // Arrange
        SetupDefaultMocks();

        // Act
        await sut.ViewAssignedCommand.ExecuteAsync(null);

        // Assert
        Assert.False(sut.IsNewSelected);
        Assert.True(sut.IsAssignedSelected);
        Assert.False(sut.IsCleanedSelected);
    }

    [Fact]
    public async Task ViewCleanedCommand_SetsCleanedState()
    {
        // Arrange
        SetupDefaultMocks();

        // Act
        await sut.ViewCleanedCommand.ExecuteAsync(null);

        // Assert
        Assert.False(sut.IsNewSelected);
        Assert.False(sut.IsAssignedSelected);
        Assert.True(sut.IsCleanedSelected);
    }

    private void SetupDefaultMocks()
    {
        mockLitterReportManager.Setup(m => m.GetLocationsByTimeRangeAsync(
                It.IsAny<DateTimeOffset>(), It.IsAny<DateTimeOffset>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new List<Location>());

        mockLitterReportManager.Setup(m => m.GetLitterReportsAsync(
                It.IsAny<LitterReportFilter>(), It.IsAny<ImageSizeEnum>(), It.IsAny<bool>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new PaginatedList<LitterReport>());
    }
}
