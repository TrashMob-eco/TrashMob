
namespace TrashMob.Shared.Managers
{
    using TrashMob.Shared.Models;
    using TrashMob.Shared.Persistence;

    public class CommunityPartnerManager : BaseManager<CommunityPartner>, IBaseManager<CommunityPartner>
    {
        public CommunityPartnerManager(IBaseRepository<CommunityPartner> repository) : base(repository)
        {
        }      
    }
}
