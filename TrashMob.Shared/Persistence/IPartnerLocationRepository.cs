namespace TrashMob.Shared.Persistence
{
    using System;
    using System.Linq;
    using System.Threading.Tasks;
    using TrashMob.Shared.Models;

    public interface IPartnerLocationRepository
    {
        IQueryable<PartnerLocation> GetPartnerLocations();

        Task<PartnerLocation> AddPartnerLocation(PartnerLocation partnerLocation);

        Task<PartnerLocation> UpdatePartnerLocation(PartnerLocation partnerLocation);

        Task<int> DeletePartnerLocation(Guid partnerLocationId);
    }
}
