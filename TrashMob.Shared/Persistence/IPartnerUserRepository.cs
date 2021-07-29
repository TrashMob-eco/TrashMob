namespace TrashMob.Shared.Persistence
{
    using System;
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using TrashMob.Shared.Models;

    public interface IPartnerUserRepository
    {
        Task<IEnumerable<PartnerUser>> GetPartnerUsers(Guid partnerId);

        Task<Partner> GetPartnerUser(Guid partnerId, Guid userId);

        Task AddPartnerUser(PartnerUser partnerUser);

        Task<PartnerUser> UpdatePartnerUser(PartnerUser partnerUser);

        Task<int> DeletePartnerUser(Guid partnerId, Guid userId);
    }
}
