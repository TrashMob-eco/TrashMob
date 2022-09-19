
namespace TrashMob.Shared.Managers
{
    using TrashMob.Shared.Models;
    using TrashMob.Shared.Persistence;

    public class CommunityAttachmentManager : KeyedManager<CommunityAttachment>, IKeyedManager<CommunityAttachment>
    {
        public CommunityAttachmentManager(IKeyedRepository<CommunityAttachment> repository) : base(repository)
        {
        }      
    }
}
