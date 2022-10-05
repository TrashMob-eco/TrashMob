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
    using Org.BouncyCastle.Bcpg.Sig;

    public class EventAttendeeManager : BaseManager<EventAttendee>, IBaseManager<EventAttendee>, IEventAttendeeManager
    {
        private readonly IKeyedRepository<Event> eventRepository;
        private readonly IEmailManager emailManager;

        public EventAttendeeManager(IBaseRepository<EventAttendee> repository, IKeyedRepository<Event> eventRepository, IEmailManager emailManager) : base(repository)
        {
            this.eventRepository = eventRepository;
            this.emailManager = emailManager;
        }

        public async Task<IEnumerable<Event>> GetEventsUserIsAttending(Guid attendeeId, bool futureEventsOnly = false, CancellationToken cancellationToken = default)
        {
            var eventAttendees = Repository.Get(ea => ea.UserId == attendeeId);

            var events = await eventRepository.Get(e => e.EventStatusId != (int)EventStatusEnum.Canceled
                                                     && (!futureEventsOnly || e.EventDate >= DateTimeOffset.UtcNow)
                                                     && eventAttendees.Select(ea => ea.EventId).Contains(e.Id))
                                              .ToListAsync(cancellationToken);
            return events;
        }

        public async Task<IEnumerable<Event>> GetCanceledEventsUserIsAttending(Guid attendeeId, bool futureEventsOnly = false, CancellationToken cancellationToken = default)
        {
            // TODO: Are there are better ways to do this?
            var eventAttendees = Repository.Get(ea => ea.UserId == attendeeId);

            var events = await eventRepository.Get(e => e.EventStatusId == (int)EventStatusEnum.Canceled
                                                   && (!futureEventsOnly || e.EventDate >= DateTimeOffset.UtcNow)
                                                   && eventAttendees.Select(ea => ea.EventId).Contains(e.Id))
                                              .ToListAsync(cancellationToken);
            return events;
        }

        public override async Task<int> Delete(Guid parentId, Guid secondId, CancellationToken cancellationToken)
        {
            var eventAttendee = await Repository.Get(ea => ea.EventId == parentId && ea.UserId == secondId).FirstOrDefaultAsync(cancellationToken);

            return await Repository.Delete(eventAttendee);
        }
    }
}
