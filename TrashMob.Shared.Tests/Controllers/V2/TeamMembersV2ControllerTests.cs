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
    using TrashMob.Models.Poco.V2;
    using TrashMob.Shared.Managers.Interfaces;
    using Xunit;

    public class TeamMembersV2ControllerTests
    {
        private readonly Mock<ITeamManager> teamManager = new();
        private readonly Mock<ITeamMemberManager> teamMemberManager = new();
        private readonly Mock<ILogger<TeamMembersV2Controller>> logger = new();
        private readonly TeamMembersV2Controller controller;

        public TeamMembersV2ControllerTests()
        {
            controller = new TeamMembersV2Controller(teamManager.Object, teamMemberManager.Object, logger.Object);
            controller.ControllerContext = new ControllerContext
            {
                HttpContext = new DefaultHttpContext(),
            };
        }

        private void SetUserId(Guid userId)
        {
            controller.ControllerContext.HttpContext.Items["UserId"] = userId.ToString();
        }

        [Fact]
        public async Task GetMembers_ReturnsOk_ForPublicTeam()
        {
            var teamId = Guid.NewGuid();
            var team = new Team { Id = teamId, Name = "Public Team", IsActive = true, IsPublic = true };
            var members = new List<TeamMember>
            {
                new() { TeamId = teamId, UserId = Guid.NewGuid(), IsTeamLead = true, User = new User { UserName = "lead" } },
                new() { TeamId = teamId, UserId = Guid.NewGuid(), IsTeamLead = false, User = new User { UserName = "member" } },
            };

            teamManager
                .Setup(m => m.GetAsync(teamId, It.IsAny<CancellationToken>()))
                .ReturnsAsync(team);

            teamMemberManager
                .Setup(m => m.GetByTeamIdAsync(teamId, It.IsAny<CancellationToken>()))
                .ReturnsAsync(members);

            var result = await controller.GetMembers(teamId, CancellationToken.None);

            var okResult = Assert.IsType<OkObjectResult>(result);
            var returnedMembers = Assert.IsAssignableFrom<IEnumerable<TeamMemberDto>>(okResult.Value);
            Assert.Equal(2, new List<TeamMemberDto>(returnedMembers).Count);
        }

        [Fact]
        public async Task GetMembers_ReturnsNotFound_WhenTeamNotFound()
        {
            var teamId = Guid.NewGuid();

            teamManager
                .Setup(m => m.GetAsync(teamId, It.IsAny<CancellationToken>()))
                .ReturnsAsync((Team)null);

            var result = await controller.GetMembers(teamId, CancellationToken.None);

            Assert.IsType<NotFoundResult>(result);
        }

        [Fact]
        public async Task GetLeads_ReturnsOk()
        {
            var teamId = Guid.NewGuid();
            var team = new Team { Id = teamId, Name = "Test Team", IsActive = true, IsPublic = true };
            var leads = new List<TeamMember>
            {
                new() { TeamId = teamId, UserId = Guid.NewGuid(), IsTeamLead = true, User = new User { UserName = "lead" } },
            };

            teamManager
                .Setup(m => m.GetAsync(teamId, It.IsAny<CancellationToken>()))
                .ReturnsAsync(team);

            teamMemberManager
                .Setup(m => m.GetTeamLeadsAsync(teamId, It.IsAny<CancellationToken>()))
                .ReturnsAsync(leads);

            var result = await controller.GetLeads(teamId, CancellationToken.None);

            var okResult = Assert.IsType<OkObjectResult>(result);
            var returnedLeads = Assert.IsAssignableFrom<IEnumerable<TeamMemberDto>>(okResult.Value);
            Assert.Single(returnedLeads);
        }

        [Fact]
        public async Task JoinTeam_ReturnsCreated_ForPublicTeam()
        {
            var teamId = Guid.NewGuid();
            var userId = Guid.NewGuid();
            var team = new Team { Id = teamId, Name = "Public Team", IsActive = true, IsPublic = true };
            var newMember = new TeamMember { TeamId = teamId, UserId = userId, IsTeamLead = false, User = new User { UserName = "newmember" } };

            SetUserId(userId);

            teamManager
                .Setup(m => m.GetAsync(teamId, It.IsAny<CancellationToken>()))
                .ReturnsAsync(team);

            teamMemberManager
                .Setup(m => m.GetByTeamAndUserAsync(teamId, userId, It.IsAny<CancellationToken>()))
                .ReturnsAsync((TeamMember)null);

            teamMemberManager
                .Setup(m => m.AddMemberAsync(teamId, userId, false, userId, It.IsAny<CancellationToken>()))
                .ReturnsAsync(newMember);

            var result = await controller.JoinTeam(teamId, CancellationToken.None);

            var createdResult = Assert.IsType<CreatedAtActionResult>(result);
            var returnedMember = Assert.IsType<TeamMemberDto>(createdResult.Value);
            Assert.Equal(teamId, returnedMember.TeamId);
            Assert.Equal(userId, returnedMember.UserId);
            Assert.False(returnedMember.IsTeamLead);
        }

        [Fact]
        public async Task JoinTeam_ReturnsBadRequest_WhenAlreadyMember()
        {
            var teamId = Guid.NewGuid();
            var userId = Guid.NewGuid();
            var team = new Team { Id = teamId, Name = "Public Team", IsActive = true, IsPublic = true };
            var existingMember = new TeamMember { TeamId = teamId, UserId = userId, IsTeamLead = false };

            SetUserId(userId);

            teamManager
                .Setup(m => m.GetAsync(teamId, It.IsAny<CancellationToken>()))
                .ReturnsAsync(team);

            teamMemberManager
                .Setup(m => m.GetByTeamAndUserAsync(teamId, userId, It.IsAny<CancellationToken>()))
                .ReturnsAsync(existingMember);

            var result = await controller.JoinTeam(teamId, CancellationToken.None);

            Assert.IsType<BadRequestObjectResult>(result);
        }

        [Fact]
        public async Task RemoveMember_ReturnsNoContent()
        {
            var teamId = Guid.NewGuid();
            var userId = Guid.NewGuid();
            var team = new Team { Id = teamId, Name = "Test Team", IsActive = true, IsPublic = true };
            var member = new TeamMember { TeamId = teamId, UserId = userId, IsTeamLead = false };

            SetUserId(userId);

            teamManager
                .Setup(m => m.GetAsync(teamId, It.IsAny<CancellationToken>()))
                .ReturnsAsync(team);

            teamMemberManager
                .Setup(m => m.GetByTeamAndUserAsync(teamId, userId, It.IsAny<CancellationToken>()))
                .ReturnsAsync(member);

            teamMemberManager
                .Setup(m => m.IsTeamLeadAsync(teamId, userId, It.IsAny<CancellationToken>()))
                .ReturnsAsync(false);

            teamMemberManager
                .Setup(m => m.RemoveMemberAsync(teamId, userId, It.IsAny<CancellationToken>()))
                .ReturnsAsync(1);

            var result = await controller.RemoveMember(teamId, userId, CancellationToken.None);

            Assert.IsType<NoContentResult>(result);
        }

        [Fact]
        public async Task PromoteToLead_ReturnsOk()
        {
            var teamId = Guid.NewGuid();
            var currentUserId = Guid.NewGuid();
            var targetUserId = Guid.NewGuid();
            var team = new Team { Id = teamId, Name = "Test Team", IsActive = true, IsPublic = true };
            var member = new TeamMember { TeamId = teamId, UserId = targetUserId, IsTeamLead = false };
            var promotedMember = new TeamMember { TeamId = teamId, UserId = targetUserId, IsTeamLead = true, User = new User { UserName = "promoted" } };

            SetUserId(currentUserId);

            teamManager
                .Setup(m => m.GetAsync(teamId, It.IsAny<CancellationToken>()))
                .ReturnsAsync(team);

            teamMemberManager
                .Setup(m => m.IsTeamLeadAsync(teamId, currentUserId, It.IsAny<CancellationToken>()))
                .ReturnsAsync(true);

            teamMemberManager
                .Setup(m => m.GetByTeamAndUserAsync(teamId, targetUserId, It.IsAny<CancellationToken>()))
                .ReturnsAsync(member);

            teamMemberManager
                .Setup(m => m.PromoteToLeadAsync(teamId, targetUserId, currentUserId, It.IsAny<CancellationToken>()))
                .ReturnsAsync(promotedMember);

            var result = await controller.PromoteToLead(teamId, targetUserId, CancellationToken.None);

            var okResult = Assert.IsType<OkObjectResult>(result);
            var returnedMember = Assert.IsType<TeamMemberDto>(okResult.Value);
            Assert.True(returnedMember.IsTeamLead);
        }

        [Fact]
        public async Task DemoteFromLead_ReturnsBadRequest_WhenLastLead()
        {
            var teamId = Guid.NewGuid();
            var currentUserId = Guid.NewGuid();
            var targetUserId = Guid.NewGuid();
            var team = new Team { Id = teamId, Name = "Test Team", IsActive = true, IsPublic = true };
            var member = new TeamMember { TeamId = teamId, UserId = targetUserId, IsTeamLead = true };

            SetUserId(currentUserId);

            teamManager
                .Setup(m => m.GetAsync(teamId, It.IsAny<CancellationToken>()))
                .ReturnsAsync(team);

            teamMemberManager
                .Setup(m => m.IsTeamLeadAsync(teamId, currentUserId, It.IsAny<CancellationToken>()))
                .ReturnsAsync(true);

            teamMemberManager
                .Setup(m => m.GetByTeamAndUserAsync(teamId, targetUserId, It.IsAny<CancellationToken>()))
                .ReturnsAsync(member);

            teamMemberManager
                .Setup(m => m.GetTeamLeadCountAsync(teamId, It.IsAny<CancellationToken>()))
                .ReturnsAsync(1);

            var result = await controller.DemoteFromLead(teamId, targetUserId, CancellationToken.None);

            Assert.IsType<BadRequestObjectResult>(result);
        }
    }
}
