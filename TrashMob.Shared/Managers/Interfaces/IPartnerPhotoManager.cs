namespace TrashMob.Shared.Managers.Interfaces
{
    using System;
    using System.Collections.Generic;
    using System.Threading;
    using System.Threading.Tasks;
    using TrashMob.Models;

    /// <summary>
    /// Manager interface for partner/community photo operations.
    /// </summary>
    public interface IPartnerPhotoManager : IKeyedManager<PartnerPhoto>
    {
        /// <summary>
        /// Gets all photos for a partner.
        /// </summary>
        /// <param name="partnerId">The partner identifier.</param>
        /// <param name="cancellationToken">Cancellation token.</param>
        /// <returns>Collection of partner photos.</returns>
        Task<IEnumerable<PartnerPhoto>> GetByPartnerIdAsync(Guid partnerId, CancellationToken cancellationToken = default);

        /// <summary>
        /// Permanently deletes a photo from the database and blob storage.
        /// </summary>
        /// <param name="id">The photo identifier.</param>
        /// <param name="cancellationToken">Cancellation token.</param>
        /// <returns>Number of records deleted.</returns>
        Task<int> HardDeleteAsync(Guid id, CancellationToken cancellationToken = default);

        /// <summary>
        /// Adds a new partner photo.
        /// </summary>
        /// <param name="partnerPhoto">The photo to add.</param>
        /// <param name="userId">The user performing the action.</param>
        /// <param name="cancellationToken">Cancellation token.</param>
        /// <returns>The created photo.</returns>
        new Task<PartnerPhoto> AddAsync(PartnerPhoto partnerPhoto, Guid userId, CancellationToken cancellationToken = default);
    }
}
