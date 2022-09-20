
namespace TrashMob.Shared.Managers
{
    using TrashMob.Shared.Models;
    using TrashMob.Shared.Persistence;

    public class PartnerSocialMediaAccountManager : KeyedManager<PartnerSocialMediaAccount>, IKeyedManager<PartnerSocialMediaAccount>
    {
        public PartnerSocialMediaAccountManager(IKeyedRepository<PartnerSocialMediaAccount> repository) : base(repository)
        {
        }      
    }
}
