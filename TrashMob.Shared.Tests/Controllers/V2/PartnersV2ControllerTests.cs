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

    public class PartnersV2ControllerTests
    {
        private readonly Mock<IPartnerManager> partnerManager = new();
        private readonly Mock<ILogger<PartnersV2Controller>> logger = new();
        private readonly PartnersV2Controller controller;

        public PartnersV2ControllerTests()
        {
            controller = new PartnersV2Controller(partnerManager.Object, logger.Object);
            controller.ControllerContext = new ControllerContext
            {
                HttpContext = new DefaultHttpContext(),
            };
        }

        [Fact]
        public async Task GetPartners_ReturnsOkWithPagedResponse()
        {
            var partners = new List<Partner>
            {
                new()
                {
                    Id = Guid.NewGuid(),
                    Name = "Seattle Parks",
                    PartnerStatusId = 1,
                    PartnerTypeId = 3,
                    City = "Seattle",
                    Region = "WA",
                    Country = "US",
                    HomePageEnabled = true,
                },
                new()
                {
                    Id = Guid.NewGuid(),
                    Name = "Portland Green",
                    PartnerStatusId = 1,
                    PartnerTypeId = 3,
                    City = "Portland",
                    Region = "OR",
                    Country = "US",
                },
            };

            var queryable = new TestAsyncEnumerable<Partner>(partners);
            var filter = new PartnerQueryParameters { Page = 1, PageSize = 25 };

            partnerManager
                .Setup(m => m.GetFilteredPartnersQueryable(filter))
                .Returns(queryable);

            var result = await controller.GetPartners(filter, CancellationToken.None);

            var okResult = Assert.IsType<OkObjectResult>(result);
            var response = Assert.IsType<PagedResponse<PartnerDto>>(okResult.Value);
            Assert.Equal(2, response.Items.Count);
            Assert.Equal("Seattle Parks", response.Items[0].Name);
        }

        [Fact]
        public async Task GetPartners_ReturnsEmptyPagedResponse_WhenNoPartners()
        {
            var queryable = new TestAsyncEnumerable<Partner>(Enumerable.Empty<Partner>());
            var filter = new PartnerQueryParameters { Page = 1, PageSize = 25 };

            partnerManager
                .Setup(m => m.GetFilteredPartnersQueryable(filter))
                .Returns(queryable);

            var result = await controller.GetPartners(filter, CancellationToken.None);

            var okResult = Assert.IsType<OkObjectResult>(result);
            var response = Assert.IsType<PagedResponse<PartnerDto>>(okResult.Value);
            Assert.Empty(response.Items);
            Assert.Equal(0, response.Pagination.TotalCount);
        }

        [Fact]
        public async Task GetPartner_ReturnsOkWithDto()
        {
            var partnerId = Guid.NewGuid();
            var partner = new Partner
            {
                Id = partnerId,
                Name = "Community Cleanup Co",
                Website = "https://cleanup.example.com",
                PartnerStatusId = 1,
                PartnerTypeId = 2,
                City = "Seattle",
                Region = "WA",
                Country = "US",
                Slug = "community-cleanup",
                HomePageEnabled = true,
                CreatedByUserId = Guid.NewGuid(),
                CreatedDate = new DateTimeOffset(2025, 3, 1, 0, 0, 0, TimeSpan.Zero),
            };

            partnerManager
                .Setup(m => m.GetAsync(partnerId, It.IsAny<CancellationToken>()))
                .ReturnsAsync(partner);

            var result = await controller.GetPartner(partnerId, CancellationToken.None);

            var okResult = Assert.IsType<OkObjectResult>(result);
            var dto = Assert.IsType<PartnerDto>(okResult.Value);
            Assert.Equal(partnerId, dto.Id);
            Assert.Equal("Community Cleanup Co", dto.Name);
            Assert.Equal("community-cleanup", dto.Slug);
        }

        [Fact]
        public async Task GetPartner_ReturnsNotFound_WhenPartnerDoesNotExist()
        {
            var partnerId = Guid.NewGuid();

            partnerManager
                .Setup(m => m.GetAsync(partnerId, It.IsAny<CancellationToken>()))
                .ReturnsAsync((Partner)null);

            var result = await controller.GetPartner(partnerId, CancellationToken.None);

            Assert.IsType<NotFoundResult>(result);
        }

        [Fact]
        public async Task GetPartner_ReturnsNotFound_WhenPartnerIsInactive()
        {
            var partnerId = Guid.NewGuid();
            var partner = new Partner
            {
                Id = partnerId,
                Name = "Defunct Partner",
                PartnerStatusId = 2, // Inactive
            };

            partnerManager
                .Setup(m => m.GetAsync(partnerId, It.IsAny<CancellationToken>()))
                .ReturnsAsync(partner);

            var result = await controller.GetPartner(partnerId, CancellationToken.None);

            Assert.IsType<NotFoundResult>(result);
        }
    }
}
