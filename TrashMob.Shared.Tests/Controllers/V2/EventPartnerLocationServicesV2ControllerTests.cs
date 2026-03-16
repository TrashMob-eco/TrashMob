namespace TrashMob.Shared.Tests.Controllers.V2
{
    using System;
    using System.Collections.Generic;
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
    using Xunit;

    public class EventPartnerLocationServicesV2ControllerTests
    {
        private readonly Mock<IEventPartnerLocationServiceManager> serviceManager = new();
        private readonly Mock<IKeyedManager<Event>> eventManager = new();
        private readonly Mock<IPartnerLocationManager> partnerLocationManager = new();
        private readonly Mock<IAuthorizationService> authorizationService = new();
        private readonly Mock<ILogger<EventPartnerLocationServicesV2Controller>> logger = new();
        private readonly EventPartnerLocationServicesV2Controller controller;

        public EventPartnerLocationServicesV2ControllerTests()
        {
            controller = new EventPartnerLocationServicesV2Controller(
                serviceManager.Object, eventManager.Object, partnerLocationManager.Object, authorizationService.Object, logger.Object);
            controller.ControllerContext = new ControllerContext
            {
                HttpContext = new DefaultHttpContext(),
            };
        }

        [Fact]
        public async Task GetByEvent_ReturnsOk()
        {
            var eventId = Guid.NewGuid();
            var partners = new List<DisplayEventPartnerLocation>
            {
                new() { PartnerLocationId = Guid.NewGuid(), PartnerName = "Test Partner" },
            };

            serviceManager.Setup(m => m.GetByEventAsync(eventId, It.IsAny<CancellationToken>()))
                .ReturnsAsync(partners);

            var result = await controller.GetByEvent(eventId, CancellationToken.None);

            var okResult = Assert.IsType<OkObjectResult>(result);
            var dtos = Assert.IsAssignableFrom<List<DisplayEventPartnerLocation>>(okResult.Value);
            Assert.Single(dtos);
        }

        [Fact]
        public async Task GetHaulingPartnerLocation_ReturnsOk()
        {
            var eventId = Guid.NewGuid();
            var partnerLocation = new PartnerLocation { Id = Guid.NewGuid(), Name = "Hauling Partner" };

            serviceManager.Setup(m => m.GetHaulingPartnerLocationForEvent(eventId, It.IsAny<CancellationToken>()))
                .ReturnsAsync(partnerLocation);

            var result = await controller.GetHaulingPartnerLocation(eventId, CancellationToken.None);

            var okResult = Assert.IsType<OkObjectResult>(result);
            Assert.IsType<PartnerLocationDto>(okResult.Value);
        }

        [Fact]
        public async Task GetByEventAndPartnerLocation_ReturnsOk()
        {
            var eventId = Guid.NewGuid();
            var partnerLocationId = Guid.NewGuid();
            var services = new List<DisplayEventPartnerLocationService>
            {
                new() { ServiceTypeId = 1 },
            };

            serviceManager.Setup(m => m.GetByEventAndPartnerLocationAsync(eventId, partnerLocationId, It.IsAny<CancellationToken>()))
                .ReturnsAsync(services);

            var result = await controller.GetByEventAndPartnerLocation(eventId, partnerLocationId, CancellationToken.None);

            var okResult = Assert.IsType<OkObjectResult>(result);
            var dtos = Assert.IsAssignableFrom<List<DisplayEventPartnerLocationService>>(okResult.Value);
            Assert.Single(dtos);
        }
    }
}
