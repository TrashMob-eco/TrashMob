namespace TrashMob.Shared.Tests.Builders
{
    using System;
    using TrashMob.Models;

    /// <summary>
    /// Builder for creating TeamMember test data with sensible defaults.
    /// </summary>
    public class TeamMemberBuilder
    {
        private readonly TeamMember _teamMember;

        public TeamMemberBuilder()
        {
            var creatorId = Guid.NewGuid();
            _teamMember = new TeamMember
            {
                Id = Guid.NewGuid(),
                TeamId = Guid.NewGuid(),
                UserId = Guid.NewGuid(),
                IsTeamLead = false,
                JoinedDate = DateTimeOffset.UtcNow.AddDays(-30),
                CreatedByUserId = creatorId,
                CreatedDate = DateTimeOffset.UtcNow.AddDays(-30),
                LastUpdatedByUserId = creatorId,
                LastUpdatedDate = DateTimeOffset.UtcNow
            };
        }

        public TeamMemberBuilder WithId(Guid id)
        {
            _teamMember.Id = id;
            return this;
        }

        public TeamMemberBuilder WithTeamId(Guid teamId)
        {
            _teamMember.TeamId = teamId;
            return this;
        }

        public TeamMemberBuilder WithUserId(Guid userId)
        {
            _teamMember.UserId = userId;
            return this;
        }

        public TeamMemberBuilder AsTeamLead()
        {
            _teamMember.IsTeamLead = true;
            return this;
        }

        public TeamMemberBuilder AsRegularMember()
        {
            _teamMember.IsTeamLead = false;
            return this;
        }

        public TeamMemberBuilder JoinedOn(DateTimeOffset joinedDate)
        {
            _teamMember.JoinedDate = joinedDate;
            return this;
        }

        public TeamMemberBuilder CreatedBy(Guid userId)
        {
            _teamMember.CreatedByUserId = userId;
            _teamMember.LastUpdatedByUserId = userId;
            return this;
        }

        public TeamMember Build() => _teamMember;
    }
}
