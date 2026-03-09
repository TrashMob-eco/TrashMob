namespace TrashMob.Shared.Tests.Controllers.V2
{
    using System;
    using System.Collections.Generic;
    using System.Threading;
    using System.Threading.Tasks;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.Extensions.Logging;
    using Moq;
    using TrashMob.Controllers.V2;
    using TrashMob.Models;
    using TrashMob.Models.Poco.V2;
    using TrashMob.Shared.Managers.Interfaces;
    using Xunit;

    public class EventSummaryV2ControllerTests
    {
        private readonly Mock<IEventSummaryManager> eventSummaryManager = new();
        private readonly Mock<ILogger<EventSummaryV2Controller>> logger = new();
        private readonly EventSummaryV2Controller controller;

        public EventSummaryV2ControllerTests()
        {
            controller = new EventSummaryV2Controller(eventSummaryManager.Object, logger.Object);
        }

        [Fact]
        public async Task GetEventSummary_ReturnsSummary_WhenExists()
        {
            var eventId = Guid.NewGuid();
            var summary = new EventSummary
            {
                EventId = eventId,
                NumberOfBuckets = 3,
                NumberOfBags = 10,
                DurationInMinutes = 90,
                ActualNumberOfAttendees = 6,
                PickedWeight = 25.5m,
                PickedWeightUnitId = 1,
                IsFromRouteData = false,
                Notes = "Good turnout",
                CreatedDate = new DateTimeOffset(2026, 3, 1, 10, 0, 0, TimeSpan.Zero),
                LastUpdatedDate = new DateTimeOffset(2026, 3, 1, 12, 0, 0, TimeSpan.Zero),
            };

            eventSummaryManager
                .Setup(m => m.GetAsync(It.IsAny<System.Linq.Expressions.Expression<System.Func<EventSummary, bool>>>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(new List<EventSummary> { summary });

            var result = await controller.GetEventSummary(eventId, CancellationToken.None);

            var okResult = Assert.IsType<OkObjectResult>(result);
            var dto = Assert.IsType<EventSummaryDto>(okResult.Value);
            Assert.Equal(eventId, dto.EventId);
            Assert.Equal(3, dto.NumberOfBuckets);
            Assert.Equal(10, dto.NumberOfBags);
            Assert.Equal(90, dto.DurationInMinutes);
            Assert.Equal(6, dto.ActualNumberOfAttendees);
            Assert.Equal(25.5m, dto.PickedWeight);
            Assert.Equal(1, dto.PickedWeightUnitId);
            Assert.False(dto.IsFromRouteData);
            Assert.Equal("Good turnout", dto.Notes);
        }

        [Fact]
        public async Task GetEventSummary_ReturnsEmptySummary_WhenNoneExists()
        {
            var eventId = Guid.NewGuid();

            eventSummaryManager
                .Setup(m => m.GetAsync(It.IsAny<System.Linq.Expressions.Expression<System.Func<EventSummary, bool>>>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(new List<EventSummary>());

            var result = await controller.GetEventSummary(eventId, CancellationToken.None);

            var okResult = Assert.IsType<OkObjectResult>(result);
            var dto = Assert.IsType<EventSummaryDto>(okResult.Value);
            Assert.Equal(eventId, dto.EventId);
            Assert.Equal(0, dto.NumberOfBags);
            Assert.Equal(0, dto.NumberOfBuckets);
            Assert.Equal(0, dto.DurationInMinutes);
            Assert.Equal(0, dto.ActualNumberOfAttendees);
            Assert.Equal(0m, dto.PickedWeight);
        }
    }
}
