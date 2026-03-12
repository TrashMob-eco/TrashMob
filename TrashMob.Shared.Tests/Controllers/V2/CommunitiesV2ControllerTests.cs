namespace TrashMob.Shared.Tests.Controllers.V2
{
    using System;
    using System.Collections.Generic;
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
    using TrashMob.Shared.Managers.Interfaces;
    using Xunit;

    public class CommunitiesV2ControllerTests
    {
        private readonly Mock<ICommunityManager> communityManager = new();
        private readonly Mock<ILogger<CommunitiesV2Controller>> logger = new();
        private readonly CommunitiesV2Controller controller;

        public CommunitiesV2ControllerTests()
        {
            controller = new CommunitiesV2Controller(communityManager.Object, logger.Object);
        }

        [Fact]
        public async Task GetCommunities_ReturnsOkWithList()
        {
            var communities = new List<Partner>
            {
                new()
                {
                    Id = Guid.NewGuid(),
                    Name = "Seattle Cleanup",
                    Slug = "seattle-wa",
                    HomePageEnabled = true,
                    PartnerStatusId = 2,
                    City = "Seattle",
                    Region = "Washington",
                    Country = "United States",
                },
                new()
                {
                    Id = Guid.NewGuid(),
                    Name = "Portland Green",
                    Slug = "portland-or",
                    HomePageEnabled = true,
                    PartnerStatusId = 2,
                },
            };

            communityManager
                .Setup(m => m.GetEnabledCommunitiesAsync(null, null, null, It.IsAny<CancellationToken>()))
                .ReturnsAsync(communities);

            var result = await controller.GetCommunities(cancellationToken: CancellationToken.None);

            var okResult = Assert.IsType<OkObjectResult>(result);
            var dtos = Assert.IsAssignableFrom<List<PartnerDto>>(okResult.Value);
            Assert.Equal(2, dtos.Count);
            Assert.Equal("Seattle Cleanup", dtos[0].Name);
            Assert.Equal("seattle-wa", dtos[0].Slug);
        }

        [Fact]
        public async Task GetCommunities_WithGeoFilter_PassesParamsToManager()
        {
            communityManager
                .Setup(m => m.GetEnabledCommunitiesAsync(47.6, -122.3, 25.0, It.IsAny<CancellationToken>()))
                .ReturnsAsync(new List<Partner>());

            var result = await controller.GetCommunities(47.6, -122.3, 25.0, CancellationToken.None);

            var okResult = Assert.IsType<OkObjectResult>(result);
            var dtos = Assert.IsAssignableFrom<List<PartnerDto>>(okResult.Value);
            Assert.Empty(dtos);

            communityManager.Verify(
                m => m.GetEnabledCommunitiesAsync(47.6, -122.3, 25.0, It.IsAny<CancellationToken>()),
                Times.Once);
        }

        [Fact]
        public async Task GetCommunityBySlug_ReturnsOk_WhenFound()
        {
            var community = new Partner
            {
                Id = Guid.NewGuid(),
                Name = "Seattle Cleanup",
                Slug = "seattle-wa",
                City = "Seattle",
                Region = "Washington",
            };

            communityManager
                .Setup(m => m.GetBySlugAsync("seattle-wa", It.IsAny<CancellationToken>()))
                .ReturnsAsync(community);

            var result = await controller.GetCommunityBySlug("seattle-wa", CancellationToken.None);

            var okResult = Assert.IsType<OkObjectResult>(result);
            var dto = Assert.IsType<PartnerDto>(okResult.Value);
            Assert.Equal("Seattle Cleanup", dto.Name);
            Assert.Equal("seattle-wa", dto.Slug);
        }

        [Fact]
        public async Task GetCommunityBySlug_ReturnsNotFound_WhenMissing()
        {
            communityManager
                .Setup(m => m.GetBySlugAsync("nonexistent", It.IsAny<CancellationToken>()))
                .ReturnsAsync((Partner)null);

            var result = await controller.GetCommunityBySlug("nonexistent", CancellationToken.None);

            var objectResult = Assert.IsType<ObjectResult>(result);
            Assert.Equal(StatusCodes.Status404NotFound, objectResult.StatusCode);
        }

        [Fact]
        public async Task GetCommunityEvents_ReturnsOkWithEvents()
        {
            var community = new Partner { Id = Guid.NewGuid(), Slug = "seattle-wa" };
            var events = new List<Event>
            {
                new()
                {
                    Id = Guid.NewGuid(),
                    Name = "Park Cleanup",
                    EventDate = new DateTimeOffset(2026, 4, 1, 9, 0, 0, TimeSpan.Zero),
                    City = "Seattle",
                    Region = "Washington",
                    Country = "United States",
                },
            };

            communityManager
                .Setup(m => m.GetBySlugAsync("seattle-wa", It.IsAny<CancellationToken>()))
                .ReturnsAsync(community);
            communityManager
                .Setup(m => m.GetCommunityEventsAsync("seattle-wa", true, It.IsAny<CancellationToken>()))
                .ReturnsAsync(events);

            var result = await controller.GetCommunityEvents("seattle-wa", cancellationToken: CancellationToken.None);

            var okResult = Assert.IsType<OkObjectResult>(result);
            var dtos = Assert.IsAssignableFrom<List<EventDto>>(okResult.Value);
            Assert.Single(dtos);
            Assert.Equal("Park Cleanup", dtos[0].Name);
        }

        [Fact]
        public async Task GetCommunityEvents_ReturnsNotFound_WhenSlugMissing()
        {
            communityManager
                .Setup(m => m.GetBySlugAsync("nonexistent", It.IsAny<CancellationToken>()))
                .ReturnsAsync((Partner)null);

            var result = await controller.GetCommunityEvents("nonexistent", cancellationToken: CancellationToken.None);

            var objectResult = Assert.IsType<ObjectResult>(result);
            Assert.Equal(StatusCodes.Status404NotFound, objectResult.StatusCode);
        }

        [Fact]
        public async Task GetCommunityTeams_ReturnsOkWithTeams()
        {
            var community = new Partner { Id = Guid.NewGuid(), Slug = "seattle-wa" };
            var teams = new List<Team>
            {
                new()
                {
                    Id = Guid.NewGuid(),
                    Name = "Green Warriors",
                    IsPublic = true,
                    IsActive = true,
                    City = "Seattle",
                },
            };

            communityManager
                .Setup(m => m.GetBySlugAsync("seattle-wa", It.IsAny<CancellationToken>()))
                .ReturnsAsync(community);
            communityManager
                .Setup(m => m.GetCommunityTeamsAsync("seattle-wa", 50, It.IsAny<CancellationToken>()))
                .ReturnsAsync(teams);

            var result = await controller.GetCommunityTeams("seattle-wa", cancellationToken: CancellationToken.None);

            var okResult = Assert.IsType<OkObjectResult>(result);
            var dtos = Assert.IsAssignableFrom<List<TeamDto>>(okResult.Value);
            Assert.Single(dtos);
            Assert.Equal("Green Warriors", dtos[0].Name);
        }

        [Fact]
        public async Task GetCommunityStats_ReturnsOkWithStats()
        {
            var community = new Partner { Id = Guid.NewGuid(), Slug = "seattle-wa" };
            var stats = new Stats
            {
                TotalEvents = 25,
                TotalBags = 150,
                TotalHours = 300,
                TotalParticipants = 200,
                TotalWeightInPounds = 1500,
                TotalWeightInKilograms = 680,
                TotalLitterReportsSubmitted = 50,
                TotalLitterReportsClosed = 40,
            };

            communityManager
                .Setup(m => m.GetBySlugAsync("seattle-wa", It.IsAny<CancellationToken>()))
                .ReturnsAsync(community);
            communityManager
                .Setup(m => m.GetCommunityStatsAsync("seattle-wa", It.IsAny<CancellationToken>()))
                .ReturnsAsync(stats);

            var result = await controller.GetCommunityStats("seattle-wa", CancellationToken.None);

            var okResult = Assert.IsType<OkObjectResult>(result);
            var dto = Assert.IsType<StatsDto>(okResult.Value);
            Assert.Equal(25, dto.TotalEvents);
            Assert.Equal(150, dto.TotalBags);
            Assert.Equal(200, dto.TotalParticipants);
        }

        [Fact]
        public async Task GetCommunityStats_ReturnsNotFound_WhenSlugMissing()
        {
            communityManager
                .Setup(m => m.GetBySlugAsync("nonexistent", It.IsAny<CancellationToken>()))
                .ReturnsAsync((Partner)null);

            var result = await controller.GetCommunityStats("nonexistent", CancellationToken.None);

            var objectResult = Assert.IsType<ObjectResult>(result);
            Assert.Equal(StatusCodes.Status404NotFound, objectResult.StatusCode);
        }
    }
}
