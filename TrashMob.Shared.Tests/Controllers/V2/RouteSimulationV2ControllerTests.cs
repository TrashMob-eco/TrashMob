namespace TrashMob.Shared.Tests.Controllers.V2
{
    using System;
    using System.Security.Claims;
    using System.Threading;
    using System.Threading.Tasks;
    using Microsoft.AspNetCore.Http;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.Extensions.Logging;
    using Moq;
    using TrashMob.Controllers.V2;
    using TrashMob.Models;
    using TrashMob.Shared.Managers.Interfaces;
    using TrashMob.Shared.Persistence.Interfaces;
    using Xunit;

    public class RouteSimulationV2ControllerTests
    {
        private readonly Mock<IEventAttendeeRouteManager> routeManager = new();
        private readonly Mock<IKeyedRepository<Event>> eventRepository = new();
        private readonly Mock<ILogger<RouteSimulationV2Controller>> logger = new();
        private readonly RouteSimulationV2Controller controller;
        private readonly Guid userId = Guid.NewGuid();

        public RouteSimulationV2ControllerTests()
        {
            controller = new RouteSimulationV2Controller(routeManager.Object, eventRepository.Object, logger.Object);

            var httpContext = new DefaultHttpContext
            {
                User = new ClaimsPrincipal(new ClaimsIdentity([], "Bearer")),
            };
            httpContext.Items["UserId"] = userId.ToString();

            controller.ControllerContext = new ControllerContext
            {
                HttpContext = httpContext,
            };
        }

        [Fact]
        public async Task GenerateSimulatedRoute_ReturnsNotFound_WhenEventNotFound()
        {
            var eventId = Guid.NewGuid();

            eventRepository
                .Setup(r => r.GetAsync(eventId, It.IsAny<CancellationToken>()))
                .ReturnsAsync((Event)null);

            controller.ControllerContext.HttpContext.Request.Host = new HostString("localhost");

            var result = await controller.GenerateSimulatedRoute(eventId, cancellationToken: CancellationToken.None);

            Assert.IsType<NotFoundObjectResult>(result);
        }

        [Fact]
        public async Task GenerateSimulatedRoute_ReturnsNotFound_OnProductionHost()
        {
            var eventId = Guid.NewGuid();
            var eventData = new Event { Id = eventId, Name = "Test Event" };

            eventRepository
                .Setup(r => r.GetAsync(eventId, It.IsAny<CancellationToken>()))
                .ReturnsAsync(eventData);

            controller.ControllerContext.HttpContext.Request.Host = new HostString("www.trashmob.eco");

            var result = await controller.GenerateSimulatedRoute(eventId, cancellationToken: CancellationToken.None);

            Assert.IsType<NotFoundResult>(result);
        }
    }
}
