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
    using TrashMob.Models.Poco.V2;
    using TrashMob.Shared.Managers.Interfaces;
    using Xunit;

    public class PickupLocationsV2ControllerTests
    {
        private readonly Mock<IPickupLocationManager> pickupLocationManager = new();
        private readonly Mock<IEventManager> eventManager = new();
        private readonly Mock<IImageManager> imageManager = new();
        private readonly Mock<IAuthorizationService> authorizationService = new();
        private readonly Mock<ILogger<PickupLocationsV2Controller>> logger = new();
        private readonly PickupLocationsV2Controller controller;

        public PickupLocationsV2ControllerTests()
        {
            controller = new PickupLocationsV2Controller(
                pickupLocationManager.Object, eventManager.Object, imageManager.Object,
                authorizationService.Object, logger.Object);
            controller.ControllerContext = new ControllerContext
            {
                HttpContext = new DefaultHttpContext(),
            };
        }

        [Fact]
        public async Task Get_ReturnsOk_WhenFound()
        {
            var id = Guid.NewGuid();
            var entity = new PickupLocation { Id = id, EventId = Guid.NewGuid(), Name = "Test" };

            pickupLocationManager.Setup(m => m.GetAsync(id, It.IsAny<CancellationToken>()))
                .ReturnsAsync(entity);

            var result = await controller.Get(id, CancellationToken.None);

            var okResult = Assert.IsType<OkObjectResult>(result);
            var dto = Assert.IsType<PickupLocationDto>(okResult.Value);
            Assert.Equal("Test", dto.Name);
        }

        [Fact]
        public async Task Get_ReturnsNotFound_WhenMissing()
        {
            pickupLocationManager.Setup(m => m.GetAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync((PickupLocation)null);

            var result = await controller.Get(Guid.NewGuid(), CancellationToken.None);

            Assert.IsType<NotFoundResult>(result);
        }

        [Fact]
        public async Task GetByEvent_ReturnsOk_WithList()
        {
            var eventId = Guid.NewGuid();
            var entities = new List<PickupLocation>
            {
                new() { Id = Guid.NewGuid(), EventId = eventId, Name = "Loc 1" },
                new() { Id = Guid.NewGuid(), EventId = eventId, Name = "Loc 2" },
            };

            pickupLocationManager.Setup(m => m.GetByParentIdAsync(eventId, It.IsAny<CancellationToken>()))
                .ReturnsAsync(entities);

            var result = await controller.GetByEvent(eventId, CancellationToken.None);

            var okResult = Assert.IsType<OkObjectResult>(result);
            var dtos = Assert.IsAssignableFrom<List<PickupLocationDto>>(okResult.Value);
            Assert.Equal(2, dtos.Count);
        }

        [Fact]
        public async Task GetImage_ReturnsOk_WhenUrlExists()
        {
            var id = Guid.NewGuid();
            imageManager.Setup(m => m.GetImageUrlAsync(id, ImageTypeEnum.Pickup, ImageSizeEnum.Raw, It.IsAny<CancellationToken>()))
                .ReturnsAsync("https://example.com/image.jpg");

            var result = await controller.GetImage(id, CancellationToken.None);

            var okResult = Assert.IsType<OkObjectResult>(result);
            Assert.Equal("https://example.com/image.jpg", okResult.Value);
        }

        [Fact]
        public async Task GetImage_ReturnsNoContent_WhenNoUrl()
        {
            var id = Guid.NewGuid();
            imageManager.Setup(m => m.GetImageUrlAsync(id, ImageTypeEnum.Pickup, ImageSizeEnum.Raw, It.IsAny<CancellationToken>()))
                .ReturnsAsync(string.Empty);

            var result = await controller.GetImage(id, CancellationToken.None);

            Assert.IsType<NoContentResult>(result);
        }
    }
}
