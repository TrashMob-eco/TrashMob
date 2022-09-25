namespace TrashMob.Shared.Persistence.Interfaces
{
    using System;
    using System.Linq;
    using System.Threading;
    using System.Threading.Tasks;
    using TrashMob.Models;

    public interface IPartnerLocationRepository
    {
        IQueryable<PartnerLocation> GetPartnerLocations(CancellationToken cancellationToken = default);

        Task<PartnerLocation> AddPartnerLocation(PartnerLocation partnerLocation);

        Task<PartnerLocation> UpdatePartnerLocation(PartnerLocation partnerLocation);

        Task<int> DeletePartnerLocation(Guid partnerLocationId);
    }
}
