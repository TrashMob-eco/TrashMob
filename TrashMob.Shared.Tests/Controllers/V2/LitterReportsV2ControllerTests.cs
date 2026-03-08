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
    using TrashMob.Shared.Tests.Fixtures;
    using Xunit;

    public class LitterReportsV2ControllerTests
    {
        private readonly Mock<ILitterReportManager> litterReportManager = new();
        private readonly Mock<ILogger<LitterReportsV2Controller>> logger = new();
        private readonly LitterReportsV2Controller controller;

        public LitterReportsV2ControllerTests()
        {
            controller = new LitterReportsV2Controller(litterReportManager.Object, logger.Object);
            controller.ControllerContext = new ControllerContext
            {
                HttpContext = new DefaultHttpContext(),
            };
        }

        [Fact]
        public async Task GetLitterReports_ReturnsOkWithPagedResponse()
        {
            var reports = new List<LitterReport>
            {
                new()
                {
                    Id = Guid.NewGuid(),
                    Name = "Park Litter",
                    LitterReportStatusId = 1,
                    CreatedByUserId = Guid.NewGuid(),
                    LitterImages = new List<LitterImage>
                    {
                        new() { Id = Guid.NewGuid(), AzureBlobURL = "img1.jpg", City = "Seattle" },
                    },
                },
                new()
                {
                    Id = Guid.NewGuid(),
                    Name = "Beach Litter",
                    LitterReportStatusId = 1,
                    CreatedByUserId = Guid.NewGuid(),
                    LitterImages = new List<LitterImage>
                    {
                        new() { Id = Guid.NewGuid(), AzureBlobURL = "img2.jpg", City = "Portland" },
                    },
                },
            };

            var queryable = new TestAsyncEnumerable<LitterReport>(reports);
            var filter = new LitterReportQueryParameters { Page = 1, PageSize = 25 };

            litterReportManager
                .Setup(m => m.GetFilteredLitterReportsQueryable(filter))
                .Returns(queryable);

            var result = await controller.GetLitterReports(filter, CancellationToken.None);

            var okResult = Assert.IsType<OkObjectResult>(result);
            var response = Assert.IsType<PagedResponse<LitterReportDto>>(okResult.Value);
            Assert.Equal(2, response.Items.Count);
            Assert.Equal("Park Litter", response.Items[0].Name);
            Assert.Single(response.Items[0].Images);
        }

        [Fact]
        public async Task GetLitterReports_ReturnsEmptyPagedResponse_WhenNoReports()
        {
            var queryable = new TestAsyncEnumerable<LitterReport>(Enumerable.Empty<LitterReport>());
            var filter = new LitterReportQueryParameters { Page = 1, PageSize = 25 };

            litterReportManager
                .Setup(m => m.GetFilteredLitterReportsQueryable(filter))
                .Returns(queryable);

            var result = await controller.GetLitterReports(filter, CancellationToken.None);

            var okResult = Assert.IsType<OkObjectResult>(result);
            var response = Assert.IsType<PagedResponse<LitterReportDto>>(okResult.Value);
            Assert.Empty(response.Items);
            Assert.Equal(0, response.Pagination.TotalCount);
        }

        [Fact]
        public async Task GetLitterReport_ReturnsOkWithDto()
        {
            var reportId = Guid.NewGuid();
            var report = new LitterReport
            {
                Id = reportId,
                Name = "Trail Litter",
                Description = "Trash along the trail",
                LitterReportStatusId = 2,
                CreatedByUserId = Guid.NewGuid(),
                CreatedDate = new DateTimeOffset(2026, 3, 1, 0, 0, 0, TimeSpan.Zero),
                LitterImages = new List<LitterImage>
                {
                    new()
                    {
                        Id = Guid.NewGuid(),
                        AzureBlobURL = "https://blob.example.com/trail.jpg",
                        City = "Seattle",
                        Region = "WA",
                        Country = "US",
                    },
                },
            };

            litterReportManager
                .Setup(m => m.GetAsync(reportId, It.IsAny<CancellationToken>()))
                .ReturnsAsync(report);

            var result = await controller.GetLitterReport(reportId, CancellationToken.None);

            var okResult = Assert.IsType<OkObjectResult>(result);
            var dto = Assert.IsType<LitterReportDto>(okResult.Value);
            Assert.Equal(reportId, dto.Id);
            Assert.Equal("Trail Litter", dto.Name);
            Assert.Single(dto.Images);
            Assert.Equal("https://blob.example.com/trail.jpg", dto.Images[0].ImageUrl);
        }

        [Fact]
        public async Task GetLitterReport_ReturnsNotFound_WhenReportDoesNotExist()
        {
            var reportId = Guid.NewGuid();

            litterReportManager
                .Setup(m => m.GetAsync(reportId, It.IsAny<CancellationToken>()))
                .ReturnsAsync((LitterReport)null);

            var result = await controller.GetLitterReport(reportId, CancellationToken.None);

            Assert.IsType<NotFoundResult>(result);
        }
    }
}
