namespace TrashMob.Shared.Managers.Interfaces
{
    using System;
    using System.Threading;
    using System.Threading.Tasks;
    using TrashMob.Models;
    using TrashMob.Models.Poco;

    /// <summary>
    /// Defines operations for managing litter images.
    /// </summary>
    public interface ILitterImageManager : IKeyedManager<LitterImage>
    {
        /// <summary>
        /// Deletes a litter image by marking it as deleted.
        /// </summary>
        /// <param name="id">The unique identifier of the litter image.</param>
        /// <param name="userId">The unique identifier of the user performing the deletion.</param>
        /// <param name="cancellationToken">A token to cancel the asynchronous operation.</param>
        /// <returns>The number of entities deleted.</returns>
        Task<int> DeleteAsync(Guid id, Guid userId, CancellationToken cancellationToken = default);

        /// <summary>
        /// Permanently deletes a litter image from the system.
        /// </summary>
        /// <param name="id">The unique identifier of the litter image.</param>
        /// <param name="cancellationToken">A token to cancel the asynchronous operation.</param>
        /// <returns>The number of entities deleted.</returns>
        Task<int> HardDeleteAsync(Guid id, CancellationToken cancellationToken = default);

        /// <summary>
        /// Adds a new litter image with full image data.
        /// </summary>
        /// <param name="instance">The full litter image data including the image content.</param>
        /// <param name="userId">The unique identifier of the user adding the image.</param>
        /// <param name="cancellationToken">A token to cancel the asynchronous operation.</param>
        /// <returns>The created litter image.</returns>
        Task<LitterImage> AddAsync(FullLitterImage instance, Guid userId,
            CancellationToken cancellationToken = default);
    }
}
