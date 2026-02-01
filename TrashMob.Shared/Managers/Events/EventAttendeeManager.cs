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

        /// <inheritdoc />
        public async Task<bool> IsEventLeadAsync(Guid eventId, Guid userId, CancellationToken cancellationToken = default)
        {
            // Check if user is a lead through EventAttendees
            var attendee = await Repository.Get(ea => ea.EventId == eventId && ea.UserId == userId)
                .FirstOrDefaultAsync(cancellationToken);

            if (attendee != null && attendee.IsEventLead)
            {
                return true;
            }

            // Also check if user is the event creator (they're always considered a lead)
            var evt = await eventRepository.GetAsync(eventId, cancellationToken);
            return evt != null && evt.CreatedByUserId == userId;
        }

        /// <inheritdoc />
        public async Task<IEnumerable<EventAttendee>> GetEventLeadsAsync(Guid eventId, CancellationToken cancellationToken = default)
        {
            return await Repository.Get(ea => ea.EventId == eventId && ea.IsEventLead)
                .Include(ea => ea.User)
                .ToListAsync(cancellationToken);
        }

        /// <inheritdoc />
        public async Task<EventAttendee> PromoteToLeadAsync(Guid eventId, Guid userId, Guid promotedByUserId, CancellationToken cancellationToken = default)
        {
            // Check max leads limit (5)
            var currentLeadCount = await Repository.Get(ea => ea.EventId == eventId && ea.IsEventLead)
                .CountAsync(cancellationToken);

            if (currentLeadCount >= 5)
            {
                throw new InvalidOperationException("Maximum of 5 co-leads per event has been reached.");
            }

            var attendee = await Repository.Get(ea => ea.EventId == eventId && ea.UserId == userId)
                .FirstOrDefaultAsync(cancellationToken);

            if (attendee == null)
            {
                throw new InvalidOperationException("User is not an attendee of this event.");
            }

            if (attendee.IsEventLead)
            {
                throw new InvalidOperationException("User is already an event lead.");
            }

            attendee.IsEventLead = true;
            attendee.LastUpdatedByUserId = promotedByUserId;
            attendee.LastUpdatedDate = DateTimeOffset.UtcNow;

            await Repository.UpdateAsync(attendee);

            return attendee;
        }

        /// <inheritdoc />
        public async Task<EventAttendee> DemoteFromLeadAsync(Guid eventId, Guid userId, Guid demotedByUserId, CancellationToken cancellationToken = default)
        {
            // Check that we're not removing the last lead
            var currentLeadCount = await Repository.Get(ea => ea.EventId == eventId && ea.IsEventLead)
                .CountAsync(cancellationToken);

            if (currentLeadCount <= 1)
            {
                throw new InvalidOperationException("Cannot remove the last event lead. At least one lead is required.");
            }

            var attendee = await Repository.Get(ea => ea.EventId == eventId && ea.UserId == userId)
                .FirstOrDefaultAsync(cancellationToken);

            if (attendee == null)
            {
                throw new InvalidOperationException("User is not an attendee of this event.");
            }

            if (!attendee.IsEventLead)
            {
                throw new InvalidOperationException("User is not an event lead.");
            }

            attendee.IsEventLead = false;
            attendee.LastUpdatedByUserId = demotedByUserId;
            attendee.LastUpdatedDate = DateTimeOffset.UtcNow;

            await Repository.UpdateAsync(attendee);

            return attendee;
        }
    }
}