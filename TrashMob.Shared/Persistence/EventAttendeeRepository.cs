namespace TrashMob.Shared.Persistence
{
    using Microsoft.EntityFrameworkCore;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using TrashMob.Shared.Models;
    using TrashMob.Shared;
    using System.Data.SqlClient;
    using System.Threading;

    public class EventAttendeeRepository : IEventAttendeeRepository
    {
        private readonly MobDbContext mobDbContext;

        public EventAttendeeRepository(MobDbContext mobDbContext)
        {
            this.mobDbContext = mobDbContext;
        }

        public async Task<IEnumerable<User>> GetEventAttendees(Guid eventId, CancellationToken cancellationToken = default)
        {
            // TODO: There are better ways to do this.
            var eventAttendees = await mobDbContext.EventAttendees
                .Where(ea => ea.EventId == eventId)
                .AsNoTracking()
                .ToListAsync().ConfigureAwait(false);

            var users = await mobDbContext.Users
                .Where(u => eventAttendees.Select(ea => ea.UserId).Contains(u.Id))
                .AsNoTracking()
                .ToListAsync().ConfigureAwait(false);
            return users;
        }

        public async Task<EventAttendee> AddEventAttendee(Guid eventId, Guid attendeeId)
        {
            var eventAttendee = new EventAttendee
            {
                EventId = eventId,
                UserId = attendeeId,
                SignUpDate = DateTimeOffset.UtcNow,
            };

            try
            {
                mobDbContext.EventAttendees.Add(eventAttendee);
                await mobDbContext.SaveChangesAsync().ConfigureAwait(false);
                return await mobDbContext.EventAttendees.FindAsync(eventId, attendeeId).ConfigureAwait(false);
            }
            catch (DbUpdateException ex)
            {
                if ((ex.InnerException.InnerException is SqlException innerException) && (innerException.Number == 2627 || innerException.Number == 2601))
                { 
                    return null;
                }
                else
                {
                    throw;
                }
            }
        }

        public async Task<EventAttendee> UpdateEventAttendee(EventAttendee eventAttendee)
        {
            mobDbContext.Entry(eventAttendee).State = EntityState.Modified;
            await mobDbContext.SaveChangesAsync().ConfigureAwait(false);
            return await mobDbContext.EventAttendees.FindAsync(eventAttendee.EventId, eventAttendee.UserId).ConfigureAwait(false);
        }

        public async Task<int> DeleteEventAttendee(Guid eventId, Guid userId)
        {
            var eventAttendee = await mobDbContext.EventAttendees.FindAsync(eventId, userId).ConfigureAwait(false);
            mobDbContext.EventAttendees.Remove(eventAttendee);
            return await mobDbContext.SaveChangesAsync().ConfigureAwait(false);
        }

        public async Task<IEnumerable<Event>> GetEventsUserIsAttending(Guid attendeeId, bool futureEventsOnly = false, CancellationToken cancellationToken = default)
        {
            // TODO: There are better ways to do this.
            var eventAttendees = await mobDbContext.EventAttendees
                .Where(ea => ea.UserId == attendeeId)                
                .AsNoTracking()
                .ToListAsync().ConfigureAwait(false);

            var events = await mobDbContext.Events
                .Where(e => e.EventStatusId != (int)EventStatusEnum.Canceled
                         && (!futureEventsOnly || e.EventDate >= DateTimeOffset.UtcNow)
                         && eventAttendees.Select(ea => ea.EventId).Contains(e.Id))
                .AsNoTracking()
                .ToListAsync().ConfigureAwait(false);
            return events;
        }

        public async Task<IEnumerable<Event>> GetCanceledEventsUserIsAttending(Guid attendeeId, bool futureEventsOnly = false, CancellationToken cancellationToken = default)
        {
            // TODO: There are better ways to do this.
            var eventAttendees = await mobDbContext.EventAttendees
                .Where(ea => ea.UserId == attendeeId)
                .AsNoTracking()
                .ToListAsync().ConfigureAwait(false);

            var events = await mobDbContext.Events
                .Where(e => e.EventStatusId == (int)EventStatusEnum.Canceled
                         && (!futureEventsOnly || e.EventDate >= DateTimeOffset.UtcNow)
                         && eventAttendees.Select(ea => ea.EventId).Contains(e.Id))
                .AsNoTracking()
                .ToListAsync().ConfigureAwait(false);
            return events;
        }
    }
}
