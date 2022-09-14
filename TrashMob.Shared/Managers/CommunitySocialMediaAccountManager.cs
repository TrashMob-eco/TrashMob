
namespace TrashMob.Shared.Managers
{
    using TrashMob.Shared.Models;
    using TrashMob.Shared.Persistence;

    public class CommunitySocialMediaAccountManager : ExtendedManager<CommunitySocialMediaAccount>, IExtendedManager<CommunitySocialMediaAccount>
    {
        public CommunitySocialMediaAccountManager(IRepository<CommunitySocialMediaAccount> repository) : base(repository)
        {
        }      
    }
}
