
namespace TrashMob.Shared.Managers
{
    using TrashMob.Shared.Models;
    using TrashMob.Shared.Persistence;

    public class CommunityContactManager : KeyedManager<CommunityContact>, IKeyedManager<CommunityContact>
    {
        public CommunityContactManager(IKeyedRepository<CommunityContact> repository) : base(repository)
        {
        }      
    }
}
