namespace TrashMob.Shared.Managers.Events
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading;
    using System.Threading.Tasks;
    using Microsoft.EntityFrameworkCore;
    using TrashMob.Models;
    using TrashMob.Shared.Managers.Interfaces;
    using TrashMob.Shared.Persistence.Interfaces;

    public class EventAttendeeRouteManager : KeyedManager<EventAttendeeRoute>, IBaseManager<EventAttendeeRoute>,
        IEventAttendeeRouteManager
    {
        public EventAttendeeRouteManager(IKeyedRepository<EventAttendeeRoute> eventAttendeeRouteRepository) : base(
            eventAttendeeRouteRepository)
        {
        }

        public override async Task<IEnumerable<EventAttendeeRoute>> GetByParentIdAsync(Guid parentId,
            CancellationToken cancellationToken)
        {
            return (await Repository.Get().Where(p => p.EventId == parentId)
                    .Include(p => p.User)
                    .ToListAsync(cancellationToken))
                .AsEnumerable();
        }
    }
}