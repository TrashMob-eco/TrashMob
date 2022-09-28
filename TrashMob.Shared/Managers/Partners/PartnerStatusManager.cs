namespace TrashMob.Shared.Managers.Partners
{
    using TrashMob.Models;
    using TrashMob.Shared.Managers.Interfaces;
    using TrashMob.Shared.Persistence.Interfaces;

    public class PartnerStatusManager : LookupManager<PartnerStatus>, ILookupManager<PartnerStatus>
    {
        public PartnerStatusManager(ILookupRepository<PartnerStatus> repository) : base(repository)
        {
        }
    }
}
