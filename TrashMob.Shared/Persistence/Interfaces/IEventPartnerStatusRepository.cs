namespace TrashMob.Shared.Persistence.Interfaces
{
    using System.Collections.Generic;
    using System.Threading;
    using System.Threading.Tasks;
    using TrashMob.Models;

    public interface IEventPartnerStatusRepository
    {
        Task<IEnumerable<EventPartnerStatus>> GetAllEventPartnerStatuses(CancellationToken cancellationToken = default);
    }
}
