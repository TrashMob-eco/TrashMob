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
    using TrashMob.Shared.Managers.Interfaces;
    using TrashMob.Shared.Persistence.Interfaces;
    using TrashMob.Shared.Tests.Fixtures;
    using Xunit;

    public class TeamEventsV2ControllerTests
    {
        private readonly Mock<ITeamManager> teamManager = new();
        private readonly Mock<ITeamMemberManager> teamMemberManager = new();
        private readonly Mock<IKeyedRepository<TeamEvent>> teamEventRepository = new();
        private readonly Mock<IKeyedManager<Event>> eventManager = new();
        private readonly Mock<ILogger<TeamEventsV2Controller>> logger = new();
        private readonly TeamEventsV2Controller controller;
        private readonly Guid userId = Guid.NewGuid();

        public TeamEventsV2ControllerTests()
        {
            controller = new TeamEventsV2Controller(
                teamManager.Object,
                teamMemberManager.Object,
                teamEventRepository.Object,
                eventManager.Object,
                logger.Object);
            controller.ControllerContext = new ControllerContext
            {
                HttpContext = new DefaultHttpContext(),
            };
            controller.HttpContext.Items["UserId"] = userId.ToString();

            // Default setup: repository returns empty queryable
            var teamEvents = new List<TeamEvent>().AsQueryable();
            teamEventRepository.Setup(r => r.Get()).Returns(new TestAsyncEnumerable<TeamEvent>(teamEvents));
        }

        [Fact]
        public async Task GetUpcomingTeamEvents_ReturnsNotFound_WhenTeamNotFound()
        {
            var teamId = Guid.NewGuid();

            teamManager
                .Setup(m => m.GetAsync(teamId, It.IsAny<CancellationToken>()))
                .ReturnsAsync((Team)null);

            var result = await controller.GetUpcomingTeamEvents(teamId, CancellationToken.None);

            var objectResult = Assert.IsType<ObjectResult>(result);
            Assert.Equal(StatusCodes.Status404NotFound, objectResult.StatusCode);
        }

        [Fact]
        public async Task GetPastTeamEvents_ReturnsNotFound_WhenTeamNotActive()
        {
            var teamId = Guid.NewGuid();
            var team = new Team
            {
                Id = teamId,
                Name = "Inactive Team",
                IsActive = false,
                IsPublic = true,
            };

            teamManager
                .Setup(m => m.GetAsync(teamId, It.IsAny<CancellationToken>()))
                .ReturnsAsync(team);

            var result = await controller.GetPastTeamEvents(teamId, CancellationToken.None);

            var objectResult = Assert.IsType<ObjectResult>(result);
            Assert.Equal(StatusCodes.Status404NotFound, objectResult.StatusCode);
        }

        [Fact]
        public async Task LinkEventToTeam_ReturnsForbid_WhenNotLead()
        {
            var teamId = Guid.NewGuid();
            var eventId = Guid.NewGuid();
            var team = new Team
            {
                Id = teamId,
                Name = "Test Team",
                IsActive = true,
                IsPublic = true,
            };

            teamManager
                .Setup(m => m.GetAsync(teamId, It.IsAny<CancellationToken>()))
                .ReturnsAsync(team);

            teamMemberManager
                .Setup(m => m.IsTeamLeadAsync(teamId, userId, It.IsAny<CancellationToken>()))
                .ReturnsAsync(false);

            var result = await controller.LinkEventToTeam(teamId, eventId, CancellationToken.None);

            Assert.IsType<ForbidResult>(result);
        }

        [Fact]
        public async Task UnlinkEventFromTeam_ReturnsNotFound_WhenLinkDoesNotExist()
        {
            var teamId = Guid.NewGuid();
            var eventId = Guid.NewGuid();
            var team = new Team
            {
                Id = teamId,
                Name = "Test Team",
                IsActive = true,
                IsPublic = true,
            };

            teamManager
                .Setup(m => m.GetAsync(teamId, It.IsAny<CancellationToken>()))
                .ReturnsAsync(team);

            teamMemberManager
                .Setup(m => m.IsTeamLeadAsync(teamId, userId, It.IsAny<CancellationToken>()))
                .ReturnsAsync(true);

            // Repository returns empty list, so no link exists
            var emptyTeamEvents = new List<TeamEvent>().AsQueryable();
            teamEventRepository
                .Setup(r => r.Get())
                .Returns(new TestAsyncEnumerable<TeamEvent>(emptyTeamEvents));

            var result = await controller.UnlinkEventFromTeam(teamId, eventId, CancellationToken.None);

            var objectResult = Assert.IsType<ObjectResult>(result);
            Assert.Equal(StatusCodes.Status404NotFound, objectResult.StatusCode);
        }
    }
}
