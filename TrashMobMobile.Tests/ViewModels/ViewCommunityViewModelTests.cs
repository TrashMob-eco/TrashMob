namespace TrashMobMobile.Tests.ViewModels;

using Moq;
using TrashMob.Models;
using TrashMob.Models.Poco;
using TrashMobMobile.Services;
using TrashMobMobile.Tests.Helpers;
using TrashMobMobile.ViewModels;
using Xunit;

public class ViewCommunityViewModelTests
{
    private readonly Mock<ICommunityManager> mockCommunityManager;
    private readonly Mock<INotificationService> mockNotificationService;
    private readonly Mock<IUserManager> mockUserManager;
    private readonly ViewCommunityViewModel sut;
    private const string TestSlug = "test-community";

    public ViewCommunityViewModelTests()
    {
        mockCommunityManager = new Mock<ICommunityManager>();
        mockNotificationService = new Mock<INotificationService>();
        mockUserManager = new Mock<IUserManager>();

        var testUser = TestHelpers.CreateTestUser();
        mockUserManager.Setup(m => m.CurrentUser).Returns(testUser);

        sut = new ViewCommunityViewModel(
            mockCommunityManager.Object,
            mockNotificationService.Object,
            mockUserManager.Object);
    }

    [Fact]
    public async Task Init_SetsCommunityName()
    {
        // Arrange
        SetupDefaultMocks();

        // Act
        await sut.Init(TestSlug);

        // Assert
        Assert.Equal("Test Community", sut.CommunityName);
    }

    [Fact]
    public async Task Init_SetsTagline()
    {
        // Arrange
        SetupDefaultMocks();

        // Act
        await sut.Init(TestSlug);

        // Assert
        Assert.Equal("A great community", sut.Tagline);
    }

    [Fact]
    public async Task Init_SetsLocation()
    {
        // Arrange
        SetupDefaultMocks();

        // Act
        await sut.Init(TestSlug);

        // Assert
        Assert.Contains("Seattle", sut.Location);
        Assert.Contains("WA", sut.Location);
        Assert.Contains("United States", sut.Location);
    }

    [Fact]
    public async Task Init_SetsWebsite()
    {
        // Arrange
        SetupDefaultMocks();

        // Act
        await sut.Init(TestSlug);

        // Assert
        Assert.Equal("https://example.com", sut.Website);
    }

    [Fact]
    public async Task Init_HasWebsite_WhenWebsiteNotEmpty()
    {
        // Arrange
        SetupDefaultMocks();

        // Act
        await sut.Init(TestSlug);

        // Assert
        Assert.True(sut.HasWebsite);
    }

    [Fact]
    public async Task Init_SetsContactEmail()
    {
        // Arrange
        SetupDefaultMocks();

        // Act
        await sut.Init(TestSlug);

        // Assert
        Assert.Equal("info@example.com", sut.ContactEmail);
        Assert.True(sut.HasContactEmail);
    }

    [Fact]
    public async Task Init_LoadsStats()
    {
        // Arrange
        SetupDefaultMocks();

        // Act
        await sut.Init(TestSlug);

        // Assert
        Assert.Equal(1000, sut.Stats.TotalAttendees);
        Assert.Equal(200, sut.Stats.TotalEvents);
        Assert.Equal(5000, sut.Stats.TotalBags);
        Assert.Equal(3000, sut.Stats.TotalHours);
    }

    [Fact]
    public async Task Init_WithEvents_SetsAreEventsFound()
    {
        // Arrange
        var events = TestHelpers.CreateTestEvents(3);
        SetupDefaultMocks(events: events);

        // Act
        await sut.Init(TestSlug);

        // Assert
        Assert.True(sut.AreEventsFound);
        Assert.False(sut.AreNoEventsFound);
        Assert.Equal(3, sut.UpcomingEvents.Count);
    }

    [Fact]
    public async Task Init_WithNoEvents_SetsAreNoEventsFound()
    {
        // Arrange
        SetupDefaultMocks();

        // Act
        await sut.Init(TestSlug);

        // Assert
        Assert.False(sut.AreEventsFound);
        Assert.True(sut.AreNoEventsFound);
    }

    [Fact]
    public async Task Init_WithActiveTeams_SetsAreTeamsFound()
    {
        // Arrange
        var teams = CreateTestTeams(2, allActive: true);
        SetupDefaultMocks(teams: teams);

        // Act
        await sut.Init(TestSlug);

        // Assert
        Assert.True(sut.AreTeamsFound);
        Assert.False(sut.AreNoTeamsFound);
        Assert.Equal(2, sut.NearbyTeams.Count);
    }

    [Fact]
    public async Task Init_WithNoTeams_SetsAreNoTeamsFound()
    {
        // Arrange
        SetupDefaultMocks();

        // Act
        await sut.Init(TestSlug);

        // Assert
        Assert.False(sut.AreTeamsFound);
        Assert.True(sut.AreNoTeamsFound);
    }

    private void SetupDefaultMocks(
        List<Event>? events = null,
        List<Team>? teams = null)
    {
        var testPartner = new Partner
        {
            Id = Guid.NewGuid(),
            Name = "Test Community",
            City = "Seattle",
            Region = "WA",
            Country = "United States",
            Slug = TestSlug,
            Tagline = "A great community",
            Website = "https://example.com",
            ContactEmail = "info@example.com",
        };

        mockCommunityManager.Setup(m => m.GetCommunityBySlugAsync(It.IsAny<string>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(testPartner);

        mockCommunityManager.Setup(m => m.GetCommunityStatsAsync(It.IsAny<string>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(TestHelpers.CreateTestStats());

        mockCommunityManager.Setup(m => m.GetCommunityEventsAsync(It.IsAny<string>(), It.IsAny<bool>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(events ?? new List<Event>());

        mockCommunityManager.Setup(m => m.GetCommunityTeamsAsync(It.IsAny<string>(), It.IsAny<double>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(teams ?? new List<Team>());
    }

    private static List<Team> CreateTestTeams(int count, bool allActive = true)
    {
        var teams = new List<Team>();
        for (var i = 0; i < count; i++)
        {
            teams.Add(new Team
            {
                Id = Guid.NewGuid(),
                Name = $"Team {i + 1}",
                Description = $"Description for team {i + 1}",
                City = "Seattle",
                Region = "WA",
                Country = "United States",
                IsPublic = true,
                IsActive = allActive,
                Members = [],
            });
        }

        return teams;
    }
}
