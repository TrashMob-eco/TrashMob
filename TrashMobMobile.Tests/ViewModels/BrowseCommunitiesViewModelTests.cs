namespace TrashMobMobile.Tests.ViewModels;

using Moq;
using TrashMob.Models;
using TrashMobMobile.Services;
using TrashMobMobile.Tests.Helpers;
using TrashMobMobile.ViewModels;
using Xunit;

public class BrowseCommunitiesViewModelTests
{
    private readonly Mock<ICommunityManager> mockCommunityManager;
    private readonly Mock<INotificationService> mockNotificationService;
    private readonly Mock<IUserManager> mockUserManager;
    private readonly BrowseCommunitiesViewModel sut;

    public BrowseCommunitiesViewModelTests()
    {
        mockCommunityManager = new Mock<ICommunityManager>();
        mockNotificationService = new Mock<INotificationService>();
        mockUserManager = new Mock<IUserManager>();

        var testUser = TestHelpers.CreateTestUser();
        mockUserManager.Setup(m => m.CurrentUser).Returns(testUser);

        sut = new BrowseCommunitiesViewModel(
            mockCommunityManager.Object,
            mockNotificationService.Object,
            mockUserManager.Object);
    }

    [Fact]
    public async Task Init_WithCommunities_SetsAreCommunitiesFound()
    {
        // Arrange
        var partners = CreateTestPartners(2);
        mockCommunityManager.Setup(m => m.GetCommunitiesAsync(
                It.IsAny<double?>(), It.IsAny<double?>(), It.IsAny<double?>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(partners);

        // Act
        await sut.Init();

        // Assert
        Assert.True(sut.AreCommunitiesFound);
        Assert.False(sut.AreNoCommunitiesFound);
    }

    [Fact]
    public async Task Init_WithNoCommunities_SetsAreNoCommunitiesFound()
    {
        // Arrange
        mockCommunityManager.Setup(m => m.GetCommunitiesAsync(
                It.IsAny<double?>(), It.IsAny<double?>(), It.IsAny<double?>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new List<Partner>());

        // Act
        await sut.Init();

        // Assert
        Assert.False(sut.AreCommunitiesFound);
        Assert.True(sut.AreNoCommunitiesFound);
    }

    [Fact]
    public async Task Init_PopulatesCommunitiesCollection()
    {
        // Arrange
        var partners = CreateTestPartners(3);
        mockCommunityManager.Setup(m => m.GetCommunitiesAsync(
                It.IsAny<double?>(), It.IsAny<double?>(), It.IsAny<double?>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(partners);

        // Act
        await sut.Init();

        // Assert
        Assert.Equal(3, sut.Communities.Count);
    }

    [Fact]
    public async Task Init_WithCommunities_MapsToViewModels()
    {
        // Arrange
        var partners = new List<Partner>
        {
            new Partner
            {
                Id = Guid.NewGuid(),
                Name = "Seattle Community",
                City = "Seattle",
                Region = "WA",
                Country = "United States",
                Slug = "seattle",
            },
        };

        mockCommunityManager.Setup(m => m.GetCommunitiesAsync(
                It.IsAny<double?>(), It.IsAny<double?>(), It.IsAny<double?>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(partners);

        // Act
        await sut.Init();

        // Assert
        Assert.Single(sut.Communities);
        Assert.Equal("Seattle Community", sut.Communities[0].Name);
    }

    [Fact]
    public async Task Init_ClearsExistingCommunitiesBeforeReloading()
    {
        // Arrange - first load
        var partners1 = CreateTestPartners(2);
        mockCommunityManager.Setup(m => m.GetCommunitiesAsync(
                It.IsAny<double?>(), It.IsAny<double?>(), It.IsAny<double?>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(partners1);

        await sut.Init();
        Assert.Equal(2, sut.Communities.Count);

        // Arrange - second load with different count
        var partners2 = CreateTestPartners(1);
        mockCommunityManager.Setup(m => m.GetCommunitiesAsync(
                It.IsAny<double?>(), It.IsAny<double?>(), It.IsAny<double?>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(partners2);

        // Act
        await sut.Init();

        // Assert
        Assert.Single(sut.Communities);
    }

    private static List<Partner> CreateTestPartners(int count)
    {
        var partners = new List<Partner>();
        for (var i = 0; i < count; i++)
        {
            partners.Add(new Partner
            {
                Id = Guid.NewGuid(),
                Name = $"Community {i + 1}",
                City = "Seattle",
                Region = "WA",
                Country = "United States",
                Slug = $"community-{i + 1}",
            });
        }

        return partners;
    }
}
