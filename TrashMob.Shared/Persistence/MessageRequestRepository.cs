namespace TrashMob.Shared.Persistence
{
    using System;
    using System.Threading.Tasks;
    using TrashMob.Shared.Models;
    using TrashMob.Shared.Persistence.Interfaces;

    public class MessageRequestRepository : IMessageRequestRepository
    {
        private readonly MobDbContext mobDbContext;

        public MessageRequestRepository(MobDbContext mobDbContext)
        {
            this.mobDbContext = mobDbContext;
        }

        public async Task<MessageRequest> AddMessageRequest(MessageRequest messageRequest)
        {
            messageRequest.Id = Guid.NewGuid();
            messageRequest.CreatedDate = DateTimeOffset.UtcNow;
            mobDbContext.MessageRequests.Add(messageRequest);

            await mobDbContext.SaveChangesAsync().ConfigureAwait(false);

            return await mobDbContext.MessageRequests.FindAsync(messageRequest.Id).ConfigureAwait(false);
        }
    }
}
