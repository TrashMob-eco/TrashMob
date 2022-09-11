namespace TrashMob.Shared.Persistence
{
    using System;
    using System.Threading.Tasks;
    using TrashMob.Shared.Models;

    public class CommunityRequestRepository : ICommunityRequestRepository
    {
        private readonly MobDbContext mobDbContext;

        public CommunityRequestRepository(MobDbContext mobDbContext)
        {
            this.mobDbContext = mobDbContext;
        }

        public async Task<CommunityRequest> AddCommunityRequest(CommunityRequest communityRequest)
        {
            communityRequest.Id = Guid.NewGuid();
            communityRequest.CreatedDate = DateTimeOffset.UtcNow;
            mobDbContext.CommunityRequests.Add(communityRequest);

            await mobDbContext.SaveChangesAsync().ConfigureAwait(false);

            return await mobDbContext.CommunityRequests.FindAsync(communityRequest.Id).ConfigureAwait(false);
        }
    }
}
