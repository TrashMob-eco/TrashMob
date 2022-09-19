
namespace TrashMob.Shared.Managers
{
    using TrashMob.Shared.Models;
    using TrashMob.Shared.Persistence;

    public class CommunitySocialMediaAccountManager : BaseManager<CommunitySocialMediaAccount>, IBaseManager<CommunitySocialMediaAccount>
    {
        public CommunitySocialMediaAccountManager(IBaseRepository<CommunitySocialMediaAccount> repository) : base(repository)
        {
        }      
    }
}
