namespace TrashMob.Shared.Tests.Managers.Events
{
    using System;
    using System.Collections.Generic;
    using System.Threading;
    using System.Threading.Tasks;
    using Microsoft.Extensions.Logging;
    using Moq;
    using TrashMob.Models;
    using TrashMob.Shared.Managers.Events;
    using TrashMob.Shared.Managers.Interfaces;
    using TrashMob.Shared.Poco;
    using TrashMob.Shared.Tests.Builders;
    using Xunit;

    /// <summary>
    /// Unit tests for <see cref="ParticipationReportService"/>.
    /// </summary>
    public class ParticipationReportServiceTests
    {
        private readonly Mock<IEventManager> _eventManager = new();
        private readonly Mock<IEventAttendeeMetricsManager> _metricsManager = new();
        private readonly Mock<IEventAttendeeManager> _attendeeManager = new();
        private readonly Mock<IUserManager> _userManager = new();
        private readonly Mock<IEmailManager> _emailManager = new();
        private readonly Mock<ILogger<ParticipationReportService>> _logger = new();
        private readonly ParticipationReportService _sut;

        public ParticipationReportServiceTests()
        {
            _emailManager.Setup(e => e.GetHtmlEmailCopy(It.IsAny<string>()))
                .Returns("<p>{eventName} {eventDate} {duration} {bagsCollected} {weightPicked} {verifiedBy}</p>");
            _emailManager.Setup(e => e.SendTemplatedEmailAsync(
                    It.IsAny<string>(), It.IsAny<string>(), It.IsAny<int>(),
                    It.IsAny<object>(), It.IsAny<List<EmailAddress>>(),
                    It.IsAny<List<EmailAttachment>>(), It.IsAny<CancellationToken>()))
                .Returns(Task.CompletedTask);

            _sut = new ParticipationReportService(
                _eventManager.Object,
                _metricsManager.Object,
                _attendeeManager.Object,
                _userManager.Object,
                _emailManager.Object,
                _logger.Object);
        }

        #region SendReportAsync Tests

        [Fact]
        public async Task SendReportAsync_WithApprovedMetrics_SendsEmailWithPdf()
        {
            // Arrange
            var eventId = Guid.NewGuid();
            var userId = Guid.NewGuid();
            var reviewerId = Guid.NewGuid();

            var evt = new EventBuilder().WithId(eventId).WithName("Beach Cleanup").Build();
            var user = new UserBuilder().WithId(userId).WithEmail("volunteer@test.com").Build();
            var reviewer = new UserBuilder().WithId(reviewerId).WithUserName("leaduser").Build();
            var metrics = new EventAttendeeMetricsBuilder()
                .ForEvent(eventId).ForUser(userId)
                .AsApproved(reviewerId)
                .Build();

            _eventManager.Setup(e => e.GetAsync(eventId, It.IsAny<CancellationToken>())).ReturnsAsync(evt);
            _metricsManager.Setup(m => m.GetMyMetricsAsync(eventId, userId, It.IsAny<CancellationToken>())).ReturnsAsync(metrics);
            _userManager.Setup(u => u.GetUserByInternalIdAsync(userId, It.IsAny<CancellationToken>())).ReturnsAsync(user);
            _userManager.Setup(u => u.GetUserByInternalIdAsync(reviewerId, It.IsAny<CancellationToken>())).ReturnsAsync(reviewer);

            // Act
            var result = await _sut.SendReportAsync(eventId, userId, CancellationToken.None);

            // Assert
            Assert.True(result.IsSuccess);
            _emailManager.Verify(e => e.SendTemplatedEmailAsync(
                It.Is<string>(s => s.Contains("Participation Report")),
                It.IsAny<string>(), It.IsAny<int>(), It.IsAny<object>(),
                It.Is<List<EmailAddress>>(r => r[0].Email == "volunteer@test.com"),
                It.Is<List<EmailAttachment>>(a => a.Count == 1 && a[0].MimeType == "application/pdf"),
                It.IsAny<CancellationToken>()), Times.Once);
        }

        [Fact]
        public async Task SendReportAsync_WithPendingMetrics_ReturnsFailure()
        {
            // Arrange
            var eventId = Guid.NewGuid();
            var userId = Guid.NewGuid();

            var evt = new EventBuilder().WithId(eventId).Build();
            var metrics = new EventAttendeeMetricsBuilder()
                .ForEvent(eventId).ForUser(userId)
                .AsPending()
                .Build();

            _eventManager.Setup(e => e.GetAsync(eventId, It.IsAny<CancellationToken>())).ReturnsAsync(evt);
            _metricsManager.Setup(m => m.GetMyMetricsAsync(eventId, userId, It.IsAny<CancellationToken>())).ReturnsAsync(metrics);

            // Act
            var result = await _sut.SendReportAsync(eventId, userId, CancellationToken.None);

            // Assert
            Assert.False(result.IsSuccess);
            Assert.Contains("not been approved", result.ErrorMessage);
        }

