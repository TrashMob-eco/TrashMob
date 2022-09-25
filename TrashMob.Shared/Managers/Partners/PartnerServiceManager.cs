namespace TrashMob.Shared.Managers.Partners
{
    using TrashMob.Models;
    using TrashMob.Shared.Managers.Interfaces;
    using TrashMob.Shared.Persistence.Interfaces;

    public class PartnerServiceManager : BaseManager<PartnerService>, IBaseManager<PartnerService>
    {
        public PartnerServiceManager(IBaseRepository<PartnerService> partnerServiceRepository) : base(partnerServiceRepository)
        {
        }
    }
}
