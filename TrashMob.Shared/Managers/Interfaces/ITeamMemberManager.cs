namespace TrashMob.Shared.Managers.Interfaces
{
    using System;
    using System.Collections.Generic;
    using System.Threading;
    using System.Threading.Tasks;
    using TrashMob.Models;

    /// <summary>
    /// Defines operations for managing team membership.
    /// </summary>
    public interface ITeamMemberManager : IKeyedManager<TeamMember>
    {
        /// <summary>
        /// Gets all members of a team.
        /// </summary>
        /// <param name="teamId">The unique identifier of the team.</param>
        /// <param name="cancellationToken">A token to cancel the asynchronous operation.</param>
        /// <returns>A collection of team members.</returns>
        Task<IEnumerable<TeamMember>> GetByTeamIdAsync(Guid teamId, CancellationToken cancellationToken = default);

        /// <summary>
        /// Gets all team leads for a team.
        /// </summary>
        /// <param name="teamId">The unique identifier of the team.</param>
        /// <param name="cancellationToken">A token to cancel the asynchronous operation.</param>
        /// <returns>A collection of team members who are leads.</returns>
        Task<IEnumerable<TeamMember>> GetTeamLeadsAsync(Guid teamId, CancellationToken cancellationToken = default);

        /// <summary>
        /// Gets a specific team membership.
        /// </summary>
        /// <param name="teamId">The unique identifier of the team.</param>
        /// <param name="userId">The unique identifier of the user.</param>
        /// <param name="cancellationToken">A token to cancel the asynchronous operation.</param>
        /// <returns>The team member or null if not found.</returns>
        Task<TeamMember> GetByTeamAndUserAsync(Guid teamId, Guid userId, CancellationToken cancellationToken = default);

        /// <summary>
        /// Checks if a user is a member of a team.
        /// </summary>
        /// <param name="teamId">The unique identifier of the team.</param>
        /// <param name="userId">The unique identifier of the user.</param>
        /// <param name="cancellationToken">A token to cancel the asynchronous operation.</param>
        /// <returns>True if the user is a member; otherwise, false.</returns>
        Task<bool> IsMemberAsync(Guid teamId, Guid userId, CancellationToken cancellationToken = default);

        /// <summary>
        /// Checks if a user is a team lead.
        /// </summary>
        /// <param name="teamId">The unique identifier of the team.</param>
        /// <param name="userId">The unique identifier of the user.</param>
        /// <param name="cancellationToken">A token to cancel the asynchronous operation.</param>
        /// <returns>True if the user is a team lead; otherwise, false.</returns>
        Task<bool> IsTeamLeadAsync(Guid teamId, Guid userId, CancellationToken cancellationToken = default);

        /// <summary>
        /// Adds a user to a team.
        /// </summary>
        /// <param name="teamId">The unique identifier of the team.</param>
        /// <param name="userId">The unique identifier of the user to add.</param>
        /// <param name="isTeamLead">Whether the user should be a team lead.</param>
        /// <param name="addedByUserId">The unique identifier of the user performing the action.</param>
        /// <param name="cancellationToken">A token to cancel the asynchronous operation.</param>
        /// <returns>The created team member.</returns>
        Task<TeamMember> AddMemberAsync(Guid teamId, Guid userId, bool isTeamLead, Guid addedByUserId, CancellationToken cancellationToken = default);

        /// <summary>
        /// Removes a user from a team.
        /// </summary>
        /// <param name="teamId">The unique identifier of the team.</param>
        /// <param name="userId">The unique identifier of the user to remove.</param>
        /// <param name="cancellationToken">A token to cancel the asynchronous operation.</param>
        /// <returns>The number of records affected.</returns>
        Task<int> RemoveMemberAsync(Guid teamId, Guid userId, CancellationToken cancellationToken = default);

        /// <summary>
        /// Promotes a team member to lead status.
        /// </summary>
        /// <param name="teamId">The unique identifier of the team.</param>
        /// <param name="userId">The unique identifier of the user to promote.</param>
        /// <param name="promotedByUserId">The unique identifier of the user performing the promotion.</param>
        /// <param name="cancellationToken">A token to cancel the asynchronous operation.</param>
        /// <returns>The updated team member.</returns>
        Task<TeamMember> PromoteToLeadAsync(Guid teamId, Guid userId, Guid promotedByUserId, CancellationToken cancellationToken = default);

        /// <summary>
        /// Demotes a team lead back to regular member status.
        /// </summary>
        /// <param name="teamId">The unique identifier of the team.</param>
        /// <param name="userId">The unique identifier of the user to demote.</param>
        /// <param name="demotedByUserId">The unique identifier of the user performing the demotion.</param>
        /// <param name="cancellationToken">A token to cancel the asynchronous operation.</param>
        /// <returns>The updated team member.</returns>
        Task<TeamMember> DemoteFromLeadAsync(Guid teamId, Guid userId, Guid demotedByUserId, CancellationToken cancellationToken = default);

        /// <summary>
        /// Gets the count of team leads for a team.
        /// </summary>
        /// <param name="teamId">The unique identifier of the team.</param>
        /// <param name="cancellationToken">A token to cancel the asynchronous operation.</param>
        /// <returns>The number of team leads.</returns>
        Task<int> GetTeamLeadCountAsync(Guid teamId, CancellationToken cancellationToken = default);
    }
}
