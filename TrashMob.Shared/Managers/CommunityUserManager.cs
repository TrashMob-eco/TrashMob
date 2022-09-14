
namespace TrashMob.Shared.Managers
{
    using TrashMob.Shared.Models;
    using TrashMob.Shared.Persistence;

    public class CommunityUserManager : ExtendedManager<CommunityUser>, IExtendedManager<CommunityUser>
    {
        public CommunityUserManager(IRepository<CommunityUser> repository) : base(repository)
        {
        }      
    }
}
