
namespace TrashMob.Shared.Managers
{
    using TrashMob.Shared.Models;
    using TrashMob.Shared.Persistence;

    public class SocialMediaAccountManager : KeyedManager<SocialMediaAccount>, IKeyedManager<SocialMediaAccount>
    {
        public SocialMediaAccountManager(IKeyedRepository<SocialMediaAccount> repository) : base(repository)
        {
        }      
    }
}
