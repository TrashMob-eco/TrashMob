namespace TrashMob.Shared.Persistence
{
    using Microsoft.EntityFrameworkCore;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using TrashMob.Shared.Models;
    using System.Data.SqlClient;
    using System.Threading;

    public class EventPartnerRepository : IEventPartnerRepository
    {
        private readonly MobDbContext mobDbContext;

        public EventPartnerRepository(MobDbContext mobDbContext)
        {
            this.mobDbContext = mobDbContext;
        }

        public async Task<IEnumerable<EventPartner>> GetEventPartners(Guid eventId, CancellationToken cancellationToken = default)
        {
            var eventPartners = await mobDbContext.EventPartners
                .Where(ea => ea.EventId == eventId)
                .AsNoTracking()
                .ToListAsync(cancellationToken: cancellationToken).ConfigureAwait(false);

            return eventPartners;
        }

        public async Task<IEnumerable<EventPartner>> GetPartnerEvents(Guid partnerId, CancellationToken cancellationToken = default)
        {
            var eventPartners = await mobDbContext.EventPartners
                .Where(ea => ea.PartnerId == partnerId)
                .AsNoTracking()
                .ToListAsync(cancellationToken: cancellationToken).ConfigureAwait(false);

            return eventPartners;
        }

        public async Task<EventPartner> AddEventPartner(EventPartner eventPartner)
        {
            try
            {
                eventPartner.CreatedDate = DateTimeOffset.UtcNow;
                eventPartner.LastUpdatedDate = DateTimeOffset.UtcNow;
                mobDbContext.EventPartners.Add(eventPartner);
                await mobDbContext.SaveChangesAsync().ConfigureAwait(false);
                return await mobDbContext.EventPartners.FindAsync(eventPartner.EventId, eventPartner.PartnerId, eventPartner.PartnerLocationId).ConfigureAwait(false);
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

        public async Task<EventPartner> UpdateEventPartner(EventPartner eventPartner)
        {
            var originalEventPartner = mobDbContext.EventPartners
                .FirstOrDefault(ep => ep.EventId == eventPartner.EventId && ep.PartnerId == eventPartner.PartnerId && ep.PartnerLocationId == eventPartner.PartnerLocationId);

            originalEventPartner.LastUpdatedDate = DateTimeOffset.UtcNow;
            originalEventPartner.LastUpdatedByUserId = eventPartner.LastUpdatedByUserId;
            originalEventPartner.EventPartnerStatusId = eventPartner.EventPartnerStatusId;

            await mobDbContext.SaveChangesAsync().ConfigureAwait(false);
            return await mobDbContext.EventPartners.FindAsync(eventPartner.EventId, eventPartner.PartnerId, eventPartner.PartnerLocationId).ConfigureAwait(false);
        }

        public async Task<IEnumerable<EventPartner>> GetEventsForPartnerLocation(Guid partnerLocationId, CancellationToken cancellationToken = default)
        {
            // TODO: There are better ways to do this.
            var eventPartnerLocations = await mobDbContext.EventPartners
                .Where(ea => ea.PartnerLocationId == partnerLocationId)                
                .AsNoTracking()
                .ToListAsync(cancellationToken: cancellationToken).ConfigureAwait(false);

            return eventPartnerLocations;
        }

        public async Task<IEnumerable<PartnerLocation>> GetPotentialEventPartners(Guid eventId, CancellationToken cancellationToken = default)
        {
            var mobEvent = await mobDbContext.Events.FindAsync(new object[] { eventId }, cancellationToken: cancellationToken).ConfigureAwait(false);

            // Simple match on postal code or city first. Radius later
            var partnerLocations = mobDbContext.PartnerLocations                
                .Where(pl => pl.IsActive && pl.Partner.PartnerStatusId == (int)PartnerStatusEnum.Active && 
                        (pl.PostalCode == mobEvent.PostalCode || pl.City == mobEvent.City));
                        
            return partnerLocations;
        }
    }
}
