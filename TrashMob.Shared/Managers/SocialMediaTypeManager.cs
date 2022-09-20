
namespace TrashMob.Shared.Managers
{
    using TrashMob.Shared.Managers.Interfaces;
    using TrashMob.Shared.Models;
    using TrashMob.Shared.Persistence.Interfaces;

    public class SocialMediaAccountTypeManager : LookupManager<SocialMediaAccountType>, ILookupManager<SocialMediaAccountType>
    {
        public SocialMediaAccountTypeManager(ILookupRepository<SocialMediaAccountType> repository) : base(repository)
        {
        }      
    }
}
