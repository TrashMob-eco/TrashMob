namespace TrashMob.Shared.Tests.Controllers.V2
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Security.Claims;
    using System.Threading;
    using System.Threading.Tasks;
    using Microsoft.AspNetCore.Http;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.Extensions.Logging;
    using Moq;
    using TrashMob.Controllers;
    using TrashMob.Controllers.V2;
    using TrashMob.Models;
    using TrashMob.Models.Poco.V2;
    using TrashMob.Shared.Managers.Interfaces;
    using TrashMob.Shared.Poco;
    using Xunit;

    public class AdoptionEventsV2ControllerTests
    {
        private readonly Mock<ITeamAdoptionEventManager> adoptionEventManager = new();
        private readonly Mock<ITeamAdoptionManager> adoptionManager = new();
        private readonly Mock<ITeamMemberManager> teamMemberManager = new();
        private readonly Mock<ILogger<AdoptionEventsV2Controller>> logger = new();
        private readonly AdoptionEventsV2Controller controller;
        private readonly Guid testUserId = Guid.NewGuid();

        public AdoptionEventsV2ControllerTests()
        {
            controller = new AdoptionEventsV2Controller(
                adoptionEventManager.Object,
                adoptionManager.Object,
                teamMemberManager.Object,
                logger.Object);

            var httpContext = new DefaultHttpContext();
            httpContext.Items["UserId"] = testUserId.ToString();
            httpContext.User = new ClaimsPrincipal(new ClaimsIdentity(
                [new Claim(ClaimTypes.NameIdentifier, testUserId.ToString())], "TestAuth"));
            controller.ControllerContext = new ControllerContext { HttpContext = httpContext };
        }

        [Fact]
        public async Task GetLinkedEvents_ReturnsOkWithList()
        {
            var adoptionId = Guid.NewGuid();
            var teamId = Guid.NewGuid();
            var adoption = new TeamAdoption { Id = adoptionId, TeamId = teamId };
            var events = new List<TeamAdoptionEvent>
            {
                new() { Id = Guid.NewGuid(), TeamAdoptionId = adoptionId, EventId = Guid.NewGuid() },
                new() { Id = Guid.NewGuid(), TeamAdoptionId = adoptionId, EventId = Guid.NewGuid() },
            };

            adoptionManager
                .Setup(m => m.GetAsync(adoptionId, It.IsAny<CancellationToken>()))
                .ReturnsAsync(adoption);
            teamMemberManager
                .Setup(m => m.IsMemberAsync(teamId, testUserId, It.IsAny<CancellationToken>()))
                .ReturnsAsync(true);
            adoptionEventManager
                .Setup(m => m.GetByAdoptionIdAsync(adoptionId, It.IsAny<CancellationToken>()))
                .ReturnsAsync(events);

            var result = await controller.GetLinkedEvents(adoptionId, CancellationToken.None);

            var okResult = Assert.IsType<OkObjectResult>(result);
            var dtos = Assert.IsAssignableFrom<IEnumerable<TeamAdoptionEventDto>>(okResult.Value);
            Assert.Equal(2, dtos.Count());
        }

        [Fact]
        public async Task GetLinkedEvents_ReturnsForbid_WhenNotTeamMember()
        {
            var adoptionId = Guid.NewGuid();
            var teamId = Guid.NewGuid();
            var adoption = new TeamAdoption { Id = adoptionId, TeamId = teamId };

            adoptionManager
                .Setup(m => m.GetAsync(adoptionId, It.IsAny<CancellationToken>()))
                .ReturnsAsync(adoption);
            teamMemberManager
                .Setup(m => m.IsMemberAsync(teamId, testUserId, It.IsAny<CancellationToken>()))
                .ReturnsAsync(false);

            var result = await controller.GetLinkedEvents(adoptionId, CancellationToken.None);

            Assert.IsType<ForbidResult>(result);
        }

        [Fact]
        public async Task LinkEvent_ReturnsCreated()
        {
            var adoptionId = Guid.NewGuid();
            var eventId = Guid.NewGuid();
            var teamId = Guid.NewGuid();
            var adoption = new TeamAdoption { Id = adoptionId, TeamId = teamId };
            var adoptionEvent = new TeamAdoptionEvent
            {
                Id = Guid.NewGuid(),
                TeamAdoptionId = adoptionId,
                EventId = eventId,
            };
            var request = new LinkEventRequest { Notes = "Cleanup event" };

            adoptionManager
                .Setup(m => m.GetAsync(adoptionId, It.IsAny<CancellationToken>()))
                .ReturnsAsync(adoption);
            teamMemberManager
                .Setup(m => m.IsTeamLeadAsync(teamId, testUserId, It.IsAny<CancellationToken>()))
                .ReturnsAsync(true);
            adoptionEventManager
                .Setup(m => m.LinkEventAsync(adoptionId, eventId, "Cleanup event", testUserId, It.IsAny<CancellationToken>()))
                .ReturnsAsync(ServiceResult<TeamAdoptionEvent>.Success(adoptionEvent));

            var result = await controller.LinkEvent(adoptionId, eventId, request, CancellationToken.None);

            var createdResult = Assert.IsType<CreatedAtActionResult>(result);
            Assert.Equal(201, createdResult.StatusCode);
            var dto = Assert.IsType<TeamAdoptionEventDto>(createdResult.Value);
            Assert.Equal(adoptionId, dto.TeamAdoptionId);
        }

        [Fact]
        public async Task UnlinkEvent_ReturnsNoContent()
        {
            var adoptionId = Guid.NewGuid();
            var adoptionEventId = Guid.NewGuid();
            var teamId = Guid.NewGuid();
            var adoption = new TeamAdoption { Id = adoptionId, TeamId = teamId };

            adoptionManager
                .Setup(m => m.GetAsync(adoptionId, It.IsAny<CancellationToken>()))
                .ReturnsAsync(adoption);
            teamMemberManager
                .Setup(m => m.IsTeamLeadAsync(teamId, testUserId, It.IsAny<CancellationToken>()))
                .ReturnsAsync(true);
            adoptionEventManager
                .Setup(m => m.UnlinkEventAsync(adoptionEventId, testUserId, It.IsAny<CancellationToken>()))
                .ReturnsAsync(ServiceResult<bool>.Success(true));

            var result = await controller.UnlinkEvent(adoptionId, adoptionEventId, CancellationToken.None);

            Assert.IsType<NoContentResult>(result);
        }
    }
}
