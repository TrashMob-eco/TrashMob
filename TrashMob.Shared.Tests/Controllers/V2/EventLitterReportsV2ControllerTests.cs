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
    using TrashMob.Models.Poco;
    using TrashMob.Shared.Managers.Interfaces;
    using Xunit;

    public class EventLitterReportsV2ControllerTests
    {
        private readonly Mock<IEventLitterReportManager> reportManager = new();
        private readonly Mock<IUserManager> userManager = new();
        private readonly Mock<ILogger<EventLitterReportsV2Controller>> logger = new();
        private readonly EventLitterReportsV2Controller controller;

        public EventLitterReportsV2ControllerTests()
        {
            controller = new EventLitterReportsV2Controller(reportManager.Object, userManager.Object, logger.Object);
            controller.ControllerContext = new ControllerContext
            {
                HttpContext = new DefaultHttpContext(),
            };
        }

        [Fact]
        public async Task GetByEvent_ReturnsOk_WithReports()
        {
            var eventId = Guid.NewGuid();
            var userId = Guid.NewGuid();
            var litterReportId = Guid.NewGuid();

            var reports = new List<EventLitterReport>
            {
                new()
                {
                    EventId = eventId,
                    LitterReportId = litterReportId,
                    LitterReport = new LitterReport
                    {
                        Id = litterReportId,
                        Name = "Test Report",
                        CreatedByUserId = userId,
                    },
                },
            };

            reportManager.Setup(m => m.GetByParentIdAsync(eventId, It.IsAny<CancellationToken>()))
                .ReturnsAsync(reports);
            userManager.Setup(m => m.GetAsync(userId, It.IsAny<CancellationToken>()))
                .ReturnsAsync(new User { Id = userId, UserName = "testuser" });

            var result = await controller.GetByEvent(eventId, CancellationToken.None);

            var okResult = Assert.IsType<OkObjectResult>(result);
            var fullReports = Assert.IsAssignableFrom<List<FullEventLitterReport>>(okResult.Value);
            Assert.Single(fullReports);
            Assert.Equal(eventId, fullReports[0].EventId);
        }

        [Fact]
        public async Task Delete_ReturnsNoContent()
        {
            var eventId = Guid.NewGuid();
            var litterReportId = Guid.NewGuid();
            controller.HttpContext.Items["UserId"] = Guid.NewGuid().ToString();

            var result = await controller.Delete(eventId, litterReportId, CancellationToken.None);

            Assert.IsType<NoContentResult>(result);
            reportManager.Verify(m => m.Delete(eventId, litterReportId, It.IsAny<CancellationToken>()), Times.Once);
        }

        [Fact]
        public async Task Add_ReturnsCreated()
        {
            controller.HttpContext.Items["UserId"] = Guid.NewGuid().ToString();
            var report = new EventLitterReport { EventId = Guid.NewGuid(), LitterReportId = Guid.NewGuid() };

            var result = await controller.Add(report, CancellationToken.None);

            Assert.IsType<StatusCodeResult>(result);
            Assert.Equal(201, ((StatusCodeResult)result).StatusCode);
        }
    }
}
