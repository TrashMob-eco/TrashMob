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

    public class SponsorPortalV2ControllerTests
    {
        private readonly Mock<ISponsorManager> sponsorManager = new();
        private readonly Mock<IPartnerAdminManager> partnerAdminManager = new();
        private readonly Mock<IProfessionalCleanupLogManager> logManager = new();
        private readonly Mock<IKeyedManager<Partner>> partnerManager = new();
        private readonly Mock<IAuthorizationService> authorizationService = new();
        private readonly Mock<ILogger<SponsorPortalV2Controller>> logger = new();
        private readonly SponsorPortalV2Controller controller;
        private readonly Guid testUserId = Guid.NewGuid();

        public SponsorPortalV2ControllerTests()
        {
            controller = new SponsorPortalV2Controller(
                sponsorManager.Object, partnerAdminManager.Object, logManager.Object,
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
        public async Task GetMySponsors_ReturnsOk()
        {
            var partnerId = Guid.NewGuid();
            var partner = new Partner { Id = partnerId, Name = "Test Partner" };
            var sponsors = new List<Sponsor>
            {
                new() { Id = Guid.NewGuid(), Name = "Sponsor A", PartnerId = partnerId, IsActive = true },
                new() { Id = Guid.NewGuid(), Name = "Sponsor B", PartnerId = partnerId, IsActive = true },
            };

            partnerAdminManager
                .Setup(m => m.GetPartnersByUserIdAsync(testUserId, It.IsAny<CancellationToken>()))
                .ReturnsAsync(new List<Partner> { partner });
            sponsorManager
                .Setup(m => m.GetByCommunityAsync(partnerId, It.IsAny<CancellationToken>()))
                .ReturnsAsync(sponsors);

            var result = await controller.GetMySponsors(CancellationToken.None);

            var okResult = Assert.IsType<OkObjectResult>(result);
            var dtos = Assert.IsAssignableFrom<IEnumerable<SponsorDto>>(okResult.Value);
            Assert.Equal(2, dtos.Count());
        }

        [Fact]
        public async Task GetSponsorCleanupLogs_SponsorNotFound_ReturnsNotFound()
        {
            var sponsorId = Guid.NewGuid();

            sponsorManager
                .Setup(m => m.GetAsync(sponsorId, It.IsAny<CancellationToken>()))
                .ReturnsAsync((Sponsor)null);

            var result = await controller.GetSponsorCleanupLogs(sponsorId, CancellationToken.None);

            Assert.IsType<NotFoundResult>(result);
        }

        [Fact]
        public async Task GetSponsorCleanupLogs_Success_ReturnsOk()
        {
            SetupAuthSuccess();

            var partnerId = Guid.NewGuid();
            var sponsorId = Guid.NewGuid();
            var sponsor = new Sponsor { Id = sponsorId, PartnerId = partnerId, Name = "Test Sponsor", IsActive = true };
            var partner = new Partner { Id = partnerId, Name = "Test Partner" };
            var logs = new List<ProfessionalCleanupLog>
            {
                new()
                {
                    Id = Guid.NewGuid(),
                    SponsoredAdoptionId = Guid.NewGuid(),
                    ProfessionalCompanyId = Guid.NewGuid(),
                    CleanupDate = DateTimeOffset.UtcNow,
                    DurationMinutes = 60,
                    BagsCollected = 5,
                    Notes = "Test cleanup",
                },
            };

            sponsorManager
                .Setup(m => m.GetAsync(sponsorId, It.IsAny<CancellationToken>()))
                .ReturnsAsync(sponsor);
            partnerManager
                .Setup(m => m.GetAsync(partnerId, It.IsAny<CancellationToken>()))
                .ReturnsAsync(partner);
            logManager
                .Setup(m => m.GetBySponsorIdAsync(sponsorId, It.IsAny<CancellationToken>()))
                .ReturnsAsync(logs);

            var result = await controller.GetSponsorCleanupLogs(sponsorId, CancellationToken.None);

            var okResult = Assert.IsType<OkObjectResult>(result);
            var dtos = Assert.IsAssignableFrom<IEnumerable<ProfessionalCleanupLogDto>>(okResult.Value);
            Assert.Single(dtos);
        }

        [Fact]
        public async Task ExportCleanupLogs_Success_ReturnsFile()
        {
            SetupAuthSuccess();

            var partnerId = Guid.NewGuid();
            var sponsorId = Guid.NewGuid();
            var sponsor = new Sponsor { Id = sponsorId, PartnerId = partnerId, Name = "Test Sponsor", IsActive = true };
            var partner = new Partner { Id = partnerId, Name = "Test Partner" };
            var logs = new List<ProfessionalCleanupLog>
            {
                new()
                {
                    Id = Guid.NewGuid(),
                    SponsoredAdoptionId = Guid.NewGuid(),
                    ProfessionalCompanyId = Guid.NewGuid(),
                    CleanupDate = DateTimeOffset.UtcNow,
                    DurationMinutes = 45,
                    BagsCollected = 3,
                    Notes = "Export test",
                },
            };

            sponsorManager
                .Setup(m => m.GetAsync(sponsorId, It.IsAny<CancellationToken>()))
                .ReturnsAsync(sponsor);
            partnerManager
                .Setup(m => m.GetAsync(partnerId, It.IsAny<CancellationToken>()))
                .ReturnsAsync(partner);
            logManager
                .Setup(m => m.GetBySponsorIdAsync(sponsorId, It.IsAny<CancellationToken>()))
                .ReturnsAsync(logs);

            var result = await controller.ExportCleanupLogs(sponsorId, CancellationToken.None);

            var fileResult = Assert.IsType<FileContentResult>(result);
            Assert.Equal("text/csv", fileResult.ContentType);
        }
    }
}
