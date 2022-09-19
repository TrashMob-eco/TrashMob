
namespace TrashMob.Shared.Managers
{
    using TrashMob.Shared.Models;
    using TrashMob.Shared.Persistence;

    public class PartnerSocialMediaAccountManager : BaseManager<PartnerSocialMediaAccount>, IBaseManager<PartnerSocialMediaAccount>
    {
        public PartnerSocialMediaAccountManager(IBaseRepository<PartnerSocialMediaAccount> repository) : base(repository)
        {
        }      
    }
}
