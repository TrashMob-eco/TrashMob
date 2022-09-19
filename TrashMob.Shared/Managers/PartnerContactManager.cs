
namespace TrashMob.Shared.Managers
{
    using TrashMob.Shared.Models;
    using TrashMob.Shared.Persistence;

    public class PartnerContactManager : KeyedManager<PartnerContact>, IKeyedManager<PartnerContact>
    {
        public PartnerContactManager(IKeyedRepository<PartnerContact> repository) : base(repository)
        {
        }      
    }
}
