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

    public class PartnerLocationServicesV2ControllerTests
    {
        private readonly Mock<IBaseManager<PartnerLocationService>> partnerLocationServiceManager = new();
        private readonly Mock<IPartnerLocationManager> partnerLocationManager = new();
        private readonly Mock<IKeyedManager<Partner>> partnerManager = new();
        private readonly Mock<IAuthorizationService> authorizationService = new();
        private readonly Mock<ILogger<PartnerLocationServicesV2Controller>> logger = new();
        private readonly PartnerLocationServicesV2Controller controller;
        private readonly Guid testUserId = Guid.NewGuid();

        public PartnerLocationServicesV2ControllerTests()
        {
            controller = new PartnerLocationServicesV2Controller(
                partnerLocationServiceManager.Object,
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
        public async Task GetByLocation_ReturnsOkWithList()
        {
            var partnerLocationId = Guid.NewGuid();
            var services = new List<PartnerLocationService>
            {
                new()
                {
                    PartnerLocationId = partnerLocationId,
                    ServiceTypeId = 1,
                    IsAutoApproved = true,
                    IsAdvanceNoticeRequired = false,
                    Notes = "Dumpster available",
                },
                new()
                {
                    PartnerLocationId = partnerLocationId,
                    ServiceTypeId = 2,
                    IsAutoApproved = false,
                    IsAdvanceNoticeRequired = true,
                },
            };

            partnerLocationServiceManager
                .Setup(m => m.GetByParentIdAsync(partnerLocationId, It.IsAny<CancellationToken>()))
                .ReturnsAsync(services);

            var result = await controller.GetByLocation(partnerLocationId, CancellationToken.None);

            var okResult = Assert.IsType<OkObjectResult>(result);
            var dtos = Assert.IsAssignableFrom<IEnumerable<PartnerLocationServiceDto>>(okResult.Value);
            Assert.Equal(2, dtos.Count());
            Assert.Equal(1, dtos.First().ServiceTypeId);
        }

        [Fact]
        public async Task Get_Found_ReturnsOk()
        {
            var partnerLocationId = Guid.NewGuid();
            var serviceTypeId = 1;
            var service = new PartnerLocationService
            {
                PartnerLocationId = partnerLocationId,
                ServiceTypeId = serviceTypeId,
                IsAutoApproved = true,
                IsAdvanceNoticeRequired = false,
            };

            partnerLocationServiceManager
                .Setup(m => m.GetAsync(partnerLocationId, serviceTypeId, It.IsAny<CancellationToken>()))
                .ReturnsAsync(service);

            var result = await controller.Get(partnerLocationId, serviceTypeId, CancellationToken.None);

            var okResult = Assert.IsType<OkObjectResult>(result);
            var dto = Assert.IsType<PartnerLocationServiceDto>(okResult.Value);
            Assert.Equal(partnerLocationId, dto.PartnerLocationId);
            Assert.Equal(serviceTypeId, dto.ServiceTypeId);
        }

        [Fact]
        public async Task Get_NotFound_ReturnsNotFound()
        {
            var partnerLocationId = Guid.NewGuid();

            partnerLocationServiceManager
                .Setup(m => m.GetAsync(partnerLocationId, 99, It.IsAny<CancellationToken>()))
                .ReturnsAsync((PartnerLocationService)null);

            var result = await controller.Get(partnerLocationId, 99, CancellationToken.None);

            Assert.IsType<NotFoundResult>(result);
        }

        [Fact]
        public async Task Add_Authorized_ReturnsCreated()
        {
            var partnerLocationId = Guid.NewGuid();
            var partnerId = Guid.NewGuid();
            var partnerLocation = new PartnerLocation
            {
                Id = partnerLocationId,
                PartnerId = partnerId,
                Name = "Test Location",
            };
            var partner = new Partner { Id = partnerId, Name = "Test Partner" };

            var dto = new PartnerLocationServiceDto
            {
                PartnerLocationId = partnerLocationId,
                ServiceTypeId = 1,
                IsAutoApproved = true,
                IsAdvanceNoticeRequired = false,
                Notes = "Available weekdays",
            };

            var createdEntity = new PartnerLocationService
            {
                PartnerLocationId = partnerLocationId,
                ServiceTypeId = 1,
                IsAutoApproved = true,
                IsAdvanceNoticeRequired = false,
                Notes = "Available weekdays",
            };

            partnerLocationManager
                .Setup(m => m.GetAsync(partnerLocationId, It.IsAny<CancellationToken>()))
                .ReturnsAsync(partnerLocation);

            partnerManager
                .Setup(m => m.GetAsync(partnerId, It.IsAny<CancellationToken>()))
                .ReturnsAsync(partner);

            authorizationService
                .Setup(a => a.AuthorizeAsync(It.IsAny<ClaimsPrincipal>(), It.IsAny<object>(), It.IsAny<string>()))
                .ReturnsAsync(AuthorizationResult.Success());

            partnerLocationServiceManager
                .Setup(m => m.AddAsync(It.IsAny<PartnerLocationService>(), testUserId, It.IsAny<CancellationToken>()))
                .ReturnsAsync(createdEntity);

            var result = await controller.Add(dto, CancellationToken.None);

            var createdResult = Assert.IsType<CreatedAtActionResult>(result);
            Assert.Equal(StatusCodes.Status201Created, createdResult.StatusCode);
            var resultDto = Assert.IsType<PartnerLocationServiceDto>(createdResult.Value);
            Assert.Equal(1, resultDto.ServiceTypeId);
        }

        [Fact]
        public async Task Add_LocationNotFound_ReturnsNotFound()
        {
            var dto = new PartnerLocationServiceDto
            {
                PartnerLocationId = Guid.NewGuid(),
                ServiceTypeId = 1,
            };

            partnerLocationManager
                .Setup(m => m.GetAsync(dto.PartnerLocationId, It.IsAny<CancellationToken>()))
                .ReturnsAsync((PartnerLocation)null);

            var result = await controller.Add(dto, CancellationToken.None);

            Assert.IsType<NotFoundObjectResult>(result);
        }

        [Fact]
        public async Task Delete_Authorized_ReturnsNoContent()
        {
            var partnerLocationId = Guid.NewGuid();
            var serviceTypeId = 1;
            var partner = new Partner { Id = Guid.NewGuid(), Name = "Test Partner" };

            partnerLocationManager
                .Setup(m => m.GetPartnerForLocationAsync(partnerLocationId, It.IsAny<CancellationToken>()))
                .ReturnsAsync(partner);

            authorizationService
                .Setup(a => a.AuthorizeAsync(It.IsAny<ClaimsPrincipal>(), It.IsAny<object>(), It.IsAny<string>()))
                .ReturnsAsync(AuthorizationResult.Success());

            partnerLocationServiceManager
                .Setup(m => m.Delete(partnerLocationId, serviceTypeId, It.IsAny<CancellationToken>()))
                .ReturnsAsync(1);

            var result = await controller.Delete(partnerLocationId, serviceTypeId, CancellationToken.None);

            Assert.IsType<NoContentResult>(result);
        }

        [Fact]
        public async Task Delete_PartnerNotFound_ReturnsNotFound()
        {
            var partnerLocationId = Guid.NewGuid();

            partnerLocationManager
                .Setup(m => m.GetPartnerForLocationAsync(partnerLocationId, It.IsAny<CancellationToken>()))
                .ReturnsAsync((Partner)null);

            var result = await controller.Delete(partnerLocationId, 1, CancellationToken.None);

            Assert.IsType<NotFoundResult>(result);
        }
    }
}
