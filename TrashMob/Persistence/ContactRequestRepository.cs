namespace TrashMob.Persistence
{
    using Microsoft.EntityFrameworkCore;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using TrashMob.Extensions;
    using TrashMob.Models;

    public class ContactRequestRepository : IContactRequestRepository
    {
        private readonly MobDbContext mobDbContext;

        public ContactRequestRepository(MobDbContext mobDbContext)
        {
            this.mobDbContext = mobDbContext;
        }

        public async Task AddContactRequest(ContactRequest contactRequest)
        {
            contactRequest.Id = Guid.NewGuid();
            contactRequest.CreatedDate = DateTimeOffset.UtcNow;
            mobDbContext.ContactRequests.Add(contactRequest);

            await mobDbContext.SaveChangesAsync().ConfigureAwait(false);
        }

        // Update the records of a particluar Event  
        public Task<int> UpdateEvent(Event mobEvent)
        {
            mobDbContext.Entry(mobEvent).State = EntityState.Modified;
            var eventHistory = mobEvent.ToEventHistory();
            mobDbContext.EventHistories.Add(eventHistory);
            return mobDbContext.SaveChangesAsync();
        }

        // Get the details of a particular Event    
        public async Task<Event> GetEvent(Guid id)
        {
            return await mobDbContext.Events.FindAsync(id).ConfigureAwait(false);
        }

        // Delete the record of a particular Mob Event    
        public async Task<int> DeleteEvent(Guid id)
        {
            var mobEvent = await mobDbContext.Events.FindAsync(id).ConfigureAwait(false);
            mobDbContext.Events.Remove(mobEvent);
            return await mobDbContext.SaveChangesAsync().ConfigureAwait(false);
        }
    }
}
