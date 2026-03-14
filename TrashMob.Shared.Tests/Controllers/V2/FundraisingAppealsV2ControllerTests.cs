namespace TrashMob.Shared.Tests.Controllers.V2
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Linq.Expressions;
    using System.Security.Claims;
    using System.Security.Principal;
    using System.Threading;
    using System.Threading.Tasks;
    using Microsoft.AspNetCore.Http;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.Extensions.Logging;
    using Moq;
    using TrashMob.Controllers.V2;
    using TrashMob.Models;
    using TrashMob.Models.Poco;
    using TrashMob.Models.Poco.V2;
    using TrashMob.Shared.Managers.Contacts;
    using TrashMob.Shared.Managers.Interfaces;
    using Xunit;

    public class FundraisingAppealsV2ControllerTests
    {
        private readonly Mock<IDonationEmailManager> donationEmailManager = new();
        private readonly Mock<ILogger<FundraisingAppealsV2Controller>> logger = new();
        private readonly FundraisingAppealsV2Controller controller;
        private readonly Guid testUserId = Guid.NewGuid();

        public FundraisingAppealsV2ControllerTests()
        {
            controller = new FundraisingAppealsV2Controller(donationEmailManager.Object, logger.Object);

            var httpContext = new DefaultHttpContext();
            httpContext.Items["UserId"] = testUserId.ToString();
            httpContext.User = new ClaimsPrincipal(new ClaimsIdentity(
            [
                new Claim(ClaimTypes.NameIdentifier, testUserId.ToString()),
            ], "TestAuth"));
            controller.ControllerContext = new ControllerContext { HttpContext = httpContext };
        }

        [Fact]
        public async Task Send_ReturnsNoContent()
        {
            var request = new AppealRequestDto
            {
                ContactId = Guid.NewGuid(),
                Subject = "Annual Appeal",
                Body = "Please consider donating.",
            };

            donationEmailManager
                .Setup(m => m.SendAppealAsync(
                    request.ContactId, request.Subject, request.Body,
                    testUserId, It.IsAny<CancellationToken>()))
                .Returns(Task.CompletedTask);

            var result = await controller.Send(request, CancellationToken.None);

            Assert.IsType<NoContentResult>(result);
            donationEmailManager.Verify(
                m => m.SendAppealAsync(
                    request.ContactId, request.Subject, request.Body,
                    testUserId, It.IsAny<CancellationToken>()),
                Times.Once);
        }

        [Fact]
        public async Task SendBulk_ReturnsOk()
        {
            var request = new BulkAppealRequestDto
            {
                ContactIds = [Guid.NewGuid(), Guid.NewGuid(), Guid.NewGuid()],
                Subject = "Bulk Appeal",
                Body = "Thank you for your support.",
            };

            var bulkResult = new BulkAppealResult
            {
                SentCount = 5,
                FailedCount = 0,
                SkippedCount = 1,
            };

            donationEmailManager
                .Setup(m => m.SendBulkAppealAsync(
                    It.IsAny<IEnumerable<Guid>>(), request.Subject, request.Body,
                    testUserId, It.IsAny<CancellationToken>()))
                .ReturnsAsync(bulkResult);

            var result = await controller.SendBulk(request, CancellationToken.None);

            var okResult = Assert.IsType<OkObjectResult>(result);
            Assert.NotNull(okResult.Value);
        }
    }
}
