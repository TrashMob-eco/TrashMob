namespace TrashMobMobileApp.Data
{
    using System.Threading.Tasks;
    using TrashMobMobileApp.Models;

    public interface IMapRestService
    {
        Task<Address> GetAddressAsync(double latitude, double longitude);
    }
}