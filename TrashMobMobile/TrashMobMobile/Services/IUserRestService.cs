namespace TrashMobMobile.Services
{
    using System.Threading.Tasks;
    using TrashMobMobile.Models;

    public interface IUserRestService
    {
        Task<User> GetUserAsync(string userId);

        Task<User> AddUserAsync(User user);

        Task<User> UpdateUserAsync(User user);
    }
}