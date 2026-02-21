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

    public class TeamManagerTests
    {
        private readonly Mock<IKeyedRepository<Team>> _teamRepository;
        private readonly Mock<IKeyedRepository<TeamMember>> _teamMemberRepository;
        private readonly TeamManager _sut;

        public TeamManagerTests()
        {
            _teamRepository = new Mock<IKeyedRepository<Team>>();
            _teamMemberRepository = new Mock<IKeyedRepository<TeamMember>>();

            _sut = new TeamManager(
                _teamRepository.Object,
                _teamMemberRepository.Object);
        }

        #region GetPublicTeamsAsync

        [Fact]
        public async Task GetPublicTeamsAsync_ReturnsOnlyPublicActiveTeams()
        {
            // Arrange
            var publicTeam = new TeamBuilder()
                .WithName("Public Team")
                .AsPublic()
                .AsActive()
                .Build();
            var privateTeam = new TeamBuilder()
                .WithName("Private Team")
                .AsPrivate()
                .AsActive()
                .Build();
            var inactiveTeam = new TeamBuilder()
                .WithName("Inactive Team")
                .AsPublic()
                .AsInactive()
                .Build();

            var teams = new List<Team> { publicTeam, privateTeam, inactiveTeam };
            _teamRepository.SetupGet(teams);

            // Act
            var result = await _sut.GetPublicTeamsAsync();

            // Assert
            var resultList = result.ToList();
            Assert.Single(resultList);
            Assert.Equal(publicTeam.Id, resultList[0].Id);
        }

        [Fact]
        public async Task GetPublicTeamsAsync_WithLocationFilter_ReturnsTeamsWithinRadius()
        {
            // Arrange
            // Seattle coordinates: 47.6062, -122.3321
            var seattleTeam = new TeamBuilder()
                .WithName("Seattle Team")
                .AsPublic()
                .AsActive()
                .WithCoordinates(47.6062, -122.3321)
                .Build();

            // Portland coordinates: 45.5152, -122.6784 (~145 miles from Seattle)
            var portlandTeam = new TeamBuilder()
                .WithName("Portland Team")
                .AsPublic()
                .AsActive()
                .WithCoordinates(45.5152, -122.6784)
                .Build();

            // Tacoma coordinates: 47.2529, -122.4443 (~30 miles from Seattle)
            var tacomaTeam = new TeamBuilder()
                .WithName("Tacoma Team")
                .AsPublic()
                .AsActive()
                .WithCoordinates(47.2529, -122.4443)
                .Build();

            var teams = new List<Team> { seattleTeam, portlandTeam, tacomaTeam };
            _teamRepository.SetupGet(teams);

            // Act - 50 mile radius from Seattle
            var result = await _sut.GetPublicTeamsAsync(47.6062, -122.3321, 50);

            // Assert
            var resultList = result.ToList();
            Assert.Equal(2, resultList.Count);
            Assert.Contains(resultList, t => t.Name == "Seattle Team");
            Assert.Contains(resultList, t => t.Name == "Tacoma Team");
            Assert.DoesNotContain(resultList, t => t.Name == "Portland Team");
        }

        [Fact]
        public async Task GetPublicTeamsAsync_WithLocationFilter_ExcludesTeamsWithoutCoordinates()
        {
            // Arrange
            var teamWithCoords = new TeamBuilder()
                .WithName("Team With Coords")
                .AsPublic()
                .AsActive()
                .WithCoordinates(47.6062, -122.3321)
                .Build();

            var teamWithoutCoords = new TeamBuilder()
                .WithName("Team Without Coords")
                .AsPublic()
                .AsActive()
                .Build();
            // Ensure no coordinates
            teamWithoutCoords.Latitude = null;
            teamWithoutCoords.Longitude = null;

            var teams = new List<Team> { teamWithCoords, teamWithoutCoords };
            _teamRepository.SetupGet(teams);

            // Act
            var result = await _sut.GetPublicTeamsAsync(47.6062, -122.3321, 100);

            // Assert
            var resultList = result.ToList();
            Assert.Single(resultList);
            Assert.Equal("Team With Coords", resultList[0].Name);
        }

        [Fact]
        public async Task GetPublicTeamsAsync_WithNoLocationFilter_ReturnsAllPublicTeams()
        {
            // Arrange
            var team1 = new TeamBuilder().WithName("Team 1").AsPublic().AsActive().Build();
            var team2 = new TeamBuilder().WithName("Team 2").AsPublic().AsActive().Build();

            var teams = new List<Team> { team1, team2 };
            _teamRepository.SetupGet(teams);

            // Act
            var result = await _sut.GetPublicTeamsAsync();

            // Assert
            Assert.Equal(2, result.Count());
        }

        #endregion

        #region GetTeamsByUserAsync

        [Fact]
        public async Task GetTeamsByUserAsync_ReturnsActiveTeamsUserBelongsTo()
        {
            // Arrange
            var userId = Guid.NewGuid();
            var team1 = new TeamBuilder().WithName("Team 1").AsActive().Build();
            var team2 = new TeamBuilder().WithName("Team 2").AsActive().Build();
            var inactiveTeam = new TeamBuilder().WithName("Inactive").AsInactive().Build();

            var membership1 = new TeamMemberBuilder()
                .WithTeamId(team1.Id)
                .WithUserId(userId)
                .Build();
            var membership2 = new TeamMemberBuilder()
                .WithTeamId(team2.Id)
                .WithUserId(userId)
                .Build();
            var inactiveMembership = new TeamMemberBuilder()
                .WithTeamId(inactiveTeam.Id)
                .WithUserId(userId)
                .Build();

            var teams = new List<Team> { team1, team2, inactiveTeam };
            var memberships = new List<TeamMember> { membership1, membership2, inactiveMembership };

            _teamRepository.SetupGet(teams);
            _teamMemberRepository.SetupGet(memberships);

            // Act
            var result = await _sut.GetTeamsByUserAsync(userId);

            // Assert
            var resultList = result.ToList();
            Assert.Equal(2, resultList.Count);
            Assert.Contains(resultList, t => t.Name == "Team 1");
            Assert.Contains(resultList, t => t.Name == "Team 2");
            Assert.DoesNotContain(resultList, t => t.Name == "Inactive");
        }

        [Fact]
        public async Task GetTeamsByUserAsync_ReturnsEmptyWhenUserHasNoTeams()
        {
            // Arrange
            var userId = Guid.NewGuid();
            var otherUserId = Guid.NewGuid();

            var team = new TeamBuilder().WithName("Other Team").AsActive().Build();
            var membership = new TeamMemberBuilder()
                .WithTeamId(team.Id)
                .WithUserId(otherUserId)
                .Build();

            _teamRepository.SetupGet(new List<Team> { team });
            _teamMemberRepository.SetupGet(new List<TeamMember> { membership });

            // Act
            var result = await _sut.GetTeamsByUserAsync(userId);

            // Assert
            Assert.Empty(result);
        }

        #endregion

        #region GetTeamsUserLeadsAsync

        [Fact]
        public async Task GetTeamsUserLeadsAsync_ReturnsOnlyTeamsWhereUserIsLead()
        {
            // Arrange
            var userId = Guid.NewGuid();
            var leadTeam = new TeamBuilder().WithName("Lead Team").AsActive().Build();
            var memberTeam = new TeamBuilder().WithName("Member Team").AsActive().Build();

            var leadMembership = new TeamMemberBuilder()
                .WithTeamId(leadTeam.Id)
                .WithUserId(userId)
                .AsTeamLead()
                .Build();
            var memberMembership = new TeamMemberBuilder()
                .WithTeamId(memberTeam.Id)
                .WithUserId(userId)
                .AsRegularMember()
                .Build();

            var teams = new List<Team> { leadTeam, memberTeam };
            var memberships = new List<TeamMember> { leadMembership, memberMembership };

            _teamRepository.SetupGet(teams);
            _teamMemberRepository.SetupGet(memberships);

            // Act
            var result = await _sut.GetTeamsUserLeadsAsync(userId);

            // Assert
            var resultList = result.ToList();
            Assert.Single(resultList);
            Assert.Equal("Lead Team", resultList[0].Name);
        }

        [Fact]
        public async Task GetTeamsUserLeadsAsync_ExcludesInactiveTeams()
        {
            // Arrange
            var userId = Guid.NewGuid();
            var activeTeam = new TeamBuilder().WithName("Active Team").AsActive().Build();
            var inactiveTeam = new TeamBuilder().WithName("Inactive Team").AsInactive().Build();

            var activeMembership = new TeamMemberBuilder()
                .WithTeamId(activeTeam.Id)
                .WithUserId(userId)
                .AsTeamLead()
                .Build();
            var inactiveMembership = new TeamMemberBuilder()
                .WithTeamId(inactiveTeam.Id)
                .WithUserId(userId)
                .AsTeamLead()
                .Build();

            var teams = new List<Team> { activeTeam, inactiveTeam };
            var memberships = new List<TeamMember> { activeMembership, inactiveMembership };

            _teamRepository.SetupGet(teams);
            _teamMemberRepository.SetupGet(memberships);

            // Act
            var result = await _sut.GetTeamsUserLeadsAsync(userId);

            // Assert
            var resultList = result.ToList();
            Assert.Single(resultList);
            Assert.Equal("Active Team", resultList[0].Name);
        }

        #endregion

        #region IsTeamNameAvailableAsync

        [Fact]
        public async Task IsTeamNameAvailableAsync_ReturnsTrueWhenNameIsUnique()
        {
            // Arrange
            var existingTeam = new TeamBuilder().WithName("Existing Team").Build();
            _teamRepository.SetupGet(new List<Team> { existingTeam });

            // Act
            var result = await _sut.IsTeamNameAvailableAsync("New Team");

            // Assert
            Assert.True(result);
        }

        [Fact]
        public async Task IsTeamNameAvailableAsync_ReturnsFalseWhenNameExists()
        {
            // Arrange
            var existingTeam = new TeamBuilder().WithName("Existing Team").Build();
            _teamRepository.SetupGet(new List<Team> { existingTeam });

            // Act
            var result = await _sut.IsTeamNameAvailableAsync("Existing Team");

            // Assert
            Assert.False(result);
        }

        [Fact]
        public async Task IsTeamNameAvailableAsync_IsCaseInsensitive()
        {
            // Arrange
            var existingTeam = new TeamBuilder().WithName("Existing Team").Build();
            _teamRepository.SetupGet(new List<Team> { existingTeam });

            // Act
            var result = await _sut.IsTeamNameAvailableAsync("EXISTING TEAM");

            // Assert
            Assert.False(result);
        }

        [Fact]
        public async Task IsTeamNameAvailableAsync_ReturnsTrueWhenExcludingOwnId()
        {
            // Arrange
            var existingTeam = new TeamBuilder().WithName("My Team").Build();
            _teamRepository.SetupGet(new List<Team> { existingTeam });

            // Act - Check if name is available while updating the same team
            var result = await _sut.IsTeamNameAvailableAsync("My Team", existingTeam.Id);

            // Assert
            Assert.True(result);
        }

        [Fact]
        public async Task IsTeamNameAvailableAsync_TrimsWhitespace()
        {
            // Arrange
            var existingTeam = new TeamBuilder().WithName("Test Team").Build();
            _teamRepository.SetupGet(new List<Team> { existingTeam });

            // Act
            var result = await _sut.IsTeamNameAvailableAsync("  Test Team  ");

            // Assert
            Assert.False(result);
        }

        #endregion

        #region GetByNameAsync

        [Fact]
        public async Task GetByNameAsync_ReturnsTeamWhenFound()
        {
            // Arrange
            var team = new TeamBuilder().WithName("Find Me").Build();
            _teamRepository.SetupGet(new List<Team> { team });

            // Act
            var result = await _sut.GetByNameAsync("Find Me");

            // Assert
            Assert.NotNull(result);
            Assert.Equal(team.Id, result.Id);
        }

        [Fact]
        public async Task GetByNameAsync_ReturnsNullWhenNotFound()
        {
            // Arrange
            var team = new TeamBuilder().WithName("Other Team").Build();
            _teamRepository.SetupGet(new List<Team> { team });

            // Act
            var result = await _sut.GetByNameAsync("Not Found");

            // Assert
            Assert.Null(result);
        }

        [Fact]
        public async Task GetByNameAsync_IsCaseInsensitive()
        {
            // Arrange
            var team = new TeamBuilder().WithName("Test Team").Build();
            _teamRepository.SetupGet(new List<Team> { team });

            // Act
            var result = await _sut.GetByNameAsync("TEST TEAM");

            // Assert
            Assert.NotNull(result);
            Assert.Equal(team.Id, result.Id);
        }

        #endregion
    }
}
