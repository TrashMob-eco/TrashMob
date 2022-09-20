namespace TrashMob.Shared.Managers.Partners
{
    using TrashMob.Shared.Managers.Interfaces;
    using TrashMob.Shared.Models;
    using TrashMob.Shared.Persistence.Interfaces;

    public class PartnerSocialMediaAccountManager : KeyedManager<PartnerSocialMediaAccount>, IKeyedManager<PartnerSocialMediaAccount>
    {
        public PartnerSocialMediaAccountManager(IKeyedRepository<PartnerSocialMediaAccount> repository) : base(repository)
        {
        }
    }
}
