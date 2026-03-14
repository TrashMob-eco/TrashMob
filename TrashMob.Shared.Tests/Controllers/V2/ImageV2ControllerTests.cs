namespace TrashMob.Shared.Tests.Controllers.V2
{
    using System;
    using System.Security.Claims;
    using System.Threading;
    using System.Threading.Tasks;
    using Microsoft.AspNetCore.Authorization;
    using Microsoft.AspNetCore.Http;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.Extensions.Logging;
    using Moq;
    using TrashMob.Controllers.V2;
    using TrashMob.Models;
    using TrashMob.Shared.Managers.Interfaces;
    using TrashMob.Shared.Poco;
    using Xunit;

    public class ImageV2ControllerTests
    {
        private readonly Mock<IImageManager> imageManager = new();
        private readonly Mock<IEventManager> eventManager = new();
        private readonly Mock<IAuthorizationService> authorizationService = new();
        private readonly Mock<ILogger<ImageV2Controller>> logger = new();
        private readonly ImageV2Controller controller;
        private readonly Guid testUserId = Guid.NewGuid();

        public ImageV2ControllerTests()
        {
            controller = new ImageV2Controller(
                imageManager.Object,
                eventManager.Object,
                authorizationService.Object,
                logger.Object);

            var httpContext = new DefaultHttpContext();
            httpContext.Items["UserId"] = testUserId.ToString();
            httpContext.User = new ClaimsPrincipal(new ClaimsIdentity(
            [
                new Claim(ClaimTypes.NameIdentifier, testUserId.ToString()),
            ], "TestAuth"));

            controller.ControllerContext = new ControllerContext
            {
                HttpContext = httpContext,
            };
        }

        [Fact]
        public async Task UploadImage_Authorized_ReturnsOk()
        {
            var eventId = Guid.NewGuid();
            var mobEvent = new Event { Id = eventId, CreatedByUserId = testUserId };
            var imageUpload = new ImageUpload { ParentId = eventId };

            eventManager.Setup(m => m.GetAsync(eventId, It.IsAny<CancellationToken>()))
                .ReturnsAsync(mobEvent);

            authorizationService
                .Setup(a => a.AuthorizeAsync(It.IsAny<ClaimsPrincipal>(), mobEvent, It.IsAny<string>()))
                .ReturnsAsync(AuthorizationResult.Success());

            var result = await controller.UploadImage(imageUpload);

            Assert.IsType<OkResult>(result);
            imageManager.Verify(m => m.UploadImageAsync(imageUpload), Times.Once);
        }

        [Fact]
        public async Task UploadImage_NotAuthorized_ReturnsForbid()
        {
            var eventId = Guid.NewGuid();
            var mobEvent = new Event { Id = eventId, CreatedByUserId = Guid.NewGuid() };
            var imageUpload = new ImageUpload { ParentId = eventId };

            eventManager.Setup(m => m.GetAsync(eventId, It.IsAny<CancellationToken>()))
                .ReturnsAsync(mobEvent);

            authorizationService
                .Setup(a => a.AuthorizeAsync(It.IsAny<ClaimsPrincipal>(), mobEvent, It.IsAny<string>()))
                .ReturnsAsync(AuthorizationResult.Failed());

            var result = await controller.UploadImage(imageUpload);

            Assert.IsType<ForbidResult>(result);
        }

        [Fact]
        public async Task DeleteImage_Authorized_ReturnsNoContent()
        {
            var eventId = Guid.NewGuid();
            var mobEvent = new Event { Id = eventId, CreatedByUserId = testUserId };

            eventManager.Setup(m => m.GetAsync(eventId, It.IsAny<CancellationToken>()))
                .ReturnsAsync(mobEvent);

            authorizationService
                .Setup(a => a.AuthorizeAsync(It.IsAny<ClaimsPrincipal>(), mobEvent, It.IsAny<string>()))
                .ReturnsAsync(AuthorizationResult.Success());

            imageManager.Setup(m => m.DeleteImageAsync(eventId, ImageTypeEnum.Before))
                .ReturnsAsync(true);

            var result = await controller.DeleteImage(eventId, ImageTypeEnum.Before);

            Assert.IsType<NoContentResult>(result);
        }

        [Fact]
        public async Task DeleteImage_DeleteFails_ReturnsBadRequest()
        {
            var eventId = Guid.NewGuid();
            var mobEvent = new Event { Id = eventId, CreatedByUserId = testUserId };

            eventManager.Setup(m => m.GetAsync(eventId, It.IsAny<CancellationToken>()))
                .ReturnsAsync(mobEvent);

            authorizationService
                .Setup(a => a.AuthorizeAsync(It.IsAny<ClaimsPrincipal>(), mobEvent, It.IsAny<string>()))
                .ReturnsAsync(AuthorizationResult.Success());

            imageManager.Setup(m => m.DeleteImageAsync(eventId, ImageTypeEnum.Before))
                .ReturnsAsync(false);

            var result = await controller.DeleteImage(eventId, ImageTypeEnum.Before);

            var objectResult = Assert.IsType<ObjectResult>(result);
            Assert.Equal(400, objectResult.StatusCode);
        }
    }
}
