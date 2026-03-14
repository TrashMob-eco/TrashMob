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

    public class FundraisingAnalyticsV2ControllerTests
    {
        private readonly Mock<IFundraisingAnalyticsManager> analyticsManager = new();
        private readonly Mock<ILogger<FundraisingAnalyticsV2Controller>> logger = new();
        private readonly FundraisingAnalyticsV2Controller controller;
        private readonly Guid testUserId = Guid.NewGuid();

        public FundraisingAnalyticsV2ControllerTests()
        {
            controller = new FundraisingAnalyticsV2Controller(analyticsManager.Object, logger.Object);

            var httpContext = new DefaultHttpContext();
            httpContext.Items["UserId"] = testUserId.ToString();
            httpContext.User = new ClaimsPrincipal(new ClaimsIdentity(
            [
                new Claim(ClaimTypes.NameIdentifier, testUserId.ToString()),
            ], "TestAuth"));
            controller.ControllerContext = new ControllerContext { HttpContext = httpContext };
        }

        [Fact]
        public async Task GetEngagementScores_ReturnsOk()
        {
            var scores = new List<ContactEngagementScore>
            {
                new ContactEngagementScore { ContactId = Guid.NewGuid(), ContactName = "Alice", EngagementScore = 85 },
                new ContactEngagementScore { ContactId = Guid.NewGuid(), ContactName = "Bob", EngagementScore = 42 },
            };

            analyticsManager
                .Setup(m => m.GetEngagementScoresAsync(It.IsAny<CancellationToken>()))
                .ReturnsAsync(scores);

            var result = await controller.GetEngagementScores(CancellationToken.None);

            var okResult = Assert.IsType<OkObjectResult>(result);
            Assert.NotNull(okResult.Value);
        }

        [Fact]
        public async Task GetDashboard_ReturnsOk()
        {
            var dashboard = new FundraisingDashboard
            {
                TotalRaisedYtd = 50000m,
                DonorCountYtd = 120,
                AverageGiftSizeYtd = 416.67m,
            };

            analyticsManager
                .Setup(m => m.GetDashboardAsync(It.IsAny<CancellationToken>()))
                .ReturnsAsync(dashboard);

            var result = await controller.GetDashboard(CancellationToken.None);

            var okResult = Assert.IsType<OkObjectResult>(result);
            Assert.NotNull(okResult.Value);
        }

        [Fact]
        public async Task ExportDonorReport_ReturnsFile()
        {
            var csv = "ContactName,Email,TotalDonations\nAlice,alice@test.com,500.00\n";

            analyticsManager
                .Setup(m => m.GenerateDonorReportCsvAsync(It.IsAny<CancellationToken>()))
                .ReturnsAsync(csv);

            var result = await controller.ExportDonorReport(CancellationToken.None);

            var fileResult = Assert.IsType<FileContentResult>(result);
            Assert.Equal("text/csv", fileResult.ContentType);
            Assert.NotNull(fileResult.FileContents);
            Assert.True(fileResult.FileContents.Length > 0);
        }

        [Fact]
        public async Task GetLybuntContacts_ReturnsOk()
        {
            var contacts = new List<ContactEngagementScore>
            {
                new ContactEngagementScore { ContactId = Guid.NewGuid(), ContactName = "Lapsed Donor", IsLybunt = true },
            };

            analyticsManager
                .Setup(m => m.GetLybuntContactsAsync(It.IsAny<CancellationToken>()))
                .ReturnsAsync(contacts);

            var result = await controller.GetLybuntContacts(CancellationToken.None);

            var okResult = Assert.IsType<OkObjectResult>(result);
            Assert.NotNull(okResult.Value);
        }
    }
}
