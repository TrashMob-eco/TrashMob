namespace TrashMob.Shared.Managers.Partners
{
    using System.Threading.Tasks;
    using TrashMob.Shared.Extensions;
    using TrashMob.Shared.Managers.Interfaces;
    using TrashMob.Shared.Models;
    using TrashMob.Shared.Persistence.Interfaces;

    public class PartnerUserManager : BaseManager<PartnerUser>, IBaseManager<PartnerUser>
    {
        private readonly IBaseRepository<PartnerUser> partnerUser;

        public PartnerUserManager(IBaseRepository<PartnerUser> partnerUserRepository) : base(partnerUserRepository)
        {
            this.partnerUser = partnerUserRepository;
        }

    }
}
