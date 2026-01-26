namespace TrashMob.Shared.Managers.Interfaces
{
    using System;
    using System.Collections.Generic;
    using System.Threading;
    using System.Threading.Tasks;
    using TrashMob.Models;

    /// <summary>
    /// Defines operations for managing partner administrators.
    /// </summary>
    public interface IPartnerAdminManager : IBaseManager<PartnerAdmin>
    {
        /// <summary>
        /// Gets all administrator users for a specific partner.
        /// </summary>
        /// <param name="partnerId">The unique identifier of the partner.</param>
        /// <param name="cancellationToken">A token to cancel the asynchronous operation.</param>
        /// <returns>A collection of users who are administrators for the partner.</returns>
        Task<IEnumerable<User>> GetAdminsForPartnerAsync(Guid partnerId, CancellationToken cancellationToken = default);

        /// <summary>
        /// Gets all partners that a user is an administrator of.
        /// </summary>
        /// <param name="userId">The unique identifier of the user.</param>
        /// <param name="cancellationToken">A token to cancel the asynchronous operation.</param>
        /// <returns>A collection of partners the user administers.</returns>
        Task<IEnumerable<Partner>> GetPartnersByUserIdAsync(Guid userId, CancellationToken cancellationToken = default);

        /// <summary>
        /// Gets all hauling partner locations that a user administers.
        /// </summary>
        /// <param name="userId">The unique identifier of the user.</param>
        /// <param name="cancellationToken">A token to cancel the asynchronous operation.</param>
        /// <returns>A collection of hauling partner locations the user administers.</returns>
        Task<IEnumerable<PartnerLocation>> GetHaulingPartnerLocationsByUserIdAsync(Guid userId,
            CancellationToken cancellationToken = default);
    }
}
