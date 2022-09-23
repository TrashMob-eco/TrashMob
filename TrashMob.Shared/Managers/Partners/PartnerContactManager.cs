namespace TrashMob.Shared.Managers.Partners
{
    using TrashMob.Shared.Managers.Interfaces;
    using TrashMob.Shared.Models;
    using TrashMob.Shared.Persistence.Interfaces;

    public class PartnerContactManager : KeyedManager<PartnerContact>, IKeyedManager<PartnerContact>
    {
        public PartnerContactManager(IKeyedRepository<PartnerContact> repository) : base(repository)
        {
        }
    }
}
