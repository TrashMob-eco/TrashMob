namespace TrashMob.Shared.Managers.Events
{
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using System.Threading;
    using System;
    using TrashMob.Models;
    using TrashMob.Shared.Managers.Interfaces;
    using TrashMob.Shared.Persistence;
    using TrashMob.Shared.Persistence.Interfaces;
    using Microsoft.EntityFrameworkCore;

    public class EventManager : KeyedManager<Event>, IKeyedManager<Event>, IEventManager
    {
        private readonly IEmailManager emailManager;
        private const int StandardEventWindowInMinutes = 120;

        public EventManager(IKeyedRepository<Event> repository, IEmailManager emailManager) : base(repository)
        {
            this.emailManager = emailManager;
        }

        public async Task<IEnumerable<Event>> GetActiveEventsAsync(CancellationToken cancellationToken = default)
        {
            return await Repo.Get(e => (e.EventStatusId == (int)EventStatusEnum.Active || e.EventStatusId == (int)EventStatusEnum.Full) 
                                        && e.IsEventPublic 
                                        && e.EventDate >= DateTimeOffset.UtcNow.AddMinutes(-1 * StandardEventWindowInMinutes))
                             .ToListAsync(cancellationToken).ConfigureAwait(false);
        }

        public async Task<IEnumerable<Event>> GetCompletedEventsAsync(CancellationToken cancellationToken = default)
        {
            return await Repo.Get(e => e.EventDate < DateTimeOffset.UtcNow 
                                  && e.EventStatusId != (int)EventStatusEnum.Canceled)
                             .ToListAsync(cancellationToken).ConfigureAwait(false);
        }

        public async Task<IEnumerable<Event>> GetUserEventsAsync(Guid userId, bool futureEventsOnly, CancellationToken cancellationToken = default)
        {
            return await Repo.Get(e => e.CreatedByUserId == userId
                                  && e.EventStatusId != (int)EventStatusEnum.Canceled
                                  && (!futureEventsOnly || e.EventDate >= DateTimeOffset.UtcNow))
                             .ToListAsync(cancellationToken).ConfigureAwait(false);
        }

        public async Task<IEnumerable<Event>> GetCanceledUserEventsAsync(Guid userId, bool futureEventsOnly, CancellationToken cancellationToken = default)
        {
            return await Repo.Get(e => e.CreatedByUserId == userId
                                  && e.EventStatusId == (int)EventStatusEnum.Canceled
                                  && (!futureEventsOnly || e.EventDate >= DateTimeOffset.UtcNow))
                             .ToListAsync(cancellationToken).ConfigureAwait(false);
        }

        // Delete the record of a particular Mob Event    
        public async Task<int> DeleteAsync(Guid id, string cancellationReason, CancellationToken cancellationToken)
        {
            var mobEvent = await Repo.GetAsync(id, cancellationToken).ConfigureAwait(false);
            mobEvent.EventStatusId = (int)EventStatusEnum.Canceled;
            mobEvent.CancellationReason = cancellationReason;
            await base.UpdateAsync(mobEvent, cancellationToken);
            return 1;
        }
    }
}
