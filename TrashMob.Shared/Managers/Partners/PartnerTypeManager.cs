namespace TrashMob.Shared.Managers.Partners
{
    using TrashMob.Shared.Managers.Interfaces;
    using TrashMob.Shared.Models;
    using TrashMob.Shared.Persistence.Interfaces;

    public class PartnerTypeManager : LookupManager<PartnerType>, ILookupManager<PartnerType>
    {
        public PartnerTypeManager(ILookupRepository<PartnerType> repository) : base(repository)
        {
        }
    }
}
