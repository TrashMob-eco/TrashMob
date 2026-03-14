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

    public class PartnerLocationsV2ControllerTests
    {
        private readonly Mock<IPartnerLocationManager> partnerLocationManager = new();
        private readonly Mock<IKeyedManager<Partner>> partnerManager = new();
        private readonly Mock<IAuthorizationService> authorizationService = new();
        private readonly Mock<ILogger<PartnerLocationsV2Controller>> logger = new();
        private readonly PartnerLocationsV2Controller controller;
        private readonly Guid testUserId = Guid.NewGuid();

        public PartnerLocationsV2ControllerTests()
        {
            controller = new PartnerLocationsV2Controller(
                partnerLocationManager.Object,
                partnerManager.Object,
                authorizationService.Object,
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
        public async Task GetByPartner_ReturnsOkWithList()
        {
            var partnerId = Guid.NewGuid();
            var locations = new List<PartnerLocation>
            {
                new()
                {
                    Id = Guid.NewGuid(),
                    PartnerId = partnerId,
                    Name = "Downtown Office",
                    City = "Seattle",
                    Region = "WA",
                    Country = "US",
                    IsActive = true,
                },
                new()
                {
                    Id = Guid.NewGuid(),
                    PartnerId = partnerId,
                    Name = "Eastside Branch",
                    City = "Bellevue",
                    Region = "WA",
                    Country = "US",
                    IsActive = true,
                },
            };

            partnerLocationManager
                .Setup(m => m.GetByParentIdAsync(partnerId, It.IsAny<CancellationToken>()))
                .ReturnsAsync(locations);

            var result = await controller.GetByPartner(partnerId, CancellationToken.None);

            var okResult = Assert.IsType<OkObjectResult>(result);
            var dtos = Assert.IsAssignableFrom<IEnumerable<PartnerLocationDto>>(okResult.Value);
            Assert.Equal(2, dtos.Count());
            Assert.Equal("Downtown Office", dtos.First().Name);
        }

        [Fact]
        public async Task Get_Found_ReturnsOk()
        {
            var locationId = Guid.NewGuid();
            var location = new PartnerLocation
            {
                Id = locationId,
                PartnerId = Guid.NewGuid(),
                Name = "Main Location",
                City = "Portland",
                Region = "OR",
                Country = "US",
                IsActive = true,
            };

            partnerLocationManager
                .Setup(m => m.GetAsync(locationId, It.IsAny<CancellationToken>()))
                .ReturnsAsync(location);

            var result = await controller.Get(locationId, CancellationToken.None);

            var okResult = Assert.IsType<OkObjectResult>(result);
            var dto = Assert.IsType<PartnerLocationDto>(okResult.Value);
            Assert.Equal(locationId, dto.Id);
            Assert.Equal("Main Location", dto.Name);
        }

        [Fact]
        public async Task Get_NotFound_ReturnsNotFound()
        {
            var locationId = Guid.NewGuid();

            partnerLocationManager
                .Setup(m => m.GetAsync(locationId, It.IsAny<CancellationToken>()))
                .ReturnsAsync((PartnerLocation)null);

            var result = await controller.Get(locationId, CancellationToken.None);

            Assert.IsType<NotFoundResult>(result);
        }

        [Fact]
        public async Task Add_Authorized_Returns201()
        {
            var partnerId = Guid.NewGuid();
            var partner = new Partner { Id = partnerId, Name = "Test Partner", PartnerStatusId = 1 };
            var dto = new PartnerLocationDto
            {
                PartnerId = partnerId,
                Name = "New Location",
                City = "Tacoma",
                Region = "WA",
                Country = "US",
                IsActive = true,
            };

            var createdEntity = new PartnerLocation
            {
                Id = Guid.NewGuid(),
                PartnerId = partnerId,
                Name = "New Location",
                City = "Tacoma",
                Region = "WA",
                Country = "US",
                IsActive = true,
            };

            partnerManager
                .Setup(m => m.GetAsync(partnerId, It.IsAny<CancellationToken>()))
                .ReturnsAsync(partner);

            authorizationService
                .Setup(a => a.AuthorizeAsync(It.IsAny<ClaimsPrincipal>(), It.IsAny<object>(), It.IsAny<string>()))
                .ReturnsAsync(AuthorizationResult.Success());

            partnerLocationManager
                .Setup(m => m.AddAsync(It.IsAny<PartnerLocation>(), testUserId, It.IsAny<CancellationToken>()))
                .ReturnsAsync(createdEntity);

            var result = await controller.Add(dto, CancellationToken.None);

            var createdResult = Assert.IsType<CreatedAtActionResult>(result);
            Assert.Equal(StatusCodes.Status201Created, createdResult.StatusCode);
            var resultDto = Assert.IsType<PartnerLocationDto>(createdResult.Value);
            Assert.Equal("New Location", resultDto.Name);
        }

        [Fact]
        public async Task Add_EmptyPartnerId_ReturnsBadRequest()
        {
            var dto = new PartnerLocationDto
            {
                PartnerId = Guid.Empty,
                Name = "Bad Location",
            };

            var result = await controller.Add(dto, CancellationToken.None);

            Assert.IsType<BadRequestObjectResult>(result);
        }

        [Fact]
        public async Task Delete_Authorized_ReturnsNoContent()
        {
            var locationId = Guid.NewGuid();
            var partner = new Partner { Id = Guid.NewGuid(), Name = "Test Partner" };

            partnerLocationManager
                .Setup(m => m.GetPartnerForLocationAsync(locationId, It.IsAny<CancellationToken>()))
                .ReturnsAsync(partner);

            authorizationService
                .Setup(a => a.AuthorizeAsync(It.IsAny<ClaimsPrincipal>(), It.IsAny<object>(), It.IsAny<string>()))
                .ReturnsAsync(AuthorizationResult.Success());

            partnerLocationManager
                .Setup(m => m.DeleteAsync(locationId, It.IsAny<CancellationToken>()))
                .ReturnsAsync(1);

            var result = await controller.Delete(locationId, CancellationToken.None);

            Assert.IsType<NoContentResult>(result);
        }

        [Fact]
        public async Task Delete_PartnerNotFound_ReturnsNotFound()
        {
            var locationId = Guid.NewGuid();

            partnerLocationManager
                .Setup(m => m.GetPartnerForLocationAsync(locationId, It.IsAny<CancellationToken>()))
                .ReturnsAsync((Partner)null);

            var result = await controller.Delete(locationId, CancellationToken.None);

            Assert.IsType<NotFoundResult>(result);
        }
    }
}
