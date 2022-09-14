
namespace TrashMob.Shared.Managers
{
    using TrashMob.Shared.Models;
    using TrashMob.Shared.Persistence;

    public class CommunityAttachmentManager : ExtendedManager<CommunityAttachment>, IExtendedManager<CommunityAttachment>
    {
        public CommunityAttachmentManager(IRepository<CommunityAttachment> repository) : base(repository)
        {
        }      
    }
}
