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
    /// Unit tests for <see cref="EventAttendeeMetricsManager"/>.
    /// </summary>
    public class EventAttendeeMetricsManagerTests
    {
        private readonly Mock<IKeyedRepository<EventAttendeeMetrics>> _metricsRepository;
        private readonly Mock<IEventAttendeeManager> _eventAttendeeManager;
        private readonly Mock<IKeyedManager<Event>> _eventManager;
        private readonly EventAttendeeMetricsManager _sut;

        // Weight unit IDs as used in the manager
        private const int WeightUnitPounds = 1;
        private const int WeightUnitKilograms = 2;
        private const decimal KgToLbsConversion = 2.20462m;

        public EventAttendeeMetricsManagerTests()
        {
            _metricsRepository = new Mock<IKeyedRepository<EventAttendeeMetrics>>();
            _eventAttendeeManager = new Mock<IEventAttendeeManager>();
            _eventManager = new Mock<IKeyedManager<Event>>();

            // Default setup for common operations
            _metricsRepository.SetupAddAsync();
            _metricsRepository.SetupUpdateAsync();

            _sut = new EventAttendeeMetricsManager(
                _metricsRepository.Object,
                _eventAttendeeManager.Object,
                _eventManager.Object);
        }

        #region SubmitMetricsAsync Tests

        [Fact]
        public async Task SubmitMetricsAsync_WithValidData_CreatesNewMetrics()
        {
            // Arrange
            var eventId = Guid.NewGuid();
            var userId = Guid.NewGuid();
            var eventEntity = new EventBuilder().WithId(eventId).Build();
            var attendingEvents = new List<Event> { eventEntity };

            var metricsToSubmit = new EventAttendeeMetricsBuilder()
                .ForEvent(eventId)
                .ForUser(userId)
                .WithBagsCollected(5)
                .WithWeightInPounds(10.5m)
                .WithDuration(120)
                .Build();

            _eventManager.Setup(m => m.GetAsync(eventId, It.IsAny<CancellationToken>()))
                .ReturnsAsync(eventEntity);
            _eventAttendeeManager.Setup(m => m.GetEventsUserIsAttendingAsync(userId, false, It.IsAny<CancellationToken>()))
                .ReturnsAsync(attendingEvents);
            _metricsRepository.SetupGet(new List<EventAttendeeMetrics>());

            // Act
            var result = await _sut.SubmitMetricsAsync(eventId, userId, metricsToSubmit);

            // Assert
            Assert.True(result.IsSuccess);
            Assert.Equal(5, result.Data.BagsCollected);
            Assert.Equal(10.5m, result.Data.PickedWeight);
            Assert.Equal(120, result.Data.DurationMinutes);
            Assert.Equal("Pending", result.Data.Status);
        }

        [Fact]
        public async Task SubmitMetricsAsync_EventNotFound_ReturnsFailure()
        {
            // Arrange
            var eventId = Guid.NewGuid();
            var userId = Guid.NewGuid();
            var metricsToSubmit = new EventAttendeeMetricsBuilder().Build();

            _eventManager.Setup(m => m.GetAsync(eventId, It.IsAny<CancellationToken>()))
                .ReturnsAsync((Event)null);

            // Act
            var result = await _sut.SubmitMetricsAsync(eventId, userId, metricsToSubmit);

            // Assert
            Assert.False(result.IsSuccess);
            Assert.Equal("Event not found.", result.ErrorMessage);
        }

        [Fact]
        public async Task SubmitMetricsAsync_UserNotAttendee_ReturnsFailure()
        {
            // Arrange
            var eventId = Guid.NewGuid();
            var userId = Guid.NewGuid();
            var eventEntity = new EventBuilder().WithId(eventId).Build();
            var metricsToSubmit = new EventAttendeeMetricsBuilder().Build();

            _eventManager.Setup(m => m.GetAsync(eventId, It.IsAny<CancellationToken>()))
                .ReturnsAsync(eventEntity);
            _eventAttendeeManager.Setup(m => m.GetEventsUserIsAttendingAsync(userId, false, It.IsAny<CancellationToken>()))
                .ReturnsAsync(new List<Event>()); // User is not attending any events

            // Act
            var result = await _sut.SubmitMetricsAsync(eventId, userId, metricsToSubmit);

            // Assert
            Assert.False(result.IsSuccess);
            Assert.Contains("must be registered as an attendee", result.ErrorMessage);
        }

        [Fact]
        public async Task SubmitMetricsAsync_ExistingPendingSubmission_UpdatesExisting()
        {
            // Arrange
            var eventId = Guid.NewGuid();
            var userId = Guid.NewGuid();
            var metricsId = Guid.NewGuid();
            var eventEntity = new EventBuilder().WithId(eventId).Build();
            var attendingEvents = new List<Event> { eventEntity };

            var existingMetrics = new EventAttendeeMetricsBuilder()
                .WithId(metricsId)
                .ForEvent(eventId)
                .ForUser(userId)
                .WithBagsCollected(3)
                .AsPending()
                .Build();

            var updatedMetrics = new EventAttendeeMetricsBuilder()
                .WithBagsCollected(10)
                .WithWeightInPounds(15.0m)
                .Build();

            _eventManager.Setup(m => m.GetAsync(eventId, It.IsAny<CancellationToken>()))
                .ReturnsAsync(eventEntity);
            _eventAttendeeManager.Setup(m => m.GetEventsUserIsAttendingAsync(userId, false, It.IsAny<CancellationToken>()))
                .ReturnsAsync(attendingEvents);
            _metricsRepository.SetupGet(new List<EventAttendeeMetrics> { existingMetrics });

            // Act
            var result = await _sut.SubmitMetricsAsync(eventId, userId, updatedMetrics);

            // Assert
            Assert.True(result.IsSuccess);
            Assert.Equal(10, result.Data.BagsCollected);
            Assert.Equal(15.0m, result.Data.PickedWeight);
            _metricsRepository.Verify(r => r.UpdateAsync(It.IsAny<EventAttendeeMetrics>()), Times.Once);
        }

        [Fact]
        public async Task SubmitMetricsAsync_ExistingNonPendingSubmission_ReturnsFailure()
        {
            // Arrange
            var eventId = Guid.NewGuid();
            var userId = Guid.NewGuid();
            var eventEntity = new EventBuilder().WithId(eventId).Build();
            var attendingEvents = new List<Event> { eventEntity };

            var existingApprovedMetrics = new EventAttendeeMetricsBuilder()
                .ForEvent(eventId)
                .ForUser(userId)
                .AsApproved()
                .Build();

            var updatedMetrics = new EventAttendeeMetricsBuilder()
                .WithBagsCollected(10)
                .Build();

            _eventManager.Setup(m => m.GetAsync(eventId, It.IsAny<CancellationToken>()))
                .ReturnsAsync(eventEntity);
            _eventAttendeeManager.Setup(m => m.GetEventsUserIsAttendingAsync(userId, false, It.IsAny<CancellationToken>()))
                .ReturnsAsync(attendingEvents);
            _metricsRepository.SetupGet(new List<EventAttendeeMetrics> { existingApprovedMetrics });

            // Act
            var result = await _sut.SubmitMetricsAsync(eventId, userId, updatedMetrics);

            // Assert
            Assert.False(result.IsSuccess);
            Assert.Contains("already been reviewed", result.ErrorMessage);
        }

        #endregion

        #region ApproveAsync Tests

        [Fact]
        public async Task ApproveAsync_PendingMetrics_SetsStatusToApproved()
        {
            // Arrange
            var metricsId = Guid.NewGuid();
            var reviewerId = Guid.NewGuid();
            var metrics = new EventAttendeeMetricsBuilder()
                .WithId(metricsId)
                .AsPending()
                .Build();

            _metricsRepository.SetupGetAsync(metrics);

            // Act
            var result = await _sut.ApproveAsync(metricsId, reviewerId);

            // Assert
            Assert.True(result.IsSuccess);
            Assert.Equal("Approved", result.Data.Status);
            Assert.Equal(reviewerId, result.Data.ReviewedByUserId);
            Assert.NotNull(result.Data.ReviewedDate);
        }

        [Fact]
        public async Task ApproveAsync_MetricsNotFound_ReturnsFailure()
        {
            // Arrange
            var metricsId = Guid.NewGuid();
            var reviewerId = Guid.NewGuid();

            _metricsRepository.Setup(r => r.GetAsync(metricsId, It.IsAny<CancellationToken>()))
                .ReturnsAsync((EventAttendeeMetrics)null);

            // Act
            var result = await _sut.ApproveAsync(metricsId, reviewerId);

            // Assert
            Assert.False(result.IsSuccess);
            Assert.Equal("Metrics submission not found.", result.ErrorMessage);
        }

        [Fact]
        public async Task ApproveAsync_AlreadyApproved_ReturnsFailure()
        {
            // Arrange
            var metricsId = Guid.NewGuid();
            var reviewerId = Guid.NewGuid();
            var metrics = new EventAttendeeMetricsBuilder()
                .WithId(metricsId)
                .AsApproved()
                .Build();

            _metricsRepository.SetupGetAsync(metrics);

            // Act
            var result = await _sut.ApproveAsync(metricsId, reviewerId);

            // Assert
            Assert.False(result.IsSuccess);
            Assert.Contains("Only pending submissions", result.ErrorMessage);
        }

        #endregion

        #region RejectAsync Tests

        [Fact]
        public async Task RejectAsync_PendingMetrics_SetsStatusToRejected()
        {
            // Arrange
            var metricsId = Guid.NewGuid();
            var reviewerId = Guid.NewGuid();
            var rejectionReason = "Values seem unrealistic";
            var metrics = new EventAttendeeMetricsBuilder()
                .WithId(metricsId)
                .AsPending()
                .Build();

            _metricsRepository.SetupGetAsync(metrics);

            // Act
            var result = await _sut.RejectAsync(metricsId, rejectionReason, reviewerId);

            // Assert
            Assert.True(result.IsSuccess);
            Assert.Equal("Rejected", result.Data.Status);
            Assert.Equal(rejectionReason, result.Data.RejectionReason);
            Assert.Equal(reviewerId, result.Data.ReviewedByUserId);
        }

        [Fact]
        public async Task RejectAsync_MetricsNotFound_ReturnsFailure()
        {
            // Arrange
            var metricsId = Guid.NewGuid();
            var reviewerId = Guid.NewGuid();

            _metricsRepository.Setup(r => r.GetAsync(metricsId, It.IsAny<CancellationToken>()))
                .ReturnsAsync((EventAttendeeMetrics)null);

            // Act
            var result = await _sut.RejectAsync(metricsId, "reason", reviewerId);

            // Assert
            Assert.False(result.IsSuccess);
            Assert.Equal("Metrics submission not found.", result.ErrorMessage);
        }

        [Fact]
        public async Task RejectAsync_AlreadyRejected_ReturnsFailure()
        {
            // Arrange
            var metricsId = Guid.NewGuid();
            var reviewerId = Guid.NewGuid();
            var metrics = new EventAttendeeMetricsBuilder()
                .WithId(metricsId)
                .AsRejected("Previous reason")
                .Build();

            _metricsRepository.SetupGetAsync(metrics);

            // Act
            var result = await _sut.RejectAsync(metricsId, "New reason", reviewerId);

            // Assert
            Assert.False(result.IsSuccess);
            Assert.Contains("Only pending submissions", result.ErrorMessage);
        }

        #endregion

        #region AdjustAsync Tests

        [Fact]
        public async Task AdjustAsync_PendingMetrics_SetsStatusToAdjusted()
        {
            // Arrange
            var metricsId = Guid.NewGuid();
            var reviewerId = Guid.NewGuid();
            var metrics = new EventAttendeeMetricsBuilder()
                .WithId(metricsId)
                .WithBagsCollected(5)
                .WithWeightInPounds(10m)
                .AsPending()
                .Build();

            // The AdjustAsync method uses AdjustedBagsCollected if set, otherwise BagsCollected
            var adjustedValues = new EventAttendeeMetrics
            {
                AdjustedBagsCollected = 3,
                AdjustedPickedWeight = 8m,
                AdjustedPickedWeightUnitId = 1
            };

            _metricsRepository.SetupGetAsync(metrics);

            // Act
            var result = await _sut.AdjustAsync(metricsId, adjustedValues, "Values adjusted after review", reviewerId);

            // Assert
            Assert.True(result.IsSuccess);
            Assert.Equal("Adjusted", result.Data.Status);
            Assert.Equal(3, result.Data.AdjustedBagsCollected);
            Assert.Equal("Values adjusted after review", result.Data.AdjustmentReason);
            Assert.Equal(reviewerId, result.Data.ReviewedByUserId);
        }

        [Fact]
        public async Task AdjustAsync_MetricsNotFound_ReturnsFailure()
        {
            // Arrange
            var metricsId = Guid.NewGuid();
            var reviewerId = Guid.NewGuid();
            var adjustedValues = new EventAttendeeMetricsBuilder().Build();

            _metricsRepository.Setup(r => r.GetAsync(metricsId, It.IsAny<CancellationToken>()))
                .ReturnsAsync((EventAttendeeMetrics)null);

            // Act
            var result = await _sut.AdjustAsync(metricsId, adjustedValues, "reason", reviewerId);

            // Assert
            Assert.False(result.IsSuccess);
            Assert.Equal("Metrics submission not found.", result.ErrorMessage);
        }

        [Fact]
        public async Task AdjustAsync_AlreadyAdjusted_ReturnsFailure()
        {
            // Arrange
            var metricsId = Guid.NewGuid();
            var reviewerId = Guid.NewGuid();
            var metrics = new EventAttendeeMetricsBuilder()
                .WithId(metricsId)
                .AsAdjusted(bags: 3)
                .Build();
            var adjustedValues = new EventAttendeeMetricsBuilder().Build();

            _metricsRepository.SetupGetAsync(metrics);

            // Act
            var result = await _sut.AdjustAsync(metricsId, adjustedValues, "New adjustment", reviewerId);

            // Assert
            Assert.False(result.IsSuccess);
            Assert.Contains("Only pending submissions", result.ErrorMessage);
        }

        #endregion

        #region ApproveAllPendingAsync Tests

        [Fact]
        public async Task ApproveAllPendingAsync_WithPendingMetrics_ApprovesAll()
        {
            // Arrange
            var eventId = Guid.NewGuid();
            var reviewerId = Guid.NewGuid();

            var pendingMetrics1 = new EventAttendeeMetricsBuilder()
                .ForEvent(eventId)
                .AsPending()
                .Build();
            var pendingMetrics2 = new EventAttendeeMetricsBuilder()
                .ForEvent(eventId)
                .AsPending()
                .Build();

            _metricsRepository.SetupGet(new List<EventAttendeeMetrics> { pendingMetrics1, pendingMetrics2 });

            // Act
            var result = await _sut.ApproveAllPendingAsync(eventId, reviewerId);

            // Assert
            Assert.True(result.IsSuccess);
            Assert.Equal(2, result.Data);
            _metricsRepository.Verify(r => r.UpdateAsync(It.IsAny<EventAttendeeMetrics>()), Times.Exactly(2));
        }

        [Fact]
        public async Task ApproveAllPendingAsync_NoPendingMetrics_ReturnsZero()
        {
            // Arrange
            var eventId = Guid.NewGuid();
            var reviewerId = Guid.NewGuid();

            _metricsRepository.SetupGet(new List<EventAttendeeMetrics>());

            // Act
            var result = await _sut.ApproveAllPendingAsync(eventId, reviewerId);

            // Assert
            Assert.True(result.IsSuccess);
            Assert.Equal(0, result.Data);
        }

        #endregion

        #region CalculateTotalsAsync Tests

        [Fact]
        public async Task CalculateTotalsAsync_WithApprovedMetrics_CalculatesCorrectTotals()
        {
            // Arrange
            var eventId = Guid.NewGuid();

            var approvedMetrics1 = new EventAttendeeMetricsBuilder()
                .ForEvent(eventId)
                .WithBagsCollected(5)
                .WithWeightInPounds(10m)
                .WithDuration(60)
                .AsApproved()
                .Build();
            var approvedMetrics2 = new EventAttendeeMetricsBuilder()
                .ForEvent(eventId)
                .WithBagsCollected(3)
                .WithWeightInPounds(5m)
                .WithDuration(45)
                .AsApproved()
                .Build();
            var pendingMetrics = new EventAttendeeMetricsBuilder()
                .ForEvent(eventId)
                .WithBagsCollected(10)
                .AsPending()
                .Build();

            _metricsRepository.SetupGet(new List<EventAttendeeMetrics> { approvedMetrics1, approvedMetrics2, pendingMetrics });

            // Act
            var result = await _sut.CalculateTotalsAsync(eventId);

            // Assert
            Assert.Equal(3, result.TotalSubmissions);
            Assert.Equal(2, result.ApprovedSubmissions);
            Assert.Equal(1, result.PendingSubmissions);
            Assert.Equal(8, result.TotalBagsCollected); // 5 + 3 from approved only
            Assert.Equal(15m, result.TotalWeightPounds); // 10 + 5 from approved only
            Assert.Equal(105, result.TotalDurationMinutes); // 60 + 45 from approved only
        }

        [Fact]
        public async Task CalculateTotalsAsync_WithKilogramWeights_ConvertsToLbs()
        {
            // Arrange
            var eventId = Guid.NewGuid();

            var metricsInKg = new EventAttendeeMetricsBuilder()
                .ForEvent(eventId)
                .WithBagsCollected(5)
                .WithWeightInKilograms(10m) // 10 kg = ~22.0462 lbs
                .WithDuration(60)
                .AsApproved()
                .Build();

            _metricsRepository.SetupGet(new List<EventAttendeeMetrics> { metricsInKg });

            // Act
            var result = await _sut.CalculateTotalsAsync(eventId);

            // Assert
            Assert.Equal(1, result.ApprovedSubmissions);
            // 10 kg * 2.20462 = 22.0462 lbs
            Assert.Equal(10m * KgToLbsConversion, result.TotalWeightPounds);
        }

        [Fact]
        public async Task CalculateTotalsAsync_WithAdjustedMetrics_UsesAdjustedValues()
        {
            // Arrange
            var eventId = Guid.NewGuid();

            var adjustedMetrics = new EventAttendeeMetricsBuilder()
                .ForEvent(eventId)
                .WithBagsCollected(10) // Original value
                .WithWeightInPounds(20m) // Original value
                .WithDuration(120) // Original value
                .AsAdjusted(bags: 5, weight: 10m, duration: 60) // Adjusted values
                .Build();

            _metricsRepository.SetupGet(new List<EventAttendeeMetrics> { adjustedMetrics });

            // Act
            var result = await _sut.CalculateTotalsAsync(eventId);

            // Assert
            Assert.Equal(1, result.ApprovedSubmissions);
            Assert.Equal(5, result.TotalBagsCollected); // Uses adjusted value
            Assert.Equal(10m, result.TotalWeightPounds); // Uses adjusted value
            Assert.Equal(60, result.TotalDurationMinutes); // Uses adjusted value
        }

        [Fact]
        public async Task CalculateTotalsAsync_NoMetrics_ReturnsZeros()
        {
            // Arrange
            var eventId = Guid.NewGuid();
            _metricsRepository.SetupGet(new List<EventAttendeeMetrics>());

            // Act
            var result = await _sut.CalculateTotalsAsync(eventId);

            // Assert
            Assert.Equal(0, result.TotalSubmissions);
            Assert.Equal(0, result.ApprovedSubmissions);
            Assert.Equal(0, result.TotalBagsCollected);
            Assert.Equal(0, result.TotalWeightPounds);
            Assert.Equal(0, result.TotalDurationMinutes);
        }

        [Fact]
        public async Task CalculateTotalsAsync_WithNullValues_HandlesGracefully()
        {
            // Arrange
            var eventId = Guid.NewGuid();

            var metricsWithNulls = new EventAttendeeMetricsBuilder()
                .ForEvent(eventId)
                .AsApproved()
                .Build();
            metricsWithNulls.BagsCollected = null;
            metricsWithNulls.PickedWeight = null;
            metricsWithNulls.DurationMinutes = null;

            _metricsRepository.SetupGet(new List<EventAttendeeMetrics> { metricsWithNulls });

            // Act
            var result = await _sut.CalculateTotalsAsync(eventId);

            // Assert
            Assert.Equal(1, result.ApprovedSubmissions);
            Assert.Equal(0, result.TotalBagsCollected);
            Assert.Equal(0, result.TotalWeightPounds);
            Assert.Equal(0, result.TotalDurationMinutes);
        }

        #endregion

        #region HasSubmittedMetricsAsync Tests

        [Fact]
        public async Task HasSubmittedMetricsAsync_WithExistingMetrics_ReturnsTrue()
        {
            // Arrange
            var eventId = Guid.NewGuid();
            var userId = Guid.NewGuid();

            var existingMetrics = new EventAttendeeMetricsBuilder()
                .ForEvent(eventId)
                .ForUser(userId)
                .Build();

            _metricsRepository.SetupGet(new List<EventAttendeeMetrics> { existingMetrics });

            // Act
            var result = await _sut.HasSubmittedMetricsAsync(eventId, userId);

            // Assert
            Assert.True(result);
        }

        [Fact]
        public async Task HasSubmittedMetricsAsync_NoExistingMetrics_ReturnsFalse()
        {
            // Arrange
            var eventId = Guid.NewGuid();
            var userId = Guid.NewGuid();

            _metricsRepository.SetupGet(new List<EventAttendeeMetrics>());

            // Act
            var result = await _sut.HasSubmittedMetricsAsync(eventId, userId);

            // Assert
            Assert.False(result);
        }

        #endregion

        #region GetPublicMetricsSummaryAsync Tests

        [Fact]
        public async Task GetPublicMetricsSummaryAsync_WithApprovedMetrics_ReturnsCorrectSummary()
        {
            // Arrange
            var eventId = Guid.NewGuid();
            var user1 = new UserBuilder().WithId(Guid.NewGuid()).WithUserName("User1").Build();
            var user2 = new UserBuilder().WithId(Guid.NewGuid()).WithUserName("User2").Build();

            var metrics1 = new EventAttendeeMetricsBuilder()
                .ForEvent(eventId)
                .WithUser(user1)
                .WithBagsCollected(5)
                .WithWeightInPounds(10m)
                .WithDuration(60)
                .AsApproved()
                .Build();
            var metrics2 = new EventAttendeeMetricsBuilder()
                .ForEvent(eventId)
                .WithUser(user2)
                .WithBagsCollected(3)
                .WithWeightInPounds(5m)
                .WithDuration(45)
                .AsApproved()
                .Build();

            _metricsRepository.SetupGet(new List<EventAttendeeMetrics> { metrics1, metrics2 });

            // Act
            var result = await _sut.GetPublicMetricsSummaryAsync(eventId);

            // Assert
            Assert.Equal(eventId, result.EventId);
            Assert.Equal(2, result.ContributorCount);
            Assert.Equal(8, result.TotalBagsCollected);
            Assert.Equal(15m, result.TotalWeightPounds);
            Assert.Equal(105, result.TotalDurationMinutes);
            Assert.Equal(2, result.Contributors.Count);
        }

        [Fact]
        public async Task GetPublicMetricsSummaryAsync_SortsContributorsByBagsThenWeight()
        {
            // Arrange
            var eventId = Guid.NewGuid();
            var user1 = new UserBuilder().WithId(Guid.NewGuid()).WithUserName("LowBags").Build();
            var user2 = new UserBuilder().WithId(Guid.NewGuid()).WithUserName("HighBags").Build();

            var metricsLowBags = new EventAttendeeMetricsBuilder()
                .ForEvent(eventId)
                .WithUser(user1)
                .WithBagsCollected(2)
                .WithWeightInPounds(20m)
                .AsApproved()
                .Build();
            var metricsHighBags = new EventAttendeeMetricsBuilder()
                .ForEvent(eventId)
                .WithUser(user2)
                .WithBagsCollected(10)
                .WithWeightInPounds(5m)
                .AsApproved()
                .Build();

            _metricsRepository.SetupGet(new List<EventAttendeeMetrics> { metricsLowBags, metricsHighBags });

            // Act
            var result = await _sut.GetPublicMetricsSummaryAsync(eventId);

            // Assert
            Assert.Equal("HighBags", result.Contributors[0].UserName);
            Assert.Equal("LowBags", result.Contributors[1].UserName);
        }

        #endregion

        #region GetUserImpactStatsAsync Tests

        [Fact]
        public async Task GetUserImpactStatsAsync_WithApprovedMetrics_CalculatesCorrectStats()
        {
            // Arrange
            var userId = Guid.NewGuid();
            var event1 = new EventBuilder().WithId(Guid.NewGuid()).WithName("Event 1").InThePast().Build();
            var event2 = new EventBuilder().WithId(Guid.NewGuid()).WithName("Event 2").InThePast().Build();

            var metrics1 = new EventAttendeeMetricsBuilder()
                .ForUser(userId)
                .WithEvent(event1)
                .WithBagsCollected(5)
                .WithWeightInPounds(10m)
                .WithDuration(60)
                .AsApproved()
                .Build();
            var metrics2 = new EventAttendeeMetricsBuilder()
                .ForUser(userId)
                .WithEvent(event2)
                .WithBagsCollected(3)
                .WithWeightInKilograms(5m) // 5 kg = ~11.02 lbs
                .WithDuration(45)
                .AsApproved()
                .Build();

            _metricsRepository.SetupGet(new List<EventAttendeeMetrics> { metrics1, metrics2 });

            // Act
            var result = await _sut.GetUserImpactStatsAsync(userId);

            // Assert
            Assert.Equal(2, result.EventsWithMetrics);
            Assert.Equal(8, result.TotalBagsCollected);
            Assert.Equal(10m + (5m * KgToLbsConversion), result.TotalWeightPounds);
            Assert.Equal(105, result.TotalDurationMinutes);
            Assert.Equal(2, result.EventBreakdown.Count);
        }

        [Fact]
        public async Task GetUserImpactStatsAsync_NoApprovedMetrics_ReturnsEmptyStats()
        {
            // Arrange
            var userId = Guid.NewGuid();

            var pendingMetrics = new EventAttendeeMetricsBuilder()
                .ForUser(userId)
                .AsPending()
                .Build();

            // Only return approved metrics (none for this user)
            _metricsRepository.SetupGet(new List<EventAttendeeMetrics>());

            // Act
            var result = await _sut.GetUserImpactStatsAsync(userId);

            // Assert
            Assert.Equal(0, result.EventsWithMetrics);
            Assert.Equal(0, result.TotalBagsCollected);
            Assert.Equal(0, result.TotalWeightPounds);
        }

        [Fact]
        public async Task GetUserImpactStatsAsync_WithAdjustedMetrics_UsesAdjustedValues()
        {
            // Arrange
            var userId = Guid.NewGuid();
            var eventEntity = new EventBuilder().WithId(Guid.NewGuid()).WithName("Test Event").InThePast().Build();

            var adjustedMetrics = new EventAttendeeMetricsBuilder()
                .ForUser(userId)
                .WithEvent(eventEntity)
                .WithBagsCollected(10) // Original
                .WithWeightInPounds(20m) // Original
                .AsAdjusted(bags: 5, weight: 10m) // Adjusted
                .Build();

            _metricsRepository.SetupGet(new List<EventAttendeeMetrics> { adjustedMetrics });

            // Act
            var result = await _sut.GetUserImpactStatsAsync(userId);

            // Assert
            Assert.Equal(1, result.EventsWithMetrics);
            Assert.Equal(5, result.TotalBagsCollected); // Uses adjusted value
            Assert.Equal(10m, result.TotalWeightPounds); // Uses adjusted value
        }

        #endregion
    }

}
