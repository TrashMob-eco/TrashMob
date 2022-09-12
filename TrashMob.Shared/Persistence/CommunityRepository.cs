namespace TrashMob.Shared.Persistence
{
    using TrashMob.Shared.Models;

    public class CommunityRepository : Repository<Community>, IRepository<Community>
    {
        public CommunityRepository(MobDbContext mobDbContext) : base(mobDbContext)
        {
        }
    }
}
