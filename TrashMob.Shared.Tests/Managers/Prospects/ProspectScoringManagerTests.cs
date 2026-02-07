namespace TrashMob.Shared.Tests.Managers.Prospects
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using Moq;
    using TrashMob.Models;
    using TrashMob.Shared.Managers.Prospects;
    using TrashMob.Shared.Persistence.Interfaces;
    using TrashMob.Shared.Tests.Builders;
    using TrashMob.Shared.Tests.Fixtures;
    using Xunit;

    public class ProspectScoringManagerTests
    {
        private readonly Mock<IKeyedRepository<Event>> _eventRepo;
        private readonly Mock<IKeyedRepository<Partner>> _partnerRepo;
        private readonly Mock<IKeyedRepository<CommunityProspect>> _prospectRepo;
        private readonly ProspectScoringManager _sut;

        public ProspectScoringManagerTests()
        {
            _eventRepo = new Mock<IKeyedRepository<Event>>();
            _partnerRepo = new Mock<IKeyedRepository<Partner>>();
            _prospectRepo = new Mock<IKeyedRepository<CommunityProspect>>();
            _sut = new ProspectScoringManager(_eventRepo.Object, _partnerRepo.Object, _prospectRepo.Object);
        }

        [Fact]
        public async Task CalculateFitScore_MunicipalityWithManyEventsAndNoPartner_ReturnsHighScore()
        {
            // Arrange: Municipality (15 type pts), 200K+ population (25 pts), many nearby events (30 pts), no partner (30 gap pts)
            var prospect = new CommunityProspectBuilder()
                .WithType("Municipality")
                .WithPopulation(250000)
                .WithCoordinates(47.6062, -122.3321)
                .Build();

            var events = Enumerable.Range(0, 20).Select(i =>
                new EventBuilder()
                    .WithCoordinates(47.6062 + i * 0.001, -122.3321 + i * 0.001)
                    .AsActive()
                    .Build()).ToList();

            _prospectRepo.SetupGetAsync(prospect);
            _eventRepo.SetupGet(events);
            _partnerRepo.SetupGet(new List<Partner>());

            // Act
            var breakdown = await _sut.CalculateFitScoreAsync(prospect.Id);

            // Assert
            Assert.NotNull(breakdown);
            Assert.True(breakdown.TotalScore >= 70, $"Expected high score >= 70, got {breakdown.TotalScore}");
            Assert.Equal(30, breakdown.EventDensityScore);
            Assert.Equal(25, breakdown.PopulationScore);
            Assert.Equal(30, breakdown.GeographicGapScore);
            Assert.Equal(15, breakdown.CommunityTypeFitScore);
        }

        [Fact]
        public async Task CalculateFitScore_ZeroEventsWithNearbyPartner_ReturnsLowScore()
        {
            // Arrange: Other type (5 pts), no population (10 pts), 0 events (0 pts), partner nearby (0 gap pts)
            var prospect = new CommunityProspectBuilder()
                .WithType("Other")
                .WithPopulation(null)
                .WithCoordinates(47.6062, -122.3321)
                .Build();

            var partner = new PartnerBuilder()
                .AsActive()
                .WithHomePageEnabled()
                .WithCoordinates(47.6062, -122.3321) // Same location
                .Build();

            _prospectRepo.SetupGetAsync(prospect);
            _eventRepo.SetupGet(new List<Event>());
            _partnerRepo.SetupGet(new List<Partner> { partner });

            // Act
            var breakdown = await _sut.CalculateFitScoreAsync(prospect.Id);

            // Assert
            Assert.NotNull(breakdown);
            Assert.True(breakdown.TotalScore < 30, $"Expected low score < 30, got {breakdown.TotalScore}");
            Assert.Equal(0, breakdown.EventDensityScore);
            Assert.Equal(0, breakdown.GeographicGapScore);
        }

        [Theory]
        [InlineData(null, 10)]
        [InlineData(5000, 5)]
        [InlineData(30000, 15)]
        [InlineData(100000, 20)]
        [InlineData(500000, 25)]
        public async Task CalculateFitScore_PopulationThresholds_ReturnCorrectScore(int? population, int expectedScore)
        {
            var prospect = new CommunityProspectBuilder()
                .WithPopulation(population)
                .WithCoordinates(47.6062, -122.3321)
                .Build();

            _prospectRepo.SetupGetAsync(prospect);
            _eventRepo.SetupGet(new List<Event>());
            _partnerRepo.SetupGet(new List<Partner>());

            var breakdown = await _sut.CalculateFitScoreAsync(prospect.Id);

            Assert.Equal(expectedScore, breakdown.PopulationScore);
        }

        [Theory]
        [InlineData("Municipality", 15)]
        [InlineData("Nonprofit", 12)]
        [InlineData("CivicOrg", 10)]
        [InlineData("HOA", 8)]
        [InlineData("Other", 5)]
        [InlineData("Unknown", 5)]
        public async Task CalculateFitScore_TypeScoring_ReturnCorrectScore(string type, int expectedScore)
        {
            var prospect = new CommunityProspectBuilder()
                .WithType(type)
                .WithCoordinates(47.6062, -122.3321)
                .Build();

            _prospectRepo.SetupGetAsync(prospect);
            _eventRepo.SetupGet(new List<Event>());
            _partnerRepo.SetupGet(new List<Partner>());

            var breakdown = await _sut.CalculateFitScoreAsync(prospect.Id);

            Assert.Equal(expectedScore, breakdown.CommunityTypeFitScore);
        }

        [Fact]
        public async Task CalculateFitScore_NonexistentProspect_ReturnsNull()
        {
            _prospectRepo.Setup(r => r.GetAsync(It.IsAny<Guid>(), default))
                .ReturnsAsync((CommunityProspect)null);

            var result = await _sut.CalculateFitScoreAsync(Guid.NewGuid());

            Assert.Null(result);
        }

        [Fact]
        public async Task GetGeographicGaps_ExcludesCitiesWithActivePartners()
        {
            // Seattle has an active partner, Portland does not
            var events = new List<Event>
            {
                new EventBuilder().WithLocation("Seattle", "WA", "United States")
                    .WithCoordinates(47.6062, -122.3321).AsActive().Build(),
                new EventBuilder().WithLocation("Seattle", "WA", "United States")
                    .WithCoordinates(47.6072, -122.3331).AsActive().Build(),
                new EventBuilder().WithLocation("Portland", "OR", "United States")
                    .WithCoordinates(45.5152, -122.6784).AsActive().Build(),
            };

            var partners = new List<Partner>
            {
                new PartnerBuilder()
                    .WithLocation("Seattle", "WA", "United States")
                    .WithCoordinates(47.6062, -122.3321)
                    .AsActive()
                    .WithHomePageEnabled()
                    .Build(),
            };

            _eventRepo.SetupGet(events);
            _partnerRepo.SetupGet(partners);

            var gaps = (await _sut.GetGeographicGapsAsync()).ToList();

            Assert.Single(gaps);
            Assert.Equal("Portland", gaps[0].City);
            Assert.Equal(1, gaps[0].EventCount);
        }

        [Fact]
        public async Task GetGeographicGaps_ExcludesCancelledEvents()
        {
            var events = new List<Event>
            {
                new EventBuilder().WithLocation("Portland", "OR", "United States")
                    .WithCoordinates(45.5152, -122.6784).AsActive().Build(),
                new EventBuilder().WithLocation("Portland", "OR", "United States")
                    .WithCoordinates(45.5162, -122.6794).AsCancelled().Build(),
            };

            _eventRepo.SetupGet(events);
            _partnerRepo.SetupGet(new List<Partner>());

            var gaps = (await _sut.GetGeographicGapsAsync()).ToList();

            Assert.Single(gaps);
            Assert.Equal(1, gaps[0].EventCount); // Only the active event
        }

        [Fact]
        public async Task RecalculateAllScores_UpdatesProspectsWithChangedScores()
        {
            var prospect = new CommunityProspectBuilder()
                .WithType("Municipality")
                .WithPopulation(250000)
                .WithCoordinates(47.6062, -122.3321)
                .WithFitScore(0) // Out of date
                .Build();

            _prospectRepo.SetupGet(new List<CommunityProspect> { prospect });
            _prospectRepo.SetupGetAsync(prospect);
            _prospectRepo.SetupUpdateAsync();
            _eventRepo.SetupGet(new List<Event>());
            _partnerRepo.SetupGet(new List<Partner>());

            var userId = Guid.NewGuid();
            var count = await _sut.RecalculateAllScoresAsync(userId);

            Assert.Equal(1, count);
            _prospectRepo.Verify(r => r.UpdateAsync(It.Is<CommunityProspect>(p => p.FitScore > 0)), Times.Once);
        }

        [Fact]
        public void CalculateDistanceMiles_SamePoint_ReturnsZero()
        {
            var distance = ProspectScoringManager.CalculateDistanceMiles(47.6, -122.3, 47.6, -122.3);
            Assert.Equal(0, distance, 1);
        }

        [Fact]
        public void CalculateDistanceMiles_SeattleToPortland_ReturnsReasonableDistance()
        {
            // Seattle to Portland is roughly 145 miles
            var distance = ProspectScoringManager.CalculateDistanceMiles(47.6062, -122.3321, 45.5152, -122.6784);
            Assert.InRange(distance, 140, 150);
        }
    }
}
