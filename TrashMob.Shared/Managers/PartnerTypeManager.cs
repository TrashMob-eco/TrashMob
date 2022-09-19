
namespace TrashMob.Shared.Managers
{
    using TrashMob.Shared.Models;
    using TrashMob.Shared.Persistence;

    public class PartnerTypeManager : LookupManager<PartnerType>, ILookupManager<PartnerType>
    {
        public PartnerTypeManager(ILookupRepository<PartnerType> repository) : base(repository)
        {
        }      
    }
}
