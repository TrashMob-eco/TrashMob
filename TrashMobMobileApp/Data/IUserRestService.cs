namespace TrashMobMobileApp.Data
{
    using System.Threading.Tasks;
    using TrashMobMobileApp.Models;

    public interface IUserRestService
    {
        Task<User> GetUserAsync(string userId);

        Task<User> AddUserAsync(User user);

        Task<User> UpdateUserAsync(User user);
    }
}