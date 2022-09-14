
namespace TrashMob.Shared.Managers
{
    using TrashMob.Shared.Models;
    using TrashMob.Shared.Persistence;

    public class SocialMediaAccountManager : ExtendedManager<SocialMediaAccount>, IExtendedManager<SocialMediaAccount>
    {
        public SocialMediaAccountManager(IRepository<SocialMediaAccount> repository) : base(repository)
        {
        }      
    }
}
