namespace TrashMob.Shared.Tests.Managers.Events
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading;
    using System.Threading.Tasks;
    using Moq;
    using TrashMob.Models;
    using TrashMob.Shared.Managers.Events;
    using TrashMob.Shared.Managers.Interfaces;
    using TrashMob.Shared.Persistence.Interfaces;
    using TrashMob.Shared.Tests.Builders;
    using TrashMob.Shared.Tests.Fixtures;
    using Xunit;

    /// <summary>
    /// Verifies that event aggregate metrics (totals, public summaries, user stats)
    /// remain correct after user deletion anonymizes EventAttendeeMetrics records
    /// (UserId → Guid.Empty). This is Phase 2 of Project 49 (Privacy & Compliance).
    /// </summary>
    public class AggregatePreservationAfterDeletionTests
    {
        private readonly Mock<IKeyedRepository<EventAttendeeMetrics>> _metricsRepository;
        private readonly Mock<IEventAttendeeManager> _eventAttendeeManager;
        private readonly Mock<IKeyedManager<Event>> _eventManager;
        private readonly EventAttendeeMetricsManager _sut;

        private const int WeightUnitPounds = 1;
        private const int WeightUnitKilograms = 2;
        private const decimal KgToLbsConversion = 2.20462m;

        public AggregatePreservationAfterDeletionTests()
        {
            _metricsRepository = new Mock<IKeyedRepository<EventAttendeeMetrics>>();
            _eventAttendeeManager = new Mock<IEventAttendeeManager>();
            _eventManager = new Mock<IKeyedManager<Event>>();

            _metricsRepository.SetupAddAsync();
            _metricsRepository.SetupUpdateAsync();

            _sut = new EventAttendeeMetricsManager(
                _metricsRepository.Object,
                _eventAttendeeManager.Object,
                _eventManager.Object);
        }

        #region CalculateTotalsAsync — Event-Level Aggregates

        [Fact]
        public async Task CalculateTotalsAsync_WithAnonymizedApprovedMetrics_IncludesInTotals()
        {
            // Arrange: 2 active users + 1 anonymized (deleted) user, all approved
            var eventId = Guid.NewGuid();

            var metrics = new List<EventAttendeeMetrics>
            {
                new EventAttendeeMetricsBuilder()
                    .ForEvent(eventId)
                    .WithBagsCollected(5)
                    .WithWeightInPounds(10m)
                    .WithDuration(60)
                    .AsApproved()
                    .Build(),
                new EventAttendeeMetricsBuilder()
                    .ForEvent(eventId)
                    .WithBagsCollected(3)
                    .WithWeightInPounds(5m)
                    .WithDuration(45)
                    .AsApproved()
                    .Build(),
                new EventAttendeeMetricsBuilder()
                    .ForEvent(eventId)
                    .WithBagsCollected(4)
                    .WithWeightInPounds(8m)
                    .WithDuration(30)
                    .AsApproved()
                    .AsAnonymized()
                    .Build()
            };

            _metricsRepository.SetupGet(metrics);

            // Act
            var totals = await _sut.CalculateTotalsAsync(eventId);

            // Assert: all 3 metrics included in totals (anonymized user's contribution preserved)
            Assert.Equal(12, totals.TotalBagsCollected);    // 5 + 3 + 4
            Assert.Equal(23m, totals.TotalWeightPounds);     // 10 + 5 + 8
            Assert.Equal(135, totals.TotalDurationMinutes);  // 60 + 45 + 30
            Assert.Equal(3, totals.ApprovedSubmissions);
            Assert.Equal(3, totals.TotalSubmissions);
        }

        [Fact]
        public async Task CalculateTotalsAsync_WithAnonymizedAdjustedMetrics_UsesAdjustedValues()
        {
            // Arrange: 1 adjusted metric from a deleted user
            var eventId = Guid.NewGuid();

            var metrics = new List<EventAttendeeMetrics>
            {
                new EventAttendeeMetricsBuilder()
                    .ForEvent(eventId)
                    .WithBagsCollected(10)
                    .WithWeightInPounds(20m)
                    .WithDuration(90)
                    .AsAdjusted(bags: 5, weight: 10m, duration: 60, reason: "Reviewer correction")
                    .AsAnonymized()
                    .Build()
            };

            _metricsRepository.SetupGet(metrics);

            // Act
            var totals = await _sut.CalculateTotalsAsync(eventId);

            // Assert: uses adjusted values, not originals
            Assert.Equal(5, totals.TotalBagsCollected);
            Assert.Equal(10m, totals.TotalWeightPounds);
            Assert.Equal(60, totals.TotalDurationMinutes);
        }

        [Fact]
        public async Task CalculateTotalsAsync_WithMixOfAnonymizedAndActiveMetrics_ExcludesPending()
        {
            // Arrange: 1 approved active, 1 approved anonymized, 1 pending anonymized
            var eventId = Guid.NewGuid();

            var metrics = new List<EventAttendeeMetrics>
            {
                new EventAttendeeMetricsBuilder()
                    .ForEvent(eventId)
                    .WithBagsCollected(5)
                    .WithWeightInPounds(10m)
                    .AsApproved()
                    .Build(),
                new EventAttendeeMetricsBuilder()
                    .ForEvent(eventId)
                    .WithBagsCollected(3)
                    .WithWeightInPounds(5m)
                    .AsApproved()
                    .AsAnonymized()
                    .Build(),
                new EventAttendeeMetricsBuilder()
                    .ForEvent(eventId)
                    .WithBagsCollected(100)
                    .WithWeightInPounds(200m)
                    .AsPending()
                    .AsAnonymized()
                    .Build()
            };

            _metricsRepository.SetupGet(metrics);

            // Act
            var totals = await _sut.CalculateTotalsAsync(eventId);

            // Assert: pending metric excluded even though anonymized
            Assert.Equal(8, totals.TotalBagsCollected);     // 5 + 3 (not 100)
            Assert.Equal(15m, totals.TotalWeightPounds);     // 10 + 5 (not 200)
            Assert.Equal(2, totals.ApprovedSubmissions);
            Assert.Equal(1, totals.PendingSubmissions);
        }

        [Fact]
        public async Task CalculateTotalsAsync_AllMetricsAnonymized_StillCalculatesTotals()
        {
            // Arrange: all attendees deleted — every metric is anonymized
            var eventId = Guid.NewGuid();

            var metrics = new List<EventAttendeeMetrics>
            {
                new EventAttendeeMetricsBuilder()
                    .ForEvent(eventId)
                    .WithBagsCollected(7)
                    .WithWeightInPounds(15m)
                    .WithDuration(90)
                    .AsApproved()
                    .AsAnonymized()
                    .Build(),
                new EventAttendeeMetricsBuilder()
                    .ForEvent(eventId)
                    .WithBagsCollected(3)
                    .WithWeightInPounds(5m)
                    .WithDuration(45)
                    .AsApproved()
                    .AsAnonymized()
                    .Build(),
                new EventAttendeeMetricsBuilder()
                    .ForEvent(eventId)
                    .WithBagsCollected(2)
                    .WithWeightInPounds(3m)
                    .WithDuration(30)
                    .AsApproved()
                    .AsAnonymized()
                    .Build()
            };

            _metricsRepository.SetupGet(metrics);

            // Act
            var totals = await _sut.CalculateTotalsAsync(eventId);

            // Assert: non-zero totals even with all users deleted
            Assert.Equal(12, totals.TotalBagsCollected);    // 7 + 3 + 2
            Assert.Equal(23m, totals.TotalWeightPounds);     // 15 + 5 + 3
            Assert.Equal(165, totals.TotalDurationMinutes);  // 90 + 45 + 30
            Assert.Equal(3, totals.ApprovedSubmissions);
        }

        [Fact]
        public async Task CalculateTotalsAsync_WithAnonymizedKilogramMetrics_ConvertsCorrectly()
        {
            // Arrange: anonymized metric with weight in kilograms
            var eventId = Guid.NewGuid();

            var metrics = new List<EventAttendeeMetrics>
            {
                new EventAttendeeMetricsBuilder()
                    .ForEvent(eventId)
                    .WithBagsCollected(2)
                    .WithWeightInKilograms(10m)
                    .WithDuration(60)
                    .AsApproved()
                    .AsAnonymized()
                    .Build()
            };

            _metricsRepository.SetupGet(metrics);

            // Act
            var totals = await _sut.CalculateTotalsAsync(eventId);

            // Assert: kg → lbs conversion works for anonymized metrics
            Assert.Equal(2, totals.TotalBagsCollected);
            Assert.Equal(10m * KgToLbsConversion, totals.TotalWeightPounds);
            Assert.Equal(60, totals.TotalDurationMinutes);
        }

        #endregion

        #region GetPublicMetricsSummaryAsync — Public Summary

        [Fact]
        public async Task GetPublicMetricsSummaryAsync_IncludesAnonymizedInTotalsButExcludesFromContributors()
        {
            // Arrange: 2 active users + 1 anonymized, all approved
            var eventId = Guid.NewGuid();

            var alice = new UserBuilder().WithUserName("Alice").Build();
            var bob = new UserBuilder().WithUserName("Bob").Build();

            var metrics = new List<EventAttendeeMetrics>
            {
                new EventAttendeeMetricsBuilder()
                    .ForEvent(eventId)
                    .WithUser(alice)
                    .WithBagsCollected(5)
                    .WithWeightInPounds(10m)
                    .WithDuration(60)
                    .AsApproved()
                    .Build(),
                new EventAttendeeMetricsBuilder()
                    .ForEvent(eventId)
                    .WithUser(bob)
                    .WithBagsCollected(3)
                    .WithWeightInPounds(5m)
                    .WithDuration(45)
                    .AsApproved()
                    .Build(),
                new EventAttendeeMetricsBuilder()
                    .ForEvent(eventId)
                    .WithBagsCollected(4)
                    .WithWeightInPounds(8m)
                    .WithDuration(30)
                    .AsApproved()
                    .AsAnonymized()
                    .Build()
            };

            _metricsRepository.SetupGet(metrics);

            // Act
            var summary = await _sut.GetPublicMetricsSummaryAsync(eventId);

            // Assert: totals include all 3 metrics
            Assert.Equal(12, summary.TotalBagsCollected);    // 5 + 3 + 4
            Assert.Equal(23m, summary.TotalWeightPounds);     // 10 + 5 + 8
            Assert.Equal(135, summary.TotalDurationMinutes);  // 60 + 45 + 30

            // Assert: contributor list excludes anonymized user
            Assert.Equal(2, summary.ContributorCount);
            Assert.Equal(2, summary.Contributors.Count);
            Assert.Contains(summary.Contributors, c => c.UserName == "Alice");
            Assert.Contains(summary.Contributors, c => c.UserName == "Bob");
            Assert.DoesNotContain(summary.Contributors, c => c.UserId == Guid.Empty);
        }

        [Fact]
        public async Task GetPublicMetricsSummaryAsync_AllAnonymized_TotalsPreservedContributorsEmpty()
        {
            // Arrange: all attendees deleted
            var eventId = Guid.NewGuid();

            var metrics = new List<EventAttendeeMetrics>
            {
                new EventAttendeeMetricsBuilder()
                    .ForEvent(eventId)
                    .WithBagsCollected(7)
                    .WithWeightInPounds(15m)
                    .WithDuration(90)
                    .AsApproved()
                    .AsAnonymized()
                    .Build(),
                new EventAttendeeMetricsBuilder()
                    .ForEvent(eventId)
                    .WithBagsCollected(3)
                    .WithWeightInPounds(5m)
                    .WithDuration(45)
                    .AsApproved()
                    .AsAnonymized()
                    .Build()
            };

            _metricsRepository.SetupGet(metrics);

            // Act
            var summary = await _sut.GetPublicMetricsSummaryAsync(eventId);

            // Assert: totals preserved
            Assert.Equal(10, summary.TotalBagsCollected);
            Assert.Equal(20m, summary.TotalWeightPounds);

            // Assert: no contributors (all deleted)
            Assert.Equal(0, summary.ContributorCount);
            Assert.Empty(summary.Contributors);
        }

        [Fact]
        public async Task GetPublicMetricsSummaryAsync_AnonymizedWithUserNavProp_StillExcludedFromContributors()
        {
            // Arrange: anonymized metric where EF loaded the Guid.Empty seed user nav property
            var eventId = Guid.NewGuid();
            var seedUser = new UserBuilder()
                .WithId(Guid.Empty)
                .WithUserName("TrashMob")
                .Build();

            var metrics = new List<EventAttendeeMetrics>
            {
                new EventAttendeeMetricsBuilder()
                    .ForEvent(eventId)
                    .WithUser(seedUser)  // Nav prop loaded to seed user
                    .WithBagsCollected(5)
                    .WithWeightInPounds(10m)
                    .AsApproved()
                    .Build()
            };

            // The WithUser call sets UserId = Guid.Empty (since seedUser.Id == Guid.Empty)
            _metricsRepository.SetupGet(metrics);

            // Act
            var summary = await _sut.GetPublicMetricsSummaryAsync(eventId);

            // Assert: totals include the metric
            Assert.Equal(5, summary.TotalBagsCollected);

            // Assert: Guid.Empty check prevents "TrashMob" from appearing as contributor
            Assert.Equal(0, summary.ContributorCount);
            Assert.Empty(summary.Contributors);
        }

        #endregion

        #region GetUserImpactStatsAsync — Per-User Stats

        [Fact]
        public async Task GetUserImpactStatsAsync_ForDeletedUser_ReturnsEmptyStats()
        {
            // Arrange: metrics anonymized (UserId = Guid.Empty), query with the original userId
            var deletedUserId = Guid.NewGuid();
            var eventId = Guid.NewGuid();
            var eventEntity = new EventBuilder().WithId(eventId).Build();

            var metrics = new List<EventAttendeeMetrics>
            {
                new EventAttendeeMetricsBuilder()
                    .ForEvent(eventId)
                    .WithEvent(eventEntity)
                    .WithBagsCollected(10)
                    .WithWeightInPounds(20m)
                    .WithDuration(90)
                    .AsApproved()
                    .AsAnonymized()  // Was this user's, now anonymized
                    .Build()
            };

            _metricsRepository.SetupGet(metrics);

            // Act: query with the deleted user's original ID
            var stats = await _sut.GetUserImpactStatsAsync(deletedUserId);

            // Assert: no matching records — user's stats are empty
            Assert.Equal(0, stats.EventsWithMetrics);
            Assert.Equal(0, stats.TotalBagsCollected);
            Assert.Equal(0, stats.TotalWeightPounds);
            Assert.Equal(0, stats.TotalDurationMinutes);
            Assert.Empty(stats.EventBreakdown);
        }

        [Fact]
        public async Task GetUserImpactStatsAsync_ActiveUserUnaffectedByAnonymizedMetrics()
        {
            // Arrange: active user has 2 metrics, plus 1 anonymized metric for same events
            var activeUserId = Guid.NewGuid();
            var event1 = new EventBuilder().WithName("Cleanup 1").Build();
            var event2 = new EventBuilder().WithName("Cleanup 2").Build();

            var metrics = new List<EventAttendeeMetrics>
            {
                new EventAttendeeMetricsBuilder()
                    .ForEvent(event1.Id)
                    .ForUser(activeUserId)
                    .WithEvent(event1)
                    .WithBagsCollected(5)
                    .WithWeightInPounds(10m)
                    .WithDuration(60)
                    .AsApproved()
                    .Build(),
                new EventAttendeeMetricsBuilder()
                    .ForEvent(event2.Id)
                    .ForUser(activeUserId)
                    .WithEvent(event2)
                    .WithBagsCollected(3)
                    .WithWeightInPounds(5m)
                    .WithDuration(45)
                    .AsApproved()
                    .Build(),
                // Anonymized metric for same event — should NOT be included in active user's stats
                new EventAttendeeMetricsBuilder()
                    .ForEvent(event1.Id)
                    .WithEvent(event1)
                    .WithBagsCollected(10)
                    .WithWeightInPounds(20m)
                    .WithDuration(120)
                    .AsApproved()
                    .AsAnonymized()
                    .Build()
            };

            _metricsRepository.SetupGet(metrics);

            // Act
            var stats = await _sut.GetUserImpactStatsAsync(activeUserId);

            // Assert: only the active user's own metrics counted
            Assert.Equal(2, stats.EventsWithMetrics);
            Assert.Equal(8, stats.TotalBagsCollected);       // 5 + 3 (not +10)
            Assert.Equal(15m, stats.TotalWeightPounds);       // 10 + 5 (not +20)
            Assert.Equal(105, stats.TotalDurationMinutes);    // 60 + 45 (not +120)
        }

        [Fact]
        public async Task GetUserImpactStatsAsync_ForGuidEmpty_ReturnsAnonymizedMetrics()
        {
            // This test documents the edge case behavior: querying with Guid.Empty
            // returns all anonymized metrics. This is not a real user scenario but
            // documents the behavior for awareness.
            var event1 = new EventBuilder().WithName("Cleanup 1").Build();
            var event2 = new EventBuilder().WithName("Cleanup 2").Build();

            var metrics = new List<EventAttendeeMetrics>
            {
                new EventAttendeeMetricsBuilder()
                    .ForEvent(event1.Id)
                    .WithEvent(event1)
                    .WithBagsCollected(5)
                    .WithWeightInPounds(10m)
                    .AsApproved()
                    .AsAnonymized()
                    .Build(),
                new EventAttendeeMetricsBuilder()
                    .ForEvent(event2.Id)
                    .WithEvent(event2)
                    .WithBagsCollected(3)
                    .WithWeightInPounds(5m)
                    .AsApproved()
                    .AsAnonymized()
                    .Build()
            };

            _metricsRepository.SetupGet(metrics);

            // Act: query with Guid.Empty (the anonymized user ID)
            var stats = await _sut.GetUserImpactStatsAsync(Guid.Empty);

            // Assert: returns all anonymized metrics (documents known edge case)
            Assert.Equal(2, stats.EventsWithMetrics);
            Assert.Equal(8, stats.TotalBagsCollected);
            Assert.Equal(15m, stats.TotalWeightPounds);
        }

        #endregion
    }
}
