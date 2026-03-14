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

    public class CommunitySponsoredAdoptionsV2ControllerTests
    {
        private readonly Mock<ISponsoredAdoptionManager> adoptionManager = new();
        private readonly Mock<IKeyedManager<Partner>> partnerManager = new();
        private readonly Mock<IAuthorizationService> authorizationService = new();
        private readonly Mock<ILogger<CommunitySponsoredAdoptionsV2Controller>> logger = new();
        private readonly CommunitySponsoredAdoptionsV2Controller controller;
        private readonly Guid testUserId = Guid.NewGuid();

        public CommunitySponsoredAdoptionsV2ControllerTests()
        {
            controller = new CommunitySponsoredAdoptionsV2Controller(
                adoptionManager.Object, partnerManager.Object,
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
        public async Task GetSponsoredAdoptions_Authorized_ReturnsOkWithList()
        {
            SetupAuthSuccess();

            var partnerId = Guid.NewGuid();
            var partner = new Partner { Id = partnerId, Name = "Test Partner" };
            var adoptions = new List<SponsoredAdoption>
            {
                new() { Id = Guid.NewGuid(), SponsorId = Guid.NewGuid(), AdoptableAreaId = Guid.NewGuid(), ProfessionalCompanyId = Guid.NewGuid(), StartDate = DateOnly.FromDateTime(DateTime.Today), Status = "Active" },
                new() { Id = Guid.NewGuid(), SponsorId = Guid.NewGuid(), AdoptableAreaId = Guid.NewGuid(), ProfessionalCompanyId = Guid.NewGuid(), StartDate = DateOnly.FromDateTime(DateTime.Today), Status = "Expired" },
            };

            partnerManager
                .Setup(m => m.GetAsync(partnerId, It.IsAny<CancellationToken>()))
                .ReturnsAsync(partner);
            adoptionManager
                .Setup(m => m.GetByCommunityAsync(partnerId, It.IsAny<CancellationToken>()))
                .ReturnsAsync(adoptions);

            var result = await controller.GetSponsoredAdoptions(partnerId, CancellationToken.None);

            var okResult = Assert.IsType<OkObjectResult>(result);
            var dtos = Assert.IsAssignableFrom<IEnumerable<SponsoredAdoptionDto>>(okResult.Value);
            Assert.Equal(2, dtos.Count());
        }

        [Fact]
        public async Task CreateSponsoredAdoption_Authorized_ReturnsCreated()
        {
            SetupAuthSuccess();

            var partnerId = Guid.NewGuid();
            var partner = new Partner { Id = partnerId, Name = "Test Partner" };
            var adoptionDto = new SponsoredAdoptionDto
            {
                AdoptableAreaId = Guid.NewGuid(),
                SponsorId = Guid.NewGuid(),
                ProfessionalCompanyId = Guid.NewGuid(),
                StartDate = DateOnly.FromDateTime(DateTime.Today),
                CleanupFrequencyDays = 14,
                Status = "Active",
            };
            var createdAdoption = new SponsoredAdoption
            {
                Id = Guid.NewGuid(),
                AdoptableAreaId = adoptionDto.AdoptableAreaId,
                SponsorId = adoptionDto.SponsorId,
                ProfessionalCompanyId = adoptionDto.ProfessionalCompanyId,
                StartDate = adoptionDto.StartDate,
                CleanupFrequencyDays = 14,
                Status = "Active",
            };

            partnerManager
                .Setup(m => m.GetAsync(partnerId, It.IsAny<CancellationToken>()))
                .ReturnsAsync(partner);
            adoptionManager
                .Setup(m => m.AddAsync(It.IsAny<SponsoredAdoption>(), testUserId, It.IsAny<CancellationToken>()))
                .ReturnsAsync(createdAdoption);

            var result = await controller.CreateSponsoredAdoption(partnerId, adoptionDto, CancellationToken.None);

            var createdResult = Assert.IsType<CreatedAtActionResult>(result);
            Assert.Equal(StatusCodes.Status201Created, createdResult.StatusCode);
            var dto = Assert.IsType<SponsoredAdoptionDto>(createdResult.Value);
            Assert.Equal(createdAdoption.Id, dto.Id);
        }

        [Fact]
        public async Task GetComplianceStats_Authorized_ReturnsOk()
        {
            SetupAuthSuccess();

            var partnerId = Guid.NewGuid();
            var partner = new Partner { Id = partnerId, Name = "Test Partner" };
            var stats = new SponsoredAdoptionComplianceStats
            {
                TotalSponsoredAdoptions = 10,
                ActiveAdoptions = 8,
                AdoptionsOnSchedule = 6,
                AdoptionsOverdue = 2,
            };

            partnerManager
                .Setup(m => m.GetAsync(partnerId, It.IsAny<CancellationToken>()))
                .ReturnsAsync(partner);
            adoptionManager
                .Setup(m => m.GetComplianceByCommunityAsync(partnerId, It.IsAny<CancellationToken>()))
                .ReturnsAsync(stats);

            var result = await controller.GetComplianceStats(partnerId, CancellationToken.None);

            var okResult = Assert.IsType<OkObjectResult>(result);
            var returnedStats = Assert.IsType<SponsoredAdoptionComplianceStats>(okResult.Value);
            Assert.Equal(10, returnedStats.TotalSponsoredAdoptions);
            Assert.Equal(8, returnedStats.ActiveAdoptions);
        }
    }
}
