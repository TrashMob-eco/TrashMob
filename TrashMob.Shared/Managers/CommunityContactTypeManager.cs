
namespace TrashMob.Shared.Managers
{
    using TrashMob.Shared.Models;
    using TrashMob.Shared.Persistence;

    public class CommunityContactTypeManager : LookupManager<CommunityContactType>, ILookupManager<CommunityContactType>
    {
        public CommunityContactTypeManager(ILookupRepository<CommunityContactType> repository) : base(repository)
        {
        }      
    }
}
