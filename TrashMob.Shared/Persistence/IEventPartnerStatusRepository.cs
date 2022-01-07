namespace TrashMob.Shared.Persistence
{
    using System.Collections.Generic;
    using System.Threading;
    using System.Threading.Tasks;
    using TrashMob.Shared.Models;

    public interface IEventPartnerStatusRepository
    {
        Task<IEnumerable<EventPartnerStatus>> GetAllEventPartnerStatuses(CancellationToken cancellationToken = default);
    }
}
