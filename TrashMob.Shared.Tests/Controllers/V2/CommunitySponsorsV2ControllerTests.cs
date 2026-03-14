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

    public class CommunitySponsorsV2ControllerTests
    {
        private readonly Mock<ISponsorManager> sponsorManager = new();
        private readonly Mock<IKeyedManager<Partner>> partnerManager = new();
        private readonly Mock<IImageManager> imageManager = new();
        private readonly Mock<IAuthorizationService> authorizationService = new();
        private readonly Mock<ILogger<CommunitySponsorsV2Controller>> logger = new();
        private readonly CommunitySponsorsV2Controller controller;
        private readonly Guid testUserId = Guid.NewGuid();

        public CommunitySponsorsV2ControllerTests()
        {
            controller = new CommunitySponsorsV2Controller(
                sponsorManager.Object, partnerManager.Object, imageManager.Object,
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
        public async Task GetSponsors_Authorized_ReturnsOkWithList()
        {
            SetupAuthSuccess();

            var partnerId = Guid.NewGuid();
            var partner = new Partner { Id = partnerId, Name = "Test Partner" };
            var sponsors = new List<Sponsor>
            {
                new() { Id = Guid.NewGuid(), Name = "Sponsor A", PartnerId = partnerId, IsActive = true },
                new() { Id = Guid.NewGuid(), Name = "Sponsor B", PartnerId = partnerId, IsActive = true },
            };

            partnerManager
                .Setup(m => m.GetAsync(partnerId, It.IsAny<CancellationToken>()))
                .ReturnsAsync(partner);
            sponsorManager
                .Setup(m => m.GetByCommunityAsync(partnerId, It.IsAny<CancellationToken>()))
                .ReturnsAsync(sponsors);

            var result = await controller.GetSponsors(partnerId, CancellationToken.None);

            var okResult = Assert.IsType<OkObjectResult>(result);
            var dtos = Assert.IsAssignableFrom<IEnumerable<SponsorDto>>(okResult.Value);
            Assert.Equal(2, dtos.Count());
            Assert.Equal("Sponsor A", dtos.First().Name);
        }

        [Fact]
        public async Task CreateSponsor_Authorized_ReturnsCreated()
        {
            SetupAuthSuccess();

            var partnerId = Guid.NewGuid();
            var partner = new Partner { Id = partnerId, Name = "Test Partner" };
            var sponsorDto = new SponsorDto
            {
                Name = "New Sponsor",
                ContactEmail = "sponsor@test.com",
                IsActive = true,
            };
            var createdSponsor = new Sponsor
            {
                Id = Guid.NewGuid(),
                Name = "New Sponsor",
                ContactEmail = "sponsor@test.com",
                PartnerId = partnerId,
                IsActive = true,
            };

            partnerManager
                .Setup(m => m.GetAsync(partnerId, It.IsAny<CancellationToken>()))
                .ReturnsAsync(partner);
            sponsorManager
                .Setup(m => m.AddAsync(It.IsAny<Sponsor>(), testUserId, It.IsAny<CancellationToken>()))
                .ReturnsAsync(createdSponsor);

            var result = await controller.CreateSponsor(partnerId, sponsorDto, CancellationToken.None);

            var createdResult = Assert.IsType<CreatedAtActionResult>(result);
            Assert.Equal(StatusCodes.Status201Created, createdResult.StatusCode);
            var dto = Assert.IsType<SponsorDto>(createdResult.Value);
            Assert.Equal("New Sponsor", dto.Name);
        }

        [Fact]
        public async Task DeactivateSponsor_Authorized_ReturnsNoContent()
        {
            SetupAuthSuccess();

            var partnerId = Guid.NewGuid();
            var sponsorId = Guid.NewGuid();
            var partner = new Partner { Id = partnerId, Name = "Test Partner" };
            var existingSponsor = new Sponsor
            {
                Id = sponsorId,
                Name = "Sponsor To Deactivate",
                PartnerId = partnerId,
                IsActive = true,
            };

            partnerManager
                .Setup(m => m.GetAsync(partnerId, It.IsAny<CancellationToken>()))
                .ReturnsAsync(partner);
            sponsorManager
                .Setup(m => m.GetAsync(sponsorId, It.IsAny<CancellationToken>()))
                .ReturnsAsync(existingSponsor);
            sponsorManager
                .Setup(m => m.UpdateAsync(It.IsAny<Sponsor>(), testUserId, It.IsAny<CancellationToken>()))
                .ReturnsAsync(existingSponsor);

            var result = await controller.DeactivateSponsor(partnerId, sponsorId, CancellationToken.None);

            Assert.IsType<NoContentResult>(result);
            sponsorManager.Verify(
                m => m.UpdateAsync(It.Is<Sponsor>(s => !s.IsActive), testUserId, It.IsAny<CancellationToken>()),
                Times.Once);
        }
    }
}
