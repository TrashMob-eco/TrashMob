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

    public class TeamsV2ControllerTests
    {
        private readonly Mock<ITeamManager> teamManager = new();
        private readonly Mock<ILogger<TeamsV2Controller>> logger = new();
        private readonly TeamsV2Controller controller;

        public TeamsV2ControllerTests()
        {
            controller = new TeamsV2Controller(teamManager.Object, logger.Object);
            controller.ControllerContext = new ControllerContext
            {
                HttpContext = new DefaultHttpContext(),
            };
        }

        [Fact]
        public async Task GetTeams_ReturnsOkWithPagedResponse()
        {
            var teams = new List<Team>
            {
                new()
                {
                    Id = Guid.NewGuid(),
                    Name = "Seattle Cleanup Crew",
                    IsPublic = true,
                    IsActive = true,
                    City = "Seattle",
                    Region = "WA",
                    Country = "US",
                },
                new()
                {
                    Id = Guid.NewGuid(),
                    Name = "Portland Green Team",
                    IsPublic = true,
                    IsActive = true,
                    City = "Portland",
                    Region = "OR",
                    Country = "US",
                },
            };

            var queryable = new TestAsyncEnumerable<Team>(teams);
            var filter = new TeamQueryParameters { Page = 1, PageSize = 25 };

            teamManager
                .Setup(m => m.GetFilteredTeamsQueryable(filter))
                .Returns(queryable);

            var result = await controller.GetTeams(filter, CancellationToken.None);

            var okResult = Assert.IsType<OkObjectResult>(result);
            var response = Assert.IsType<PagedResponse<TeamDto>>(okResult.Value);
            Assert.Equal(2, response.Items.Count);
            Assert.Equal("Seattle Cleanup Crew", response.Items[0].Name);
        }

        [Fact]
        public async Task GetTeams_ReturnsEmptyPagedResponse_WhenNoTeams()
        {
            var queryable = new TestAsyncEnumerable<Team>(Enumerable.Empty<Team>());
            var filter = new TeamQueryParameters { Page = 1, PageSize = 25 };

            teamManager
                .Setup(m => m.GetFilteredTeamsQueryable(filter))
                .Returns(queryable);

            var result = await controller.GetTeams(filter, CancellationToken.None);

            var okResult = Assert.IsType<OkObjectResult>(result);
            var response = Assert.IsType<PagedResponse<TeamDto>>(okResult.Value);
            Assert.Empty(response.Items);
            Assert.Equal(0, response.Pagination.TotalCount);
        }

        [Fact]
        public async Task GetTeam_ReturnsOkWithDto()
        {
            var teamId = Guid.NewGuid();
            var team = new Team
            {
                Id = teamId,
                Name = "Trail Blazers",
                Description = "Cleaning trails in the Pacific NW",
                IsPublic = true,
                IsActive = true,
                City = "Seattle",
                Region = "WA",
                Country = "US",
                CreatedByUserId = Guid.NewGuid(),
                CreatedDate = new DateTimeOffset(2025, 6, 1, 0, 0, 0, TimeSpan.Zero),
            };

            teamManager
                .Setup(m => m.GetAsync(teamId, It.IsAny<CancellationToken>()))
                .ReturnsAsync(team);

            var result = await controller.GetTeam(teamId, CancellationToken.None);

            var okResult = Assert.IsType<OkObjectResult>(result);
            var dto = Assert.IsType<TeamDto>(okResult.Value);
            Assert.Equal(teamId, dto.Id);
            Assert.Equal("Trail Blazers", dto.Name);
            Assert.Equal("Seattle", dto.City);
        }

        [Fact]
        public async Task GetTeam_ReturnsNotFound_WhenTeamDoesNotExist()
        {
            var teamId = Guid.NewGuid();

            teamManager
                .Setup(m => m.GetAsync(teamId, It.IsAny<CancellationToken>()))
                .ReturnsAsync((Team)null);

            var result = await controller.GetTeam(teamId, CancellationToken.None);

            Assert.IsType<NotFoundResult>(result);
        }

        [Fact]
        public async Task GetTeam_ReturnsNotFound_WhenTeamIsInactive()
        {
            var teamId = Guid.NewGuid();
            var team = new Team
            {
                Id = teamId,
                Name = "Defunct Team",
                IsActive = false,
            };

            teamManager
                .Setup(m => m.GetAsync(teamId, It.IsAny<CancellationToken>()))
                .ReturnsAsync(team);

            var result = await controller.GetTeam(teamId, CancellationToken.None);

            Assert.IsType<NotFoundResult>(result);
        }
    }
}
