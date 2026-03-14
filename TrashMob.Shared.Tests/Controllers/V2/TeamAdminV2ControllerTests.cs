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
    using Xunit;

    public class TeamAdminV2ControllerTests
    {
        private readonly Mock<ITeamManager> teamManager = new();
        private readonly Mock<ILogger<TeamAdminV2Controller>> logger = new();
        private readonly TeamAdminV2Controller controller;
        private readonly Guid userId = Guid.NewGuid();

        public TeamAdminV2ControllerTests()
        {
            controller = new TeamAdminV2Controller(teamManager.Object, logger.Object);

            var httpContext = new DefaultHttpContext();
            httpContext.Items["UserId"] = userId.ToString();
            var identity = new GenericIdentity("test", "Bearer");
            var principal = new ClaimsPrincipal(identity);
            httpContext.User = principal;

            controller.ControllerContext = new ControllerContext
            {
                HttpContext = httpContext,
            };
        }

        [Fact]
        public async Task GetAllTeams_ReturnsOkWithTeamDtos()
        {
            var teams = new List<Team>
            {
                new()
                {
                    Id = Guid.NewGuid(),
                    Name = "Team Alpha",
                    IsActive = true,
                    IsPublic = true,
                    City = "Seattle",
                    Region = "WA",
                    Country = "US",
                },
                new()
                {
                    Id = Guid.NewGuid(),
                    Name = "Team Beta",
                    IsActive = false,
                    IsPublic = false,
                    City = "Portland",
                    Region = "OR",
                    Country = "US",
                },
            };

            teamManager
                .Setup(m => m.GetAllTeamsAsync(It.IsAny<CancellationToken>()))
                .ReturnsAsync(teams);

            var result = await controller.GetAllTeams(CancellationToken.None);

            var okResult = Assert.IsType<OkObjectResult>(result);
            var dtos = Assert.IsAssignableFrom<IEnumerable<TeamDto>>(okResult.Value);
            Assert.Equal(2, dtos.Count());
        }

        [Fact]
        public async Task DeleteTeam_TeamNotFound_ReturnsNotFound()
        {
            var teamId = Guid.NewGuid();

            teamManager
                .Setup(m => m.GetAsync(teamId, It.IsAny<CancellationToken>()))
                .ReturnsAsync((Team)null);

            var result = await controller.DeleteTeam(teamId, CancellationToken.None);

            Assert.IsType<NotFoundResult>(result);
        }

        [Fact]
        public async Task DeleteTeam_TeamExists_ReturnsNoContent()
        {
            var teamId = Guid.NewGuid();
            var team = new Team { Id = teamId, Name = "Doomed Team", IsActive = true };

            teamManager
                .Setup(m => m.GetAsync(teamId, It.IsAny<CancellationToken>()))
                .ReturnsAsync(team);

            var result = await controller.DeleteTeam(teamId, CancellationToken.None);

            Assert.IsType<NoContentResult>(result);
            teamManager.Verify(m => m.DeleteAsync(teamId, It.IsAny<CancellationToken>()), Times.Once);
        }

        [Fact]
        public async Task ReactivateTeam_TeamNotFound_ReturnsNotFound()
        {
            var teamId = Guid.NewGuid();

            teamManager
                .Setup(m => m.GetAsync(teamId, It.IsAny<CancellationToken>()))
                .ReturnsAsync((Team)null);

            var result = await controller.ReactivateTeam(teamId, CancellationToken.None);

            Assert.IsType<NotFoundResult>(result);
        }

        [Fact]
        public async Task ReactivateTeam_TeamExists_ReturnsOkWithDto()
        {
            var teamId = Guid.NewGuid();
            var team = new Team
            {
                Id = teamId,
                Name = "Inactive Team",
                IsActive = false,
                City = "Denver",
                Region = "CO",
                Country = "US",
            };

            teamManager
                .Setup(m => m.GetAsync(teamId, It.IsAny<CancellationToken>()))
                .ReturnsAsync(team);

            teamManager
                .Setup(m => m.UpdateAsync(It.IsAny<Team>(), userId, It.IsAny<CancellationToken>()))
                .ReturnsAsync((Team t, Guid _, CancellationToken _) => t);

            var result = await controller.ReactivateTeam(teamId, CancellationToken.None);

            var okResult = Assert.IsType<OkObjectResult>(result);
            var dto = Assert.IsType<TeamDto>(okResult.Value);
            Assert.Equal(teamId, dto.Id);
            Assert.Equal("Inactive Team", dto.Name);
        }
    }
}
