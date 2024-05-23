namespace TrashMobMobile.Data
{
    using TrashMob.Models;

    public interface IMapRestService
    {
        Task<Address> GetAddressAsync(double latitude, double longitude, CancellationToken cancellationToken = default);
    }
}