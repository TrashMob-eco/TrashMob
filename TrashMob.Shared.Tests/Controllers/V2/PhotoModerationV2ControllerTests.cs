namespace TrashMob.Shared.Tests.Controllers.V2
{
    using System;
    using System.Collections.Generic;
    using System.Security.Claims;
    using System.Threading;
    using System.Threading.Tasks;
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

    public class PhotoModerationV2ControllerTests
    {
        private readonly Mock<IPhotoModerationManager> photoModerationManager = new();
        private readonly Mock<ILogger<PhotoModerationV2Controller>> logger = new();
        private readonly PhotoModerationV2Controller controller;
        private readonly Guid testUserId = Guid.NewGuid();

        public PhotoModerationV2ControllerTests()
        {
            controller = new PhotoModerationV2Controller(photoModerationManager.Object, logger.Object);

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
        public async Task GetPendingPhotos_ReturnsOk()
        {
            var item = new PhotoModerationItem
            {
                PhotoId = Guid.NewGuid(),
                PhotoType = "LitterImage",
                ImageUrl = "https://example.com/photo.jpg",
                ModerationStatus = PhotoModerationStatus.Pending,
            };

            var paginatedResult = new PaginatedList<PhotoModerationItem>(
                new List<PhotoModerationItem> { item }, 1, 1, 50);

            photoModerationManager
                .Setup(m => m.GetPendingPhotosAsync(1, 50, It.IsAny<CancellationToken>()))
                .ReturnsAsync(paginatedResult);

            var result = await controller.GetPendingPhotos(1, 50, CancellationToken.None);

            var okResult = Assert.IsType<OkObjectResult>(result);
            var returnedList = Assert.IsType<PaginatedList<PhotoModerationItem>>(okResult.Value);
            Assert.Single(returnedList);
            Assert.Equal(item.PhotoId, returnedList[0].PhotoId);
        }

        [Fact]
        public async Task GetFlaggedPhotos_ReturnsOk()
        {
            var paginatedResult = new PaginatedList<PhotoModerationItem>(
                new List<PhotoModerationItem>(), 0, 1, 50);

            photoModerationManager
                .Setup(m => m.GetFlaggedPhotosAsync(1, 50, It.IsAny<CancellationToken>()))
                .ReturnsAsync(paginatedResult);

            var result = await controller.GetFlaggedPhotos(1, 50, CancellationToken.None);

            var okResult = Assert.IsType<OkObjectResult>(result);
            Assert.IsType<PaginatedList<PhotoModerationItem>>(okResult.Value);
        }

        [Fact]
        public async Task ApprovePhoto_ReturnsOk()
        {
            var photoId = Guid.NewGuid();
            var item = new PhotoModerationItem
            {
                PhotoId = photoId,
                PhotoType = "LitterImage",
                ImageUrl = "https://example.com/photo.jpg",
                ModerationStatus = PhotoModerationStatus.Approved,
            };

            photoModerationManager
                .Setup(m => m.ApprovePhotoAsync("LitterImage", photoId, testUserId, It.IsAny<CancellationToken>()))
                .ReturnsAsync(item);

            var result = await controller.ApprovePhoto("LitterImage", photoId, CancellationToken.None);

            var okResult = Assert.IsType<OkObjectResult>(result);
            var returned = Assert.IsType<PhotoModerationItem>(okResult.Value);
            Assert.Equal(PhotoModerationStatus.Approved, returned.ModerationStatus);
        }

        [Fact]
        public async Task ApprovePhoto_NotFound_ReturnsNotFound()
        {
            var photoId = Guid.NewGuid();

            photoModerationManager
                .Setup(m => m.ApprovePhotoAsync("LitterImage", photoId, testUserId, It.IsAny<CancellationToken>()))
                .ReturnsAsync((PhotoModerationItem)null);

            var result = await controller.ApprovePhoto("LitterImage", photoId, CancellationToken.None);

            Assert.IsType<NotFoundResult>(result);
        }

        [Fact]
        public async Task RejectPhoto_EmptyReason_ReturnsBadRequest()
        {
            var request = new RejectPhotoRequestDto { Reason = "" };

            var result = await controller.RejectPhoto("LitterImage", Guid.NewGuid(), request, CancellationToken.None);

            var objectResult = Assert.IsType<ObjectResult>(result);
            Assert.Equal(400, objectResult.StatusCode);
        }

        [Fact]
        public async Task RejectPhoto_Success_ReturnsOk()
        {
            var photoId = Guid.NewGuid();
            var request = new RejectPhotoRequestDto { Reason = "Inappropriate content" };
            var item = new PhotoModerationItem
            {
                PhotoId = photoId,
                PhotoType = "LitterImage",
                ImageUrl = "https://example.com/photo.jpg",
                ModerationStatus = PhotoModerationStatus.Rejected,
            };

            photoModerationManager
                .Setup(m => m.RejectPhotoAsync("LitterImage", photoId, "Inappropriate content", testUserId, It.IsAny<CancellationToken>()))
                .ReturnsAsync(item);

            var result = await controller.RejectPhoto("LitterImage", photoId, request, CancellationToken.None);

            var okResult = Assert.IsType<OkObjectResult>(result);
            var returned = Assert.IsType<PhotoModerationItem>(okResult.Value);
            Assert.Equal(PhotoModerationStatus.Rejected, returned.ModerationStatus);
        }
    }
}
