namespace TrashMobMobileApp.Data
{
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using TrashMob.Models;

    public interface IServiceTypeRestService
    {
        Task<IEnumerable<ServiceType>> GetServiceTypesAsync(CancellationToken cancellationToken = default);
    }
}