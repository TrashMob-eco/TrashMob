namespace TrashMob.Shared.Persistence
{
    using System.Threading.Tasks;
    using TrashMob.Shared.Models;

    public interface IPartnerRequestRepository
    {
        Task AddPartnerRequest(PartnerRequest partnerRequest);

        Task<PartnerRequest> UpdatePartnerRequest(PartnerRequest partnerRequest);
    }
}
