
namespace TrashMob.Shared.Managers
{
    using TrashMob.Shared.Models;
    using TrashMob.Shared.Persistence;

    public class CommunityUserManager : BaseManager<CommunityUser>, IBaseManager<CommunityUser>
    {
        public CommunityUserManager(IBaseRepository<CommunityUser> repository) : base(repository)
        {
        }
    }
}
