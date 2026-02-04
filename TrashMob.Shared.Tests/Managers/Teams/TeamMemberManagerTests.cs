namespace TrashMob.Shared.Tests.Managers.Teams
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading;
    using System.Threading.Tasks;
    using Moq;
    using TrashMob.Models;
    using TrashMob.Shared.Managers.Teams;
    using TrashMob.Shared.Persistence.Interfaces;
    using TrashMob.Shared.Tests.Builders;
    using TrashMob.Shared.Tests.Fixtures;
    using Xunit;

    public class TeamMemberManagerTests
    {
        private readonly Mock<IKeyedRepository<TeamMember>> _teamMemberRepository;
        private readonly TeamMemberManager _sut;

        public TeamMemberManagerTests()
        {
            _teamMemberRepository = new Mock<IKeyedRepository<TeamMember>>();
            _sut = new TeamMemberManager(_teamMemberRepository.Object);
        }

        #region GetByTeamIdAsync

        [Fact]
        public async Task GetByTeamIdAsync_ReturnsAllMembersForTeam()
        {
            // Arrange
            var teamId = Guid.NewGuid();
            var member1 = new TeamMemberBuilder().WithTeamId(teamId).WithUserId(Guid.NewGuid()).Build();
            var member2 = new TeamMemberBuilder().WithTeamId(teamId).WithUserId(Guid.NewGuid()).Build();
            var otherTeamMember = new TeamMemberBuilder().WithTeamId(Guid.NewGuid()).WithUserId(Guid.NewGuid()).Build();

            var allMembers = new List<TeamMember> { member1, member2, otherTeamMember };
            _teamMemberRepository.SetupGet(allMembers);

            // Act
            var result = await _sut.GetByTeamIdAsync(teamId);

            // Assert
            var resultList = result.ToList();
            Assert.Equal(2, resultList.Count);
            Assert.All(resultList, m => Assert.Equal(teamId, m.TeamId));
        }

        [Fact]
        public async Task GetByTeamIdAsync_ReturnsEmptyWhenNoMembers()
        {
            // Arrange
            var teamId = Guid.NewGuid();
            var otherTeamMember = new TeamMemberBuilder().WithTeamId(Guid.NewGuid()).Build();

            _teamMemberRepository.SetupGet(new List<TeamMember> { otherTeamMember });

            // Act
            var result = await _sut.GetByTeamIdAsync(teamId);

            // Assert
            Assert.Empty(result);
        }

        #endregion

        #region GetTeamLeadsAsync

        [Fact]
        public async Task GetTeamLeadsAsync_ReturnsOnlyLeads()
        {
            // Arrange
            var teamId = Guid.NewGuid();
            var lead1 = new TeamMemberBuilder().WithTeamId(teamId).AsTeamLead().Build();
            var lead2 = new TeamMemberBuilder().WithTeamId(teamId).AsTeamLead().Build();
            var regularMember = new TeamMemberBuilder().WithTeamId(teamId).AsRegularMember().Build();

            var allMembers = new List<TeamMember> { lead1, lead2, regularMember };
            _teamMemberRepository.SetupGet(allMembers);

            // Act
            var result = await _sut.GetTeamLeadsAsync(teamId);

            // Assert
            var resultList = result.ToList();
            Assert.Equal(2, resultList.Count);
            Assert.All(resultList, m => Assert.True(m.IsTeamLead));
        }

        [Fact]
        public async Task GetTeamLeadsAsync_ReturnsEmptyWhenNoLeads()
        {
            // Arrange
            var teamId = Guid.NewGuid();
            var regularMember = new TeamMemberBuilder().WithTeamId(teamId).AsRegularMember().Build();

            _teamMemberRepository.SetupGet(new List<TeamMember> { regularMember });

            // Act
            var result = await _sut.GetTeamLeadsAsync(teamId);

            // Assert
            Assert.Empty(result);
        }

        #endregion

        #region GetByTeamAndUserAsync

        [Fact]
        public async Task GetByTeamAndUserAsync_ReturnsMemberWhenExists()
        {
            // Arrange
            var teamId = Guid.NewGuid();
            var userId = Guid.NewGuid();
            var member = new TeamMemberBuilder().WithTeamId(teamId).WithUserId(userId).Build();

            _teamMemberRepository.SetupGet(new List<TeamMember> { member });

            // Act
            var result = await _sut.GetByTeamAndUserAsync(teamId, userId);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(teamId, result.TeamId);
            Assert.Equal(userId, result.UserId);
        }

        [Fact]
        public async Task GetByTeamAndUserAsync_ReturnsNullWhenNotFound()
        {
            // Arrange
            var teamId = Guid.NewGuid();
            var userId = Guid.NewGuid();
            var otherMember = new TeamMemberBuilder().WithTeamId(Guid.NewGuid()).WithUserId(Guid.NewGuid()).Build();

            _teamMemberRepository.SetupGet(new List<TeamMember> { otherMember });

            // Act
            var result = await _sut.GetByTeamAndUserAsync(teamId, userId);

            // Assert
            Assert.Null(result);
        }

        #endregion

        #region IsMemberAsync

        [Fact]
        public async Task IsMemberAsync_ReturnsTrueWhenMember()
        {
            // Arrange
            var teamId = Guid.NewGuid();
            var userId = Guid.NewGuid();
            var member = new TeamMemberBuilder().WithTeamId(teamId).WithUserId(userId).Build();

            _teamMemberRepository.SetupGet(new List<TeamMember> { member });

            // Act
            var result = await _sut.IsMemberAsync(teamId, userId);

            // Assert
            Assert.True(result);
        }

        [Fact]
        public async Task IsMemberAsync_ReturnsFalseWhenNotMember()
        {
            // Arrange
            var teamId = Guid.NewGuid();
            var userId = Guid.NewGuid();

            _teamMemberRepository.SetupGet(new List<TeamMember>());

            // Act
            var result = await _sut.IsMemberAsync(teamId, userId);

            // Assert
            Assert.False(result);
        }

        #endregion

        #region IsTeamLeadAsync

        [Fact]
        public async Task IsTeamLeadAsync_ReturnsTrueWhenLead()
        {
            // Arrange
            var teamId = Guid.NewGuid();
            var userId = Guid.NewGuid();
            var member = new TeamMemberBuilder().WithTeamId(teamId).WithUserId(userId).AsTeamLead().Build();

            _teamMemberRepository.SetupGet(new List<TeamMember> { member });

            // Act
            var result = await _sut.IsTeamLeadAsync(teamId, userId);

            // Assert
            Assert.True(result);
        }

        [Fact]
        public async Task IsTeamLeadAsync_ReturnsFalseWhenRegularMember()
        {
            // Arrange
            var teamId = Guid.NewGuid();
            var userId = Guid.NewGuid();
            var member = new TeamMemberBuilder().WithTeamId(teamId).WithUserId(userId).AsRegularMember().Build();

            _teamMemberRepository.SetupGet(new List<TeamMember> { member });

            // Act
            var result = await _sut.IsTeamLeadAsync(teamId, userId);

            // Assert
            Assert.False(result);
        }

        [Fact]
        public async Task IsTeamLeadAsync_ReturnsFalseWhenNotMember()
        {
            // Arrange
            var teamId = Guid.NewGuid();
            var userId = Guid.NewGuid();

            _teamMemberRepository.SetupGet(new List<TeamMember>());

            // Act
            var result = await _sut.IsTeamLeadAsync(teamId, userId);

            // Assert
            Assert.False(result);
        }

        #endregion

        #region AddMemberAsync

        [Fact]
        public async Task AddMemberAsync_CreatesNewMembership()
        {
            // Arrange
            var teamId = Guid.NewGuid();
            var userId = Guid.NewGuid();
            var addedByUserId = Guid.NewGuid();

            TeamMember capturedMember = null;
            _teamMemberRepository.Setup(r => r.AddAsync(It.IsAny<TeamMember>()))
                .Callback<TeamMember>(m => capturedMember = m)
                .ReturnsAsync((TeamMember m) => m);

            // Act
            var result = await _sut.AddMemberAsync(teamId, userId, false, addedByUserId);

            // Assert
            Assert.NotNull(capturedMember);
            Assert.Equal(teamId, capturedMember.TeamId);
            Assert.Equal(userId, capturedMember.UserId);
            Assert.False(capturedMember.IsTeamLead);
            Assert.Equal(addedByUserId, capturedMember.CreatedByUserId);
        }

        [Fact]
        public async Task AddMemberAsync_SetsTeamLeadWhenRequested()
        {
            // Arrange
            var teamId = Guid.NewGuid();
            var userId = Guid.NewGuid();
            var addedByUserId = Guid.NewGuid();

            TeamMember capturedMember = null;
            _teamMemberRepository.Setup(r => r.AddAsync(It.IsAny<TeamMember>()))
                .Callback<TeamMember>(m => capturedMember = m)
                .ReturnsAsync((TeamMember m) => m);

            // Act
            var result = await _sut.AddMemberAsync(teamId, userId, true, addedByUserId);

            // Assert
            Assert.True(capturedMember.IsTeamLead);
        }

        #endregion

        #region RemoveMemberAsync

        [Fact]
        public async Task RemoveMemberAsync_RemovesMemberWhenExists()
        {
            // Arrange
            var teamId = Guid.NewGuid();
            var userId = Guid.NewGuid();
            var memberId = Guid.NewGuid();
            var member = new TeamMemberBuilder()
                .WithId(memberId)
                .WithTeamId(teamId)
                .WithUserId(userId)
                .Build();

            _teamMemberRepository.SetupGet(new List<TeamMember> { member });
            _teamMemberRepository.Setup(r => r.DeleteAsync(memberId)).ReturnsAsync(1);

            // Act
            var result = await _sut.RemoveMemberAsync(teamId, userId);

            // Assert
            Assert.Equal(1, result);
            _teamMemberRepository.Verify(r => r.DeleteAsync(memberId), Times.Once);
        }

        [Fact]
        public async Task RemoveMemberAsync_ReturnsZeroWhenMemberNotFound()
        {
            // Arrange
            var teamId = Guid.NewGuid();
            var userId = Guid.NewGuid();

            _teamMemberRepository.SetupGet(new List<TeamMember>());

            // Act
            var result = await _sut.RemoveMemberAsync(teamId, userId);

            // Assert
            Assert.Equal(0, result);
            _teamMemberRepository.Verify(r => r.DeleteAsync(It.IsAny<Guid>()), Times.Never);
        }

        #endregion

        #region PromoteToLeadAsync

        [Fact]
        public async Task PromoteToLeadAsync_SetsMemberAsLead()
        {
            // Arrange
            var teamId = Guid.NewGuid();
            var userId = Guid.NewGuid();
            var promotedByUserId = Guid.NewGuid();
            var member = new TeamMemberBuilder()
                .WithTeamId(teamId)
                .WithUserId(userId)
                .AsRegularMember()
                .Build();

            _teamMemberRepository.SetupGet(new List<TeamMember> { member });
            _teamMemberRepository.Setup(r => r.UpdateAsync(It.IsAny<TeamMember>()))
                .ReturnsAsync((TeamMember m) => m);

            // Act
            var result = await _sut.PromoteToLeadAsync(teamId, userId, promotedByUserId);

            // Assert
            Assert.True(result.IsTeamLead);
            Assert.Equal(promotedByUserId, result.LastUpdatedByUserId);
        }

        [Fact]
        public async Task PromoteToLeadAsync_ThrowsWhenMemberNotFound()
        {
            // Arrange
            var teamId = Guid.NewGuid();
            var userId = Guid.NewGuid();
            var promotedByUserId = Guid.NewGuid();

            _teamMemberRepository.SetupGet(new List<TeamMember>());

            // Act & Assert
            var ex = await Assert.ThrowsAsync<InvalidOperationException>(() =>
                _sut.PromoteToLeadAsync(teamId, userId, promotedByUserId));

            Assert.Contains(userId.ToString(), ex.Message);
            Assert.Contains(teamId.ToString(), ex.Message);
        }

        #endregion

        #region DemoteFromLeadAsync

        [Fact]
        public async Task DemoteFromLeadAsync_RemovesLeadStatus()
        {
            // Arrange
            var teamId = Guid.NewGuid();
            var userId = Guid.NewGuid();
            var demotedByUserId = Guid.NewGuid();
            var member = new TeamMemberBuilder()
                .WithTeamId(teamId)
                .WithUserId(userId)
                .AsTeamLead()
                .Build();

            _teamMemberRepository.SetupGet(new List<TeamMember> { member });
            _teamMemberRepository.Setup(r => r.UpdateAsync(It.IsAny<TeamMember>()))
                .ReturnsAsync((TeamMember m) => m);

            // Act
            var result = await _sut.DemoteFromLeadAsync(teamId, userId, demotedByUserId);

            // Assert
            Assert.False(result.IsTeamLead);
            Assert.Equal(demotedByUserId, result.LastUpdatedByUserId);
        }

        [Fact]
        public async Task DemoteFromLeadAsync_ThrowsWhenMemberNotFound()
        {
            // Arrange
            var teamId = Guid.NewGuid();
            var userId = Guid.NewGuid();
            var demotedByUserId = Guid.NewGuid();

            _teamMemberRepository.SetupGet(new List<TeamMember>());

            // Act & Assert
            var ex = await Assert.ThrowsAsync<InvalidOperationException>(() =>
                _sut.DemoteFromLeadAsync(teamId, userId, demotedByUserId));

            Assert.Contains(userId.ToString(), ex.Message);
            Assert.Contains(teamId.ToString(), ex.Message);
        }

        #endregion

        #region GetTeamLeadCountAsync

        [Fact]
        public async Task GetTeamLeadCountAsync_ReturnsCorrectCount()
        {
            // Arrange
            var teamId = Guid.NewGuid();
            var lead1 = new TeamMemberBuilder().WithTeamId(teamId).AsTeamLead().Build();
            var lead2 = new TeamMemberBuilder().WithTeamId(teamId).AsTeamLead().Build();
            var regularMember = new TeamMemberBuilder().WithTeamId(teamId).AsRegularMember().Build();

            var allMembers = new List<TeamMember> { lead1, lead2, regularMember };
            _teamMemberRepository.SetupGet(allMembers);

            // Act
            var result = await _sut.GetTeamLeadCountAsync(teamId);

            // Assert
            Assert.Equal(2, result);
        }

        [Fact]
        public async Task GetTeamLeadCountAsync_ReturnsZeroWhenNoLeads()
        {
            // Arrange
            var teamId = Guid.NewGuid();
            var regularMember = new TeamMemberBuilder().WithTeamId(teamId).AsRegularMember().Build();

            _teamMemberRepository.SetupGet(new List<TeamMember> { regularMember });

            // Act
            var result = await _sut.GetTeamLeadCountAsync(teamId);

            // Assert
            Assert.Equal(0, result);
        }

        #endregion
    }
}
