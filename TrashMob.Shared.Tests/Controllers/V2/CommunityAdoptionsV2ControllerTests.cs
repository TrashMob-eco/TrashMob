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
    using TrashMob.Shared.Poco;
    using Xunit;

    public class CommunityAdoptionsV2ControllerTests
    {
        private readonly Mock<ITeamAdoptionManager> adoptionManager = new();
        private readonly Mock<IKeyedManager<Partner>> partnerManager = new();
        private readonly Mock<IAuthorizationService> authorizationService = new();
        private readonly Mock<ILogger<CommunityAdoptionsV2Controller>> logger = new();
        private readonly CommunityAdoptionsV2Controller controller;
        private readonly Guid testUserId = Guid.NewGuid();

        public CommunityAdoptionsV2ControllerTests()
        {
            controller = new CommunityAdoptionsV2Controller(
                adoptionManager.Object,
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
        public async Task GetPendingApplications_ReturnsOkWithList()
        {
            var partnerId = Guid.NewGuid();
            var partner = new Partner { Id = partnerId, Name = "Test Community" };
            var adoptions = new List<TeamAdoption>
            {
                new() { Id = Guid.NewGuid(), TeamId = Guid.NewGuid(), AdoptableAreaId = Guid.NewGuid(), Status = "Pending" },
                new() { Id = Guid.NewGuid(), TeamId = Guid.NewGuid(), AdoptableAreaId = Guid.NewGuid(), Status = "Pending" },
            };

            partnerManager
                .Setup(m => m.GetAsync(partnerId, It.IsAny<CancellationToken>()))
                .ReturnsAsync(partner);
            authorizationService
                .Setup(a => a.AuthorizeAsync(It.IsAny<ClaimsPrincipal>(), It.IsAny<object>(), It.IsAny<string>()))
                .ReturnsAsync(AuthorizationResult.Success());
            adoptionManager
                .Setup(m => m.GetPendingByCommunityAsync(partnerId, It.IsAny<CancellationToken>()))
                .ReturnsAsync(adoptions);

            var result = await controller.GetPendingApplications(partnerId, CancellationToken.None);

            var okResult = Assert.IsType<OkObjectResult>(result);
            var dtos = Assert.IsAssignableFrom<IEnumerable<TeamAdoptionDto>>(okResult.Value);
            Assert.Equal(2, dtos.Count());
        }

        [Fact]
        public async Task ApproveApplication_ReturnsOk()
        {
            var partnerId = Guid.NewGuid();
            var adoptionId = Guid.NewGuid();
            var partner = new Partner { Id = partnerId, Name = "Test Community" };
            var adoption = new TeamAdoption
            {
                Id = adoptionId,
                TeamId = Guid.NewGuid(),
                AdoptableAreaId = Guid.NewGuid(),
                Status = "Approved",
            };

            partnerManager
                .Setup(m => m.GetAsync(partnerId, It.IsAny<CancellationToken>()))
                .ReturnsAsync(partner);
            authorizationService
                .Setup(a => a.AuthorizeAsync(It.IsAny<ClaimsPrincipal>(), It.IsAny<object>(), It.IsAny<string>()))
                .ReturnsAsync(AuthorizationResult.Success());
            adoptionManager
                .Setup(m => m.ApproveApplicationAsync(adoptionId, testUserId, It.IsAny<CancellationToken>()))
                .ReturnsAsync(ServiceResult<TeamAdoption>.Success(adoption));

            var result = await controller.ApproveApplication(partnerId, adoptionId, CancellationToken.None);

            var okResult = Assert.IsType<OkObjectResult>(result);
            var dto = Assert.IsType<TeamAdoptionDto>(okResult.Value);
            Assert.Equal(adoptionId, dto.Id);
        }

        [Fact]
        public async Task GetComplianceStats_ReturnsOk()
        {
            var partnerId = Guid.NewGuid();
            var partner = new Partner { Id = partnerId, Name = "Test Community" };
            var stats = new AdoptionComplianceStats
            {
                TotalAdoptions = 10,
                CompliantAdoptions = 8,
                DelinquentAdoptions = 2,
                TotalAvailableAreas = 20,
                AdoptedAreas = 10,
            };

            partnerManager
                .Setup(m => m.GetAsync(partnerId, It.IsAny<CancellationToken>()))
                .ReturnsAsync(partner);
            authorizationService
                .Setup(a => a.AuthorizeAsync(It.IsAny<ClaimsPrincipal>(), It.IsAny<object>(), It.IsAny<string>()))
                .ReturnsAsync(AuthorizationResult.Success());
            adoptionManager
                .Setup(m => m.GetComplianceStatsByCommunityAsync(partnerId, It.IsAny<CancellationToken>()))
                .ReturnsAsync(stats);

            var result = await controller.GetComplianceStats(partnerId, CancellationToken.None);

            var okResult = Assert.IsType<OkObjectResult>(result);
            var returnedStats = Assert.IsType<AdoptionComplianceStats>(okResult.Value);
            Assert.Equal(10, returnedStats.TotalAdoptions);
            Assert.Equal(8, returnedStats.CompliantAdoptions);
        }

        [Fact]
        public async Task GetPendingApplications_ReturnsForbid_WhenNotAuthorized()
        {
            var partnerId = Guid.NewGuid();
            var partner = new Partner { Id = partnerId, Name = "Test Community" };

            partnerManager
                .Setup(m => m.GetAsync(partnerId, It.IsAny<CancellationToken>()))
                .ReturnsAsync(partner);
            authorizationService
                .Setup(a => a.AuthorizeAsync(It.IsAny<ClaimsPrincipal>(), It.IsAny<object>(), It.IsAny<string>()))
                .ReturnsAsync(AuthorizationResult.Failed());

            var result = await controller.GetPendingApplications(partnerId, CancellationToken.None);

            Assert.IsType<ForbidResult>(result);
        }
    }
}
