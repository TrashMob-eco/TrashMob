namespace TrashMob.Shared.Managers.Events
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading;
    using System.Threading.Tasks;
    using Microsoft.EntityFrameworkCore;
    using TrashMob.Models;
    using TrashMob.Models.Poco;
    using TrashMob.Shared.Managers.Interfaces;
    using TrashMob.Shared.Persistence.Interfaces;

    /// <summary>
    /// Manages event attendee registrations and tracks which events users are attending.
    /// </summary>
    public class EventAttendeeManager : BaseManager<EventAttendee>, IBaseManager<EventAttendee>, IEventAttendeeManager
    {
        private readonly IEmailManager emailManager;
        private readonly IKeyedRepository<Event> eventRepository;

        /// <summary>
        /// Initializes a new instance of the <see cref="EventAttendeeManager"/> class.
        /// </summary>
        /// <param name="repository">The repository for event attendee data access.</param>
        /// <param name="eventRepository">The repository for event data access.</param>
        /// <param name="emailManager">The email manager for sending notifications.</param>
        public EventAttendeeManager(IBaseRepository<EventAttendee> repository, IKeyedRepository<Event> eventRepository,
            IEmailManager emailManager) : base(repository)
        {
            this.eventRepository = eventRepository;
            this.emailManager = emailManager;
        }

        /// <inheritdoc />
        public override async Task<IEnumerable<EventAttendee>> GetByParentIdAsync(Guid parentId,
            CancellationToken cancellationToken)
        {
            return (await Repository.Get().Where(p => p.EventId == parentId)
                    .Include(p => p.User)
                    .ToListAsync(cancellationToken))
                .AsEnumerable();
        }

        /// <inheritdoc />
        public override async Task<int> Delete(Guid parentId, Guid secondId, CancellationToken cancellationToken)
        {
            var eventAttendee = await Repository.Get(ea => ea.EventId == parentId && ea.UserId == secondId)
                .FirstOrDefaultAsync(cancellationToken);

            return await Repository.DeleteAsync(eventAttendee);
        }

        /// <inheritdoc />
        public async Task<IEnumerable<Event>> GetEventsUserIsAttendingAsync(Guid attendeeId,
            bool futureEventsOnly = false, CancellationToken cancellationToken = default)
        {
            var eventAttendees = await Repository.Get(ea => ea.UserId == attendeeId).ToListAsync(cancellationToken);

            if (eventAttendees.Any())
            {
                var events = await eventRepository.Get(e => e.EventStatusId != (int)EventStatusEnum.Canceled
                                                            && (!futureEventsOnly ||
                                                                e.EventDate >= DateTimeOffset.UtcNow)
                                                            && eventAttendees.Select(ea => ea.EventId).Contains(e.Id))
                    .ToListAsync(cancellationToken);
                return events;
            }

            return new List<Event>();
        }

        /// <inheritdoc />
        public async Task<IEnumerable<Event>> GetEventsUserIsAttendingAsync(EventFilter filter, Guid attendeeId,
            CancellationToken cancellationToken = default)
        {
            var eventAttendees = await Repository.Get(ea => ea.UserId == attendeeId).ToListAsync(cancellationToken);

            if (eventAttendees.Any())
            {
                var events = await eventRepository.Get(e => e.EventStatusId != (int)EventStatusEnum.Canceled &&
                                                           (filter.StartDate == null || e.EventDate >= filter.StartDate) &&
                                                           (filter.EndDate == null || e.EventDate <= filter.EndDate) &&
                                                            eventAttendees.Select(ea => ea.EventId).Contains(e.Id))
                    .ToListAsync(cancellationToken);
                return events;
            }

            return new List<Event>();
        }

        /// <inheritdoc />
        public async Task<IEnumerable<Event>> GetCanceledEventsUserIsAttendingAsync(Guid attendeeId,
            bool futureEventsOnly = false, CancellationToken cancellationToken = default)
        {
            // TODO: Are there are better ways to do this?
            var eventAttendees = Repository.Get(ea => ea.UserId == attendeeId);

            var events = await eventRepository.Get(e => e.EventStatusId == (int)EventStatusEnum.Canceled
                                                        && (!futureEventsOnly || e.EventDate >= DateTimeOffset.UtcNow)
                                                        && eventAttendees.Select(ea => ea.EventId).Contains(e.Id))
                .ToListAsync(cancellationToken);
            return events;
        }
    }
}