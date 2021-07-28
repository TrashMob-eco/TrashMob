namespace TrashMob.Shared.Persistence
{
    using System;
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using TrashMob.Shared.Models;

    public interface IPartnerRequestRepository
    {
        Task<IEnumerable<PartnerRequest>> GetPartnerRequests();

        Task<PartnerRequest> GetPartnerRequest(Guid id);

        Task AddPartnerRequest(PartnerRequest partnerRequest);

        Task<PartnerRequest> UpdatePartnerRequest(PartnerRequest partnerRequest);
    }
}
