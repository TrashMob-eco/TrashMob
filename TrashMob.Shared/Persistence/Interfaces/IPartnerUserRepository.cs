namespace TrashMob.Shared.Persistence.Interfaces
{
    using System;
    using System.Linq;
    using System.Threading;
    using System.Threading.Tasks;
    using TrashMob.Shared.Models;

    public interface IPartnerUserRepository
    {
        IQueryable<PartnerUser> GetPartnerUsers(CancellationToken cancellationToken = default);

        Task<PartnerUser> AddPartnerUser(PartnerUser partnerUser);

        Task<PartnerUser> UpdatePartnerUser(PartnerUser partnerUser);

        Task<int> DeletePartnerUser(Guid partnerId, Guid userId);
    }
}
