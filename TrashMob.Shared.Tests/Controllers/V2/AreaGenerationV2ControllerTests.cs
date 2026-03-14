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
    using TrashMob.Models.Poco;
    using TrashMob.Models.Poco.V2;
    using TrashMob.Services;
    using TrashMob.Shared.Managers.Interfaces;
    using Xunit;

    public class AreaGenerationV2ControllerTests
    {
        private readonly Mock<IAreaGenerationBatchManager> batchManager = new();
        private readonly Mock<IKeyedManager<Partner>> partnerManager = new();
        private readonly Mock<IAreaGenerationQueue> queue = new();
        private readonly Mock<IAuthorizationService> authorizationService = new();
        private readonly Mock<ILogger<AreaGenerationV2Controller>> logger = new();
        private readonly AreaGenerationV2Controller controller;
        private readonly Guid testUserId = Guid.NewGuid();

        public AreaGenerationV2ControllerTests()
        {
            controller = new AreaGenerationV2Controller(
                batchManager.Object,
                partnerManager.Object,
                queue.Object,
                authorizationService.Object,
                logger.Object);

            var httpContext = new DefaultHttpContext();
            httpContext.Items["UserId"] = testUserId.ToString();
            httpContext.User = new ClaimsPrincipal(new ClaimsIdentity(
                [new Claim(ClaimTypes.NameIdentifier, testUserId.ToString())], "TestAuth"));
            controller.ControllerContext = new ControllerContext { HttpContext = httpContext };
        }

        [Fact]
        public async Task StartGeneration_Authorized_ReturnsCreated()
        {
            var partnerId = Guid.NewGuid();
            var partner = new Partner { Id = partnerId, Name = "Test Community" };
            var request = new AreaGenerationRequest { Category = "Park" };
            var batch = new AreaGenerationBatch
            {
                Id = Guid.NewGuid(),
                PartnerId = partnerId,
                Category = "Park",
                Status = "Queued",
            };

            partnerManager
                .Setup(m => m.GetAsync(partnerId, It.IsAny<CancellationToken>()))
                .ReturnsAsync(partner);
            authorizationService
                .Setup(a => a.AuthorizeAsync(It.IsAny<ClaimsPrincipal>(), It.IsAny<object>(), It.IsAny<string>()))
                .ReturnsAsync(AuthorizationResult.Success());
            batchManager
                .Setup(m => m.GetActiveByPartnerAsync(partnerId, It.IsAny<CancellationToken>()))
                .ReturnsAsync((AreaGenerationBatch)null);
            batchManager
                .Setup(m => m.AddAsync(It.IsAny<AreaGenerationBatch>(), testUserId, It.IsAny<CancellationToken>()))
                .ReturnsAsync(batch);

            var result = await controller.StartGeneration(partnerId, request, CancellationToken.None);

            var createdResult = Assert.IsType<CreatedAtActionResult>(result);
            Assert.Equal(201, createdResult.StatusCode);
            var dto = Assert.IsType<AreaGenerationBatchDto>(createdResult.Value);
            Assert.Equal("Park", dto.Category);
        }

        [Fact]
        public async Task GetStatus_ReturnsOk()
        {
            var partnerId = Guid.NewGuid();
            var batch = new AreaGenerationBatch
            {
                Id = Guid.NewGuid(),
                PartnerId = partnerId,
                Category = "Park",
                Status = "Running",
            };

            batchManager
                .Setup(m => m.GetActiveByPartnerAsync(partnerId, It.IsAny<CancellationToken>()))
                .ReturnsAsync(batch);

            var result = await controller.GetStatus(partnerId, CancellationToken.None);

            var okResult = Assert.IsType<OkObjectResult>(result);
            var dto = Assert.IsType<AreaGenerationBatchDto>(okResult.Value);
            Assert.Equal("Running", dto.Status);
        }

        [Fact]
        public async Task GetStatus_ReturnsNotFound_WhenNoBatchActive()
        {
            var partnerId = Guid.NewGuid();

            batchManager
                .Setup(m => m.GetActiveByPartnerAsync(partnerId, It.IsAny<CancellationToken>()))
                .ReturnsAsync((AreaGenerationBatch)null);

            var result = await controller.GetStatus(partnerId, CancellationToken.None);

            Assert.IsType<NotFoundResult>(result);
        }

        [Fact]
        public async Task GetBatches_ReturnsOkWithList()
        {
            var partnerId = Guid.NewGuid();
            var batches = new List<AreaGenerationBatch>
            {
                new() { Id = Guid.NewGuid(), PartnerId = partnerId, Category = "Park", Status = "Complete" },
                new() { Id = Guid.NewGuid(), PartnerId = partnerId, Category = "School", Status = "Queued" },
            };

            batchManager
                .Setup(m => m.GetByPartnerAsync(partnerId, It.IsAny<CancellationToken>()))
                .ReturnsAsync(batches);

            var result = await controller.GetBatches(partnerId, CancellationToken.None);

            var okResult = Assert.IsType<OkObjectResult>(result);
            var dtos = Assert.IsAssignableFrom<IEnumerable<AreaGenerationBatchDto>>(okResult.Value);
            Assert.Equal(2, dtos.Count());
        }
    }
}
