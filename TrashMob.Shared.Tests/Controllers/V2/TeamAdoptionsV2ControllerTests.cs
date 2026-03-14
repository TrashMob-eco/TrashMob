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

    public class TeamAdoptionsV2ControllerTests
    {
        private readonly Mock<ITeamAdoptionManager> adoptionManager = new();
        private readonly Mock<ITeamAdoptionEventManager> adoptionEventManager = new();
        private readonly Mock<IKeyedManager<Team>> teamManager = new();
        private readonly Mock<ITeamMemberManager> teamMemberManager = new();
        private readonly Mock<ILogger<TeamAdoptionsV2Controller>> logger = new();
        private readonly TeamAdoptionsV2Controller controller;
        private readonly Guid userId = Guid.NewGuid();

        public TeamAdoptionsV2ControllerTests()
        {
            controller = new TeamAdoptionsV2Controller(
                adoptionManager.Object,
                adoptionEventManager.Object,
                teamManager.Object,
                teamMemberManager.Object,
                logger.Object);

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
        public async Task GetTeamAdoptions_TeamNotFound_ReturnsNotFound()
        {
            var teamId = Guid.NewGuid();

            teamManager
                .Setup(m => m.GetAsync(teamId, It.IsAny<CancellationToken>()))
                .ReturnsAsync((Team)null);

            var result = await controller.GetTeamAdoptions(teamId, CancellationToken.None);

            Assert.IsType<NotFoundResult>(result);
        }

        [Fact]
        public async Task GetTeamAdoptions_NotMember_ReturnsForbid()
        {
            var teamId = Guid.NewGuid();
            var team = new Team { Id = teamId, Name = "Test Team", IsActive = true };

            teamManager
                .Setup(m => m.GetAsync(teamId, It.IsAny<CancellationToken>()))
                .ReturnsAsync(team);

            teamMemberManager
                .Setup(m => m.IsMemberAsync(teamId, userId, It.IsAny<CancellationToken>()))
                .ReturnsAsync(false);

            var result = await controller.GetTeamAdoptions(teamId, CancellationToken.None);

            Assert.IsType<ForbidResult>(result);
        }

        [Fact]
        public async Task GetTeamAdoptions_Success_ReturnsOk()
        {
            var teamId = Guid.NewGuid();
            var team = new Team { Id = teamId, Name = "Test Team", IsActive = true };
            var adoptions = new List<TeamAdoption>
            {
                new()
                {
                    Id = Guid.NewGuid(),
                    TeamId = teamId,
                    AdoptableAreaId = Guid.NewGuid(),
                    Status = "Approved",
                    ApplicationDate = DateTimeOffset.UtcNow,
                },
            };

            teamManager
                .Setup(m => m.GetAsync(teamId, It.IsAny<CancellationToken>()))
                .ReturnsAsync(team);

            teamMemberManager
                .Setup(m => m.IsMemberAsync(teamId, userId, It.IsAny<CancellationToken>()))
                .ReturnsAsync(true);

            adoptionManager
                .Setup(m => m.GetByTeamIdAsync(teamId, It.IsAny<CancellationToken>()))
                .ReturnsAsync(adoptions);

            var result = await controller.GetTeamAdoptions(teamId, CancellationToken.None);

            var okResult = Assert.IsType<OkObjectResult>(result);
            var dtos = Assert.IsAssignableFrom<IEnumerable<TeamAdoptionDto>>(okResult.Value);
            Assert.Single(dtos);
        }

        [Fact]
        public async Task SubmitApplication_NotTeamLead_ReturnsForbid()
        {
            var teamId = Guid.NewGuid();
            var team = new Team { Id = teamId, Name = "Test Team", IsActive = true };
            var request = new SubmitAdoptionRequestDto
            {
                AdoptableAreaId = Guid.NewGuid(),
                ApplicationNotes = "We want to adopt this area",
            };

            teamManager
                .Setup(m => m.GetAsync(teamId, It.IsAny<CancellationToken>()))
                .ReturnsAsync(team);

            teamMemberManager
                .Setup(m => m.IsTeamLeadAsync(teamId, userId, It.IsAny<CancellationToken>()))
                .ReturnsAsync(false);

            var result = await controller.SubmitApplication(teamId, request, CancellationToken.None);

            Assert.IsType<ForbidResult>(result);
        }

        [Fact]
        public async Task SubmitApplication_Success_ReturnsCreated()
        {
            var teamId = Guid.NewGuid();
            var areaId = Guid.NewGuid();
            var team = new Team { Id = teamId, Name = "Test Team", IsActive = true };
            var request = new SubmitAdoptionRequestDto
            {
                AdoptableAreaId = areaId,
                ApplicationNotes = "We want to adopt this area",
            };

            var adoption = new TeamAdoption
            {
                Id = Guid.NewGuid(),
                TeamId = teamId,
                AdoptableAreaId = areaId,
                Status = "Pending",
                ApplicationDate = DateTimeOffset.UtcNow,
                ApplicationNotes = "We want to adopt this area",
            };

            teamManager
                .Setup(m => m.GetAsync(teamId, It.IsAny<CancellationToken>()))
                .ReturnsAsync(team);

            teamMemberManager
                .Setup(m => m.IsTeamLeadAsync(teamId, userId, It.IsAny<CancellationToken>()))
                .ReturnsAsync(true);

            adoptionManager
                .Setup(m => m.SubmitApplicationAsync(
                    teamId, areaId, It.IsAny<string>(), userId, It.IsAny<CancellationToken>()))
                .ReturnsAsync(ServiceResult<TeamAdoption>.Success(adoption));

            var result = await controller.SubmitApplication(teamId, request, CancellationToken.None);

            var createdResult = Assert.IsType<CreatedAtActionResult>(result);
            var dto = Assert.IsType<TeamAdoptionDto>(createdResult.Value);
            Assert.Equal(adoption.Id, dto.Id);
            Assert.Equal(teamId, dto.TeamId);
        }

        [Fact]
        public async Task GetActiveAdoptions_Success_ReturnsOk()
        {
            var teamId = Guid.NewGuid();
            var team = new Team { Id = teamId, Name = "Test Team", IsActive = true };
            var adoptions = new List<TeamAdoption>
            {
                new()
                {
                    Id = Guid.NewGuid(),
                    TeamId = teamId,
                    AdoptableAreaId = Guid.NewGuid(),
                    Status = "Approved",
                    ApplicationDate = DateTimeOffset.UtcNow,
                },
                new()
                {
                    Id = Guid.NewGuid(),
                    TeamId = teamId,
                    AdoptableAreaId = Guid.NewGuid(),
                    Status = "Approved",
                    ApplicationDate = DateTimeOffset.UtcNow,
                },
            };

            teamManager
                .Setup(m => m.GetAsync(teamId, It.IsAny<CancellationToken>()))
                .ReturnsAsync(team);

            teamMemberManager
                .Setup(m => m.IsMemberAsync(teamId, userId, It.IsAny<CancellationToken>()))
                .ReturnsAsync(true);

            adoptionEventManager
                .Setup(m => m.GetActiveAdoptionsForTeamAsync(teamId, It.IsAny<CancellationToken>()))
                .ReturnsAsync(adoptions);

            var result = await controller.GetActiveAdoptions(teamId, CancellationToken.None);

            var okResult = Assert.IsType<OkObjectResult>(result);
            var dtos = Assert.IsAssignableFrom<IEnumerable<TeamAdoptionDto>>(okResult.Value);
            Assert.Equal(2, dtos.Count());
        }
    }
}
