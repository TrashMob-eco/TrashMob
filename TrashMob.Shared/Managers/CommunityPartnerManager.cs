
namespace TrashMob.Shared.Managers
{
    using TrashMob.Shared.Models;
    using TrashMob.Shared.Persistence;

    public class CommunityPartnerManager : ExtendedManager<CommunityPartner>, IExtendedManager<CommunityPartner>
    {
        public CommunityPartnerManager(IRepository<CommunityPartner> repository) : base(repository)
        {
        }      
    }
}
