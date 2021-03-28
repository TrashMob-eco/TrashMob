namespace TrashMob.Persistence
{
    using Microsoft.EntityFrameworkCore;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using TrashMob.Models;

    public class EventAttendeeRepository : IEventAttendeeRepository
    {
        private readonly MobDbContext mobDbContext;

        public EventAttendeeRepository(MobDbContext mobDbContext)
        {
            this.mobDbContext = mobDbContext;
        }

        public async Task<IEnumerable<EventAttendee>> GetAllEventAttendees(Guid eventId)
        {
            try
            {
                return await mobDbContext.EventAttendees.Where(ea => ea.EventId == eventId).ToListAsync().ConfigureAwait(false);
            }
            catch
            {
                throw;
            }
        }

        public Task<int> AddAttendeeToEvent(Guid eventId, Guid attendeeId)
        {
            try
            {
                var eventAttendee = new EventAttendee
                {
                    EventId = eventId,
                    UserId = attendeeId,
                    SignUpDate = DateTimeOffset.UtcNow,
                };

                mobDbContext.EventAttendees.Add(eventAttendee);
                return mobDbContext.SaveChangesAsync();
            }
            catch
            {
                throw;
            }
        }

        public Task<int> UpdateEventAttendee(EventAttendee eventAttendee)
        {
            try
            {
                mobDbContext.Entry(eventAttendee).State = EntityState.Modified;
                return mobDbContext.SaveChangesAsync();
            }
            catch
            {
                throw;
            }
        }
    }
}
