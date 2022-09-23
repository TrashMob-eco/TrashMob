namespace TrashMob.Shared.Persistence.Interfaces
{
    using System.Collections.Generic;
    using System.Threading;
    using System.Threading.Tasks;
    using TrashMob.Shared.Models;

    public interface IPartnerStatusRepository
    {
        Task<IEnumerable<PartnerStatus>> GetAllPartnerStatuses(CancellationToken cancellationToken = default);
    }
}
