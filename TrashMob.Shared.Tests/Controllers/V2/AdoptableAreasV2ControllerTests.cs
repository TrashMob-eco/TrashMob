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
    using TrashMob.Shared.Managers.Areas;
    using TrashMob.Shared.Managers.Interfaces;
    using Xunit;

    public class AdoptableAreasV2ControllerTests
    {
        private readonly Mock<IAdoptableAreaManager> areaManager = new();
        private readonly Mock<IKeyedManager<Partner>> partnerManager = new();
        private readonly Mock<IAreaSuggestionService> areaSuggestionService = new();
        private readonly Mock<IAreaFileParser> areaFileParser = new();
        private readonly Mock<IAreaGenerationBatchManager> batchManager = new();
        private readonly Mock<IAuthorizationService> authorizationService = new();
        private readonly Mock<ILogger<AdoptableAreasV2Controller>> logger = new();
        private readonly AdoptableAreasV2Controller controller;
        private readonly Guid testUserId = Guid.NewGuid();

        public AdoptableAreasV2ControllerTests()
        {
            controller = new AdoptableAreasV2Controller(
                areaManager.Object,
                partnerManager.Object,
                areaSuggestionService.Object,
                areaFileParser.Object,
                batchManager.Object,
                authorizationService.Object,
                logger.Object);

            var httpContext = new DefaultHttpContext();
            httpContext.Items["UserId"] = testUserId.ToString();
            httpContext.User = new ClaimsPrincipal(new ClaimsIdentity(
                [new Claim(ClaimTypes.NameIdentifier, testUserId.ToString())], "TestAuth"));
            controller.ControllerContext = new ControllerContext { HttpContext = httpContext };
        }

        [Fact]
        public async Task GetAreas_ReturnsOkWithList()
        {
            var partnerId = Guid.NewGuid();
            var partner = new Partner { Id = partnerId, Name = "Test Community" };
            var areas = new List<AdoptableArea>
            {
                new() { Id = Guid.NewGuid(), PartnerId = partnerId, Name = "Park A" },
                new() { Id = Guid.NewGuid(), PartnerId = partnerId, Name = "Trail B" },
            };

            partnerManager
                .Setup(m => m.GetAsync(partnerId, It.IsAny<CancellationToken>()))
                .ReturnsAsync(partner);
            areaManager
                .Setup(m => m.GetByCommunityAsync(partnerId, It.IsAny<CancellationToken>()))
                .ReturnsAsync(areas);

            var result = await controller.GetAreas(partnerId, CancellationToken.None);

            var okResult = Assert.IsType<OkObjectResult>(result);
            var dtos = Assert.IsAssignableFrom<IEnumerable<AdoptableAreaDto>>(okResult.Value);
            Assert.Equal(2, dtos.Count());
        }

        [Fact]
        public async Task GetAreas_ReturnsNotFound_WhenPartnerMissing()
        {
            var partnerId = Guid.NewGuid();
            partnerManager
                .Setup(m => m.GetAsync(partnerId, It.IsAny<CancellationToken>()))
                .ReturnsAsync((Partner)null);

            var result = await controller.GetAreas(partnerId, CancellationToken.None);

            Assert.IsType<NotFoundResult>(result);
        }

        [Fact]
        public async Task GetArea_Found_ReturnsOk()
        {
            var partnerId = Guid.NewGuid();
            var areaId = Guid.NewGuid();
            var area = new AdoptableArea { Id = areaId, PartnerId = partnerId, Name = "Park A" };

            areaManager
                .Setup(m => m.GetAsync(areaId, It.IsAny<CancellationToken>()))
                .ReturnsAsync(area);

            var result = await controller.GetArea(partnerId, areaId, CancellationToken.None);

            var okResult = Assert.IsType<OkObjectResult>(result);
            var dto = Assert.IsType<AdoptableAreaDto>(okResult.Value);
            Assert.Equal("Park A", dto.Name);
        }

        [Fact]
        public async Task GetArea_ReturnsNotFound_WhenAreaMissing()
        {
            var partnerId = Guid.NewGuid();
            var areaId = Guid.NewGuid();

            areaManager
                .Setup(m => m.GetAsync(areaId, It.IsAny<CancellationToken>()))
                .ReturnsAsync((AdoptableArea)null);

            var result = await controller.GetArea(partnerId, areaId, CancellationToken.None);

            Assert.IsType<NotFoundResult>(result);
        }

        [Fact]
        public async Task CreateArea_Authorized_ReturnsCreated()
        {
            var partnerId = Guid.NewGuid();
            var partner = new Partner { Id = partnerId, Name = "Test Community" };
            var areaDto = new AdoptableAreaDto { Name = "New Park", AreaType = "Park" };
            var createdArea = new AdoptableArea
            {
                Id = Guid.NewGuid(),
                PartnerId = partnerId,
                Name = "New Park",
                AreaType = "Park",
            };

            partnerManager
                .Setup(m => m.GetAsync(partnerId, It.IsAny<CancellationToken>()))
                .ReturnsAsync(partner);
            authorizationService
                .Setup(a => a.AuthorizeAsync(It.IsAny<ClaimsPrincipal>(), It.IsAny<object>(), It.IsAny<string>()))
                .ReturnsAsync(AuthorizationResult.Success());
            areaManager
                .Setup(m => m.IsNameAvailableAsync(partnerId, "New Park", null, It.IsAny<CancellationToken>()))
                .ReturnsAsync(true);
            areaManager
                .Setup(m => m.AddAsync(It.IsAny<AdoptableArea>(), testUserId, It.IsAny<CancellationToken>()))
                .ReturnsAsync(createdArea);

            var result = await controller.CreateArea(partnerId, areaDto, CancellationToken.None);

            var createdResult = Assert.IsType<CreatedAtActionResult>(result);
            Assert.Equal(201, createdResult.StatusCode);
            var dto = Assert.IsType<AdoptableAreaDto>(createdResult.Value);
            Assert.Equal("New Park", dto.Name);
        }

        [Fact]
        public async Task DeleteArea_Authorized_ReturnsNoContent()
        {
            var partnerId = Guid.NewGuid();
            var areaId = Guid.NewGuid();
            var partner = new Partner { Id = partnerId, Name = "Test Community" };
            var area = new AdoptableArea { Id = areaId, PartnerId = partnerId, Name = "Park A", IsActive = true };

            partnerManager
                .Setup(m => m.GetAsync(partnerId, It.IsAny<CancellationToken>()))
                .ReturnsAsync(partner);
            authorizationService
                .Setup(a => a.AuthorizeAsync(It.IsAny<ClaimsPrincipal>(), It.IsAny<object>(), It.IsAny<string>()))
                .ReturnsAsync(AuthorizationResult.Success());
            areaManager
                .Setup(m => m.GetAsync(areaId, It.IsAny<CancellationToken>()))
                .ReturnsAsync(area);
            areaManager
                .Setup(m => m.UpdateAsync(It.IsAny<AdoptableArea>(), testUserId, It.IsAny<CancellationToken>()))
                .ReturnsAsync(area);

            var result = await controller.DeleteArea(partnerId, areaId, CancellationToken.None);

            Assert.IsType<NoContentResult>(result);
            areaManager.Verify(
                m => m.UpdateAsync(It.Is<AdoptableArea>(a => !a.IsActive), testUserId, It.IsAny<CancellationToken>()),
                Times.Once);
        }
    }
}
