namespace TrashMobMobile.Services
{
    using System.Threading.Tasks;
    using TrashMobMobile.Models;

    public interface IMapRestService
    {
        Task<Address> GetAddressAsync(double latitude, double longitude);
    }
}