namespace TrashMob.Shared.Managers.Partners
{
    using TrashMob.Shared.Managers.Interfaces;
    using TrashMob.Shared.Models;
    using TrashMob.Shared.Persistence.Interfaces;

    public class PartnerServiceManager : BaseManager<PartnerService>, IBaseManager<PartnerService>
    {
        public PartnerServiceManager(IBaseRepository<PartnerService> partnerServiceRepository) : base(partnerServiceRepository)
        {
        }
    }
}
