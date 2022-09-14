
namespace TrashMob.Shared.Managers
{
    using TrashMob.Shared.Models;
    using TrashMob.Shared.Persistence;

    public class CommunityContactManager : ExtendedManager<CommunityContact>, IExtendedManager<CommunityContact>
    {
        public CommunityContactManager(IRepository<CommunityContact> repository) : base(repository)
        {
        }      
    }
}
