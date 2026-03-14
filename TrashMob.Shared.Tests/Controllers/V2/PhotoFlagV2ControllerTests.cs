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
    using TrashMob.Models.Poco.V2;
    using TrashMob.Shared.Managers.Interfaces;
    using Xunit;

    public class PhotoFlagV2ControllerTests
    {
        private readonly Mock<IPhotoModerationManager> photoModerationManager = new();
        private readonly Mock<ILogger<PhotoFlagV2Controller>> logger = new();
        private readonly PhotoFlagV2Controller controller;
        private readonly Guid testUserId = Guid.NewGuid();

        public PhotoFlagV2ControllerTests()
        {
            controller = new PhotoFlagV2Controller(photoModerationManager.Object, logger.Object);

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
        public async Task FlagPhoto_EmptyReason_ReturnsBadRequest()
        {
            var request = new FlagPhotoRequestDto { Reason = "" };

            var result = await controller.FlagPhoto("LitterImage", Guid.NewGuid(), request, CancellationToken.None);

            var objectResult = Assert.IsType<ObjectResult>(result);
            Assert.Equal(400, objectResult.StatusCode);
        }

        [Fact]
        public async Task FlagPhoto_Success_ReturnsOk()
        {
            var photoId = Guid.NewGuid();
            var request = new FlagPhotoRequestDto { Reason = "Offensive content" };
            var photoFlag = new PhotoFlag
            {
                Id = Guid.NewGuid(),
                PhotoId = photoId,
                PhotoType = "LitterImage",
                FlaggedByUserId = testUserId,
                FlagReason = "Offensive content",
                FlaggedDate = DateTimeOffset.UtcNow,
            };

            photoModerationManager
                .Setup(m => m.FlagPhotoAsync("LitterImage", photoId, "Offensive content", testUserId, It.IsAny<CancellationToken>()))
                .ReturnsAsync(photoFlag);

            var result = await controller.FlagPhoto("LitterImage", photoId, request, CancellationToken.None);

            var okResult = Assert.IsType<OkObjectResult>(result);
            var dto = Assert.IsType<PhotoFlagDto>(okResult.Value);
            Assert.Equal(photoId, dto.PhotoId);
            Assert.Equal("LitterImage", dto.PhotoType);
            Assert.Equal("Offensive content", dto.FlagReason);
        }

        [Fact]
        public async Task FlagPhoto_NotFound_ReturnsNotFound()
        {
            var photoId = Guid.NewGuid();
            var request = new FlagPhotoRequestDto { Reason = "Spam" };

            photoModerationManager
                .Setup(m => m.FlagPhotoAsync("LitterImage", photoId, "Spam", testUserId, It.IsAny<CancellationToken>()))
                .ReturnsAsync((PhotoFlag)null);

            var result = await controller.FlagPhoto("LitterImage", photoId, request, CancellationToken.None);

            Assert.IsType<NotFoundResult>(result);
        }
    }
}
