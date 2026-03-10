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
        private readonly Mock<ITeamMemberManager> teamMemberManager = new();
        private readonly Mock<IKeyedManager<User>> userManager = new();
        private readonly Mock<ILogger<TeamsV2Controller>> logger = new();
        private readonly TeamsV2Controller controller;

        public TeamsV2ControllerTests()
        {
            controller = new TeamsV2Controller(teamManager.Object, teamMemberManager.Object, userManager.Object, logger.Object);
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

        [Fact]
        public async Task GetMyTeams_ReturnsOk()
        {
            var userId = Guid.NewGuid();
            controller.HttpContext.Items["UserId"] = userId.ToString();
            var teams = new List<Team>
            {
                new() { Id = Guid.NewGuid(), Name = "My Team", IsActive = true },
            };

            teamManager
                .Setup(m => m.GetTeamsByUserAsync(userId, It.IsAny<CancellationToken>()))
                .ReturnsAsync(teams);

            var result = await controller.GetMyTeams(CancellationToken.None);

            var okResult = Assert.IsType<OkObjectResult>(result);
            var returnedTeams = Assert.IsAssignableFrom<IEnumerable<Team>>(okResult.Value);
            Assert.Single(returnedTeams);
        }

        [Fact]
        public async Task CreateTeam_ReturnsCreated_WhenNameAvailable()
        {
            var userId = Guid.NewGuid();
            controller.HttpContext.Items["UserId"] = userId.ToString();
            var team = new Team { Name = "New Team" };
            var user = new User { Id = userId, UserName = "creator" };

            teamManager
                .Setup(m => m.IsTeamNameAvailableAsync("New Team", It.IsAny<Guid?>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(true);
            userManager
                .Setup(m => m.GetAsync(userId, It.IsAny<CancellationToken>()))
                .ReturnsAsync(user);
            teamManager
                .Setup(m => m.AddAsync(It.IsAny<Team>(), userId, It.IsAny<CancellationToken>()))
                .ReturnsAsync((Team t, Guid _, CancellationToken _) => t);

            var result = await controller.CreateTeam(team, CancellationToken.None);

            Assert.IsType<CreatedAtActionResult>(result);
            teamMemberManager.Verify(
                m => m.AddMemberAsync(It.IsAny<Guid>(), userId, true, userId, It.IsAny<CancellationToken>()),
                Times.Once);
        }

        [Fact]
        public async Task CreateTeam_ReturnsBadRequest_WhenNameTaken()
        {
            var userId = Guid.NewGuid();
            controller.HttpContext.Items["UserId"] = userId.ToString();
            var team = new Team { Name = "Existing Team" };

            teamManager
                .Setup(m => m.IsTeamNameAvailableAsync("Existing Team", It.IsAny<Guid?>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(false);

            var result = await controller.CreateTeam(team, CancellationToken.None);

            Assert.IsType<BadRequestObjectResult>(result);
        }

        [Fact]
        public async Task DeactivateTeam_ReturnsNoContent_WhenLead()
        {
            var teamId = Guid.NewGuid();
            var userId = Guid.NewGuid();
            controller.HttpContext.Items["UserId"] = userId.ToString();
            var existingTeam = new Team { Id = teamId, Name = "Team", IsActive = true };

            teamManager
                .Setup(m => m.GetAsync(teamId, It.IsAny<CancellationToken>()))
                .ReturnsAsync(existingTeam);
            teamMemberManager
                .Setup(m => m.IsTeamLeadAsync(teamId, userId, It.IsAny<CancellationToken>()))
                .ReturnsAsync(true);

            var result = await controller.DeactivateTeam(teamId, CancellationToken.None);

            Assert.IsType<NoContentResult>(result);
            teamManager.Verify(m => m.UpdateAsync(
                It.Is<Team>(t => !t.IsActive), userId, It.IsAny<CancellationToken>()), Times.Once);
        }

        [Fact]
        public async Task DeactivateTeam_ReturnsForbid_WhenNotLead()
        {
            var teamId = Guid.NewGuid();
            var userId = Guid.NewGuid();
            controller.HttpContext.Items["UserId"] = userId.ToString();
            var existingTeam = new Team { Id = teamId, Name = "Team", IsActive = true };

            teamManager
                .Setup(m => m.GetAsync(teamId, It.IsAny<CancellationToken>()))
                .ReturnsAsync(existingTeam);
            teamMemberManager
                .Setup(m => m.IsTeamLeadAsync(teamId, userId, It.IsAny<CancellationToken>()))
                .ReturnsAsync(false);

            var result = await controller.DeactivateTeam(teamId, CancellationToken.None);

            Assert.IsType<ForbidResult>(result);
        }

        [Fact]
        public async Task CheckTeamName_ReturnsTrue_WhenAvailable()
        {
            teamManager
                .Setup(m => m.IsTeamNameAvailableAsync("Available", It.IsAny<Guid?>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(true);

            var result = await controller.CheckTeamName("Available", null, CancellationToken.None);

            var okResult = Assert.IsType<OkObjectResult>(result);
            Assert.Equal(true, okResult.Value);
        }

        [Fact]
        public async Task CheckTeamName_ReturnsFalse_WhenBlank()
        {
            var result = await controller.CheckTeamName("", null, CancellationToken.None);

            var okResult = Assert.IsType<OkObjectResult>(result);
            Assert.Equal(false, okResult.Value);
        }
    }
}
