
namespace TrashMob.Shared.Managers
{
    using TrashMob.Shared.Models;
    using TrashMob.Shared.Persistence;

    public class CommunityNoteManager : ExtendedManager<CommunityNote>, IExtendedManager<CommunityNote>
    {
        public CommunityNoteManager(IRepository<CommunityNote> repository) : base(repository)
        {
        }      
    }
}
