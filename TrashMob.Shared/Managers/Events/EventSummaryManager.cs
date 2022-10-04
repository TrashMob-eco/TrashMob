namespace TrashMob.Shared.Managers.Events
{
    using Microsoft.EntityFrameworkCore;
    using System;
    using System.Threading;
    using System.Threading.Tasks;
    using TrashMob.Models;
    using TrashMob.Shared.Managers.Interfaces;
    using TrashMob.Shared.Persistence.Interfaces;

    public class EventSummaryManager : BaseManager<EventSummary>, IBaseManager<EventSummary>
    {
        public EventSummaryManager(IBaseRepository<EventSummary> repository) : base(repository)
        {
        }

        public override async Task<int> Delete(Guid parentId, CancellationToken cancellationToken)
        {
            var eventSummary = await Repository.Get(es => es.EventId == parentId).FirstOrDefaultAsync(cancellationToken);
            return await Repository.Delete(eventSummary);
        }
    }
}
