namespace TrashMob.Shared.Persistence.Events
{
    using Microsoft.EntityFrameworkCore;
    using System;
    using System.Linq;
    using System.Threading;
    using System.Threading.Tasks;
    using TrashMob.Shared.Models;
    using TrashMob.Shared.Persistence.Interfaces;

    public class EventSummaryRepository : IEventSummaryRepository
    {
        private readonly MobDbContext mobDbContext;

        public EventSummaryRepository(MobDbContext mobDbContext)
        {
            this.mobDbContext = mobDbContext;
        }

        public async Task<EventSummary> GetEventSummary(Guid eventId, CancellationToken cancellationToken = default)
        {
            return await mobDbContext.EventSummaries.FindAsync(new object[] { eventId }, cancellationToken: cancellationToken).ConfigureAwait(false);
        }

        public IQueryable<EventSummary> GetEventSummaries(CancellationToken cancellationToken = default)
        {
            return mobDbContext.EventSummaries.AsNoTracking().AsQueryable();
        }

        // Add new EventSummary record     
        public async Task<EventSummary> AddEventSummary(EventSummary eventSummary)
        {
            eventSummary.CreatedDate = DateTimeOffset.UtcNow;
            eventSummary.LastUpdatedDate = DateTimeOffset.UtcNow;
            mobDbContext.EventSummaries.Add(eventSummary);
            await mobDbContext.SaveChangesAsync().ConfigureAwait(false);

            var summary = await mobDbContext.EventSummaries.FindAsync(eventSummary.EventId).ConfigureAwait(false);
            mobDbContext.Entry(summary).State = EntityState.Detached;
            return summary;
        }

        // Update the records of a particular EventSummary
        public async Task<EventSummary> UpdateEventSummary(EventSummary eventSummary)
        {
            eventSummary.LastUpdatedDate = DateTimeOffset.UtcNow;
            mobDbContext.Entry(eventSummary).State = EntityState.Modified;
            await mobDbContext.SaveChangesAsync().ConfigureAwait(false);
            var summary = await mobDbContext.EventSummaries.FindAsync(eventSummary.EventId).ConfigureAwait(false);
            mobDbContext.Entry(summary).State = EntityState.Detached;
            return summary;
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
