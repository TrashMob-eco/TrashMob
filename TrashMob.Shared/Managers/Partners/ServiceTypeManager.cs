namespace TrashMob.Shared.Managers.Partners
{
    using TrashMob.Models;
    using TrashMob.Shared.Managers.Interfaces;
    using TrashMob.Shared.Persistence.Interfaces;

    public class ServiceTypeManager : LookupManager<ServiceType>, ILookupManager<ServiceType>
    {
        public ServiceTypeManager(ILookupRepository<ServiceType> repository) : base(repository)
        {
        }
    }
}
