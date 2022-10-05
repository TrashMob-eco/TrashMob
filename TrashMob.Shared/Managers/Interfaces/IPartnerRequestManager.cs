namespace TrashMob.Shared.Managers.Interfaces
{
    using System.Threading;
    using System;
    using System.Threading.Tasks;
    using TrashMob.Models;

    public interface IPartnerRequestManager : IKeyedManager<PartnerRequest>
    {
        Task<PartnerRequest> ApproveBecomeAPartner(Guid partnerRequestId, CancellationToken cancellationToken);

        Task<PartnerRequest> DenyBecomeAPartner(Guid partnerRequestId, CancellationToken cancellationToken);
    }
}
