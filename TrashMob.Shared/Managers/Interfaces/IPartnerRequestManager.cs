namespace TrashMob.Shared.Managers.Interfaces
{
    using System.Threading;
    using System;
    using System.Threading.Tasks;
    using TrashMob.Models;

    public interface IPartnerRequestManager : IKeyedManager<PartnerRequest>
    {
        Task<PartnerRequest> ApproveBecomeAPartnerAsync(Guid partnerRequestId, CancellationToken cancellationToken);

        Task<PartnerRequest> DenyBecomeAPartnerAsync(Guid partnerRequestId, CancellationToken cancellationToken);
    }
}
