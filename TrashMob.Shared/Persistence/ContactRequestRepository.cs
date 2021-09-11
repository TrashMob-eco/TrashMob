namespace TrashMob.Shared.Persistence
{
    using System;
    using System.Threading.Tasks;
    using TrashMob.Shared.Models;

    public class ContactRequestRepository : IContactRequestRepository
    {
        private readonly MobDbContext mobDbContext;

        public ContactRequestRepository(MobDbContext mobDbContext)
        {
            this.mobDbContext = mobDbContext;
        }

        public async Task<ContactRequest> AddContactRequest(ContactRequest contactRequest)
        {
            contactRequest.Id = Guid.NewGuid();
            contactRequest.CreatedDate = DateTimeOffset.UtcNow;
            mobDbContext.ContactRequests.Add(contactRequest);

            await mobDbContext.SaveChangesAsync().ConfigureAwait(false);

            return await mobDbContext.ContactRequests.FindAsync(contactRequest.Id);
        }
    }
}
