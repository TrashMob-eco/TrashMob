namespace TrashMob.Shared.Tests.Controllers.V2
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Linq.Expressions;
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
    using TrashMob.Models.Poco;
    using TrashMob.Models.Poco.V2;
    using TrashMob.Shared.Managers.Contacts;
    using TrashMob.Shared.Managers.Interfaces;
    using Xunit;

    public class GrantsV2ControllerTests
    {
        private readonly Mock<IGrantManager> grantManager = new();
        private readonly Mock<IGrantDiscoveryService> grantDiscoveryService = new();
        private readonly Mock<ILogger<GrantsV2Controller>> logger = new();
        private readonly GrantsV2Controller controller;
        private readonly Guid testUserId = Guid.NewGuid();

        public GrantsV2ControllerTests()
        {
            controller = new GrantsV2Controller(
                grantManager.Object, grantDiscoveryService.Object, logger.Object);

            var httpContext = new DefaultHttpContext();
            httpContext.Items["UserId"] = testUserId.ToString();
            httpContext.User = new ClaimsPrincipal(new ClaimsIdentity(
            [
                new Claim(ClaimTypes.NameIdentifier, testUserId.ToString()),
            ], "TestAuth"));
            controller.ControllerContext = new ControllerContext { HttpContext = httpContext };
        }

        [Fact]
        public async Task GetAll_ReturnsOk()
        {
            var grants = new List<Grant>
            {
                new Grant { Id = Guid.NewGuid(), FunderName = "Foundation", Status = 0 },
                new Grant { Id = Guid.NewGuid(), FunderName = "Trust", Status = 1 },
            };

            grantManager
                .Setup(m => m.GetAsync(It.IsAny<Expression<Func<Grant, bool>>>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(grants);

            var result = await controller.GetAll(cancellationToken: CancellationToken.None);

            var okResult = Assert.IsType<OkObjectResult>(result);
            Assert.NotNull(okResult.Value);
        }

        [Fact]
        public async Task GetById_NotFound_ReturnsNotFound()
        {
            var id = Guid.NewGuid();

            grantManager
                .Setup(m => m.GetAsync(id, It.IsAny<CancellationToken>()))
                .ReturnsAsync((Grant)null);

            var result = await controller.GetById(id, CancellationToken.None);

            Assert.IsType<NotFoundResult>(result);
        }

        [Fact]
        public async Task Create_ReturnsCreated()
        {
            var grantDto = new GrantDto { Id = Guid.NewGuid(), FunderName = "Foundation" };
            var grant = new Grant { Id = grantDto.Id, FunderName = "Foundation", Status = 0 };

            grantManager
                .Setup(m => m.AddAsync(It.IsAny<Grant>(), It.IsAny<Guid>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(grant);

            var result = await controller.Create(grantDto, CancellationToken.None);

            Assert.IsType<CreatedAtActionResult>(result);
        }

        [Fact]
        public async Task Discover_ReturnsOk()
        {
            var request = new GrantDiscoveryRequest { FocusAreas = "Environmental" };
            var discoveryResult = new GrantDiscoveryResult
            {
                Grants = new List<DiscoveredGrant>(),
                TokensUsed = 100,
            };

            grantDiscoveryService
                .Setup(s => s.DiscoverGrantsAsync(It.IsAny<GrantDiscoveryRequest>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(discoveryResult);

            var result = await controller.Discover(request, CancellationToken.None);

            var okResult = Assert.IsType<OkObjectResult>(result);
            Assert.NotNull(okResult.Value);
        }
    }
}
