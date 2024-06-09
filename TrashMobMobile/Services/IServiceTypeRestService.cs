namespace TrashMobMobile.Services
{
    using TrashMob.Models;

    public interface IServiceTypeRestService
    {
        Task<IEnumerable<ServiceType>> GetServiceTypesAsync(CancellationToken cancellationToken = default);
    }
}