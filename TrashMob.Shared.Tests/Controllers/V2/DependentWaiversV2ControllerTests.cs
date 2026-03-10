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
    using TrashMob.Models;
    using TrashMob.Models.Poco.V2;
    using TrashMob.Shared.Managers.Interfaces;
    using TrashMob.Shared.Poco;
    using Xunit;

    public class DependentWaiversV2ControllerTests
    {
        private readonly Mock<IDependentWaiverManager> waiverManager = new();
        private readonly Mock<IDependentManager> dependentManager = new();
        private readonly Mock<IWaiverDocumentManager> waiverDocumentManager = new();
        private readonly Mock<IUserManager> userManager = new();
        private readonly Mock<ILogger<DependentWaiversV2Controller>> logger = new();
        private readonly DependentWaiversV2Controller controller;
        private readonly Guid userId = Guid.NewGuid();

        public DependentWaiversV2ControllerTests()
        {
            controller = new DependentWaiversV2Controller(
                waiverManager.Object,
                dependentManager.Object,
                waiverDocumentManager.Object,
                userManager.Object,
                logger.Object);

            var httpContext = new DefaultHttpContext();
            httpContext.Connection.RemoteIpAddress = System.Net.IPAddress.Loopback;
            httpContext.Request.Headers["User-Agent"] = "TestAgent/1.0";
            httpContext.Items["UserId"] = userId.ToString();

            controller.ControllerContext = new ControllerContext
            {
                HttpContext = httpContext,
            };
        }

        [Fact]
        public async Task GetCurrentWaiver_ReturnsOk_WhenDependentExistsWithCorrectParentAndWaiverExists()
        {
            var dependentId = Guid.NewGuid();
            var dependent = new Dependent { Id = dependentId, ParentUserId = userId };
            var waiver = new DependentWaiver
            {
                Id = Guid.NewGuid(),
                DependentId = dependentId,
                SignedByUserId = userId,
                AcceptedDate = DateTimeOffset.UtcNow,
            };

            dependentManager.Setup(m => m.GetAsync(dependentId, It.IsAny<CancellationToken>()))
                .ReturnsAsync(dependent);
            waiverManager.Setup(m => m.GetCurrentWaiverAsync(dependentId, It.IsAny<CancellationToken>()))
                .ReturnsAsync(waiver);

            var result = await controller.GetCurrentWaiver(dependentId, CancellationToken.None);

            var okResult = Assert.IsType<OkObjectResult>(result);
            var returnedWaiver = Assert.IsType<DependentWaiverDto>(okResult.Value);
            Assert.Equal(waiver.Id, returnedWaiver.Id);
        }

        [Fact]
        public async Task GetCurrentWaiver_ReturnsForbid_WhenNotParent()
        {
            var dependentId = Guid.NewGuid();
            var otherUserId = Guid.NewGuid();
            var dependent = new Dependent { Id = dependentId, ParentUserId = otherUserId };

            dependentManager.Setup(m => m.GetAsync(dependentId, It.IsAny<CancellationToken>()))
                .ReturnsAsync(dependent);

            var result = await controller.GetCurrentWaiver(dependentId, CancellationToken.None);

            Assert.IsType<ForbidResult>(result);
        }

        [Fact]
        public async Task GetCurrentWaiver_ReturnsNotFound_WhenNoWaiver()
        {
            var dependentId = Guid.NewGuid();
            var dependent = new Dependent { Id = dependentId, ParentUserId = userId };

            dependentManager.Setup(m => m.GetAsync(dependentId, It.IsAny<CancellationToken>()))
                .ReturnsAsync(dependent);
            waiverManager.Setup(m => m.GetCurrentWaiverAsync(dependentId, It.IsAny<CancellationToken>()))
                .ReturnsAsync((DependentWaiver)null);

            var result = await controller.GetCurrentWaiver(dependentId, CancellationToken.None);

            Assert.IsType<NotFoundResult>(result);
        }

        [Fact]
        public async Task SignWaiver_ReturnsCreated_WhenSuccess()
        {
            var dependentId = Guid.NewGuid();
            var waiverVersionId = Guid.NewGuid();
            var dependent = new Dependent { Id = dependentId, ParentUserId = userId };
            var signedWaiver = new DependentWaiver
            {
                Id = Guid.NewGuid(),
                DependentId = dependentId,
                WaiverVersionId = waiverVersionId,
                SignedByUserId = userId,
                TypedLegalName = "Jane Doe",
                AcceptedDate = DateTimeOffset.UtcNow,
            };

            dependentManager.Setup(m => m.GetAsync(dependentId, It.IsAny<CancellationToken>()))
                .ReturnsAsync(dependent);
            waiverManager.Setup(m => m.SignWaiverAsync(
                    dependentId,
                    waiverVersionId,
                    "Jane Doe",
                    It.IsAny<string>(),
                    It.IsAny<string>(),
                    userId,
                    It.IsAny<CancellationToken>()))
                .ReturnsAsync(ServiceResult<DependentWaiver>.Success(signedWaiver));
            userManager.Setup(m => m.GetAsync(userId, It.IsAny<CancellationToken>()))
                .ReturnsAsync(new User { Id = userId, UserName = "janedoe" });
            waiverDocumentManager.Setup(m => m.GenerateAndStoreDependentWaiverPdfAsync(
                    It.IsAny<DependentWaiver>(),
                    It.IsAny<Dependent>(),
                    It.IsAny<User>(),
                    It.IsAny<CancellationToken>()))
                .ReturnsAsync("https://example.com/waiver.pdf");

            var request = new DependentWaiversV2Controller.SignDependentWaiverRequest
            {
                WaiverVersionId = waiverVersionId,
                TypedLegalName = "Jane Doe",
            };

            var result = await controller.SignWaiver(dependentId, request, CancellationToken.None);

            var statusResult = Assert.IsType<ObjectResult>(result);
            Assert.Equal(StatusCodes.Status201Created, statusResult.StatusCode);
            var returnedWaiver = Assert.IsType<DependentWaiverDto>(statusResult.Value);
            Assert.Equal(signedWaiver.Id, returnedWaiver.Id);
        }

        [Fact]
        public async Task SignWaiver_ReturnsBadRequest_WhenFailed()
        {
            var dependentId = Guid.NewGuid();
            var waiverVersionId = Guid.NewGuid();
            var dependent = new Dependent { Id = dependentId, ParentUserId = userId };

            dependentManager.Setup(m => m.GetAsync(dependentId, It.IsAny<CancellationToken>()))
                .ReturnsAsync(dependent);
            waiverManager.Setup(m => m.SignWaiverAsync(
                    dependentId,
                    waiverVersionId,
                    "Jane Doe",
                    It.IsAny<string>(),
                    It.IsAny<string>(),
                    userId,
                    It.IsAny<CancellationToken>()))
                .ReturnsAsync(ServiceResult<DependentWaiver>.Failure("Waiver version not found"));

            var request = new DependentWaiversV2Controller.SignDependentWaiverRequest
            {
                WaiverVersionId = waiverVersionId,
                TypedLegalName = "Jane Doe",
            };

            var result = await controller.SignWaiver(dependentId, request, CancellationToken.None);

            var badResult = Assert.IsType<BadRequestObjectResult>(result);
            Assert.Equal("Waiver version not found", badResult.Value);
        }

        [Fact]
        public async Task GetWaiverHistory_ReturnsOk()
        {
            var dependentId = Guid.NewGuid();
            var dependent = new Dependent { Id = dependentId, ParentUserId = userId };
            var waivers = new List<DependentWaiver>
            {
                new()
                {
                    Id = Guid.NewGuid(),
                    DependentId = dependentId,
                    SignedByUserId = userId,
                    AcceptedDate = DateTimeOffset.UtcNow.AddMonths(-6),
                },
                new()
                {
                    Id = Guid.NewGuid(),
                    DependentId = dependentId,
                    SignedByUserId = userId,
                    AcceptedDate = DateTimeOffset.UtcNow,
                },
            };

            dependentManager.Setup(m => m.GetAsync(dependentId, It.IsAny<CancellationToken>()))
                .ReturnsAsync(dependent);
            waiverManager.Setup(m => m.GetByDependentIdAsync(dependentId, It.IsAny<CancellationToken>()))
                .ReturnsAsync(waivers);

            var result = await controller.GetWaiverHistory(dependentId, CancellationToken.None);

            var okResult = Assert.IsType<OkObjectResult>(result);
            var returnedWaivers = Assert.IsAssignableFrom<IEnumerable<DependentWaiverDto>>(okResult.Value);
            Assert.Equal(2, new List<DependentWaiverDto>(returnedWaivers).Count);
        }
    }
}
