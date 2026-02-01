namespace TrashMob.Shared.Managers.Teams
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading;
    using System.Threading.Tasks;
    using Microsoft.EntityFrameworkCore;
    using TrashMob.Models;
    using TrashMob.Shared.Managers.Interfaces;
    using TrashMob.Shared.Persistence.Interfaces;

    /// <summary>
    /// Manager for team membership operations.
    /// </summary>
    public class TeamMemberManager : KeyedManager<TeamMember>, ITeamMemberManager
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="TeamMemberManager"/> class.
        /// </summary>
        /// <param name="repository">The team member repository.</param>
        public TeamMemberManager(IKeyedRepository<TeamMember> repository)
            : base(repository)
        {
        }

        /// <inheritdoc />
        public async Task<IEnumerable<TeamMember>> GetByTeamIdAsync(Guid teamId, CancellationToken cancellationToken = default)
        {
            return await Repo.Get()
                .Where(tm => tm.TeamId == teamId)
                .Include(tm => tm.User)
                .ToListAsync(cancellationToken);
        }

        /// <inheritdoc />
        public async Task<IEnumerable<TeamMember>> GetTeamLeadsAsync(Guid teamId, CancellationToken cancellationToken = default)
        {
            return await Repo.Get()
                .Where(tm => tm.TeamId == teamId && tm.IsTeamLead)
                .Include(tm => tm.User)
                .ToListAsync(cancellationToken);
        }

        /// <inheritdoc />
        public async Task<TeamMember> GetByTeamAndUserAsync(Guid teamId, Guid userId, CancellationToken cancellationToken = default)
        {
            return await Repo.Get()
                .FirstOrDefaultAsync(tm => tm.TeamId == teamId && tm.UserId == userId, cancellationToken);
        }

        /// <inheritdoc />
        public async Task<bool> IsMemberAsync(Guid teamId, Guid userId, CancellationToken cancellationToken = default)
        {
            return await Repo.Get()
                .AnyAsync(tm => tm.TeamId == teamId && tm.UserId == userId, cancellationToken);
        }

        /// <inheritdoc />
        public async Task<bool> IsTeamLeadAsync(Guid teamId, Guid userId, CancellationToken cancellationToken = default)
        {
            return await Repo.Get()
                .AnyAsync(tm => tm.TeamId == teamId && tm.UserId == userId && tm.IsTeamLead, cancellationToken);
        }

        /// <inheritdoc />
        public async Task<TeamMember> AddMemberAsync(Guid teamId, Guid userId, bool isTeamLead, Guid addedByUserId, CancellationToken cancellationToken = default)
        {
            var member = new TeamMember
            {
                Id = Guid.NewGuid(),
                TeamId = teamId,
                UserId = userId,
                IsTeamLead = isTeamLead,
                JoinedDate = DateTimeOffset.UtcNow,
                CreatedByUserId = addedByUserId,
                CreatedDate = DateTimeOffset.UtcNow,
                LastUpdatedByUserId = addedByUserId,
                LastUpdatedDate = DateTimeOffset.UtcNow,
            };

            return await Repo.AddAsync(member);
        }

        /// <inheritdoc />
        public async Task<int> RemoveMemberAsync(Guid teamId, Guid userId, CancellationToken cancellationToken = default)
        {
            var member = await GetByTeamAndUserAsync(teamId, userId, cancellationToken);
            if (member == null)
            {
                return 0;
            }

            return await Repo.DeleteAsync(member.Id);
        }

        /// <inheritdoc />
        public async Task<TeamMember> PromoteToLeadAsync(Guid teamId, Guid userId, Guid promotedByUserId, CancellationToken cancellationToken = default)
        {
            var member = await GetByTeamAndUserAsync(teamId, userId, cancellationToken);
            if (member == null)
            {
                throw new InvalidOperationException($"User {userId} is not a member of team {teamId}");
            }

            member.IsTeamLead = true;
            member.LastUpdatedByUserId = promotedByUserId;
            member.LastUpdatedDate = DateTimeOffset.UtcNow;

            return await Repo.UpdateAsync(member);
        }

        /// <inheritdoc />
        public async Task<TeamMember> DemoteFromLeadAsync(Guid teamId, Guid userId, Guid demotedByUserId, CancellationToken cancellationToken = default)
        {
            var member = await GetByTeamAndUserAsync(teamId, userId, cancellationToken);
            if (member == null)
            {
                throw new InvalidOperationException($"User {userId} is not a member of team {teamId}");
            }

            member.IsTeamLead = false;
            member.LastUpdatedByUserId = demotedByUserId;
            member.LastUpdatedDate = DateTimeOffset.UtcNow;

            return await Repo.UpdateAsync(member);
        }

        /// <inheritdoc />
        public async Task<int> GetTeamLeadCountAsync(Guid teamId, CancellationToken cancellationToken = default)
        {
            return await Repo.Get()
                .CountAsync(tm => tm.TeamId == teamId && tm.IsTeamLead, cancellationToken);
        }
    }
}