        [Fact]
        public async Task SendReportAsync_WithRejectedMetrics_ReturnsFailure()
        {
            // Arrange
            var eventId = Guid.NewGuid();
            var userId = Guid.NewGuid();

            var evt = new EventBuilder().WithId(eventId).Build();
            var metrics = new EventAttendeeMetricsBuilder()
                .ForEvent(eventId).ForUser(userId)
                .AsRejected("Inaccurate")
                .Build();

            _eventManager.Setup(e => e.GetAsync(eventId, It.IsAny<CancellationToken>())).ReturnsAsync(evt);
            _metricsManager.Setup(m => m.GetMyMetricsAsync(eventId, userId, It.IsAny<CancellationToken>())).ReturnsAsync(metrics);

            // Act
            var result = await _sut.SendReportAsync(eventId, userId, CancellationToken.None);

            // Assert
            Assert.False(result.IsSuccess);
        }

        [Fact]
        public async Task SendReportAsync_WithNoMetrics_ReturnsFailure()
        {
            // Arrange
            var eventId = Guid.NewGuid();
            var userId = Guid.NewGuid();

            var evt = new EventBuilder().WithId(eventId).Build();

            _eventManager.Setup(e => e.GetAsync(eventId, It.IsAny<CancellationToken>())).ReturnsAsync(evt);
            _metricsManager.Setup(m => m.GetMyMetricsAsync(eventId, userId, It.IsAny<CancellationToken>())).ReturnsAsync((EventAttendeeMetrics)null);

            // Act
            var result = await _sut.SendReportAsync(eventId, userId, CancellationToken.None);

            // Assert
            Assert.False(result.IsSuccess);
            Assert.Contains("No metrics", result.ErrorMessage);
        }

        [Fact]
        public async Task SendReportAsync_WithNonExistentEvent_ReturnsFailure()
        {
            // Arrange
            var eventId = Guid.NewGuid();
            var userId = Guid.NewGuid();

            _eventManager.Setup(e => e.GetAsync(eventId, It.IsAny<CancellationToken>())).ReturnsAsync((Event)null);

            // Act
            var result = await _sut.SendReportAsync(eventId, userId, CancellationToken.None);

            // Assert
            Assert.False(result.IsSuccess);
            Assert.Contains("Event not found", result.ErrorMessage);
        }

        [Fact]
        public async Task SendReportAsync_WithAdjustedMetrics_SendsEmail()
        {
            // Arrange
            var eventId = Guid.NewGuid();
            var userId = Guid.NewGuid();
            var reviewerId = Guid.NewGuid();

            var evt = new EventBuilder().WithId(eventId).WithName("Park Cleanup").Build();
            var user = new UserBuilder().WithId(userId).WithEmail("user@test.com").Build();
            var reviewer = new UserBuilder().WithId(reviewerId).Build();
            var metrics = new EventAttendeeMetricsBuilder()
                .ForEvent(eventId).ForUser(userId)
                .AsAdjusted(bags: 3, weight: 8.0m, duration: 90, reviewerId: reviewerId)
                .Build();

            _eventManager.Setup(e => e.GetAsync(eventId, It.IsAny<CancellationToken>())).ReturnsAsync(evt);
            _metricsManager.Setup(m => m.GetMyMetricsAsync(eventId, userId, It.IsAny<CancellationToken>())).ReturnsAsync(metrics);
            _userManager.Setup(u => u.GetUserByInternalIdAsync(userId, It.IsAny<CancellationToken>())).ReturnsAsync(user);
            _userManager.Setup(u => u.GetUserByInternalIdAsync(reviewerId, It.IsAny<CancellationToken>())).ReturnsAsync(reviewer);

            // Act
            var result = await _sut.SendReportAsync(eventId, userId, CancellationToken.None);

            // Assert
            Assert.True(result.IsSuccess);
            _emailManager.Verify(e => e.SendTemplatedEmailAsync(
                It.IsAny<string>(), It.IsAny<string>(), It.IsAny<int>(), It.IsAny<object>(),
                It.IsAny<List<EmailAddress>>(), It.IsAny<List<EmailAttachment>>(),
                It.IsAny<CancellationToken>()), Times.Once);
        }

