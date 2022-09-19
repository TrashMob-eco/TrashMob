
namespace TrashMob.Shared.Managers
{
    using TrashMob.Shared.Models;
    using TrashMob.Shared.Persistence;

    public class CommunityNoteManager : KeyedManager<CommunityNote>, IKeyedManager<CommunityNote>
    {
        public CommunityNoteManager(IKeyedRepository<CommunityNote> repository) : base(repository)
        {
        }      
    }
}
