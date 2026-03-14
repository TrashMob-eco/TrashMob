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

    public class TeamInvitesV2ControllerTests
    {
        private readonly Mock<IEmailInviteManager> emailInviteManager = new();
        private readonly Mock<IKeyedManager<Team>> teamManager = new();
        private readonly Mock<ITeamMemberManager> teamMemberManager = new();
        private readonly Mock<ILogger<TeamInvitesV2Controller>> logger = new();
        private readonly TeamInvitesV2Controller controller;
        private readonly Guid userId = Guid.NewGuid();

        public TeamInvitesV2ControllerTests()
        {
            controller = new TeamInvitesV2Controller(
                emailInviteManager.Object,
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
        public async Task GetBatches_TeamNotFound_ReturnsNotFound()
        {
            var teamId = Guid.NewGuid();

            teamManager
                .Setup(m => m.GetAsync(teamId, It.IsAny<CancellationToken>()))
                .ReturnsAsync((Team)null);

            var result = await controller.GetBatches(teamId, CancellationToken.None);

            Assert.IsType<NotFoundResult>(result);
        }

        [Fact]
        public async Task GetBatches_NotTeamLead_ReturnsForbid()
        {
            var teamId = Guid.NewGuid();
            var team = new Team { Id = teamId, Name = "Test Team", IsActive = true };

            teamManager
                .Setup(m => m.GetAsync(teamId, It.IsAny<CancellationToken>()))
                .ReturnsAsync(team);

            teamMemberManager
                .Setup(m => m.IsTeamLeadAsync(teamId, userId, It.IsAny<CancellationToken>()))
                .ReturnsAsync(false);

            var result = await controller.GetBatches(teamId, CancellationToken.None);

            Assert.IsType<ForbidResult>(result);
        }

        [Fact]
        public async Task GetBatches_Success_ReturnsOk()
        {
            var teamId = Guid.NewGuid();
            var team = new Team { Id = teamId, Name = "Active Team", IsActive = true };
            var batches = new List<EmailInviteBatch>
            {
                new()
                {
                    Id = Guid.NewGuid(),
                    SenderUserId = userId,
                    BatchType = "Team",
                    TeamId = teamId,
                    TotalCount = 5,
                    SentCount = 5,
                    Status = "Complete",
                },
            };

            teamManager
                .Setup(m => m.GetAsync(teamId, It.IsAny<CancellationToken>()))
                .ReturnsAsync(team);

            teamMemberManager
                .Setup(m => m.IsTeamLeadAsync(teamId, userId, It.IsAny<CancellationToken>()))
                .ReturnsAsync(true);

            emailInviteManager
                .Setup(m => m.GetTeamBatchesAsync(teamId, It.IsAny<CancellationToken>()))
                .ReturnsAsync(batches);

            var result = await controller.GetBatches(teamId, CancellationToken.None);

            var okResult = Assert.IsType<OkObjectResult>(result);
            var dtos = Assert.IsAssignableFrom<IEnumerable<EmailInviteBatchDto>>(okResult.Value);
            Assert.Single(dtos);
        }

        [Fact]
        public async Task CreateBatch_EmptyEmails_ReturnsBadRequest()
        {
            var teamId = Guid.NewGuid();
            var team = new Team { Id = teamId, Name = "Test Team", IsActive = true };
            var request = new CreateEmailInviteBatchDto { Emails = [] };

            teamManager
                .Setup(m => m.GetAsync(teamId, It.IsAny<CancellationToken>()))
                .ReturnsAsync(team);

            teamMemberManager
                .Setup(m => m.IsTeamLeadAsync(teamId, userId, It.IsAny<CancellationToken>()))
                .ReturnsAsync(true);

            var result = await controller.CreateBatch(teamId, request, CancellationToken.None);

            var problemResult = Assert.IsType<ObjectResult>(result);
            Assert.Equal(StatusCodes.Status400BadRequest, problemResult.StatusCode);
        }

        [Fact]
        public async Task CreateBatch_Success_ReturnsCreated()
        {
            var teamId = Guid.NewGuid();
            var batchId = Guid.NewGuid();
            var team = new Team { Id = teamId, Name = "Test Team", IsActive = true };
            var request = new CreateEmailInviteBatchDto
            {
                Emails = ["alice@example.com", "bob@example.com"],
            };

            var createdBatch = new EmailInviteBatch
            {
                Id = batchId,
                SenderUserId = userId,
                BatchType = "Team",
                TeamId = teamId,
                TotalCount = 2,
                Status = "Pending",
            };

            var processedBatch = new EmailInviteBatch
            {
                Id = batchId,
                SenderUserId = userId,
                BatchType = "Team",
                TeamId = teamId,
                TotalCount = 2,
                SentCount = 2,
                Status = "Complete",
            };

            teamManager
                .Setup(m => m.GetAsync(teamId, It.IsAny<CancellationToken>()))
                .ReturnsAsync(team);

            teamMemberManager
                .Setup(m => m.IsTeamLeadAsync(teamId, userId, It.IsAny<CancellationToken>()))
                .ReturnsAsync(true);

            emailInviteManager
                .Setup(m => m.CreateBatchAsync(
                    It.IsAny<IEnumerable<string>>(), userId, "Team", null, teamId, It.IsAny<CancellationToken>()))
                .ReturnsAsync(createdBatch);

            emailInviteManager
                .Setup(m => m.ProcessBatchAsync(batchId, It.IsAny<CancellationToken>()))
                .ReturnsAsync(processedBatch);

            var result = await controller.CreateBatch(teamId, request, CancellationToken.None);

            var createdResult = Assert.IsType<CreatedAtActionResult>(result);
            var dto = Assert.IsType<EmailInviteBatchDto>(createdResult.Value);
            Assert.Equal(batchId, dto.Id);
            Assert.Equal("Complete", dto.Status);
        }
    }
}
