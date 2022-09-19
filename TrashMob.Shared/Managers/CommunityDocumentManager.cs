
namespace TrashMob.Shared.Managers
{
    using TrashMob.Shared.Models;
    using TrashMob.Shared.Persistence;

    public class CommunityDocumentManager : KeyedManager<CommunityDocument>, IKeyedManager<CommunityDocument>
    {
        public CommunityDocumentManager(IKeyedRepository<CommunityDocument> repository) : base(repository)
        {
        }      
    }
}
