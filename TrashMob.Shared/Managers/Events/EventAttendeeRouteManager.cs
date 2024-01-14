namespace TrashMob.Shared.Managers.Events
{
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using System.Threading;
    using TrashMob.Models;
    using TrashMob.Shared.Managers.Interfaces;
    using TrashMob.Shared.Persistence.Interfaces;
    using System;
    using Microsoft.EntityFrameworkCore;
    using System.Linq;

    public class EventAttendeeRouteManager : KeyedManager<EventAttendeeRoute>, IBaseManager<EventAttendeeRoute>, IEventAttendeeRouteManager
    {
        public EventAttendeeRouteManager(IKeyedRepository<EventAttendeeRoute> eventAttendeeRouteRepository) : base(eventAttendeeRouteRepository)
        {
        }

        public override async Task<IEnumerable<EventAttendeeRoute>> GetByParentIdAsync(Guid parentId, CancellationToken cancellationToken)
        {
            return (await Repository.Get().Where(p => p.EventId == parentId)
                                          .Include(p => p.User)
                                          .ToListAsync(cancellationToken))
                                          .AsEnumerable();
        }
    }
}
