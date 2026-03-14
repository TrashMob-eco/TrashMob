namespace TrashMob.Shared.Tests.Controllers.V2
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Security.Claims;
    using System.Security.Principal;
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

    public class CommunityWaiverAdminV2ControllerTests
    {
        private readonly Mock<IWaiverVersionManager> waiverVersionManager = new();
        private readonly Mock<ILogger<CommunityWaiverAdminV2Controller>> logger = new();
        private readonly CommunityWaiverAdminV2Controller controller;
        private readonly Guid testUserId = Guid.NewGuid();

        public CommunityWaiverAdminV2ControllerTests()
        {
            controller = new CommunityWaiverAdminV2Controller(
                waiverVersionManager.Object,
                logger.Object);

            var httpContext = new DefaultHttpContext();
            httpContext.Items["UserId"] = testUserId.ToString();
            httpContext.User = new ClaimsPrincipal(new GenericIdentity("test", "Bearer"));
            controller.ControllerContext = new ControllerContext { HttpContext = httpContext };
        }

        [Fact]
        public async Task GetCommunityWaivers_ReturnsOk()
        {
            var communityId = Guid.NewGuid();
            var assignments = new List<CommunityWaiver>
            {
                new() { Id = Guid.NewGuid(), CommunityId = communityId, WaiverVersionId = Guid.NewGuid() },
                new() { Id = Guid.NewGuid(), CommunityId = communityId, WaiverVersionId = Guid.NewGuid() },
            };

            waiverVersionManager
                .Setup(m => m.GetCommunityWaiverAssignmentsAsync(communityId, It.IsAny<CancellationToken>()))
                .ReturnsAsync(assignments);

            var result = await controller.GetCommunityWaivers(communityId, CancellationToken.None);

            var okResult = Assert.IsType<OkObjectResult>(result);
            var dtos = Assert.IsAssignableFrom<IEnumerable<CommunityWaiverDto>>(okResult.Value);
            Assert.Equal(2, dtos.Count());
        }

        [Fact]
        public async Task AssignWaiver_EmptyWaiverId_ReturnsBadRequest()
        {
            var communityId = Guid.NewGuid();
            var request = new AssignWaiverRequestDto { WaiverId = Guid.Empty };

            var result = await controller.AssignWaiver(communityId, request, CancellationToken.None);

            Assert.IsType<BadRequestObjectResult>(result);
        }

        [Fact]
        public async Task AssignWaiver_Success_ReturnsCreated()
        {
            var communityId = Guid.NewGuid();
            var waiverId = Guid.NewGuid();
            var communityWaiver = new CommunityWaiver
            {
                Id = Guid.NewGuid(),
                CommunityId = communityId,
                WaiverVersionId = waiverId,
            };

            waiverVersionManager
                .Setup(m => m.AssignToCommunityAsync(waiverId, communityId, testUserId, It.IsAny<CancellationToken>()))
                .ReturnsAsync(communityWaiver);

            var request = new AssignWaiverRequestDto { WaiverId = waiverId };

            var result = await controller.AssignWaiver(communityId, request, CancellationToken.None);

            var createdResult = Assert.IsType<CreatedAtActionResult>(result);
            Assert.Equal(StatusCodes.Status201Created, createdResult.StatusCode);
            var dto = Assert.IsType<CommunityWaiverDto>(createdResult.Value);
            Assert.Equal(communityId, dto.CommunityId);
        }

        [Fact]
        public async Task RemoveWaiver_ReturnsNoContent()
        {
            var communityId = Guid.NewGuid();
            var waiverId = Guid.NewGuid();

            waiverVersionManager
                .Setup(m => m.RemoveFromCommunityAsync(waiverId, communityId, It.IsAny<CancellationToken>()))
                .Returns(Task.CompletedTask);

            var result = await controller.RemoveWaiver(communityId, waiverId, CancellationToken.None);

            Assert.IsType<NoContentResult>(result);
            waiverVersionManager.Verify(
                m => m.RemoveFromCommunityAsync(waiverId, communityId, It.IsAny<CancellationToken>()),
                Times.Once);
        }
    }
}
