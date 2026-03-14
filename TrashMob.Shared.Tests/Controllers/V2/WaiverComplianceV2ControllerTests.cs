namespace TrashMob.Shared.Tests.Controllers.V2
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
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
    using TrashMob.Models.Poco.V2;
    using TrashMob.Shared.Managers.Interfaces;
    using TrashMob.Shared.Poco;
    using Xunit;

    public class WaiverComplianceV2ControllerTests
    {
        private readonly Mock<IUserWaiverManager> userWaiverManager = new();
        private readonly Mock<ILogger<WaiverComplianceV2Controller>> logger = new();
        private readonly WaiverComplianceV2Controller controller;
        private readonly Guid testUserId = Guid.NewGuid();

        public WaiverComplianceV2ControllerTests()
        {
            controller = new WaiverComplianceV2Controller(
                userWaiverManager.Object,
                logger.Object);

            var httpContext = new DefaultHttpContext();
            httpContext.Items["UserId"] = testUserId.ToString();
            httpContext.User = new ClaimsPrincipal(new GenericIdentity("test", "Bearer"));
            controller.ControllerContext = new ControllerContext { HttpContext = httpContext };
        }

        [Fact]
        public async Task GetComplianceSummary_ReturnsOk()
        {
            var summary = new WaiverComplianceSummary
            {
                TotalActiveUsers = 100,
                CompliancePercentage = 85.5m,
                GeneratedAt = DateTimeOffset.UtcNow,
            };

            userWaiverManager
                .Setup(m => m.GetComplianceSummaryAsync(It.IsAny<CancellationToken>()))
                .ReturnsAsync(summary);

            var result = await controller.GetComplianceSummary(CancellationToken.None);

            var okResult = Assert.IsType<OkObjectResult>(result);
            var resultSummary = Assert.IsType<WaiverComplianceSummary>(okResult.Value);
            Assert.Equal(100, resultSummary.TotalActiveUsers);
        }

        [Fact]
        public async Task GetUserWaivers_InvalidPage_ReturnsBadRequest()
        {
            var filter = new UserWaiverFilter { Page = 0, PageSize = 50 };

            var result = await controller.GetUserWaivers(filter, CancellationToken.None);

            Assert.IsType<BadRequestObjectResult>(result);
        }

        [Fact]
        public async Task GetUserWaivers_Success_ReturnsOk()
        {
            var listResult = new UserWaiverListResult
            {
                Items = new List<UserWaiverDetail>
                {
                    new() { Id = Guid.NewGuid(), UserId = Guid.NewGuid(), UserName = "Test User" },
                },
                TotalCount = 1,
                Page = 1,
                PageSize = 50,
            };

            userWaiverManager
                .Setup(m => m.GetUserWaiversFilteredAsync(It.IsAny<UserWaiverFilter>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(listResult);

            var filter = new UserWaiverFilter { Page = 1, PageSize = 50 };

            var result = await controller.GetUserWaivers(filter, CancellationToken.None);

            var okResult = Assert.IsType<OkObjectResult>(result);
            var resultData = Assert.IsType<UserWaiverListResult>(okResult.Value);
            Assert.Equal(1, resultData.TotalCount);
        }

        [Fact]
        public async Task GetUsersWithExpiringWaivers_InvalidDays_ReturnsBadRequest()
        {
            var result = await controller.GetUsersWithExpiringWaivers(0, CancellationToken.None);

            Assert.IsType<BadRequestObjectResult>(result);
        }

        [Fact]
        public async Task GetUserWaiverDetails_NotFound_ReturnsNotFound()
        {
            var userWaiverId = Guid.NewGuid();

            userWaiverManager
                .Setup(m => m.GetUserWaiverWithDetailsAsync(userWaiverId, It.IsAny<CancellationToken>()))
                .ReturnsAsync((UserWaiver)null);

            var result = await controller.GetUserWaiverDetails(userWaiverId, CancellationToken.None);

            Assert.IsType<NotFoundResult>(result);
        }
    }
}
