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

    public class PartnerAdminsV2ControllerTests
    {
        private readonly Mock<IPartnerAdminManager> partnerAdminManager = new();
        private readonly Mock<IKeyedManager<Partner>> partnerManager = new();
        private readonly Mock<IAuthorizationService> authorizationService = new();
        private readonly Mock<ILogger<PartnerAdminsV2Controller>> logger = new();
        private readonly PartnerAdminsV2Controller controller;
        private readonly Guid testUserId = Guid.NewGuid();

        public PartnerAdminsV2ControllerTests()
        {
            controller = new PartnerAdminsV2Controller(
                partnerAdminManager.Object, partnerManager.Object,
                authorizationService.Object, logger.Object);

            var httpContext = new DefaultHttpContext();
            httpContext.Items["UserId"] = testUserId.ToString();
            httpContext.User = new ClaimsPrincipal(new ClaimsIdentity(
            [
                new Claim(ClaimTypes.NameIdentifier, testUserId.ToString()),
            ], "TestAuth"));
            controller.ControllerContext = new ControllerContext { HttpContext = httpContext };
        }

        private void SetupAuthSuccess()
        {
            authorizationService
                .Setup(a => a.AuthorizeAsync(It.IsAny<ClaimsPrincipal>(), It.IsAny<object>(), It.IsAny<string>()))
                .ReturnsAsync(AuthorizationResult.Success());
        }

        [Fact]
        public async Task GetAdmins_Authorized_ReturnsOkWithUserList()
        {
            SetupAuthSuccess();

            var partnerId = Guid.NewGuid();
            var partner = new Partner { Id = partnerId, Name = "Test Partner" };
            var admins = new List<User>
            {
                new() { Id = Guid.NewGuid(), UserName = "admin1" },
                new() { Id = Guid.NewGuid(), UserName = "admin2" },
            };

            partnerManager
                .Setup(m => m.GetAsync(partnerId, It.IsAny<CancellationToken>()))
                .ReturnsAsync(partner);

            partnerAdminManager
                .Setup(m => m.GetAdminsForPartnerAsync(partnerId, It.IsAny<CancellationToken>()))
                .ReturnsAsync(admins);

            var result = await controller.GetAdmins(partnerId, CancellationToken.None);

            var okResult = Assert.IsType<OkObjectResult>(result);
            var dtos = Assert.IsAssignableFrom<IEnumerable<UserDto>>(okResult.Value);
            Assert.Equal(2, dtos.Count());
        }

        [Fact]
        public async Task GetAdmins_PartnerNotFound_Returns404()
        {
            var partnerId = Guid.NewGuid();

            partnerManager
                .Setup(m => m.GetAsync(partnerId, It.IsAny<CancellationToken>()))
                .ReturnsAsync((Partner)null);

            var result = await controller.GetAdmins(partnerId, CancellationToken.None);

            Assert.IsType<NotFoundResult>(result);
        }

        [Fact]
        public async Task GetMyPartners_ReturnsOkWithPartnerList()
        {
            var partners = new List<Partner>
            {
                new() { Id = Guid.NewGuid(), Name = "Partner A" },
                new() { Id = Guid.NewGuid(), Name = "Partner B" },
            };

            partnerAdminManager
                .Setup(m => m.GetPartnersByUserIdAsync(testUserId, It.IsAny<CancellationToken>()))
                .ReturnsAsync(partners);

            var result = await controller.GetMyPartners(CancellationToken.None);

            var okResult = Assert.IsType<OkObjectResult>(result);
            var dtos = Assert.IsAssignableFrom<IEnumerable<PartnerDto>>(okResult.Value);
            Assert.Equal(2, dtos.Count());
        }

        [Fact]
        public async Task GetPartnerUser_Found_ReturnsOk()
        {
            SetupAuthSuccess();

            var partnerId = Guid.NewGuid();
            var userId = Guid.NewGuid();
            var partner = new Partner { Id = partnerId, Name = "Test Partner" };
            var admins = new List<PartnerAdmin>
            {
                new() { PartnerId = partnerId, UserId = userId },
                new() { PartnerId = partnerId, UserId = Guid.NewGuid() },
            };

            partnerManager
                .Setup(m => m.GetAsync(partnerId, It.IsAny<CancellationToken>()))
                .ReturnsAsync(partner);

            partnerAdminManager
                .Setup(m => m.GetByParentIdAsync(partnerId, It.IsAny<CancellationToken>()))
                .ReturnsAsync(admins);

            var result = await controller.GetPartnerUser(partnerId, userId, CancellationToken.None);

            var okResult = Assert.IsType<OkObjectResult>(result);
            var dto = Assert.IsType<PartnerAdminDto>(okResult.Value);
            Assert.Equal(partnerId, dto.PartnerId);
            Assert.Equal(userId, dto.UserId);
        }

        [Fact]
        public async Task GetPartnerUser_NotFound_Returns404()
        {
            SetupAuthSuccess();

            var partnerId = Guid.NewGuid();
            var userId = Guid.NewGuid();
            var partner = new Partner { Id = partnerId, Name = "Test Partner" };
            var admins = new List<PartnerAdmin>
            {
                new() { PartnerId = partnerId, UserId = Guid.NewGuid() },
            };

            partnerManager
                .Setup(m => m.GetAsync(partnerId, It.IsAny<CancellationToken>()))
                .ReturnsAsync(partner);

            partnerAdminManager
                .Setup(m => m.GetByParentIdAsync(partnerId, It.IsAny<CancellationToken>()))
                .ReturnsAsync(admins);

            var result = await controller.GetPartnerUser(partnerId, userId, CancellationToken.None);

            Assert.IsType<NotFoundResult>(result);
        }

        [Fact]
        public async Task AddPartnerUser_Authorized_Returns201()
        {
            SetupAuthSuccess();

            var partnerId = Guid.NewGuid();
            var userId = Guid.NewGuid();
            var partner = new Partner { Id = partnerId, Name = "Test Partner" };

            partnerManager
                .Setup(m => m.GetAsync(partnerId, It.IsAny<CancellationToken>()))
                .ReturnsAsync(partner);

            partnerAdminManager
                .Setup(m => m.AddAsync(It.IsAny<PartnerAdmin>(), testUserId, It.IsAny<CancellationToken>()))
                .ReturnsAsync(new PartnerAdmin { PartnerId = partnerId, UserId = userId });

            var result = await controller.AddPartnerUser(partnerId, userId, CancellationToken.None);

            var statusResult = Assert.IsType<ObjectResult>(result);
            Assert.Equal(StatusCodes.Status201Created, statusResult.StatusCode);
            var dto = Assert.IsType<PartnerAdminDto>(statusResult.Value);
            Assert.Equal(partnerId, dto.PartnerId);
            Assert.Equal(userId, dto.UserId);
        }

        [Fact]
        public async Task AddPartnerUser_PartnerNotFound_Returns404()
        {
            var partnerId = Guid.NewGuid();
            var userId = Guid.NewGuid();

            partnerManager
                .Setup(m => m.GetAsync(partnerId, It.IsAny<CancellationToken>()))
                .ReturnsAsync((Partner)null);

            var result = await controller.AddPartnerUser(partnerId, userId, CancellationToken.None);

            Assert.IsType<NotFoundResult>(result);
        }
    }
}
