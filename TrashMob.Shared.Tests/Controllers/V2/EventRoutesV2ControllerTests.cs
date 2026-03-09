namespace TrashMob.Shared.Tests.Controllers.V2
{
    using System;
    using System.Collections.Generic;
    using System.Threading;
    using System.Threading.Tasks;
    using Microsoft.AspNetCore.Http;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.Extensions.Logging;
    using Moq;
    using TrashMob.Controllers.V2;
    using TrashMob.Models.Poco;
    using TrashMob.Shared.Managers.Interfaces;
    using Xunit;

    public class EventRoutesV2ControllerTests
    {
        private readonly Mock<IEventAttendeeRouteManager> routeManager = new();
        private readonly Mock<ILogger<EventRoutesV2Controller>> logger = new();
        private readonly EventRoutesV2Controller controller;

        public EventRoutesV2ControllerTests()
        {
            controller = new EventRoutesV2Controller(routeManager.Object, logger.Object);
            controller.ControllerContext = new ControllerContext
            {
                HttpContext = new DefaultHttpContext(),
            };
        }

        [Fact]
        public async Task GetRoutes_ReturnsOk_WithAnonymizedRoutes()
        {
            var eventId = Guid.NewGuid();
            var routes = new List<DisplayAnonymizedRoute>
            {
                new() { Id = Guid.NewGuid(), EventId = eventId, TotalDistanceMeters = 1500 },
            };

            routeManager.Setup(m => m.GetAnonymizedRoutesForEventAsync(eventId, It.IsAny<CancellationToken>()))
                .ReturnsAsync(routes);

            var result = await controller.GetRoutes(eventId, CancellationToken.None);

            var okResult = Assert.IsType<OkObjectResult>(result);
            var dtos = Assert.IsAssignableFrom<List<DisplayAnonymizedRoute>>(okResult.Value);
            Assert.Single(dtos);
        }

        [Fact]
        public async Task GetStats_ReturnsOk()
        {
            var eventId = Guid.NewGuid();
            var stats = new DisplayEventRouteStats
            {
                EventId = eventId,
                TotalRoutes = 5,
                TotalDistanceMeters = 10000,
            };

            routeManager.Setup(m => m.GetEventRouteStatsAsync(eventId, It.IsAny<CancellationToken>()))
                .ReturnsAsync(stats);

            var result = await controller.GetStats(eventId, CancellationToken.None);

            var okResult = Assert.IsType<OkObjectResult>(result);
            var dto = Assert.IsType<DisplayEventRouteStats>(okResult.Value);
            Assert.Equal(5, dto.TotalRoutes);
        }

        [Fact]
        public async Task GetSummaryPrefill_ReturnsOk()
        {
            var eventId = Guid.NewGuid();
            controller.HttpContext.Items["UserId"] = Guid.NewGuid().ToString();

            var prefill = new EventSummaryPrefill
            {
                NumberOfBags = 10,
                PickedWeight = 50.5m,
            };

            routeManager.Setup(m => m.GetEventSummaryPrefillAsync(eventId, 1, It.IsAny<CancellationToken>()))
                .ReturnsAsync(prefill);

            var result = await controller.GetSummaryPrefill(eventId, cancellationToken: CancellationToken.None);

            var okResult = Assert.IsType<OkObjectResult>(result);
            var dto = Assert.IsType<EventSummaryPrefill>(okResult.Value);
            Assert.Equal(10, dto.NumberOfBags);
        }
    }
}
