namespace TrashMob.Shared.Persistence.Interfaces
{
    using System;
    using System.Collections.Generic;
    using System.Threading;
    using System.Threading.Tasks;
    using TrashMob.Shared.Models;

    public interface IPartnerRepository
    {
        Task<IEnumerable<Partner>> GetPartners(CancellationToken cancellationToken = default);

        Task<Partner> GetPartner(Guid id, CancellationToken cancellationToken = default);

        Task<Partner> AddPartner(Partner partner);

        Task<Partner> UpdatePartner(Partner partner);
    }
}
