namespace TrashMob.Shared.Managers.Partners
{
    using TrashMob.Models;
    using TrashMob.Shared.Managers.Interfaces;
    using TrashMob.Shared.Persistence.Interfaces;

    public class PartnerRequestStatusManager : LookupManager<PartnerRequestStatus>, ILookupManager<PartnerRequestStatus>
    {
        public PartnerRequestStatusManager(ILookupRepository<PartnerRequestStatus> repository) : base(repository)
        {
        }
    }
}
