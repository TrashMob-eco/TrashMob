namespace TrashMob.Shared.Persistence
{
    using System;
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using TrashMob.Shared.Models;

    public interface IPartnerRepository
    {
        Task<IEnumerable<Partner>> GetPartners();

        Task<Partner> GetPartner(Guid id);

        Task<Partner> AddPartner(Partner partner);

        Task<Partner> UpdatePartner(Partner partner);
    }
}
