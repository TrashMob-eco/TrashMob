namespace TrashMob.Shared.Tests.Controllers.V2
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading;
    using System.Threading.Tasks;
    using Microsoft.AspNetCore.Authorization;
    using Microsoft.AspNetCore.Http;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.Extensions.Logging;
    using Moq;
    using TrashMob.Controllers.V2;
    using TrashMob.Models;
    using TrashMob.Models.Poco;
    using TrashMob.Models.Poco.V2;
    using TrashMob.Shared.Managers.Interfaces;
    using TrashMob.Shared.Poco;
    using Xunit;

    public class EventAttendeeMetricsV2ControllerTests
    {
        private readonly Mock<IEventAttendeeMetricsManager> metricsManager = new();
        private readonly Mock<IKeyedManager<Event>> eventManager = new();
        private readonly Mock<IAuthorizationService> authorizationService = new();
        private readonly Mock<ILogger<EventAttendeeMetricsV2Controller>> logger = new();
        private readonly EventAttendeeMetricsV2Controller controller;
        private readonly Guid userId = Guid.NewGuid();

        public EventAttendeeMetricsV2ControllerTests()
        {
            controller = new EventAttendeeMetricsV2Controller(
                metricsManager.Object, eventManager.Object, authorizationService.Object, logger.Object);
            controller.ControllerContext = new ControllerContext
            {
                HttpContext = new DefaultHttpContext(),
            };
            controller.HttpContext.Items["UserId"] = userId.ToString();
        }

        [Fact]
        public async Task GetMyMetrics_ReturnsOk_WhenFound()
        {
            var eventId = Guid.NewGuid();
            var metrics = new EventAttendeeMetrics
            {
                Id = Guid.NewGuid(),
                EventId = eventId,
                UserId = userId,
                BagsCollected = 3,
                Status = "Pending",
            };

            metricsManager.Setup(m => m.GetMyMetricsAsync(eventId, userId, It.IsAny<CancellationToken>()))
                .ReturnsAsync(metrics);

            var result = await controller.GetMyMetrics(eventId, CancellationToken.None);

            var okResult = Assert.IsType<OkObjectResult>(result);
            var dto = Assert.IsType<EventAttendeeMetricsDto>(okResult.Value);
            Assert.Equal(3, dto.BagsCollected);
        }

        [Fact]
        public async Task GetMyMetrics_ReturnsNotFound_WhenMissing()
        {
            metricsManager.Setup(m => m.GetMyMetricsAsync(It.IsAny<Guid>(), userId, It.IsAny<CancellationToken>()))
                .ReturnsAsync((EventAttendeeMetrics)null);

            var result = await controller.GetMyMetrics(Guid.NewGuid(), CancellationToken.None);

            var objectResult = Assert.IsType<ObjectResult>(result);
            Assert.Equal(StatusCodes.Status404NotFound, objectResult.StatusCode);
        }

        [Fact]
        public async Task SubmitMyMetrics_ReturnsOk_OnSuccess()
        {
            var eventId = Guid.NewGuid();
            var metricsDto = new EventAttendeeMetricsDto { BagsCollected = 5 };
            var resultMetrics = new EventAttendeeMetrics
            {
                Id = Guid.NewGuid(),
                EventId = eventId,
                UserId = userId,
                BagsCollected = 5,
                Status = "Pending",
            };

            metricsManager.Setup(m => m.SubmitMetricsAsync(eventId, userId, It.IsAny<EventAttendeeMetrics>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(ServiceResult<EventAttendeeMetrics>.Success(resultMetrics));

            var result = await controller.SubmitMyMetrics(eventId, metricsDto, CancellationToken.None);

            var okResult = Assert.IsType<OkObjectResult>(result);
            var dto = Assert.IsType<EventAttendeeMetricsDto>(okResult.Value);
            Assert.Equal(5, dto.BagsCollected);
        }

        [Fact]
        public async Task SubmitMyMetrics_ReturnsBadRequest_OnFailure()
        {
            var eventId = Guid.NewGuid();
            metricsManager.Setup(m => m.SubmitMetricsAsync(eventId, userId, It.IsAny<EventAttendeeMetrics>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(ServiceResult<EventAttendeeMetrics>.Failure("Event not found"));

            var result = await controller.SubmitMyMetrics(eventId, new EventAttendeeMetricsDto(), CancellationToken.None);

            var objectResult = Assert.IsType<ObjectResult>(result);
            Assert.Equal(StatusCodes.Status400BadRequest, objectResult.StatusCode);
        }

        [Fact]
        public async Task GetPublicMetrics_ReturnsOk()
        {
            var eventId = Guid.NewGuid();
            eventManager.Setup(m => m.GetAsync(eventId, It.IsAny<CancellationToken>()))
                .ReturnsAsync(new Event { Id = eventId });
            metricsManager.Setup(m => m.GetPublicMetricsSummaryAsync(eventId, It.IsAny<CancellationToken>()))
                .ReturnsAsync(new EventMetricsPublicSummary { TotalBagsCollected = 50 });

            var result = await controller.GetPublicMetrics(eventId, CancellationToken.None);

            var okResult = Assert.IsType<OkObjectResult>(result);
            var summary = Assert.IsType<EventMetricsPublicSummary>(okResult.Value);
            Assert.Equal(50, summary.TotalBagsCollected);
        }

        [Fact]
        public async Task GetPublicMetrics_ReturnsNotFound_WhenEventMissing()
        {
            eventManager.Setup(m => m.GetAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync((Event)null);

            var result = await controller.GetPublicMetrics(Guid.NewGuid(), CancellationToken.None);

            var objectResult = Assert.IsType<ObjectResult>(result);
            Assert.Equal(StatusCodes.Status404NotFound, objectResult.StatusCode);
        }
    }
}
