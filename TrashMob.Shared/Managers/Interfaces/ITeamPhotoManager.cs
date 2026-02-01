namespace TrashMob.Shared.Managers.Interfaces
{
    using System;
    using System.Collections.Generic;
    using System.Threading;
    using System.Threading.Tasks;
    using TrashMob.Models;

    /// <summary>
    /// Defines operations for managing team photos.
    /// </summary>
    public interface ITeamPhotoManager : IKeyedManager<TeamPhoto>
    {
        /// <summary>
        /// Gets all photos for a team.
        /// </summary>
        /// <param name="teamId">The team identifier.</param>
        /// <param name="cancellationToken">A token to cancel the asynchronous operation.</param>
        /// <returns>A collection of team photos.</returns>
        Task<IEnumerable<TeamPhoto>> GetByTeamIdAsync(Guid teamId, CancellationToken cancellationToken = default);

        /// <summary>
        /// Permanently deletes a team photo from the system including blob storage.
        /// </summary>
        /// <param name="id">The unique identifier of the team photo.</param>
        /// <param name="cancellationToken">A token to cancel the asynchronous operation.</param>
        /// <returns>The number of entities deleted.</returns>
        Task<int> HardDeleteAsync(Guid id, CancellationToken cancellationToken = default);

        /// <summary>
        /// Adds a new team photo.
        /// </summary>
        /// <param name="teamPhoto">The team photo entity to add.</param>
        /// <param name="userId">The user performing the operation.</param>
        /// <param name="cancellationToken">A token to cancel the asynchronous operation.</param>
        /// <returns>The created team photo with updated ImageUrl.</returns>
        new Task<TeamPhoto> AddAsync(TeamPhoto teamPhoto, Guid userId, CancellationToken cancellationToken = default);
    }
}
