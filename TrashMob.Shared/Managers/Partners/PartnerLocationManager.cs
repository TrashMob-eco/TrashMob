namespace TrashMob.Shared.Managers.Partners
{
    using TrashMob.Models;
    using TrashMob.Shared.Managers.Interfaces;
    using TrashMob.Shared.Persistence.Interfaces;

    public class PartnerLocationManager : KeyedManager<PartnerLocation>, IKeyedManager<PartnerLocation>
    {
        public PartnerLocationManager(IKeyedRepository<PartnerLocation> repository) : base(repository)
        {
        }
    }
}
