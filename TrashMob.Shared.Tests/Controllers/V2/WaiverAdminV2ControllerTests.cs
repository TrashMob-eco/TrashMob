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

    public class WaiverAdminV2ControllerTests
    {
        private readonly Mock<IWaiverVersionManager> waiverVersionManager = new();
        private readonly Mock<ILogger<WaiverAdminV2Controller>> logger = new();
        private readonly WaiverAdminV2Controller controller;
        private readonly Guid testUserId = Guid.NewGuid();

        public WaiverAdminV2ControllerTests()
        {
            controller = new WaiverAdminV2Controller(
                waiverVersionManager.Object,
                logger.Object);

            var httpContext = new DefaultHttpContext();
            httpContext.Items["UserId"] = testUserId.ToString();
            httpContext.User = new ClaimsPrincipal(new GenericIdentity("test", "Bearer"));
            controller.ControllerContext = new ControllerContext { HttpContext = httpContext };
        }

        [Fact]
        public async Task GetAll_ReturnsOk()
        {
            var waivers = new List<WaiverVersion>
            {
                new() { Id = Guid.NewGuid(), Name = "Global Waiver", Version = "1.0", WaiverText = "Text", IsActive = true, EffectiveDate = DateTimeOffset.UtcNow },
                new() { Id = Guid.NewGuid(), Name = "Community Waiver", Version = "2.0", WaiverText = "Text2", IsActive = false, EffectiveDate = DateTimeOffset.UtcNow },
            };

            waiverVersionManager
                .Setup(m => m.GetAllAsync(It.IsAny<CancellationToken>()))
                .ReturnsAsync(waivers);

            var result = await controller.GetAll(CancellationToken.None);

            var okResult = Assert.IsType<OkObjectResult>(result);
            var dtos = Assert.IsAssignableFrom<IEnumerable<WaiverVersionDto>>(okResult.Value);
            Assert.Equal(2, dtos.Count());
        }

        [Fact]
        public async Task GetActive_ReturnsOk()
        {
            var waivers = new List<WaiverVersion>
            {
                new() { Id = Guid.NewGuid(), Name = "Active Waiver", Version = "1.0", WaiverText = "Text", IsActive = true, EffectiveDate = DateTimeOffset.UtcNow },
            };

            waiverVersionManager
                .Setup(m => m.GetActiveWaiversAsync(It.IsAny<CancellationToken>()))
                .ReturnsAsync(waivers);

            var result = await controller.GetActive(CancellationToken.None);

            var okResult = Assert.IsType<OkObjectResult>(result);
            var dtos = Assert.IsAssignableFrom<IEnumerable<WaiverVersionDto>>(okResult.Value);
            Assert.Single(dtos);
        }

        [Fact]
        public async Task Get_NotFound_ReturnsNotFound()
        {
            var waiverId = Guid.NewGuid();

            waiverVersionManager
                .Setup(m => m.GetAsync(waiverId, It.IsAny<CancellationToken>()))
                .ReturnsAsync((WaiverVersion)null);

            var result = await controller.Get(waiverId, CancellationToken.None);

            Assert.IsType<NotFoundResult>(result);
        }

        [Fact]
        public async Task Get_Found_ReturnsOk()
        {
            var waiverId = Guid.NewGuid();
            var waiver = new WaiverVersion
            {
                Id = waiverId,
                Name = "Test Waiver",
                Version = "1.0",
                WaiverText = "Waiver text content",
                IsActive = true,
                EffectiveDate = DateTimeOffset.UtcNow,
            };

            waiverVersionManager
                .Setup(m => m.GetAsync(waiverId, It.IsAny<CancellationToken>()))
                .ReturnsAsync(waiver);

            var result = await controller.Get(waiverId, CancellationToken.None);

            var okResult = Assert.IsType<OkObjectResult>(result);
            var dto = Assert.IsType<WaiverVersionDto>(okResult.Value);
            Assert.Equal("Test Waiver", dto.Name);
        }

        [Fact]
        public async Task Create_MissingName_ReturnsBadRequest()
        {
            var dto = new WaiverVersionDto
            {
                Name = "",
                Version = "1.0",
                WaiverText = "Some text",
                IsActive = true,
                EffectiveDate = DateTimeOffset.UtcNow,
            };

            var result = await controller.Create(dto, CancellationToken.None);

            Assert.IsType<BadRequestObjectResult>(result);
        }

        [Fact]
        public async Task Create_Success_ReturnsCreated()
        {
            var waiverId = Guid.NewGuid();
            var waiver = new WaiverVersion
            {
                Id = waiverId,
                Name = "New Waiver",
                Version = "1.0",
                WaiverText = "Waiver text",
                IsActive = true,
                EffectiveDate = DateTimeOffset.UtcNow,
            };

            waiverVersionManager
                .Setup(m => m.AddAsync(It.IsAny<WaiverVersion>(), testUserId, It.IsAny<CancellationToken>()))
                .ReturnsAsync(waiver);

            var dto = new WaiverVersionDto
            {
                Name = "New Waiver",
                Version = "1.0",
                WaiverText = "Waiver text",
                IsActive = true,
                EffectiveDate = DateTimeOffset.UtcNow,
            };

            var result = await controller.Create(dto, CancellationToken.None);

            var createdResult = Assert.IsType<CreatedAtActionResult>(result);
            Assert.Equal(StatusCodes.Status201Created, createdResult.StatusCode);
            var resultDto = Assert.IsType<WaiverVersionDto>(createdResult.Value);
            Assert.Equal("New Waiver", resultDto.Name);
        }

        [Fact]
        public async Task Deactivate_NotFound_ReturnsNotFound()
        {
            var waiverId = Guid.NewGuid();

            waiverVersionManager
                .Setup(m => m.GetAsync(waiverId, It.IsAny<CancellationToken>()))
                .ReturnsAsync((WaiverVersion)null);

            var result = await controller.Deactivate(waiverId, CancellationToken.None);

            Assert.IsType<NotFoundResult>(result);
        }
    }
}
