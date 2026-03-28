namespace TrashMob.Shared.Tests.Controllers.V2
{
    using System;
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

    public class PrivoConsentV2ControllerTests
    {
        private readonly Mock<IPrivoConsentManager> privoConsentManager = new();
        private readonly Mock<ILogger<PrivoConsentV2Controller>> logger = new();
        private readonly PrivoConsentV2Controller controller;
        private readonly Guid userId = Guid.NewGuid();

        public PrivoConsentV2ControllerTests()
        {
            controller = new PrivoConsentV2Controller(privoConsentManager.Object, logger.Object);
            controller.ControllerContext = new ControllerContext
            {
                HttpContext = new DefaultHttpContext(),
            };
            controller.HttpContext.Items["UserId"] = userId.ToString();
        }

        [Fact]
        public async Task InitiateAdultVerification_ReturnsOk_WithConsentDto()
        {
            var consent = new ParentalConsent
            {
                Id = Guid.NewGuid(),
                UserId = userId,
                ConsentType = ConsentType.AdultVerification,
                Status = ConsentStatus.Pending,
                ConsentUrl = "https://consent-svc-int.privo.com/verify/123",
                CreatedDate = DateTimeOffset.UtcNow,
            };

            privoConsentManager
                .Setup(m => m.InitiateAdultVerificationAsync(userId, It.IsAny<CancellationToken>()))
                .ReturnsAsync(consent);

            var result = await controller.InitiateAdultVerification(CancellationToken.None);

            var okResult = Assert.IsType<OkObjectResult>(result);
            var dto = Assert.IsType<ParentalConsentDto>(okResult.Value);
            Assert.Equal(consent.Id, dto.Id);
            Assert.Equal(1, dto.ConsentType);
            Assert.Equal(1, dto.Status);
            Assert.Equal("https://consent-svc-int.privo.com/verify/123", dto.ConsentUrl);
        }

        [Fact]
        public async Task InitiateParentChildConsent_ReturnsOk()
        {
            var dependentId = Guid.NewGuid();
            var consent = new ParentalConsent
            {
                Id = Guid.NewGuid(),
                UserId = userId,
                DependentId = dependentId,
                ConsentType = ConsentType.ParentInitiatedChild,
                Status = ConsentStatus.Pending,
                ConsentUrl = "https://consent-svc-int.privo.com/consent/456",
                CreatedDate = DateTimeOffset.UtcNow,
            };

            privoConsentManager
                .Setup(m => m.InitiateParentChildConsentAsync(userId, dependentId, It.IsAny<CancellationToken>()))
                .ReturnsAsync(consent);

            var result = await controller.InitiateParentChildConsent(dependentId, CancellationToken.None);

            var okResult = Assert.IsType<OkObjectResult>(result);
            var dto = Assert.IsType<ParentalConsentDto>(okResult.Value);
            Assert.Equal(dependentId, dto.DependentId);
            Assert.Equal(2, dto.ConsentType);
        }

        [Fact]
        public async Task InitiateChildConsent_ReturnsOk_WhenParentExists()
        {
            var request = new InitiateChildConsentRequest
            {
                ChildFirstName = "Alex",
                ChildEmail = "alex@example.com",
                ChildBirthDate = new DateOnly(2012, 6, 15),
                ParentEmail = "parent@example.com",
            };

            var consent = new ParentalConsent
            {
                Id = Guid.NewGuid(),
                ConsentType = ConsentType.ChildInitiated,
                Status = ConsentStatus.Pending,
                CreatedDate = DateTimeOffset.UtcNow,
            };

            privoConsentManager
                .Setup(m => m.InitiateChildConsentAsync(
                    request.ChildFirstName, request.ChildEmail, request.ChildBirthDate,
                    request.ParentEmail, It.IsAny<CancellationToken>()))
                .ReturnsAsync(consent);

            var result = await controller.InitiateChildConsent(request, CancellationToken.None);

            var okResult = Assert.IsType<OkObjectResult>(result);
            Assert.IsType<ParentalConsentDto>(okResult.Value);
        }

        [Fact]
        public async Task InitiateChildConsent_ReturnsNoContent_WhenParentDoesNotExist()
        {
            var request = new InitiateChildConsentRequest
            {
                ChildFirstName = "Alex",
                ChildEmail = "alex@example.com",
                ChildBirthDate = new DateOnly(2012, 6, 15),
                ParentEmail = "noparent@example.com",
            };

            privoConsentManager
                .Setup(m => m.InitiateChildConsentAsync(
                    It.IsAny<string>(), It.IsAny<string>(), It.IsAny<DateOnly>(),
                    It.IsAny<string>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync((ParentalConsent)null);

            var result = await controller.InitiateChildConsent(request, CancellationToken.None);

            Assert.IsType<NoContentResult>(result);
        }

        [Fact]
        public async Task GetVerificationStatus_ReturnsOk_WhenConsentExists()
        {
            var consent = new ParentalConsent
            {
                Id = Guid.NewGuid(),
                UserId = userId,
                ConsentType = ConsentType.AdultVerification,
                Status = ConsentStatus.Verified,
                VerifiedDate = DateTimeOffset.UtcNow,
                CreatedDate = DateTimeOffset.UtcNow,
            };

            privoConsentManager
                .Setup(m => m.GetConsentByUserIdAsync(userId, It.IsAny<CancellationToken>()))
                .ReturnsAsync(consent);

            var result = await controller.GetVerificationStatus(CancellationToken.None);

            var okResult = Assert.IsType<OkObjectResult>(result);
            var dto = Assert.IsType<ParentalConsentDto>(okResult.Value);
            Assert.Equal(2, dto.Status);
        }

        [Fact]
        public async Task GetVerificationStatus_ReturnsNoContent_WhenNoConsentExists()
        {
            privoConsentManager
                .Setup(m => m.GetConsentByUserIdAsync(userId, It.IsAny<CancellationToken>()))
                .ReturnsAsync((ParentalConsent)null);

            var result = await controller.GetVerificationStatus(CancellationToken.None);

            Assert.IsType<NoContentResult>(result);
        }

        [Fact]
        public async Task RevokeConsent_ReturnsNoContent()
        {
            var consentId = Guid.NewGuid();

            var result = await controller.RevokeConsent(consentId, "Parent requested", CancellationToken.None);

            Assert.IsType<NoContentResult>(result);
            privoConsentManager.Verify(m => m.RevokeConsentAsync(
                consentId, userId, "Parent requested", It.IsAny<CancellationToken>()), Times.Once);
        }
    }
}
