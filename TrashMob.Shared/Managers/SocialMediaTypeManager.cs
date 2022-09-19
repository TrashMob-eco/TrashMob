
namespace TrashMob.Shared.Managers
{
    using TrashMob.Shared.Models;
    using TrashMob.Shared.Persistence;

    public class SocialMediaAccountTypeManager : LookupManager<SocialMediaAccountType>, ILookupManager<SocialMediaAccountType>
    {
        public SocialMediaAccountTypeManager(ILookupRepository<SocialMediaAccountType> repository) : base(repository)
        {
        }      
    }
}
