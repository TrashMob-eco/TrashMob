namespace TrashMob.Shared.Managers.Interfaces
{
    using System;
    using System.Threading;
    using System.Threading.Tasks;
    using TrashMob.Models;

    public interface IPartnerRequestManager : IKeyedManager<PartnerRequest>
    {
        Task<PartnerRequest> ApproveBecomeAPartnerAsync(Guid partnerRequestId, Guid userId,
            CancellationToken cancellationToken);

        Task<PartnerRequest> DenyBecomeAPartnerAsync(Guid partnerRequestId, Guid userId,
            CancellationToken cancellationToken);
    }
}