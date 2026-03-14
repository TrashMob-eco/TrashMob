namespace TrashMob.Shared.Tests.Controllers.V2
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
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
    using TrashMob.Models.Poco.V2;
    using TrashMob.Shared.Managers.Interfaces;
    using Xunit;

    public class StagedAreasV2ControllerTests
    {
        private readonly Mock<IStagedAdoptableAreaManager> stagedManager = new();
        private readonly Mock<IKeyedManager<Partner>> partnerManager = new();
        private readonly Mock<IAuthorizationService> authorizationService = new();
        private readonly Mock<ILogger<StagedAreasV2Controller>> logger = new();
        private readonly StagedAreasV2Controller controller;
        private readonly Guid testUserId = Guid.NewGuid();

        public StagedAreasV2ControllerTests()
        {
            controller = new StagedAreasV2Controller(
                stagedManager.Object,
                partnerManager.Object,
                authorizationService.Object,
                logger.Object);

            var httpContext = new DefaultHttpContext();
            httpContext.Items["UserId"] = testUserId.ToString();
            httpContext.User = new ClaimsPrincipal(new ClaimsIdentity(
                [new Claim(ClaimTypes.NameIdentifier, testUserId.ToString())], "TestAuth"));
            controller.ControllerContext = new ControllerContext { HttpContext = httpContext };
        }

        [Fact]
        public async Task GetStagedAreas_ReturnsOkWithList()
        {
            var partnerId = Guid.NewGuid();
            var batchId = Guid.NewGuid();
            var areas = new List<StagedAdoptableArea>
            {
                new() { Id = Guid.NewGuid(), BatchId = batchId, Name = "Staged Park A" },
                new() { Id = Guid.NewGuid(), BatchId = batchId, Name = "Staged Trail B" },
            };

            stagedManager
                .Setup(m => m.GetByBatchAsync(batchId, It.IsAny<CancellationToken>()))
                .ReturnsAsync(areas);

            var result = await controller.GetStagedAreas(partnerId, batchId, CancellationToken.None);

            var okResult = Assert.IsType<OkObjectResult>(result);
            var dtos = Assert.IsAssignableFrom<IEnumerable<StagedAdoptableAreaDto>>(okResult.Value);
            Assert.Equal(2, dtos.Count());
        }

        [Fact]
        public async Task Approve_Authorized_ReturnsNoContent()
        {
            var partnerId = Guid.NewGuid();
            var areaId = Guid.NewGuid();
            var partner = new Partner { Id = partnerId, Name = "Test Community" };

            partnerManager
                .Setup(m => m.GetAsync(partnerId, It.IsAny<CancellationToken>()))
                .ReturnsAsync(partner);
            authorizationService
                .Setup(a => a.AuthorizeAsync(It.IsAny<ClaimsPrincipal>(), It.IsAny<object>(), It.IsAny<string>()))
                .ReturnsAsync(AuthorizationResult.Success());
            stagedManager
                .Setup(m => m.ApproveAsync(areaId, testUserId, It.IsAny<CancellationToken>()))
                .Returns(Task.CompletedTask);

            var result = await controller.Approve(partnerId, areaId, CancellationToken.None);

            Assert.IsType<NoContentResult>(result);
            stagedManager.Verify(m => m.ApproveAsync(areaId, testUserId, It.IsAny<CancellationToken>()), Times.Once);
        }

        [Fact]
        public async Task Reject_Authorized_ReturnsNoContent()
        {
            var partnerId = Guid.NewGuid();
            var areaId = Guid.NewGuid();
            var partner = new Partner { Id = partnerId, Name = "Test Community" };

            partnerManager
                .Setup(m => m.GetAsync(partnerId, It.IsAny<CancellationToken>()))
                .ReturnsAsync(partner);
            authorizationService
                .Setup(a => a.AuthorizeAsync(It.IsAny<ClaimsPrincipal>(), It.IsAny<object>(), It.IsAny<string>()))
                .ReturnsAsync(AuthorizationResult.Success());
            stagedManager
                .Setup(m => m.RejectAsync(areaId, testUserId, It.IsAny<CancellationToken>()))
                .Returns(Task.CompletedTask);

            var result = await controller.Reject(partnerId, areaId, CancellationToken.None);

            Assert.IsType<NoContentResult>(result);
            stagedManager.Verify(m => m.RejectAsync(areaId, testUserId, It.IsAny<CancellationToken>()), Times.Once);
        }

        [Fact]
        public async Task Approve_ReturnsForbid_WhenNotAuthorized()
        {
            var partnerId = Guid.NewGuid();
            var areaId = Guid.NewGuid();
            var partner = new Partner { Id = partnerId, Name = "Test Community" };

            partnerManager
                .Setup(m => m.GetAsync(partnerId, It.IsAny<CancellationToken>()))
                .ReturnsAsync(partner);
            authorizationService
                .Setup(a => a.AuthorizeAsync(It.IsAny<ClaimsPrincipal>(), It.IsAny<object>(), It.IsAny<string>()))
                .ReturnsAsync(AuthorizationResult.Failed());

            var result = await controller.Approve(partnerId, areaId, CancellationToken.None);

            Assert.IsType<ForbidResult>(result);
        }
    }
}
