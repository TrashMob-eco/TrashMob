
namespace TrashMob.Shared.Managers
{
    using TrashMob.Shared.Models;
    using TrashMob.Shared.Persistence;

    public class CommunityStatusManager : LookupManager<CommunityStatus>, ILookupManager<CommunityStatus>
    {
        public CommunityStatusManager(ILookupRepository<CommunityStatus> repository) : base(repository)
        {
        }      
    }
}
