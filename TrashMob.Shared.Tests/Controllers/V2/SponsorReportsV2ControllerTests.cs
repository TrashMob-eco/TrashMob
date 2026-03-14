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

    public class SponsorReportsV2ControllerTests
    {
        private readonly Mock<ISponsoredAdoptionManager> adoptionManager = new();
        private readonly Mock<IProfessionalCleanupLogManager> logManager = new();
        private readonly Mock<ISponsorManager> sponsorManager = new();
        private readonly Mock<IKeyedManager<Partner>> partnerManager = new();
        private readonly Mock<IAuthorizationService> authorizationService = new();
        private readonly Mock<ILogger<SponsorReportsV2Controller>> logger = new();
        private readonly SponsorReportsV2Controller controller;
        private readonly Guid testUserId = Guid.NewGuid();

        public SponsorReportsV2ControllerTests()
        {
            controller = new SponsorReportsV2Controller(
                adoptionManager.Object, logManager.Object, sponsorManager.Object,
                partnerManager.Object, authorizationService.Object, logger.Object);

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
        public async Task GetAdoptions_SponsorNotFound_ReturnsNotFound()
        {
            var sponsorId = Guid.NewGuid();

            sponsorManager
                .Setup(m => m.GetAsync(sponsorId, It.IsAny<CancellationToken>()))
                .ReturnsAsync((Sponsor)null);

            var result = await controller.GetAdoptions(sponsorId, CancellationToken.None);

            Assert.IsType<NotFoundResult>(result);
        }

        [Fact]
        public async Task GetAdoptions_Success_ReturnsOk()
        {
            SetupAuthSuccess();

            var partnerId = Guid.NewGuid();
            var sponsorId = Guid.NewGuid();
            var sponsor = new Sponsor { Id = sponsorId, PartnerId = partnerId, Name = "Test Sponsor", IsActive = true };
            var partner = new Partner { Id = partnerId, Name = "Test Partner" };
            var adoptions = new List<SponsoredAdoption>
            {
                new()
                {
                    Id = Guid.NewGuid(),
                    SponsorId = sponsorId,
                    AdoptableAreaId = Guid.NewGuid(),
                    ProfessionalCompanyId = Guid.NewGuid(),
                    StartDate = DateOnly.FromDateTime(DateTime.UtcNow),
                    Status = "Active",
                },
            };

            sponsorManager
                .Setup(m => m.GetAsync(sponsorId, It.IsAny<CancellationToken>()))
                .ReturnsAsync(sponsor);
            partnerManager
                .Setup(m => m.GetAsync(partnerId, It.IsAny<CancellationToken>()))
                .ReturnsAsync(partner);
            adoptionManager
                .Setup(m => m.GetBySponsorIdAsync(sponsorId, It.IsAny<CancellationToken>()))
                .ReturnsAsync(adoptions);

            var result = await controller.GetAdoptions(sponsorId, CancellationToken.None);

            var okResult = Assert.IsType<OkObjectResult>(result);
            var dtos = Assert.IsAssignableFrom<IEnumerable<SponsoredAdoptionDto>>(okResult.Value);
            Assert.Single(dtos);
        }

        [Fact]
        public async Task GetAdoptionReports_AdoptionNotFound_ReturnsNotFound()
        {
            SetupAuthSuccess();

            var partnerId = Guid.NewGuid();
            var sponsorId = Guid.NewGuid();
            var adoptionId = Guid.NewGuid();
            var sponsor = new Sponsor { Id = sponsorId, PartnerId = partnerId, Name = "Test Sponsor", IsActive = true };
            var partner = new Partner { Id = partnerId, Name = "Test Partner" };

            sponsorManager
                .Setup(m => m.GetAsync(sponsorId, It.IsAny<CancellationToken>()))
                .ReturnsAsync(sponsor);
            partnerManager
                .Setup(m => m.GetAsync(partnerId, It.IsAny<CancellationToken>()))
                .ReturnsAsync(partner);
            adoptionManager
                .Setup(m => m.GetAsync(adoptionId, It.IsAny<CancellationToken>()))
                .ReturnsAsync((SponsoredAdoption)null);

            var result = await controller.GetAdoptionReports(sponsorId, adoptionId, CancellationToken.None);

            Assert.IsType<NotFoundResult>(result);
        }

        [Fact]
        public async Task GetAdoptionReports_Success_ReturnsOk()
        {
            SetupAuthSuccess();

            var partnerId = Guid.NewGuid();
            var sponsorId = Guid.NewGuid();
            var adoptionId = Guid.NewGuid();
            var sponsor = new Sponsor { Id = sponsorId, PartnerId = partnerId, Name = "Test Sponsor", IsActive = true };
            var partner = new Partner { Id = partnerId, Name = "Test Partner" };
            var adoption = new SponsoredAdoption
            {
                Id = adoptionId,
                SponsorId = sponsorId,
                AdoptableAreaId = Guid.NewGuid(),
                ProfessionalCompanyId = Guid.NewGuid(),
                StartDate = DateOnly.FromDateTime(DateTime.UtcNow),
                Status = "Active",
            };
            var logs = new List<ProfessionalCleanupLog>
            {
                new()
                {
                    Id = Guid.NewGuid(),
                    SponsoredAdoptionId = adoptionId,
                    ProfessionalCompanyId = Guid.NewGuid(),
                    CleanupDate = DateTimeOffset.UtcNow,
                    DurationMinutes = 30,
                    BagsCollected = 2,
                    Notes = "Report test",
                },
            };

            sponsorManager
                .Setup(m => m.GetAsync(sponsorId, It.IsAny<CancellationToken>()))
                .ReturnsAsync(sponsor);
            partnerManager
                .Setup(m => m.GetAsync(partnerId, It.IsAny<CancellationToken>()))
                .ReturnsAsync(partner);
            adoptionManager
                .Setup(m => m.GetAsync(adoptionId, It.IsAny<CancellationToken>()))
                .ReturnsAsync(adoption);
            logManager
                .Setup(m => m.GetBySponsoredAdoptionIdAsync(adoptionId, It.IsAny<CancellationToken>()))
                .ReturnsAsync(logs);

            var result = await controller.GetAdoptionReports(sponsorId, adoptionId, CancellationToken.None);

            var okResult = Assert.IsType<OkObjectResult>(result);
            var dtos = Assert.IsAssignableFrom<IEnumerable<ProfessionalCleanupLogDto>>(okResult.Value);
            Assert.Single(dtos);
        }
    }
}
