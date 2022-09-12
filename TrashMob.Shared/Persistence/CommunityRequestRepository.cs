namespace TrashMob.Shared.Persistence
{
    using System;
    using System.Threading.Tasks;
    using TrashMob.Shared.Models;

    public class CommunityRequestRepository : Repository<CommunityRequest>, IRepository<CommunityRequest>
    {
        public CommunityRequestRepository(MobDbContext mobDbContext) : base(mobDbContext)
        {
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
