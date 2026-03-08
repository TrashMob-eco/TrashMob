namespace TrashMob.Shared.Tests.Controllers.V2
{
    using System;
    using System.Threading;
    using System.Threading.Tasks;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.Extensions.Logging;
    using Moq;
    using TrashMob.Controllers.V2;
    using TrashMob.Models.Poco;
    using TrashMob.Models.Poco.V2;
    using TrashMob.Shared.Managers.Interfaces;
    using Xunit;

    public class StatsV2ControllerTests
    {
        private readonly Mock<IEventSummaryManager> eventSummaryManager = new();
        private readonly Mock<ILogger<StatsV2Controller>> logger = new();
        private readonly StatsV2Controller controller;

        public StatsV2ControllerTests()
        {
            controller = new StatsV2Controller(eventSummaryManager.Object, logger.Object);
        }

        [Fact]
        public async Task GetStats_ReturnsOkWithStatsDto()
        {
            var stats = new Stats
            {
                TotalBags = 10,
                TotalHours = 20,
                TotalEvents = 5,
                TotalWeightInPounds = 100.5m,
                TotalWeightInKilograms = 45.6m,
                TotalParticipants = 30,
                TotalLitterReportsSubmitted = 8,
                TotalLitterReportsClosed = 6,
            };

            eventSummaryManager
                .Setup(m => m.GetStatsAsync(It.IsAny<CancellationToken>()))
                .ReturnsAsync(stats);

            var result = await controller.GetStats(CancellationToken.None);

            var okResult = Assert.IsType<OkObjectResult>(result);
            var dto = Assert.IsType<StatsDto>(okResult.Value);
            Assert.Equal(10, dto.TotalBags);
            Assert.Equal(20, dto.TotalHours);
            Assert.Equal(5, dto.TotalEvents);
            Assert.Equal(100.5m, dto.TotalWeightInPounds);
            Assert.Equal(45.6m, dto.TotalWeightInKilograms);
            Assert.Equal(30, dto.TotalParticipants);
            Assert.Equal(8, dto.TotalLitterReportsSubmitted);
            Assert.Equal(6, dto.TotalLitterReportsClosed);
        }

        [Fact]
        public async Task GetStatsByUser_ReturnsOkWithStatsDto()
        {
            var userId = Guid.NewGuid();
            var stats = new Stats
            {
                TotalBags = 3,
                TotalHours = 5,
                TotalEvents = 2,
            };

            eventSummaryManager
                .Setup(m => m.GetStatsByUser(userId, It.IsAny<CancellationToken>()))
                .ReturnsAsync(stats);

            var result = await controller.GetStatsByUser(userId, CancellationToken.None);

            var okResult = Assert.IsType<OkObjectResult>(result);
            var dto = Assert.IsType<StatsDto>(okResult.Value);
            Assert.Equal(3, dto.TotalBags);
            Assert.Equal(5, dto.TotalHours);
            Assert.Equal(2, dto.TotalEvents);
        }
    }
}
