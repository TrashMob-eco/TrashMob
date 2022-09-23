namespace TrashMob.Shared.Persistence.Interfaces
{
    using System;
    using System.Collections.Generic;
    using System.Threading;
    using System.Threading.Tasks;
    using TrashMob.Shared.Models;

    public interface IPartnerRequestRepository
    {
        Task<IEnumerable<PartnerRequest>> GetPartnerRequests(CancellationToken cancellationToken = default);

        Task<PartnerRequest> GetPartnerRequest(Guid id, CancellationToken cancellationToken = default);

        Task<PartnerRequest> AddPartnerRequest(PartnerRequest partnerRequest);

        Task<PartnerRequest> UpdatePartnerRequest(PartnerRequest partnerRequest);
    }
}
