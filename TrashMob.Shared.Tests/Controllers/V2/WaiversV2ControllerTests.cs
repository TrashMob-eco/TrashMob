namespace TrashMob.Shared.Tests.Controllers.V2
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
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
    using TrashMob.Shared.Poco;
    using Xunit;

    public class WaiversV2ControllerTests
    {
        private readonly Mock<IUserWaiverManager> waiverManager = new();
        private readonly Mock<IWaiverDocumentManager> documentManager = new();
        private readonly Mock<IUserManager> userManager = new();
        private readonly Mock<ILogger<WaiversV2Controller>> logger = new();
        private readonly WaiversV2Controller controller;
        private readonly Guid userId = Guid.NewGuid();

        public WaiversV2ControllerTests()
        {
            controller = new WaiversV2Controller(
                waiverManager.Object, documentManager.Object, userManager.Object, logger.Object);
            controller.ControllerContext = new ControllerContext
            {
                HttpContext = new DefaultHttpContext(),
            };
            controller.HttpContext.Items["UserId"] = userId.ToString();
        }

        [Fact]
        public async Task GetRequired_ReturnsOk_WithWaiverVersions()
        {
            var waivers = new List<WaiverVersion>
            {
                new()
                {
                    Id = Guid.NewGuid(),
                    Name = "Standard Waiver",
                    Version = "1.0",
                    WaiverText = "By signing...",
                    IsActive = true,
                    Scope = WaiverScope.Global,
                },
            };

            waiverManager.Setup(m => m.GetPendingWaiversForUserAsync(userId, null, It.IsAny<CancellationToken>()))
                .ReturnsAsync(waivers);

            var result = await controller.GetRequired(cancellationToken: CancellationToken.None);

            var okResult = Assert.IsType<OkObjectResult>(result);
            var dtos = Assert.IsAssignableFrom<List<WaiverVersionDto>>(okResult.Value);
            Assert.Single(dtos);
            Assert.Equal("Standard Waiver", dtos[0].Name);
        }

        [Fact]
        public async Task GetMyWaivers_ReturnsOk_WithUserWaivers()
        {
            var waivers = new List<UserWaiver>
            {
                new()
                {
                    Id = Guid.NewGuid(),
                    UserId = userId,
                    WaiverVersionId = Guid.NewGuid(),
                    AcceptedDate = DateTimeOffset.UtcNow,
                    TypedLegalName = "Jane Doe",
                    SigningMethod = "Digital",
                },
            };

            waiverManager.Setup(m => m.GetUserWaiversAsync(userId, It.IsAny<CancellationToken>()))
                .ReturnsAsync(waivers);

            var result = await controller.GetMyWaivers(CancellationToken.None);

            var okResult = Assert.IsType<OkObjectResult>(result);
            var dtos = Assert.IsAssignableFrom<List<UserWaiverDto>>(okResult.Value);
            Assert.Single(dtos);
            Assert.Equal("Jane Doe", dtos[0].TypedLegalName);
        }

        [Fact]
        public async Task Accept_ReturnsCreated_OnSuccess()
        {
            var waiverVersionId = Guid.NewGuid();
            var userWaiver = new UserWaiver
            {
                Id = Guid.NewGuid(),
                UserId = userId,
                WaiverVersionId = waiverVersionId,
                AcceptedDate = DateTimeOffset.UtcNow,
                TypedLegalName = "Jane Doe",
                SigningMethod = "Digital",
            };

            waiverManager.Setup(m => m.AcceptWaiverAsync(It.IsAny<AcceptWaiverRequest>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(ServiceResult<UserWaiver>.Success(userWaiver));
            userManager.Setup(m => m.GetAsync(userId, It.IsAny<CancellationToken>()))
                .ReturnsAsync(new User { Id = userId, UserName = "janedoe" });
            waiverManager.Setup(m => m.GetUserWaiverWithDetailsAsync(userWaiver.Id, It.IsAny<CancellationToken>()))
                .ReturnsAsync(userWaiver);
            documentManager.Setup(m => m.GenerateAndStoreWaiverPdfAsync(It.IsAny<UserWaiver>(), It.IsAny<User>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync("https://example.com/waiver.pdf");

            var request = new AcceptWaiverRequestDto
            {
                WaiverVersionId = waiverVersionId,
                TypedLegalName = "Jane Doe",
            };

            var result = await controller.Accept(request, CancellationToken.None);

            Assert.IsType<CreatedAtActionResult>(result);
        }

        [Fact]
        public async Task Accept_ReturnsBadRequest_WhenVersionIdEmpty()
        {
            var request = new AcceptWaiverRequestDto
            {
                WaiverVersionId = Guid.Empty,
                TypedLegalName = "Jane Doe",
            };

            var result = await controller.Accept(request, CancellationToken.None);

            Assert.IsType<BadRequestObjectResult>(result);
        }

        [Fact]
        public async Task Accept_ReturnsBadRequest_WhenNameEmpty()
        {
            var request = new AcceptWaiverRequestDto
            {
                WaiverVersionId = Guid.NewGuid(),
                TypedLegalName = "",
            };

            var result = await controller.Accept(request, CancellationToken.None);

            Assert.IsType<BadRequestObjectResult>(result);
        }

        [Fact]
        public async Task Accept_ReturnsBadRequest_OnFailure()
        {
            waiverManager.Setup(m => m.AcceptWaiverAsync(It.IsAny<AcceptWaiverRequest>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(ServiceResult<UserWaiver>.Failure("Waiver already signed"));

            var request = new AcceptWaiverRequestDto
            {
                WaiverVersionId = Guid.NewGuid(),
                TypedLegalName = "Jane Doe",
            };

            var result = await controller.Accept(request, CancellationToken.None);

            var badResult = Assert.IsType<BadRequestObjectResult>(result);
            Assert.Equal("Waiver already signed", badResult.Value);
        }
    }
}
