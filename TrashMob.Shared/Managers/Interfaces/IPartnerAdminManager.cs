namespace TrashMob.Shared.Managers.Interfaces
{
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using System.Threading;
    using System;
    using TrashMob.Models;

    public interface IPartnerAdminManager : IBaseManager<PartnerAdmin>
    {
        Task<IEnumerable<User>> GetAdminsForPartnerAsync(Guid partnerId, CancellationToken cancellationToken= default);

        Task<IEnumerable<Partner>> GetPartnersByUserIdAsync(Guid userId, CancellationToken cancellationToken = default);
    }
}
