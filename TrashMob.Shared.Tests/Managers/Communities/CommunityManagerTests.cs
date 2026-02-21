namespace TrashMob.Shared.Tests.Managers.Communities
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading;
    using System.Threading.Tasks;
    using Moq;
    using TrashMob.Models;
    using TrashMob.Shared.Managers.Communities;
    using TrashMob.Shared.Persistence.Interfaces;
    using TrashMob.Shared.Tests.Builders;
    using TrashMob.Shared.Tests.Fixtures;
    using Xunit;

    public class CommunityManagerTests
    {
        private readonly Mock<IKeyedRepository<Partner>> _partnerRepository;
        private readonly Mock<IKeyedRepository<Event>> _eventRepository;
        private readonly Mock<IKeyedRepository<Team>> _teamRepository;
        private readonly Mock<IKeyedRepository<LitterReport>> _litterReportRepository;
        private readonly Mock<IKeyedRepository<LitterImage>> _litterImageRepository;
        private readonly Mock<IBaseRepository<EventSummary>> _eventSummaryRepository;
        private readonly CommunityManager _sut;

        public CommunityManagerTests()
        {
            _partnerRepository = new Mock<IKeyedRepository<Partner>>();
            _eventRepository = new Mock<IKeyedRepository<Event>>();
            _teamRepository = new Mock<IKeyedRepository<Team>>();
            _litterReportRepository = new Mock<IKeyedRepository<LitterReport>>();
            _litterImageRepository = new Mock<IKeyedRepository<LitterImage>>();
            _eventSummaryRepository = new Mock<IBaseRepository<EventSummary>>();

            _sut = new CommunityManager(
                _partnerRepository.Object,
                _eventRepository.Object,
                _teamRepository.Object,
                _litterReportRepository.Object,
                _litterImageRepository.Object,
                _eventSummaryRepository.Object);
        }

        #region GetEnabledCommunitiesAsync

        [Fact]
        public async Task GetEnabledCommunitiesAsync_ReturnsOnlyEnabledActiveCommunities()
        {
            // Arrange
            var enabledCommunity = new PartnerBuilder()
                .WithName("Enabled Community")
                .WithHomePageEnabled()
                .AsActive()
                .Build();
            var disabledCommunity = new PartnerBuilder()
                .WithName("Disabled Community")
                .WithHomePageDisabled()
                .AsActive()
                .Build();
            var inactiveCommunity = new PartnerBuilder()
                .WithName("Inactive Community")
                .WithHomePageEnabled()
                .AsInactive()
                .Build();

            var partners = new List<Partner> { enabledCommunity, disabledCommunity, inactiveCommunity };
            _partnerRepository.SetupGet(partners);

            // Act
            var result = await _sut.GetEnabledCommunitiesAsync();

            // Assert
            var resultList = result.ToList();
            Assert.Single(resultList);
            Assert.Equal("Enabled Community", resultList[0].Name);
        }

        [Fact]
        public async Task GetEnabledCommunitiesAsync_RespectsDateRanges()
        {
            // Arrange
            var now = DateTimeOffset.UtcNow;
            var activeCommunity = new PartnerBuilder()
                .WithName("Active Community")
                .WithHomePageEnabled()
                .WithHomePageDates(now.AddDays(-30), now.AddDays(30))
                .AsActive()
                .Build();
            var expiredCommunity = new PartnerBuilder()
                .WithName("Expired Community")
                .WithHomePageEnabled()
                .WithHomePageDates(now.AddDays(-60), now.AddDays(-30))
                .AsActive()
                .Build();
            var futureCommunity = new PartnerBuilder()
                .WithName("Future Community")
                .WithHomePageEnabled()
                .WithHomePageDates(now.AddDays(30), now.AddDays(60))
                .AsActive()
                .Build();

            var partners = new List<Partner> { activeCommunity, expiredCommunity, futureCommunity };
            _partnerRepository.SetupGet(partners);

            // Act
            var result = await _sut.GetEnabledCommunitiesAsync();

            // Assert
            var resultList = result.ToList();
            Assert.Single(resultList);
            Assert.Equal("Active Community", resultList[0].Name);
        }

        [Fact]
        public async Task GetEnabledCommunitiesAsync_WithLocationFilter_ReturnsCommunitiesWithinRadius()
        {
            // Arrange
            // Seattle coordinates: 47.6062, -122.3321
            var seattleCommunity = new PartnerBuilder()
                .WithName("Seattle Community")
                .WithHomePageEnabled()
                .AsActive()
                .WithCoordinates(47.6062, -122.3321)
                .Build();

            // Portland coordinates: 45.5152, -122.6784 (~145 miles from Seattle)
            var portlandCommunity = new PartnerBuilder()
                .WithName("Portland Community")
                .WithHomePageEnabled()
                .AsActive()
                .WithCoordinates(45.5152, -122.6784)
                .Build();

            var partners = new List<Partner> { seattleCommunity, portlandCommunity };
            _partnerRepository.SetupGet(partners);

            // Act - 50 mile radius from Seattle
            var result = await _sut.GetEnabledCommunitiesAsync(47.6062, -122.3321, 50);

            // Assert
            var resultList = result.ToList();
            Assert.Single(resultList);
            Assert.Equal("Seattle Community", resultList[0].Name);
        }

        #endregion

        #region GetBySlugAsync

        [Fact]
        public async Task GetBySlugAsync_ReturnsCommunityWhenFound()
        {
            // Arrange
            var community = new PartnerBuilder()
                .WithName("Seattle Community")
                .WithSlug("seattle-wa")
                .WithHomePageEnabled()
                .AsActive()
                .Build();

            _partnerRepository.SetupGet(new List<Partner> { community });

            // Act
            var result = await _sut.GetBySlugAsync("seattle-wa");

            // Assert
            Assert.NotNull(result);
            Assert.Equal("Seattle Community", result.Name);
        }

        [Fact]
        public async Task GetBySlugAsync_IsCaseInsensitive()
        {
            // Arrange
            var community = new PartnerBuilder()
                .WithName("Seattle Community")
                .WithSlug("seattle-wa")
                .WithHomePageEnabled()
                .AsActive()
                .Build();

            _partnerRepository.SetupGet(new List<Partner> { community });

            // Act
            var result = await _sut.GetBySlugAsync("SEATTLE-WA");

            // Assert
            Assert.NotNull(result);
            Assert.Equal("Seattle Community", result.Name);
        }

        [Fact]
        public async Task GetBySlugAsync_ReturnsNullWhenNotEnabled()
        {
            // Arrange
            var community = new PartnerBuilder()
                .WithName("Disabled Community")
                .WithSlug("disabled")
                .WithHomePageDisabled()
                .AsActive()
                .Build();

            _partnerRepository.SetupGet(new List<Partner> { community });

            // Act
            var result = await _sut.GetBySlugAsync("disabled");

            // Assert
            Assert.Null(result);
        }

        [Fact]
        public async Task GetBySlugAsync_ReturnsNullWhenExpired()
        {
            // Arrange
            var now = DateTimeOffset.UtcNow;
            var community = new PartnerBuilder()
                .WithName("Expired Community")
                .WithSlug("expired")
                .WithHomePageEnabled()
                .WithHomePageDates(now.AddDays(-60), now.AddDays(-30))
                .AsActive()
                .Build();

            _partnerRepository.SetupGet(new List<Partner> { community });

            // Act
            var result = await _sut.GetBySlugAsync("expired");

            // Assert
            Assert.Null(result);
        }

        #endregion

        #region IsSlugAvailableAsync

        [Fact]
        public async Task IsSlugAvailableAsync_ReturnsTrueWhenSlugIsUnique()
        {
            // Arrange
            var existingCommunity = new PartnerBuilder()
                .WithSlug("existing-slug")
                .Build();

            _partnerRepository.SetupGet(new List<Partner> { existingCommunity });

            // Act
            var result = await _sut.IsSlugAvailableAsync("new-slug");

            // Assert
            Assert.True(result);
        }

        [Fact]
        public async Task IsSlugAvailableAsync_ReturnsFalseWhenSlugExists()
        {
            // Arrange
            var existingCommunity = new PartnerBuilder()
                .WithSlug("existing-slug")
                .Build();

            _partnerRepository.SetupGet(new List<Partner> { existingCommunity });

            // Act
            var result = await _sut.IsSlugAvailableAsync("existing-slug");

            // Assert
            Assert.False(result);
        }

        [Fact]
        public async Task IsSlugAvailableAsync_IsCaseInsensitive()
        {
            // Arrange
            var existingCommunity = new PartnerBuilder()
                .WithSlug("existing-slug")
                .Build();

            _partnerRepository.SetupGet(new List<Partner> { existingCommunity });

            // Act
            var result = await _sut.IsSlugAvailableAsync("EXISTING-SLUG");

            // Assert
            Assert.False(result);
        }

        [Fact]
        public async Task IsSlugAvailableAsync_ReturnsTrueWhenExcludingOwnId()
        {
            // Arrange
            var existingCommunity = new PartnerBuilder()
                .WithSlug("my-slug")
                .Build();

            _partnerRepository.SetupGet(new List<Partner> { existingCommunity });

            // Act - Check availability while updating the same partner
            var result = await _sut.IsSlugAvailableAsync("my-slug", existingCommunity.Id);

            // Assert
            Assert.True(result);
        }

        #endregion

        #region GetCommunityEventsAsync

        [Fact]
        public async Task GetCommunityEventsAsync_ReturnsEventsInCommunityLocation()
        {
            // Arrange
            var community = new PartnerBuilder()
                .WithName("Seattle Community")
                .WithSlug("seattle-wa")
                .WithLocation("Seattle", "WA", "United States")
                .WithHomePageEnabled()
                .AsActive()
                .Build();

            var seattleEvent = new EventBuilder()
                .WithName("Seattle Cleanup")
                .InCity("Seattle")
                .InRegion("WA")
                .OnDate(DateTimeOffset.UtcNow.AddDays(7))
                .AsActive()
                .Build();
            var portlandEvent = new EventBuilder()
                .WithName("Portland Cleanup")
                .InCity("Portland")
                .InRegion("OR")
                .OnDate(DateTimeOffset.UtcNow.AddDays(7))
                .AsActive()
                .Build();

            _partnerRepository.SetupGet(new List<Partner> { community });
            _eventRepository.SetupGet(new List<Event> { seattleEvent, portlandEvent });

            // Act
            var result = await _sut.GetCommunityEventsAsync("seattle-wa");

            // Assert
            var resultList = result.ToList();
            Assert.Single(resultList);
            Assert.Equal("Seattle Cleanup", resultList[0].Name);
        }

        [Fact]
        public async Task GetCommunityEventsAsync_ExcludesCancelledEvents()
        {
            // Arrange
            var community = new PartnerBuilder()
                .WithName("Seattle Community")
                .WithSlug("seattle-wa")
                .WithLocation("Seattle", "WA", "United States")
                .WithHomePageEnabled()
                .AsActive()
                .Build();

            var activeEvent = new EventBuilder()
                .WithName("Active Event")
                .InCity("Seattle")
                .InRegion("WA")
                .OnDate(DateTimeOffset.UtcNow.AddDays(7))
                .AsActive()
                .Build();
            var cancelledEvent = new EventBuilder()
                .WithName("Cancelled Event")
                .InCity("Seattle")
                .InRegion("WA")
                .OnDate(DateTimeOffset.UtcNow.AddDays(7))
                .AsCancelled()
                .Build();

            _partnerRepository.SetupGet(new List<Partner> { community });
            _eventRepository.SetupGet(new List<Event> { activeEvent, cancelledEvent });

            // Act
            var result = await _sut.GetCommunityEventsAsync("seattle-wa");

            // Assert
            var resultList = result.ToList();
            Assert.Single(resultList);
            Assert.Equal("Active Event", resultList[0].Name);
        }

        [Fact]
        public async Task GetCommunityEventsAsync_ReturnsEmptyWhenCommunityNotFound()
        {
            // Arrange
            _partnerRepository.SetupGet(new List<Partner>());

            // Act
            var result = await _sut.GetCommunityEventsAsync("nonexistent");

            // Assert
            Assert.Empty(result);
        }

        #endregion

        #region GetCommunityTeamsAsync

        [Fact]
        public async Task GetCommunityTeamsAsync_ReturnsTeamsWithinRadius()
        {
            // Arrange
            var community = new PartnerBuilder()
                .WithName("Seattle Community")
                .WithSlug("seattle-wa")
                .WithCoordinates(47.6062, -122.3321)
                .WithHomePageEnabled()
                .AsActive()
                .Build();

            // Tacoma team (~30 miles from Seattle)
            var tacomaTeam = new TeamBuilder()
                .WithName("Tacoma Team")
                .WithCoordinates(47.2529, -122.4443)
                .AsPublic()
                .AsActive()
                .Build();

            // Portland team (~145 miles from Seattle)
            var portlandTeam = new TeamBuilder()
                .WithName("Portland Team")
                .WithCoordinates(45.5152, -122.6784)
                .AsPublic()
                .AsActive()
                .Build();

            _partnerRepository.SetupGet(new List<Partner> { community });
            _teamRepository.SetupGet(new List<Team> { tacomaTeam, portlandTeam });

            // Act - default 50 mile radius
            var result = await _sut.GetCommunityTeamsAsync("seattle-wa");

            // Assert
            var resultList = result.ToList();
            Assert.Single(resultList);
            Assert.Equal("Tacoma Team", resultList[0].Name);
        }

        [Fact]
        public async Task GetCommunityTeamsAsync_ReturnsOnlyPublicActiveTeams()
        {
            // Arrange
            var community = new PartnerBuilder()
                .WithName("Seattle Community")
                .WithSlug("seattle-wa")
                .WithCoordinates(47.6062, -122.3321)
                .WithHomePageEnabled()
                .AsActive()
                .Build();

            var publicTeam = new TeamBuilder()
                .WithName("Public Team")
                .WithCoordinates(47.6062, -122.3321)
                .AsPublic()
                .AsActive()
                .Build();

            var privateTeam = new TeamBuilder()
                .WithName("Private Team")
                .WithCoordinates(47.6062, -122.3321)
                .AsPrivate()
                .AsActive()
                .Build();

            var inactiveTeam = new TeamBuilder()
                .WithName("Inactive Team")
                .WithCoordinates(47.6062, -122.3321)
                .AsPublic()
                .AsInactive()
                .Build();

            _partnerRepository.SetupGet(new List<Partner> { community });
            _teamRepository.SetupGet(new List<Team> { publicTeam, privateTeam, inactiveTeam });

            // Act
            var result = await _sut.GetCommunityTeamsAsync("seattle-wa");

            // Assert
            var resultList = result.ToList();
            Assert.Single(resultList);
            Assert.Equal("Public Team", resultList[0].Name);
        }

        #endregion

        #region GetByIdAsync

        [Fact]
        public async Task GetByIdAsync_ReturnsPartnerWhenFound()
        {
            // Arrange
            var partnerId = Guid.NewGuid();
            var partner = new PartnerBuilder()
                .WithId(partnerId)
                .WithName("Test Partner")
                .Build();

            _partnerRepository.SetupGetAsync(partner);

            // Act
            var result = await _sut.GetByIdAsync(partnerId);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(partnerId, result.Id);
        }

        [Fact]
        public async Task GetByIdAsync_ReturnsNullWhenNotFound()
        {
            // Arrange
            _partnerRepository.Setup(r => r.GetAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync((Partner)null);

            // Act
            var result = await _sut.GetByIdAsync(Guid.NewGuid());

            // Assert
            Assert.Null(result);
        }

        #endregion

        #region Regional Community Filtering

        [Fact]
        public async Task GetCommunityEventsAsync_CountyCommunity_ReturnsEventsWithinBounds()
        {
            // Arrange - King County bounding box (roughly)
            var community = new PartnerBuilder()
                .WithName("King County")
                .WithSlug("king-county-wa")
                .WithHomePageEnabled()
                .AsActive()
                .AsCountyCommunity("King County", "WA", 47.0, 48.0, -122.5, -121.5)
                .WithCoordinates(47.5, -122.0)
                .Build();

            // Event inside King County bounds
            var insideEvent = new EventBuilder()
                .WithName("Inside Event")
                .WithCoordinates(47.5, -122.0)
                .InCity("Bellevue")
                .InRegion("WA")
                .OnDate(DateTimeOffset.UtcNow.AddDays(7))
                .AsActive()
                .Build();

            // Event outside bounds (Portland area)
            var outsideEvent = new EventBuilder()
                .WithName("Outside Event")
                .WithCoordinates(45.5, -122.6)
                .InCity("Portland")
                .InRegion("OR")
                .OnDate(DateTimeOffset.UtcNow.AddDays(7))
                .AsActive()
                .Build();

            _partnerRepository.SetupGet(new List<Partner> { community });
            _eventRepository.SetupGet(new List<Event> { insideEvent, outsideEvent });

            // Act
            var result = await _sut.GetCommunityEventsAsync("king-county-wa");

            // Assert
            var resultList = result.ToList();
            Assert.Single(resultList);
            Assert.Equal("Inside Event", resultList[0].Name);
        }

        [Fact]
        public async Task GetCommunityEventsAsync_CountyCommunityWithNullCity_StillWorks()
        {
            // Arrange - County community has null City (set by AsCountyCommunity)
            var community = new PartnerBuilder()
                .WithName("King County")
                .WithSlug("king-county-wa")
                .WithHomePageEnabled()
                .AsActive()
                .AsCountyCommunity("King County", "WA", 47.0, 48.0, -122.5, -121.5)
                .WithCoordinates(47.5, -122.0)
                .Build();

            var insideEvent = new EventBuilder()
                .WithName("County Event")
                .WithCoordinates(47.5, -122.0)
                .OnDate(DateTimeOffset.UtcNow.AddDays(7))
                .AsActive()
                .Build();

            _partnerRepository.SetupGet(new List<Partner> { community });
            _eventRepository.SetupGet(new List<Event> { insideEvent });

            // Act - Should not return empty despite null City
            var result = await _sut.GetCommunityEventsAsync("king-county-wa");

            // Assert
            Assert.Single(result);
        }

        [Fact]
        public async Task GetCommunityEventsAsync_CityCommunitiesWithNullRegionType_UseExactMatch()
        {
            // Arrange - Existing city community (null RegionType = backward compat)
            var community = new PartnerBuilder()
                .WithName("Seattle Community")
                .WithSlug("seattle-wa")
                .WithLocation("Seattle", "WA", "United States")
                .WithHomePageEnabled()
                .AsActive()
                .Build();

            // Verify RegionType is null (backward compat)
            Assert.Null(community.RegionType);

            var seattleEvent = new EventBuilder()
                .WithName("Seattle Event")
                .InCity("Seattle")
                .InRegion("WA")
                .OnDate(DateTimeOffset.UtcNow.AddDays(7))
                .AsActive()
                .Build();

            var bellevueEvent = new EventBuilder()
                .WithName("Bellevue Event")
                .InCity("Bellevue")
                .InRegion("WA")
                .OnDate(DateTimeOffset.UtcNow.AddDays(7))
                .AsActive()
                .Build();

            _partnerRepository.SetupGet(new List<Partner> { community });
            _eventRepository.SetupGet(new List<Event> { seattleEvent, bellevueEvent });

            // Act
            var result = await _sut.GetCommunityEventsAsync("seattle-wa");

            // Assert - Only exact city match, not Bellevue
            var resultList = result.ToList();
            Assert.Single(resultList);
            Assert.Equal("Seattle Event", resultList[0].Name);
        }

        [Fact]
        public async Task GetCommunityStatsAsync_CountyCommunity_CalculatesStatsWithinBounds()
        {
            // Arrange
            var community = new PartnerBuilder()
                .WithName("King County")
                .WithSlug("king-county-wa")
                .WithHomePageEnabled()
                .AsActive()
                .AsCountyCommunity("King County", "WA", 47.0, 48.0, -122.5, -121.5)
                .WithCoordinates(47.5, -122.0)
                .Build();

            var insideEvent = new EventBuilder()
                .WithName("Inside Event")
                .WithCoordinates(47.5, -122.0)
                .AsActive()
                .Build();

            var outsideEvent = new EventBuilder()
                .WithName("Outside Event")
                .WithCoordinates(45.5, -122.6)
                .AsActive()
                .Build();

            _partnerRepository.SetupGet(new List<Partner> { community });
            _eventRepository.SetupGet(new List<Event> { insideEvent, outsideEvent });
            _eventSummaryRepository.SetupGet(new List<EventSummary>());
            _litterImageRepository.SetupGet(new List<LitterImage>());
            _litterReportRepository.SetupGet(new List<LitterReport>());

            // Act
            var result = await _sut.GetCommunityStatsAsync("king-county-wa");

            // Assert - Only the inside event should be counted
            Assert.Equal(1, result.TotalEvents);
        }

        #endregion

        #region UpdateCommunityContentAsync

        [Fact]
        public async Task UpdateCommunityContentAsync_UpdatesAllowedFields()
        {
            // Arrange
            var partnerId = Guid.NewGuid();
            var userId = Guid.NewGuid();
            var existingPartner = new PartnerBuilder()
                .WithId(partnerId)
                .WithName("Original Partner")
                .Build();

            var updatedPartner = new PartnerBuilder()
                .WithId(partnerId)
                .WithName("Updated Partner")
                .WithBranding("#FF0000", "#00FF00")
                .WithContactInfo("new@email.com", "555-1234")
                .Build();
            updatedPartner.PublicNotes = "New notes";
            updatedPartner.Tagline = "New tagline";

            _partnerRepository.Setup(r => r.GetAsync(partnerId, It.IsAny<CancellationToken>()))
                .ReturnsAsync(existingPartner);
            _partnerRepository.Setup(r => r.UpdateAsync(It.IsAny<Partner>()))
                .ReturnsAsync((Partner p) => p);

            // Act
            var result = await _sut.UpdateCommunityContentAsync(updatedPartner, userId);

            // Assert
            Assert.NotNull(result);
            Assert.Equal("New notes", result.PublicNotes);
            Assert.Equal("New tagline", result.Tagline);
            Assert.Equal("#FF0000", result.BrandingPrimaryColor);
            Assert.Equal(userId, result.LastUpdatedByUserId);
        }

        [Fact]
        public async Task UpdateCommunityContentAsync_ReturnsNullWhenPartnerNotFound()
        {
            // Arrange
            var partnerId = Guid.NewGuid();
            var userId = Guid.NewGuid();
            var updatedPartner = new PartnerBuilder()
                .WithId(partnerId)
                .Build();

            _partnerRepository.Setup(r => r.GetAsync(partnerId, It.IsAny<CancellationToken>()))
                .ReturnsAsync((Partner)null);

            // Act
            var result = await _sut.UpdateCommunityContentAsync(updatedPartner, userId);

            // Assert
            Assert.Null(result);
        }

        #endregion
    }
}
