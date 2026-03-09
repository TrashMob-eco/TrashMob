namespace TrashMob.Shared.Tests.Controllers.V2
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading;
    using System.Threading.Tasks;
    using Microsoft.AspNetCore.Http;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.Extensions.Logging;
    using Moq;
    using TrashMob.Controllers.V2;
    using TrashMob.Models;
    using TrashMob.Models.Poco.V2;
    using TrashMob.Shared.Managers.Interfaces;
    using Xunit;

    public class EventPhotosV2ControllerTests
    {
        private readonly Mock<IEventPhotoManager> photoManager = new();
        private readonly Mock<IEventManager> eventManager = new();
        private readonly Mock<IEventAttendeeManager> attendeeManager = new();
        private readonly Mock<IImageManager> imageManager = new();
        private readonly Mock<ILogger<EventPhotosV2Controller>> logger = new();
        private readonly EventPhotosV2Controller controller;

        public EventPhotosV2ControllerTests()
        {
            controller = new EventPhotosV2Controller(
                photoManager.Object, eventManager.Object, attendeeManager.Object,
                imageManager.Object, logger.Object);
            controller.ControllerContext = new ControllerContext
            {
                HttpContext = new DefaultHttpContext(),
            };
        }

        [Fact]
        public async Task GetEventPhotos_ReturnsOk_WithPhotos()
        {
            var eventId = Guid.NewGuid();
            var photos = new List<EventPhoto>
            {
                new() { Id = Guid.NewGuid(), EventId = eventId, ImageUrl = "url1", ThumbnailUrl = "thumb1", PhotoType = EventPhotoType.During },
                new() { Id = Guid.NewGuid(), EventId = eventId, ImageUrl = "url2", ThumbnailUrl = "thumb2", PhotoType = EventPhotoType.After },
            };

            eventManager.Setup(m => m.GetAsync(eventId, It.IsAny<CancellationToken>()))
                .ReturnsAsync(new Event { Id = eventId });
            photoManager.Setup(m => m.GetByEventIdAsync(eventId, false, It.IsAny<CancellationToken>()))
                .ReturnsAsync(photos);

            var result = await controller.GetEventPhotos(eventId, null, CancellationToken.None);

            var okResult = Assert.IsType<OkObjectResult>(result);
            var dtos = Assert.IsAssignableFrom<List<EventPhotoDto>>(okResult.Value);
            Assert.Equal(2, dtos.Count);
        }

        [Fact]
        public async Task GetEventPhotos_WithTypeFilter_FiltersPhotos()
        {
            var eventId = Guid.NewGuid();
            var photos = new List<EventPhoto>
            {
                new() { Id = Guid.NewGuid(), EventId = eventId, PhotoType = EventPhotoType.Before },
            };

            eventManager.Setup(m => m.GetAsync(eventId, It.IsAny<CancellationToken>()))
                .ReturnsAsync(new Event { Id = eventId });
            photoManager.Setup(m => m.GetByEventIdAndTypeAsync(eventId, EventPhotoType.Before, It.IsAny<CancellationToken>()))
                .ReturnsAsync(photos);

            var result = await controller.GetEventPhotos(eventId, EventPhotoType.Before, CancellationToken.None);

            var okResult = Assert.IsType<OkObjectResult>(result);
            var dtos = Assert.IsAssignableFrom<List<EventPhotoDto>>(okResult.Value);
            Assert.Single(dtos);
        }

        [Fact]
        public async Task GetEventPhotos_ReturnsNotFound_WhenEventMissing()
        {
            var eventId = Guid.NewGuid();
            eventManager.Setup(m => m.GetAsync(eventId, It.IsAny<CancellationToken>()))
                .ReturnsAsync((Event)null);

            var result = await controller.GetEventPhotos(eventId, null, CancellationToken.None);

            Assert.IsType<NotFoundResult>(result);
        }

        [Fact]
        public async Task DeletePhoto_ReturnsNoContent_WhenAuthorized()
        {
            var userId = Guid.NewGuid();
            var eventId = Guid.NewGuid();
            var photoId = Guid.NewGuid();

            controller.HttpContext.Items["UserId"] = userId.ToString();

            photoManager.Setup(m => m.GetAsync(photoId, It.IsAny<CancellationToken>()))
                .ReturnsAsync(new EventPhoto { Id = photoId, EventId = eventId, UploadedByUserId = userId });
            eventManager.Setup(m => m.GetAsync(eventId, It.IsAny<CancellationToken>()))
                .ReturnsAsync(new Event { Id = eventId, CreatedByUserId = Guid.NewGuid() });
            photoManager.Setup(m => m.DeletePhotoAsync(photoId, userId, It.IsAny<CancellationToken>()))
                .ReturnsAsync(true);

            var result = await controller.DeletePhoto(eventId, photoId, CancellationToken.None);

            Assert.IsType<NoContentResult>(result);
        }

        [Fact]
        public async Task DeletePhoto_ReturnsNotFound_WhenPhotoMissing()
        {
            var userId = Guid.NewGuid();
            controller.HttpContext.Items["UserId"] = userId.ToString();

            photoManager.Setup(m => m.GetAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync((EventPhoto)null);

            var result = await controller.DeletePhoto(Guid.NewGuid(), Guid.NewGuid(), CancellationToken.None);

            Assert.IsType<NotFoundResult>(result);
        }
    }
}
