namespace TrashMob.Shared.Managers.Interfaces
{
    using System;
    using System.Threading;
    using System.Threading.Tasks;
    using TrashMob.Models;

    /// <summary>
    /// Defines operations for managing partner requests.
    /// </summary>
    public interface IPartnerRequestManager : IKeyedManager<PartnerRequest>
    {
        /// <summary>
        /// Approves a request to become a partner.
        /// </summary>
        /// <param name="partnerRequestId">The unique identifier of the partner request.</param>
        /// <param name="userId">The unique identifier of the user approving the request.</param>
        /// <param name="cancellationToken">A token to cancel the asynchronous operation.</param>
        /// <returns>The approved partner request.</returns>
        Task<PartnerRequest> ApproveBecomeAPartnerAsync(Guid partnerRequestId, Guid userId,
            CancellationToken cancellationToken);

        /// <summary>
        /// Denies a request to become a partner.
        /// </summary>
        /// <param name="partnerRequestId">The unique identifier of the partner request.</param>
        /// <param name="userId">The unique identifier of the user denying the request.</param>
        /// <param name="cancellationToken">A token to cancel the asynchronous operation.</param>
        /// <returns>The denied partner request.</returns>
        Task<PartnerRequest> DenyBecomeAPartnerAsync(Guid partnerRequestId, Guid userId,
            CancellationToken cancellationToken);
    }
}
