namespace TrashMob.Shared.Persistence
{
    using Microsoft.EntityFrameworkCore;
    using System;
    using System.Linq;
    using System.Threading.Tasks;
    using TrashMob.Shared.Models;

    public class EventSummaryRepository : IEventSummaryRepository
    {
        private readonly MobDbContext mobDbContext;

        public EventSummaryRepository(MobDbContext mobDbContext)
        {
            this.mobDbContext = mobDbContext;
        }

        public async Task<EventSummary> GetEventSummary(Guid eventId)
        {
            return await mobDbContext.EventSummaries.FirstOrDefaultAsync(es => es.EventId == eventId).ConfigureAwait(false);
        }

        public IQueryable<EventSummary> GetEventSummaries()
        {
            return mobDbContext.EventSummaries.AsQueryable();
        }

        // Add new EventSummary record     
        public async Task<EventSummary> AddEventSummary(EventSummary eventSummary)
        {
            eventSummary.CreatedDate = DateTimeOffset.UtcNow;
            eventSummary.LastUpdatedDate = DateTimeOffset.UtcNow;
            mobDbContext.EventSummaries.Add(eventSummary);
            await mobDbContext.SaveChangesAsync().ConfigureAwait(false);
            return eventSummary;
        }

        // Update the records of a particular EventSummary
        public async Task<EventSummary> UpdateEventSummary(EventSummary eventSummary)
        {
            eventSummary.LastUpdatedDate = DateTimeOffset.UtcNow;
            mobDbContext.Entry(eventSummary).State = EntityState.Modified;
            await mobDbContext.SaveChangesAsync();
            return eventSummary;
        }

        // Delete the record of a particular EventSummary
        public async Task<int> DeleteEventSummary(Guid eventId)
        {
            var eventSummary = await mobDbContext.EventSummaries.FindAsync(eventId).ConfigureAwait(false);
            
            mobDbContext.EventSummaries.Remove(eventSummary);

            return await mobDbContext.SaveChangesAsync().ConfigureAwait(false);

        }
    }
}
