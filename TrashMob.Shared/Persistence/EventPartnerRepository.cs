namespace TrashMob.Shared.Persistence
{
    using Microsoft.EntityFrameworkCore;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using TrashMob.Shared.Models;
    using System.Data.SqlClient;

    public class EventPartnerRepository : IEventPartnerRepository
    {
        private readonly MobDbContext mobDbContext;

        public EventPartnerRepository(MobDbContext mobDbContext)
        {
            this.mobDbContext = mobDbContext;
        }

        public async Task<IEnumerable<EventPartner>> GetEventPartners(Guid eventId)
        {
            var eventPartners = await mobDbContext.EventPartners
                .Where(ea => ea.EventId == eventId)
                .AsNoTracking()
                .ToListAsync().ConfigureAwait(false);

            return eventPartners;
        }

        public Task<int> AddEventPartner(EventPartner eventPartner)
        {
            try
            {
                eventPartner.CreatedDate = DateTimeOffset.UtcNow;
                eventPartner.LastUpdatedDate = DateTimeOffset.UtcNow;
                mobDbContext.EventPartners.Add(eventPartner);
                return mobDbContext.SaveChangesAsync();
            }
            catch (DbUpdateException ex)
            {
                if ((ex.InnerException.InnerException is SqlException innerException) && (innerException.Number == 2627 || innerException.Number == 2601))
                { 
                    return Task.FromResult(0);
                }
                else
                {
                    throw;
                }
            }
        }

        public Task<int> UpdateEventPartner(EventPartner eventPartner)
        {
            eventPartner.LastUpdatedDate = DateTimeOffset.UtcNow;
            mobDbContext.Entry(eventPartner).State = EntityState.Modified;
            return mobDbContext.SaveChangesAsync();
        }

        public async Task<IEnumerable<EventPartner>> GetEventsForPartnerLocation(Guid partnerLocationId)
        {
            // TODO: There are better ways to do this.
            var eventPartnerLocations = await mobDbContext.EventPartners
                .Where(ea => ea.PartnerLocationId == partnerLocationId)                
                .AsNoTracking()
                .ToListAsync().ConfigureAwait(false);

            return eventPartnerLocations;
        }

        public async Task<IEnumerable<PartnerLocation>> GetPotentialEventPartners(Guid eventId)
        {
            var mobEvent = await mobDbContext.Events.FindAsync(eventId).ConfigureAwait(false);

            // Simple match on postal code or city first. Radius later
            var partnerLocations = mobDbContext.PartnerLocations                
                .Where(pl => pl.PostalCode == mobEvent.PostalCode
                       || pl.City == mobEvent.City);
                        
            return partnerLocations;
        }
    }
}