        #endregion

        #region SendAllReportsAsync Tests

        [Fact]
        public async Task SendAllReportsAsync_AsEventLead_SendsToAllApproved()
        {
            // Arrange
            var eventId = Guid.NewGuid();
            var leadId = Guid.NewGuid();
            var user1Id = Guid.NewGuid();
            var user2Id = Guid.NewGuid();

            var evt = new EventBuilder().WithId(eventId).WithName("River Cleanup").Build();
            var user1 = new UserBuilder().WithId(user1Id).WithEmail("user1@test.com").Build();
            var user2 = new UserBuilder().WithId(user2Id).WithEmail("user2@test.com").Build();
            var reviewer = new UserBuilder().WithId(leadId).Build();

            var metrics = new List<EventAttendeeMetrics>
            {
                new EventAttendeeMetricsBuilder().ForEvent(eventId).ForUser(user1Id).AsApproved(leadId).Build(),
                new EventAttendeeMetricsBuilder().ForEvent(eventId).ForUser(user2Id).AsApproved(leadId).Build(),
                new EventAttendeeMetricsBuilder().ForEvent(eventId).ForUser(Guid.NewGuid()).AsPending().Build(),
            };

            _attendeeManager.Setup(a => a.IsEventLeadAsync(eventId, leadId, It.IsAny<CancellationToken>())).ReturnsAsync(true);
            _eventManager.Setup(e => e.GetAsync(eventId, It.IsAny<CancellationToken>())).ReturnsAsync(evt);
            _metricsManager.Setup(m => m.GetByEventIdAsync(eventId, It.IsAny<CancellationToken>())).ReturnsAsync(metrics);
            _userManager.Setup(u => u.GetUserByInternalIdAsync(user1Id, It.IsAny<CancellationToken>())).ReturnsAsync(user1);
            _userManager.Setup(u => u.GetUserByInternalIdAsync(user2Id, It.IsAny<CancellationToken>())).ReturnsAsync(user2);
            _userManager.Setup(u => u.GetUserByInternalIdAsync(leadId, It.IsAny<CancellationToken>())).ReturnsAsync(reviewer);

            // Act
            var result = await _sut.SendAllReportsAsync(eventId, leadId, CancellationToken.None);

            // Assert
            Assert.True(result.IsSuccess);
            Assert.Equal(2, result.Data);
            _emailManager.Verify(e => e.SendTemplatedEmailAsync(
                It.IsAny<string>(), It.IsAny<string>(), It.IsAny<int>(), It.IsAny<object>(),
                It.IsAny<List<EmailAddress>>(), It.IsAny<List<EmailAttachment>>(),
                It.IsAny<CancellationToken>()), Times.Exactly(2));
        }

        [Fact]
        public async Task SendAllReportsAsync_AsNonLead_ReturnsFailure()
        {
            // Arrange
            var eventId = Guid.NewGuid();
            var userId = Guid.NewGuid();

            _attendeeManager.Setup(a => a.IsEventLeadAsync(eventId, userId, It.IsAny<CancellationToken>())).ReturnsAsync(false);

            // Act
            var result = await _sut.SendAllReportsAsync(eventId, userId, CancellationToken.None);

            // Assert
            Assert.False(result.IsSuccess);
            Assert.Contains("event lead", result.ErrorMessage, StringComparison.OrdinalIgnoreCase);
        }

        [Fact]
        public async Task SendAllReportsAsync_WithNoApprovedMetrics_ReturnsFailure()
        {
            // Arrange
            var eventId = Guid.NewGuid();
            var leadId = Guid.NewGuid();

            var evt = new EventBuilder().WithId(eventId).Build();
            var metrics = new List<EventAttendeeMetrics>
            {
                new EventAttendeeMetricsBuilder().ForEvent(eventId).AsPending().Build(),
            };

            _attendeeManager.Setup(a => a.IsEventLeadAsync(eventId, leadId, It.IsAny<CancellationToken>())).ReturnsAsync(true);
            _eventManager.Setup(e => e.GetAsync(eventId, It.IsAny<CancellationToken>())).ReturnsAsync(evt);
            _metricsManager.Setup(m => m.GetByEventIdAsync(eventId, It.IsAny<CancellationToken>())).ReturnsAsync(metrics);

            // Act
            var result = await _sut.SendAllReportsAsync(eventId, leadId, CancellationToken.None);

            // Assert
            Assert.False(result.IsSuccess);
            Assert.Contains("No approved", result.ErrorMessage);
        }

        #endregion
    }
}
