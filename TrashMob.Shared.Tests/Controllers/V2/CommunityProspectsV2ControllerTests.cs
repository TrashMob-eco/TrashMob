namespace TrashMob.Shared.Tests.Controllers.V2
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
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
    using TrashMob.Shared.Managers.Prospects;
    using Xunit;

    public class CommunityProspectsV2ControllerTests
    {
        private readonly Mock<ICommunityProspectManager> communityProspectManager = new();
        private readonly Mock<IProspectActivityManager> prospectActivityManager = new();
        private readonly Mock<IClaudeDiscoveryService> claudeDiscoveryService = new();
        private readonly Mock<IProspectScoringManager> prospectScoringManager = new();
        private readonly Mock<ICsvImportManager> csvImportManager = new();
        private readonly Mock<IProspectOutreachManager> prospectOutreachManager = new();
        private readonly Mock<IPipelineAnalyticsManager> pipelineAnalyticsManager = new();
        private readonly Mock<IProspectConversionManager> prospectConversionManager = new();
        private readonly Mock<ILogger<CommunityProspectsV2Controller>> logger = new();
        private readonly CommunityProspectsV2Controller controller;
        private readonly Guid testUserId = Guid.NewGuid();

        public CommunityProspectsV2ControllerTests()
        {
            controller = new CommunityProspectsV2Controller(
                communityProspectManager.Object,
                prospectActivityManager.Object,
                claudeDiscoveryService.Object,
                prospectScoringManager.Object,
                csvImportManager.Object,
                prospectOutreachManager.Object,
                pipelineAnalyticsManager.Object,
                prospectConversionManager.Object,
                logger.Object);

            var httpContext = new DefaultHttpContext();
            httpContext.Items["UserId"] = testUserId.ToString();
            httpContext.User = new ClaimsPrincipal(new ClaimsIdentity(
            [
                new Claim(ClaimTypes.NameIdentifier, testUserId.ToString()),
            ], "TestAuth"));
            controller.ControllerContext = new ControllerContext { HttpContext = httpContext };
        }

        [Fact]
        public async Task GetAll_ReturnsOkWithList()
        {
            var prospects = new List<CommunityProspect>
            {
                new() { Id = Guid.NewGuid(), Name = "City of Seattle", City = "Seattle", Region = "WA", PipelineStage = 1 },
                new() { Id = Guid.NewGuid(), Name = "City of Portland", City = "Portland", Region = "OR", PipelineStage = 2 },
            };

            communityProspectManager
                .Setup(m => m.GetAsync(It.IsAny<CancellationToken>()))
                .ReturnsAsync(prospects);

            var result = await controller.GetAll(null, null, CancellationToken.None);

            var okResult = Assert.IsType<OkObjectResult>(result);
            var dtos = Assert.IsAssignableFrom<List<CommunityProspectDto>>(okResult.Value);
            Assert.Equal(2, dtos.Count);
            Assert.Equal("City of Seattle", dtos[0].Name);
        }

        [Fact]
        public async Task GetById_Found_ReturnsOk()
        {
            var prospectId = Guid.NewGuid();
            var prospect = new CommunityProspect
            {
                Id = prospectId,
                Name = "City of Seattle",
                City = "Seattle",
                Region = "WA",
                PipelineStage = 1,
                FitScore = 85,
            };

            communityProspectManager
                .Setup(m => m.GetAsync(prospectId, It.IsAny<CancellationToken>()))
                .ReturnsAsync(prospect);

            var result = await controller.GetById(prospectId, CancellationToken.None);

            var okResult = Assert.IsType<OkObjectResult>(result);
            var dto = Assert.IsType<CommunityProspectDto>(okResult.Value);
            Assert.Equal(prospectId, dto.Id);
            Assert.Equal("City of Seattle", dto.Name);
            Assert.Equal(85, dto.FitScore);
        }

        [Fact]
        public async Task Create_ReturnsCreated()
        {
            var prospectDto = new CommunityProspectDto
            {
                Name = "City of Bellevue",
                City = "Bellevue",
                Region = "WA",
                Country = "United States",
                PipelineStage = 0,
            };
            var createdProspect = new CommunityProspect
            {
                Id = Guid.NewGuid(),
                Name = "City of Bellevue",
                City = "Bellevue",
                Region = "WA",
                Country = "United States",
                PipelineStage = 0,
            };

            communityProspectManager
                .Setup(m => m.AddAsync(It.IsAny<CommunityProspect>(), testUserId, It.IsAny<CancellationToken>()))
                .ReturnsAsync(createdProspect);

            var result = await controller.Create(prospectDto, CancellationToken.None);

            var createdResult = Assert.IsType<CreatedAtActionResult>(result);
            Assert.Equal(StatusCodes.Status201Created, createdResult.StatusCode);
            var dto = Assert.IsType<CommunityProspectDto>(createdResult.Value);
            Assert.Equal("City of Bellevue", dto.Name);
        }

        [Fact]
        public async Task UpdateStage_ReturnsOk()
        {
            var prospectId = Guid.NewGuid();
            var updatedProspect = new CommunityProspect
            {
                Id = prospectId,
                Name = "City of Seattle",
                City = "Seattle",
                Region = "WA",
                PipelineStage = 3,
            };
            var request = new UpdateStageRequest { Stage = 3 };

            communityProspectManager
                .Setup(m => m.UpdatePipelineStageAsync(prospectId, 3, testUserId, It.IsAny<CancellationToken>()))
                .ReturnsAsync(updatedProspect);

            var result = await controller.UpdateStage(prospectId, request, CancellationToken.None);

            var okResult = Assert.IsType<OkObjectResult>(result);
            var dto = Assert.IsType<CommunityProspectDto>(okResult.Value);
            Assert.Equal(3, dto.PipelineStage);
        }
    }
}
